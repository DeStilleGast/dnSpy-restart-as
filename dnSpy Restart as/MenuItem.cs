using dnSpy.Contracts.Menus;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace dnSpy_Restart_as {
    static class MenuItem {
        static class MainMenuConstants {
            public const string APP_MENU_EXTENSION = "D5B02DE2-E1C7-482A-BFFA-4356A44954D5";
            public const string RESTART_MENU_32 = "0,BC72617A-72E8-4962-91BB-AF497A84A6D1";
        }

        [ExportMenu(OwnerGuid = MenuConstants.APP_MENU_GUID, Guid = MainMenuConstants.APP_MENU_EXTENSION, Order = MenuConstants.ORDER_APP_MENU_DEBUG + 0.1, Header = "_Restart dnSpy as")]
        sealed class RestartMenu : IMenu {
        }

        [ExportMenuItem(OwnerGuid = MainMenuConstants.APP_MENU_EXTENSION, Header = "Restart as 32 bit", Group = MainMenuConstants.RESTART_MENU_32, Order = 0)]
        sealed class Restart32Command : MenuItemBase {


            public override void Execute(IMenuItemContext context) {
                ((ICommand)ApplicationCommands.Close).Execute(context);

                Process.Start($"{AppDomain.CurrentDomain.BaseDirectory}\\dnSpy-x86.exe");
            }

            public override bool IsVisible(IMenuItemContext context) {
                return IntPtr.Size == 8;
            }
        }

        [ExportMenuItem(OwnerGuid = MainMenuConstants.APP_MENU_EXTENSION, Header = "Restart as 64 bit", Group = MainMenuConstants.RESTART_MENU_32, Order = 1)]
        sealed class Restart64Command : MenuItemBase {

            public override void Execute(IMenuItemContext context) {
                ((ICommand)ApplicationCommands.Close).Execute(context);

                Process.Start($"{AppDomain.CurrentDomain.BaseDirectory}\\dnSpy.exe");
            }

            public override bool IsVisible(IMenuItemContext context) {
                return IntPtr.Size == 4;
            }
        }
    }
}
