module MotDePasse

open System
open System.Security.Cryptography
open Dictionnaire

/// Type représentant un mot de passe généré
type MotDePasse = 
    | MotDePasse of string

/// Nombre de mots dans le mot de passe
let nombreDeMots = 5

/// Sélectionne des mots aléatoires via une fonction de tirage d'index (injectée)
let selectionnerMotsAleatoires (tirerIndex: int -> int) (mots: string list) (nombre: int) : string list =
    List.init nombre (fun _ ->
        let index = tirerIndex (List.length mots)
        List.item index mots
    )

/// Génère un mot de passe à partir d'un dictionnaire avec un tirage spécifié
/// (utilisé pour les tests)
let genererAvecTirage (tirerIndex: int -> int) (dictionnaire: Dictionnaire) : MotDePasse =
    let mots = obtenirMots dictionnaire
    let motsSelectionnes = selectionnerMotsAleatoires tirerIndex mots nombreDeMots
    String.Join("-", motsSelectionnes)
    |> MotDePasse

/// Génère un mot de passe à partir d'un dictionnaire avec le générateur crypto-sécurisé
let generer (dictionnaire: Dictionnaire) : MotDePasse =
    genererAvecTirage RandomNumberGenerator.GetInt32 dictionnaire

/// Convertit un MotDePasse en string
let valeur (MotDePasse mdp) = mdp

/// Vérifie qu'un mot de passe contient exactement le nombre de mots attendu séparés par des tirets
let estValide (MotDePasse mdp) =
    mdp.Split('-') |> Array.length = nombreDeMots
