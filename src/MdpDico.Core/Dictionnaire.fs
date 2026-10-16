module Dictionnaire

open System
open System.IO

/// Chemin vers le dictionnaire français par défaut
let cheminDefaut = "/usr/share/dict/french"

/// Type représentant un dictionnaire de mots
type Dictionnaire = 
    | Dictionnaire of string list

/// Charge les mots depuis un fichier
let chargerDepuisFichier (chemin: string) : Dictionnaire option =
    if File.Exists(chemin) then
        File.ReadAllLines(chemin)
        |> Array.filter (fun mot -> String.length mot > 2)
        |> Array.toList
        |> fun mots -> Some (Dictionnaire mots)
    else
        None

/// Dictionnaire de secours avec des mots français
let dictionnaireSecours : Dictionnaire = 
    [
        "abricot"; "avion"; "bateau"; "bibliothque"; "bouteille";
        "camion"; "champignon"; "chapeau"; "chaise"; "chien";
        "dindon"; "drap"; "ecole"; "enfant"; "epingle"; "escalier";
        "fleur"; "foret"; "garage"; "girafe"; "glace"; "gomme";
        "haricot"; "herisson"; "ile"; "immeuble"; "jardin";
        "kilo"; "lampadaire"; "livre"; "maison"; "montagne";
        "nuage"; "oiseau"; "ordinateur"; "papiers"; "parapluie";
        "quetche"; "robe"; "sac"; "tapis"; "valise"; "voiture";
        "xylophone"; "zebre"
    ]
    |> Dictionnaire

/// Charge le dictionnaire (essaye le fichier, sinon utilise le secours)
let charger () : Dictionnaire =
    match chargerDepuisFichier cheminDefaut with
    | Some dictionnaire -> dictionnaire
    | None -> dictionnaireSecours

/// Extrait la liste de mots d'un dictionnaire
let obtenirMots (Dictionnaire mots) = mots
