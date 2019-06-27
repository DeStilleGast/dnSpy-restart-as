using dnSpy.Contracts.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace dnSpy_Restart_as {
    static class FileSubMenuItem {
        static class MainMenuConstants {

            // Restart item in File menu
            public const string GROUP_RESTART_SUBMENU = "1050,EFB890F6-1575-4D00-8E78-8C674995711D";

            // Restart sub menu items
            public const string ITEM_APP_MENU_RESTART = "E42068D2-C0A7-4E07-9650-780C7B2AB336";

            // Restart without admin
            public const string GROUP_RESTART_MENU = "0,BC72617A-72E8-4962-91BB-AF497A84A6D1";

            // Restart with admin
            public const string GROUP_RESTART_ADMIN_MENU = "1,1693E15E-F9E9-4A0B-B158-FF2FDFA3A39E";
        }

        // OwnerGuild: Place in File context menu
        // Guid: ID for menu item
        // Header: Text
        // Group: to seperate different command groups
        [ExportMenuItem(OwnerGuid = MenuConstants.APP_MENU_FILE_GUID, Guid = MainMenuConstants.ITEM_APP_MENU_RESTART, Header = "R_estart as", Order = 145, Group = MainMenuConstants.GROUP_RESTART_SUBMENU)]
        sealed class RestartCommand : MenuItemBase {
            public override void Execute(IMenuItemContext context) => Debug.Fail("Shouldn't execute");
        }

        // OwnerGuild: point to RestartCommand
        // Group: non admin
        [ExportMenuItem(OwnerGuid = MainMenuConstants.ITEM_APP_MENU_RESTART, Group = MainMenuConstants.GROUP_RESTART_MENU, Order = 10)]
        sealed class RestartMenuCommand : RestartMenuCommandProvider {
            public override IEnumerable<CreatedMenuItem> Create(IMenuItemContext context) {
                var is64bit = IntPtr.Size == 8; // check if we are in 64bit
                var isAdmin = IsAdministrator();

                // So far I know, it is not posible to run as normal user if dnSpy is running as admin
                if (!isAdmin) {
                    if (is64bit) {
                        yield return CreateHackyMenuItem("Restart dnSpy as 32-bit", c => RestartAs(c, true, false));
                    } else {
                        yield return CreateHackyMenuItem("Restart dnSpy as 64-bit", c => RestartAs(c, false, false));
                    }
                }
            }
        }

        // OwnerGuild: point to RestartCommand
        // Group: admin
        [ExportMenuItem(OwnerGuid = MainMenuConstants.ITEM_APP_MENU_RESTART, Group = MainMenuConstants.GROUP_RESTART_ADMIN_MENU, Order = 20)]
        sealed class RestartAdminMenuCommand : RestartMenuCommandProvider {
            public override IEnumerable<CreatedMenuItem> Create(IMenuItemContext context) {
                yield return CreateHackyMenuItem("Restart dnSpy as 32-bit (Administrator)", c => RestartAs(c, true, true));
                yield return CreateHackyMenuItem("Restart dnSpy as 64-bit (Administrator)", c => RestartAs(c, false, true));
            }
        }
    }
}
