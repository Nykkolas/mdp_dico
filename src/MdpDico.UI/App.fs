module App

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Layout

open Main

/// Application Avalonia
type MdpDicoApp() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add(FluentTheme())

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime-> 
            desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

/// Point d'entrée de l'application
[<EntryPoint>]
let main(args: string[]) =
    AppBuilder
        .Configure<MdpDicoApp>()
        .UsePlatformDetect()
        .UseSkia()
        .LogToTrace()
        .StartWithClassicDesktopLifetime(args)
