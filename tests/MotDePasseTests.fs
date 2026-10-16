module MotDePasseTests

open Xunit
open FsUnit.Xunit
open System
open Dictionnaire
open MotDePasse

/// Module de tests pour le module MotDePasse

[<Fact>]
let ``MotDePasse - valeur retourne la string interne`` () =
    let mdp = MotDePasse "test-password"
    
    valeur mdp |> should equal "test-password"

[<Fact>]
let ``MotDePasse - estValide retourne true pour un mot de passe avec 5 mots`` () =
    let mdp = MotDePasse "mot1-mot2-mot3-mot4-mot5"
    
    estValide mdp |> should be True

[<Fact>]
let ``MotDePasse - estValide retourne false pour un mot de passe avec moins de 5 mots`` () =
    let mdp = MotDePasse "mot1-mot2-mot3"
    
    estValide mdp |> should be False

[<Fact>]
let ``MotDePasse - estValide retourne false pour un mot de passe avec plus de 5 mots`` () =
    let mdp = MotDePasse "mot1-mot2-mot3-mot4-mot5-mot6"
    
    estValide mdp |> should be False

[<Fact>]
let ``MotDePasse - selectionnerMotsAleatoires retourne le bon nombre de mots`` () =
    let random = Random(42) // Graine fixe pour la reproductibilité
    let mots = ["a"; "b"; "c"; "d"; "e"; "f"; "g"; "h"; "i"; "j"]
    
    let selection = selectionnerMotsAleatoires random.Next mots 5
    
    List.length selection |> should equal 5

[<Fact>]
let ``MotDePasse - selectionnerMotsAleatoires retourne des mots de la liste`` () =
    let random = Random(42)
    let mots = ["a"; "b"; "c"; "d"; "e"]
    
    let selection = selectionnerMotsAleatoires random.Next mots 5
    
    for mot in selection do
        mots |> should contain mot

[<Fact>]
let ``MotDePasse - genererAvecTirage produit un mot de passe valide`` () =
    let random = Random(42)
    let dictionnaire = Dictionnaire ["a"; "b"; "c"; "d"; "e"; "f"; "g"]
    
    let mdp = genererAvecTirage random.Next dictionnaire
    
    estValide mdp |> should be True
    valeur mdp |> should haveSubstring "-"

[<Fact>]
let ``MotDePasse - genererAvecTirage utilise exactement 5 mots`` () =
    let random = Random(42)
    let dictionnaire = Dictionnaire ["a"; "b"; "c"; "d"; "e"; "f"]
    
    let mdp = genererAvecTirage random.Next dictionnaire
    let mots = (valeur mdp).Split('-')
    
    Array.length mots |> should equal 5

[<Fact>]
let ``MotDePasse - genererAvecTirage avec dictionnaireSecours produit un mot de passe valide`` () =
    let random = Random(42)
    let mdp = genererAvecTirage random.Next dictionnaireSecours
    
    estValide mdp |> should be True
    valeur mdp |> should not' (haveSubstring " ")

[<Fact>]
let ``MotDePasse - generer sans Random utilise un Random par défaut`` () =
    let dictionnaire = Dictionnaire ["a"; "b"; "c"; "d"; "e"; "f"]
    
    // Ne devrait pas lever d'exception
    let mdp = generer dictionnaire
    
    estValide mdp |> should be True
