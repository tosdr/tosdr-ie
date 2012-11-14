module TOSTrayIcon

open System
open System.Drawing
open System.Windows
open System.Windows.Controls
open System.ServiceModel
open TOSCheckerTrayApp.Model
open TOSCheckerTrayApp.ViewModel
open TOSCheckerService

type ContextMenu = Forms.ContextMenu
type MenuItem = Forms.MenuItem
type NotifyIcon = Forms.NotifyIcon

let toggleWindowVisibility (window : Window) (e : Forms.MouseEventArgs) =
    match window.Visibility with
    | Visibility.Visible -> window.Hide()
    | _ when DateTime.Now.Ticks - (window.DataContext :?> MainWindowViewModel).ClosedTimestamp > 2000000L -> 
        window.Show()
        window.Activate() |> ignore
    | _ -> ()

let createTrayIconContextMenu (app : Application) =
    let menu = new ContextMenu()
    let exit = new MenuItem(Index = 0, Text = "C&lose")
    menu.MenuItems.AddRange [| exit |]
    exit.Click.Add(fun _ -> app.Shutdown())
    menu

let createTrayIcon (app : Application) (icon : IO.Stream) =
    let trayIcon = 
        new NotifyIcon
            (Visible = true, Text = "Terms of Service; Didn't Read",
             Icon = new Icon(icon, 16, 16), ContextMenu = createTrayIconContextMenu app)
    trayIcon.MouseClick.Add(fun e -> toggleWindowVisibility app.MainWindow e)
    app.Exit.Add(fun _ -> trayIcon.Dispose())
    trayIcon

let adaptRatingIcon (trayIcon : NotifyIcon) (rating : TosRating option) =
    ()
