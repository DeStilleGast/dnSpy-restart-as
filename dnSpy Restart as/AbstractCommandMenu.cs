using dnSpy.Contracts.Menus;
using System;

namespace dnSpy_Restart_as {
    class AbstractMenuCommand : MenuItemBase {

        private Action<IMenuItemContext> customAction;

        public AbstractMenuCommand(Action<IMenuItemContext> customAction) {
            this.customAction = customAction ?? throw new ArgumentNullException(nameof(customAction));
        }

        public override void Execute(IMenuItemContext context) {
            customAction.Invoke(context);
        }
    }
}
