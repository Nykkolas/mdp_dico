module MotDePasse

open System
open Dictionnaire

/// Type représentant un mot de passe généré
type MotDePasse = 
    | MotDePasse of string

/// Nombre de mots dans le mot de passe
let nombreDeMots = 5

/// Sélectionne des mots aléatoires depuis une liste en utilisant un générateur aléatoire
let selectionnerMotsAleatoires (random: Random) (mots: string list) (nombre: int) : string list =
    List.init nombre (fun _ ->
        let index = random.Next(0, List.length mots)
        List.item index mots
    )

/// Génère un mot de passe à partir d'un dictionnaire avec un générateur aléatoire spécifié
/// (utilisé pour les tests)
let genererAvecRandom (random: Random) (dictionnaire: Dictionnaire) : MotDePasse =
    let mots = obtenirMots dictionnaire
    let motsSelectionnes = selectionnerMotsAleatoires random mots nombreDeMots
    String.Join("-", motsSelectionnes)
    |> MotDePasse

/// Génère un mot de passe à partir d'un dictionnaire avec un générateur aléatoire par défaut
let generer (dictionnaire: Dictionnaire) : MotDePasse =
    genererAvecRandom (Random()) dictionnaire

/// Convertit un MotDePasse en string
let valeur (MotDePasse mdp) = mdp

/// Vérifie qu'un mot de passe contient exactement le nombre de mots attendu séparés par des tirets
let estValide (MotDePasse mdp) =
    mdp.Split('-') |> Array.length = nombreDeMots
