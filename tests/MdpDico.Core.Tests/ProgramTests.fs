module ProgramTests

open Xunit
open FsUnit.Xunit
open System
open Dictionnaire
open MotDePasse

/// Tests d'intégration pour le programme complet

[<Fact>]
let ``Intégration - générer un mot de passe complet fonctionne`` () =
    // Simule le workflow complet
    let dictionnaire = Dictionnaire [
        "abricot"; "avion"; "bateau"; "bouteille"; "camion";
        "champignon"; "chapeau"; "chaise"; "chien"; "dindon"
    ]
    
    let random = Random(123)
    let mdp = genererAvecTirage random.Next dictionnaire
    
    estValide mdp |> should be True
    valeur mdp |> should haveSubstring "-"
    
    // Vérifie que tous les mots sont dans le dictionnaire
    let mots = (valeur mdp).Split('-')
    let motsDictionnaire = obtenirMots dictionnaire
    
    for mot in mots do
        motsDictionnaire |> should contain mot

[<Fact>]
let ``Intégration - dictionnaireSecours fonctionne quand fichier manquant`` () =
    // Vérifie que le fallback fonctionne
    let dictionnaire = charger ()
    let mots = obtenirMots dictionnaire
    
    mots |> should not' (be Empty)
