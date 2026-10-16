# AGENTS.md - Règles et meilleures pratiques pour ce projet

## 🤝 Mode de collaboration

### "Propose-moi" = proposer, ne pas exécuter
- Quand l'utilisateur demande de **proposer** (améliorations, changements, refactor), NE PAS modifier les fichiers.
- Expliquer d'abord ce qui est prévu : fichiers concernés, commandes à exécuter, risques.
- Attendre l'**autorisation explicite** de l'utilisateur avant d'appliquer quoi que ce soit.
- Les demandes directes ("fais", "corrige", "ajoute") restent des autorisations à agir immédiatement.

### "Crée une version x.y.z"
- Équivaut à : `git tag vx.y.z` + `git push origin vx.y.z` (autorisation explicite incluse).
- Le workflow CI (`.github/workflows/ci.yml`) exécute les tests puis publie les binaires des 3 plateformes en GitHub Release.
- Après le push du tag, vérifier le run et la Release obtenue, et rapporter le résultat.

## 📚 Documentation et Ressources

- **Clipboard Avalonia** : [https://docs.avaloniaui.net/docs/services/clipboard#writing](https://docs.avaloniaui.net/docs/services/clipboard#writing)
- **Async Commands in Avalonia.FuncUI** : [https://brandewinder.com/2025/02/19/async-commands-in-avalonia-funcui/](https://brandewinder.com/2025/02/19/async-commands-in-avalonia-funcui/)

---

## F# / Elmish

### Pattern Elmish
- **State** : Toujours immutable. Utiliser `with` pour les mises à jour.
- **Msg** : Utiliser des Discriminated Unions pour les messages.
- **init** : Doit retourner `State * Cmd<Msg>`.
- **update** : Doit retourner `State * Cmd<Msg>`. Gérer tous les cas de Msg.

### Commandes asynchrones
- **Avec gestion d'erreur** : Utiliser `Cmd.OfAsync.either`
  ```fsharp
  Cmd.OfAsync.either
      asyncFn
      ()
      (fun () -> SuccessMsg)
      (fun ex -> FailedMsg ex.Message)
  ```
- **Sans gestion d'erreur** : Utiliser `Cmd.OfAsync.perform`
  ```fsharp
  Cmd.OfAsync.perform
      asyncFn
      SuccessMsg
      (fun ex -> FailedMsg ex.Message)
  ```

### Random
- **Ne pas recréer** `System.Random()` à chaque appel. Le stocker dans le State et le réutiliser.

---

## Avalonia.FuncUI

### Structure DSL
- **StackPanel** : Toujours utiliser `StackPanel.children` pour les éléments enfants.
  ```fsharp
  StackPanel.create [
      StackPanel.margin (Thickness 20.0)
      StackPanel.children [
          Button.create [...]
          TextBlock.create [...]
      ]
  ]
  ```

### Syntaxe des types géométriques
- **Toujours utiliser des parenthèses** :
  - ✅ `Thickness(12.0, 8.0)`
  - ❌ `Thickness 12.0, 8.0`
  - ✅ `CornerRadius(4.0)`
  - ❌ `CornerRadius 4.0`

### Lists
- **Ne pas utiliser `yield`/`yield!`** dans des listes simples. Utiliser `@` pour fusionner :
  ```fsharp
  // ✅ Bon
  Button.create (commonStyle @ [Button.content "OK"])
  
  // ❌ Éviter
  Button.create [ yield! commonStyle; Button.content "OK" ]
  ```

### Contrôles
- **TextBox** : Pour du texte sélectionnable. Utiliser `isReadOnly true` pour éviter les modifications.
- **TextBlock** : Pour du texte statique.
- **Button.content** : Peut contenir du texte ou des emojis ("📋 Copier", "🔄 Régénérer", "👁️", "🔒").

---

## UI/UX

### Feedback utilisateur
- **Statuts** : Un seul champ `Status` dans le State pour tous les messages.
- **Couleurs** : 
  - Vert (`Brushes.Green`) pour les succès
  - Rouge (`Brushes.Red`) pour les échecs
  - Noir par défaut
- **Préfixe succès** : Utiliser `✓ ` (checkmark + espace) devant les messages de succès.

### Disposition
- **Boutons et statuts** : Regrouper chaque bouton avec son statut dans un `StackPanel` horizontal.
- **Espacement** : Utiliser `StackPanel.spacing` pour l'espacement entre les enfants.

---

## Avalonia Spécifique

### Clipboard
- **Threading** : Toujours utiliser `Async.SwitchToContext (System.Threading.SynchronizationContext.Current)` avant d'accéder au clipboard depuis un thread non-UI.
- **Récupération** : 
  ```fsharp
  let clipboard =
      TopLevel.GetTopLevel(e.Source :?> Control)
      |> Option.ofObj
      |> Option.bind (fun topLevel -> Option.ofObj topLevel.Clipboard)
  ```
- **Écrire** : Utiliser `clipboard.SetTextAsync(text)`. Préférer à `DataObject`/`DataTransfer`.

### Fenêtre
- **Taille** : Définir `Width` et `Height` dans `MainWindow`.
- **Position** : `WindowStartupLocation.CenterScreen` pour centrer.

---

## Bonnes pratiques générales

### DRY
- Éviter la répétition de code. Extraire les conditions répétées en fonctions helpers.
- Exemple : Si `state.Status = "Copié !" || state.Status = "Mot de passe généré !"` est utilisé plusieurs fois, créer une fonction `isSuccessStatus`.

### Strings magiques
- Éviter les strings littérales dans le code. Préférer des constantes ou des types DU.

### Nommage
- **Messages** : Utiliser des noms explicites (`CopyRequested`, `CopyFinished`, `CopyFailed`).
- **State** : Utiliser des noms clairs (`IsCopying`, `IsRegenerating`, `Status`).
- **Fonctions** : Utiliser des verbes (`copyText`, `regeneratePassword`).

---

## Erreurs à ne pas reproduire

1. ❌ Oublier `StackPanel.children` → Erreur : `IView` incompatible avec `IAttr`
2. ❌ `Thickness 12.0, 8.0` sans parenthèses → Erreur de compilation
3. ❌ `CornerRadius 4.0` sans parenthèses → Erreur de compilation
4. ❌ Utiliser `yield` dans une liste simple → Comportement inattendu
5. ❌ Appeler le clipboard depuis un thread non-UI → `InvalidOperationException`
6. ❌ Recréer `System.Random()` à chaque appel → Moins aléatoire et moins performant

---

## Programme Elmish

- Utiliser `Program.mkProgram` quand `init` et `update` retournent `State * Cmd<Msg>`.
- Utiliser `Program.mkSimple` seulement si pas de Cmd.
- Toujours utiliser `Program.withHost this` et `Program.runWithAvaloniaSyncDispatch ()` pour Avalonia.
