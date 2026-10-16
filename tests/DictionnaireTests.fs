module DictionnaireTests

open Xunit
open FsUnit.Xunit
open System.IO
open Dictionnaire

/// Module de tests pour le module Dictionnaire

[<Fact>]
let ``Dictionnaire - obtenirMots retourne la liste des mots`` () =
    let mots = ["mot1"; "mot2"; "mot3"]
    let dictionnaire = Dictionnaire mots
    
    let resultat = obtenirMots dictionnaire
    
    resultat |> should equal mots

[<Fact>]
let ``Dictionnaire - dictionnaireSecours contient des mots`` () =
    let mots = obtenirMots dictionnaireSecours
    
    mots |> should not' (be Empty)
    mots |> List.iter (fun mot -> mot.Length > 2 |> should be True)

[<Fact>]
let ``Dictionnaire - dictionnaireSecours contient des mots français`` () =
    let mots = obtenirMots dictionnaireSecours
    let motsEnMiniscule = mots |> List.map (fun m -> m.ToLower())
    
    // Vérifier que certains mots français connus sont présents
    let motsFrancais = ["avion"; "bateau"; "chaise"; "chien"; "fleur"]
    
    for mot in motsFrancais do
        motsEnMiniscule |> should contain mot

[<Fact>]
let ``Dictionnaire - chargerDepuisFichier retourne None si fichier inexistant`` () =
    let resultat = chargerDepuisFichier "/chemin/inexistant"
    
    resultat |> should equal None

[<Fact>]
let ``Dictionnaire - chargerDepuisFichier filtre les mots trop courts`` () =
    let chemin = Path.GetTempFileName()
    try
        File.WriteAllLines(chemin, [| "ab"; "abc"; "mot"; "xy"; "dictionnaire" |])
        let resultat = chargerDepuisFichier chemin
        resultat |> should equal (Some (Dictionnaire [ "abc"; "mot"; "dictionnaire" ]))
    finally
        File.Delete chemin
