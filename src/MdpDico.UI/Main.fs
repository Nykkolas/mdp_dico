module Main

open System
open Elmish
open Avalonia
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.Input.Platform
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Hosts
open Avalonia.FuncUI.Elmish
open Avalonia.Input
open Avalonia.Media

open Dictionnaire
open MotDePasse

// Status messages type-safe
type StatusMessage =
    | Initial
    | ClipboardUnavailable
    | Copying
    | Copied
    | CopyFailed of string
    | Regenerating
    | Generated
    | RegenerateFailed of string

let formatStatus = function
    | Initial -> "Génération initiale..."
    | ClipboardUnavailable -> "Presse-papiers indisponible."
    | Copying -> "Copie en cours..."
    | Copied -> "✓ Copié !"
    | CopyFailed error -> "Échec : " + error
    | Regenerating -> "Génération en cours..."
    | Generated -> "✓ Mot de passe généré !"
    | RegenerateFailed error -> "Échec : " + error

let formatStatusColor = function
    | Copied | Generated -> Brushes.Green
    | CopyFailed _ | RegenerateFailed _ -> Brushes.Red
    | _ -> Brushes.Black

type State = 
    { Password: string
      IsPasswordVisible: bool
      IsCopying: bool
      IsRegenerating: bool
      Status: StatusMessage }

type Msg =
    | TogglePasswordVisibility
    | CopyRequested of clipboard: IClipboard option
    | CopyFinished
    | CopyFailed of error: string
    | RegenerateRequested
    | RegenerateFinished of password: string
    | RegenerateFailed of error: string

let copyText (clipboard: IClipboard) (text: string) =
    async {
        do! clipboard.SetTextAsync(text) |> Async.AwaitTask
    }

let regeneratePassword () =
    async {
        let dictionnaire = charger ()
        return valeur (generer dictionnaire)
    }

let init () : State * Cmd<Msg> =
    let cmd = 
        Cmd.OfAsync.either 
            regeneratePassword 
            ()
            RegenerateFinished 
            (fun ex -> RegenerateFailed ex.Message)
    { Password = ""
      IsPasswordVisible = false
      IsCopying = false
      IsRegenerating = true
      Status = Initial },
    cmd

let update (msg: Msg) (state: State) : State * Cmd<Msg> =
    match msg with
    | TogglePasswordVisibility ->
        { state with IsPasswordVisible = not state.IsPasswordVisible }, Cmd.none
    | CopyRequested None ->
        { state with IsCopying = false; Status = ClipboardUnavailable }, Cmd.none
    | CopyRequested (Some clipboard) ->
        let cmd =
            Cmd.OfAsync.either
                (fun t -> copyText clipboard t)
                state.Password
                (fun () -> CopyFinished)
                (fun ex -> CopyFailed ex.Message)
        { state with IsCopying = true; Status = Copying }, cmd
    | CopyFinished ->
        { state with IsCopying = false; Status = Copied }, Cmd.none
    | CopyFailed error ->
        { state with IsCopying = false; Status = StatusMessage.CopyFailed error }, Cmd.none
    | RegenerateRequested ->
        { state with IsRegenerating = true; Status = Regenerating },
        Cmd.OfAsync.either
            regeneratePassword 
            () 
            RegenerateFinished 
            (fun ex -> RegenerateFailed ex.Message)
    | RegenerateFinished password ->
        { state with Password = password; IsRegenerating = false; Status = Generated }, Cmd.none
    | RegenerateFailed error ->
        { state with IsRegenerating = false; Status = StatusMessage.RegenerateFailed error }, Cmd.none



let view (state: State) (dispatch: Msg -> unit) =
    StackPanel.create [
        StackPanel.margin (Thickness 20.0)
        StackPanel.spacing 20.0
        StackPanel.children [
            // Mot de passe
            StackPanel.create [
                StackPanel.orientation Orientation.Horizontal
                StackPanel.horizontalAlignment HorizontalAlignment.Center
                StackPanel.spacing 10.0
                StackPanel.children [
                    Button.create [
                        Button.content (if state.IsPasswordVisible then "👁️" else "🔒")
                        Button.onClick (fun _ -> dispatch TogglePasswordVisibility)
                    ]
                    
                    TextBox.create [
                        TextBox.text (if state.IsPasswordVisible then state.Password else String.replicate state.Password.Length "•")
                        TextBox.fontSize 24.0
                        TextBox.width 300.0
                        TextBox.isReadOnly true
                        TextBox.fontFamily "Consolas"
                        TextBox.textAlignment TextAlignment.Center
                    ]
                ]
            ]
            
            // Boutons
            StackPanel.create [
                StackPanel.orientation Orientation.Horizontal
                StackPanel.horizontalAlignment HorizontalAlignment.Center
                StackPanel.spacing 20.0
                StackPanel.children [
                    Button.create [
                        Button.content "📋 Copier"
                        Button.isEnabled (not state.IsCopying && not state.IsRegenerating)
                        Button.onClick (fun e ->
                            let clipboard =
                                TopLevel.GetTopLevel(e.Source :?> Control)
                                |> Option.ofObj
                                |> Option.bind (fun topLevel -> Option.ofObj topLevel.Clipboard)
                            dispatch (CopyRequested clipboard)
                        )
                    ]
                    
                    Button.create [
                        Button.content "🔄 Régénérer"
                        Button.isEnabled (not state.IsCopying && not state.IsRegenerating)
                        Button.onClick (fun _ -> dispatch RegenerateRequested)
                    ]
                ]
            ]
            
            // Status
            TextBlock.create [
                TextBlock.text (formatStatus state.Status)
                TextBlock.fontSize 14.0
                TextBlock.horizontalAlignment HorizontalAlignment.Center
                TextBlock.foreground (formatStatusColor state.Status)
            ]
        ]
    ]

type MainWindow() as this =
    inherit HostWindow()

    do
        base.Title <- "Mot de passe généré"
        base.Width <- 500.0
        base.Height <- 250.0
        base.WindowStartupLocation <- WindowStartupLocation.CenterScreen

        Program.mkProgram init update view
        |> Program.withHost this
        |> Program.withConsoleTrace
        |> Program.runWithAvaloniaSyncDispatch ()