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

        protected bool IsAdministrator() {
            using (var id = WindowsIdentity.GetCurrent())
                return new WindowsPrincipal(id).IsInRole(WindowsBuiltInRole.Administrator);
        }

        protected void RestartAs(IMenuItemContext context, bool bit32, bool asAdmin) {
            // Close dnSpy (save all settings so dnSpy can open correctly again)
            ((ICommand)ApplicationCommands.Close).Execute(context);

            ProcessStartInfo startInfo;

            if (bit32) {
                startInfo = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dnSpy-x86.exe"));
            } else {
                startInfo = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dnSpy.exe"));
            }
;
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
