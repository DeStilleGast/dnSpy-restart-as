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
            bool? isUsingNetCore = Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<TargetFrameworkAttribute>()?
                .FrameworkName.StartsWith(".NETCoreApp");

            var dnSpyLocation;
            var dnSpyFilename;
            var dnSpyFolder;
            if (isUsingNetCore == true)
            {
                /*
                 * Expect a fixed folder structure. Note that due to 0xd4d custom 
                 * patch the executable is always in the parent folder in .NET Core
                 *  --
                 *  --x86--dnSpy.exe
                 *  --x64--dnSpy.exe
                */
                dnSpyFolder = Path.Combine(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, $"{(bit32 ? "x86" : "x64")}");
                dnSpyFilename = "dnSpy.exe";
                dnSpyLocation = Path.Combine(dnSpyFolder, dnSpyFilename);
            }
            else
            {
                dnSpyFolder = AppDomain.CurrentDomain.BaseDirectory;
                dnSpyFilename = $"dnSpy{(bit32 ? "-x86" : "")}.exe";
                dnSpyLocation = Path.Combine(dnSpyFolder, dnSpyFilename);
            }

            if (!File.Exists(dnSpyLocation)) {
                MsgBox.Instance.Show($"Could not find '{dnSpyFilename}' in folder '{dnSpyFolder}', can not restart as {(bit32 ? "32" : "64")}bit !");
                return;
            }

            // Close dnSpy (save all settings so dnSpy can open correctly again)
            ((ICommand)ApplicationCommands.Close).Execute(context);

            var startInfo = new ProcessStartInfo(dnSpyLocation);

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
