using dnSpy.Contracts.Extension;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace dnSpy_Restart_as {
    [ExportAutoLoaded]
    class RestartButtonInAttachmentDialogHack : IAutoLoaded {

        public RestartButtonInAttachmentDialogHack() {
            // Register a route to add the restart button to the correct dialog
            EventManager.RegisterClassHandler(typeof(Window), Window.LoadedEvent, new RoutedEventHandler(AddRestartButton));
        }

        private void AddRestartButton(object sender, RoutedEventArgs args) {

            if (sender.GetType().ToString() == "dnSpy.Debugger.Dialogs.AttachToProcess.AttachToProcessDlg" && sender is Window window) {
                var pbObj = window.FindName("progressBar");
                if (pbObj is ProgressBar pb) {
                    var parent = pb.Parent as Grid;

                    // Add new colmn
                    parent.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                    // Shift things up
                    for (var i = 1; i < parent.Children.Count; i++) {
                        Grid.SetColumn(parent.Children[i], i++);
                    }

                    var ctx = new ContextMenu();
                    var is64bit = IntPtr.Size == 8; // check if we are in 64bit

                    if (!RestartMenuCommandProvider.IsAdministrator()) {
                        if (is64bit) {
                            ctx.Items.Add(new MenuItem() { Header = "32 bit", Command = new RestartCommand(window, true, false) });
                        } else {
                            ctx.Items.Add(new MenuItem() { Header = "64 bit", Command = new RestartCommand(window, false, false) });
                        }
                    }

                    ctx.Items.Add(new Separator());

                    ctx.Items.Add(new MenuItem() { Header = "32 bit (Administrator)", Command = new RestartCommand(window, true, true) });
                    ctx.Items.Add(new MenuItem() { Header = "64 bit (Administrator)", Command = new RestartCommand(window, true, true) });




                    var restartBtn = new Button() { Content = "Restart dnSpy as", Margin = new Thickness(5, 0, 0, 0), Padding = new Thickness(5, 0, 5, 0) };
                    Grid.SetColumn(restartBtn, 2);
                    restartBtn.ContextMenu = ctx;
                    restartBtn.Click += (s, e) => ctx.IsOpen = true; // Open context menu
                    parent.Children.Add(restartBtn);

                }
            }
        }
    }

    class RestartCommand : ICommand {

        private bool bit32, asAdmin;
        private Window window;

        public RestartCommand(Window window, bool bit32, bool asAdmin) {
            this.window = window;
            this.bit32 = bit32;
            this.asAdmin = asAdmin;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter) {
            window.DialogResult = false;
            window.Close();

            RestartMenuCommandProvider.RestartAs(parameter, bit32, asAdmin);
        }
    }
}
