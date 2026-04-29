using MFramework.Core;
using MFramework.Core.CoreEx;
using MFramework.Text;
using UnityEngine;

namespace MFramework.Examples.MText
{
    public sealed class MEntry : MEntryBase
    {
        [SerializeField] private MTextLocalizationTable localizationTable;
        [SerializeField] private string startLanguage = "zh";

        protected override IModule[] ConfigureModules()
        {
            return new IModule[]
            {
                new MTextModule(),
            };
        }

        protected override void OnInitialized(MFramework.Core.Tracker.TrackerStoppedEvent e)
        {
            MLocalizationManager manager = MLocalizationManager.Active;
            if (manager == null) return;

            if (localizationTable != null)
            {
                manager.SetTable(localizationTable);
            }
            else
            {
                manager.SetText("dialog.hello", "zh", "Hello, {wave}MText{/wave} now uses timeline driven animation.");
                manager.SetText("dialog.hello", "en", "Hello, {wave}MText{/wave} now uses timeline driven animation.");
                manager.SetText("dialog.warning", "zh", "{color=#ffcc00}Warning{/color}: finish typewriter instantly or switch language.");
                manager.SetText("dialog.warning", "en", "{color=#ffcc00}Warning{/color}: finish typewriter instantly or switch language.");
            }

            manager.SetLanguage(startLanguage);
        }
    }
}
