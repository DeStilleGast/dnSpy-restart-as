using dnSpy.Contracts.Extension;
using System.Collections.Generic;

namespace dnSpy_Restart_as {

    [ExportExtension]
    public class ExtensionLoader : IExtension {
        public ExtensionInfo ExtensionInfo => new ExtensionInfo() {
            ShortDescription = "Ability to restart as other bit version.",
            Copyright = "DeStilleGast"
        };

        public IEnumerable<string> MergedResourceDictionaries {
            get {
                yield break;
            }
        }

        public void OnEvent(ExtensionEvent @event, object obj) { }
    }
}
