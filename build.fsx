// Script de build multiplateforme : publication self-contained des 3 RID + archives zip.
// Usage : dotnet fsi build.fsx
// Prérequis : uniquement le SDK .NET (aucune dépendance externe).

open System
open System.Diagnostics
open System.IO
open System.IO.Compression

let root = __SOURCE_DIRECTORY__
let project = Path.Combine(root, "src", "MdpDico.UI", "mdp_dico.UI.fsproj")
let dist = Path.Combine(root, "dist")

// RID + nom de l'archive produite
let allPlatforms =
    [| "linux-x64", "mdp-dico-linux-x64"
       "win-x64", "mdp-dico-win-x64"
       "osx-arm64", "mdp-dico-macos-arm64" |]

// Sans argument : toutes les plateformes. Sinon uniquement les RID demandés :
//   dotnet fsi build.fsx                -> tout
//   dotnet fsi build.fsx linux-x64      -> seulement linux-x64
let requested =
    fsi.CommandLineArgs
    |> Array.filter (fun a -> a.EndsWith ".fsx" |> not)

let platforms =
    if requested.Length = 0 then allPlatforms
    else
        let validRids = allPlatforms |> Array.map fst |> String.concat ", "
        requested
        |> Array.map (fun rid ->
            allPlatforms
            |> Array.tryFind (fun (r, _) -> r = rid)
            |> Option.defaultWith (fun () -> failwith $"RID inconnu : {rid}. RID valides : {validRids}"))

/// Exécute une commande dans le répertoire racine, en héritant de la console
let run (cmd: string) (args: string) =
    let psi = ProcessStartInfo(cmd, args, WorkingDirectory = root, UseShellExecute = false)
    let p = Process.Start psi
    p.WaitForExit()
    if p.ExitCode <> 0 then
        failwith $"La commande a échoué : {cmd} {args}"

/// Publie une plateforme en self-contained dans dist/<rid>, puis la zippe
let publish (rid: string, zipName: string) =
    let outDir = Path.Combine(dist, rid)
    let zipPath = Path.Combine(dist, $"{zipName}.zip")

    if Directory.Exists outDir then Directory.Delete(outDir, recursive = true)
    if File.Exists zipPath then File.Delete zipPath

    printfn $"\n=== Publication {rid} ==="
    run "dotnet" $"publish \"{project}\" -c Release -r {rid} --self-contained true -o \"{outDir}\""

    ZipFile.CreateFromDirectory(outDir, zipPath)
    printfn $"[OK] {zipPath}"

Directory.CreateDirectory dist |> ignore
platforms |> Array.iter publish

printfn $"\nTerminé : {platforms.Length} archives dans {dist}"
