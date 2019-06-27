using dnSpy.Contracts.App;
using dnSpy.Contracts.Menus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows.Input;

namespace dnSpy_Restart_as {
    abstract class RestartMenuCommandProvider : MenuItemBase, IMenuItemProvider {

        public abstract IEnumerable<CreatedMenuItem> Create(IMenuItemContext context);
        public override void Execute(IMenuItemContext context) => Debug.Fail("Shouldn't execute");

        protected CreatedMenuItem CreateHackyMenuItem(string text, Action<IMenuItemContext> action) {
            var attr = new ExportMenuItemAttribute() { Header = text };
            return new CreatedMenuItem(attr, new AbstractMenuCommand(action));
        }

        public static bool IsAdministrator() {
            using (var id = WindowsIdentity.GetCurrent())
                return new WindowsPrincipal(id).IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void RestartAs(object context, bool bit32, bool asAdmin) {
            var dnSpyLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"dnSpy{(bit32 ? "-x86" : "")}.exe");

            ProcessStartInfo startInfo = new ProcessStartInfo(dnSpyLocation);

            if (!File.Exists(startInfo.FileName)) {
                MsgBox.Instance.Show($"Could not find '{new FileInfo(startInfo.FileName).Name}' in the folder where dnSpy is located, can not restart as {(bit32 ? "32" : "64")}bit !");
                return;
            }

            // Close dnSpy (save all settings so dnSpy can open correctly again)
            ((ICommand)ApplicationCommands.Close).Execute(context);


            if (asAdmin) {
                startInfo.UseShellExecute = true;
                startInfo.Verb = "runas";
            }

            try {
                Process.Start(startInfo);
            } catch (Win32Exception) {
                // User clicked no on UAC prompt
                startInfo.Verb = "";
                Process.Start(startInfo);

            }
        }
    }
}
