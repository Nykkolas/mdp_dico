module MainTests

open System
open Xunit
open FsUnit.Xunit
open Avalonia.Input.Platform
open Main

/// IClipboard n'est pas implementable par du code utilisateur sous Avalonia 12.
/// update ne fait que le capturer dans la Cmd, ignoree par ces tests :
/// la valeur par defaut n'est jamais dereferencee.
let pressePapiersFantome : IClipboard = Unchecked.defaultof<IClipboard>

let etatInitial () = fst (init ())

let etatEnCopie () =
    { etatInitial () with Password = "mot-de-passe"; IsCopying = true; Status = Copying }

let etatEnRegeneration () =
    { etatInitial () with Password = "ancien"; IsRegenerating = true; Status = Regenerating }

[<Fact>]
let ``init - demarre vide en generation`` () =
    let etat = etatInitial ()
    etat.Password |> should equal ""
    etat.IsPasswordVisible |> should be False
    etat.IsCopying |> should be False
    etat.IsRegenerating |> should be True
    etat.Status |> should equal Initial

[<Fact>]
let ``update - TogglePasswordVisibility inverse la visibilite`` () =
    let etat, _ = update TogglePasswordVisibility (etatInitial ())
    etat.IsPasswordVisible |> should be True

[<Fact>]
let ``update - TogglePasswordVisibility deux fois revient au depart`` () =
    let etat, _ = update TogglePasswordVisibility (etatInitial ()) |> fst |> update TogglePasswordVisibility
    etat.IsPasswordVisible |> should be False

[<Fact>]
let ``update - CopyRequested sans clipboard signale l'indisponibilite`` () =
    let etat, _ = update (CopyRequested None) (etatEnCopie ())
    etat.IsCopying |> should be False
    etat.Status |> should equal ClipboardUnavailable

[<Fact>]
let ``update - CopyRequested avec clipboard passe en copie`` () =
    let etat, _ = update (CopyRequested (Some pressePapiersFantome)) (etatEnCopie () |> fun e -> { e with IsCopying = false })
    etat.IsCopying |> should be True
    etat.Status |> should equal Copying

[<Fact>]
let ``update - CopyFinished confirme la copie`` () =
    let etat, _ = update CopyFinished (etatEnCopie ())
    etat.IsCopying |> should be False
    etat.Status |> should equal Copied

[<Fact>]
let ``update - CopyFailed remonte l'erreur`` () =
    let etat, _ = update (CopyFailed "boom") (etatEnCopie ())
    etat.IsCopying |> should be False
    etat.Status |> should equal (Main.CopyFailed "boom")

[<Fact>]
let ``update - RegenerateRequested passe en generation`` () =
    let etat, _ = update RegenerateRequested (etatInitial () |> fun e -> { e with IsRegenerating = false; Status = Generated })
    etat.IsRegenerating |> should be True
    etat.Status |> should equal Regenerating

[<Fact>]
let ``update - RegenerateFinished remplace le mot de passe`` () =
    let etat, _ = update (RegenerateFinished "nouveau-mdp") (etatEnRegeneration ())
    etat.Password |> should equal "nouveau-mdp"
    etat.IsRegenerating |> should be False
    etat.Status |> should equal Generated

[<Fact>]
let ``update - RegenerateFailed remonte l'erreur`` () =
    let etat, _ = update (RegenerateFailed "plantage") (etatEnRegeneration ())
    etat.IsRegenerating |> should be False
    etat.Status |> should equal (Main.RegenerateFailed "plantage")
