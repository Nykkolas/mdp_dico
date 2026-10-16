# TODO - mdp_dico

Cases à cocher en Markdown : `[ ]` → `[x]` (cliquable aussi dans l'UI web GitHub).

## Bugs

- [ ] Icône cadenas mal affichée au lancement de l'appli (src/MdpDico.UI/Main.fs, bouton de visibilité)

## Sécurité

- [ ] Filtrer les mots contenant `-` ou une apostrophe dans `chargerDepuisFichier` (src/MdpDico.Core/Dictionnaire.fs) — casserait le format du mot de passe
- [ ] Effacer le presse-papiers à la fermeture de l'appli, ou l'assumer explicitement

## Fonctionnalités

- [ ] Fichier README.md (description, captures, build, usage)
- [ ] Télécharger un dictionnaire depuis le web
- [ ] Configuration d'un fichier local de dictionnaire et enregistrement du chemin
- [ ] Permettre de choisir le séparateur
- [ ] Permettre de choisir le nombre de mots

## Tests

- [ ] Helper `parcourir` + tests de séquences de `update` (workflows complets)
- [ ] Décider la sémantique « copier pendant une régénération » (update accepte aujourd'hui, l'UI refuse)

## Documentation

- [ ] Mettre à jour la section Clipboard d'AGENTS.md : l'API documentée est celle d'Avalonia 11, obsolète pour Avalonia 12 (`IClipboard` non implémentable par du code utilisateur)

## Nettoyage (audit)

- [ ] Coquille : `motsEnMiniscule` → `motsEnMinuscule` (tests/MdpDico.Core.Tests/DictionnaireTests.fs)
- [ ] `estValide` et `generer` ne sont utilisés que par les tests (src/MdpDico.Core/MotDePasse.fs) — à déplacer côté tests ou assumer
- [ ] Type et module `Dictionnaire` homonymes (src/MdpDico.Core/Dictionnaire.fs) — renommer le type ou assumer
- [ ] Scinder Main.fs s'il grandit (types/status, update, view, fenêtre)
