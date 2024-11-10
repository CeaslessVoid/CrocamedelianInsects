using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CrocamedelianInsects
{
    public class Settings : ModSettings
    {
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.CrIDebugLogging, "CrIDebugLogging", this.CrIDebugLogging, true);


        }

        // Debug Logging
        public bool CrIDebugLogging = false;
    }

    internal class CrIMod : Mod
    {
        public CrIMod(ModContentPack content) : base(content)
        {
            this._settings = GetSettings<Settings>();
        }

        private Settings _settings;







        public override string SettingsCategory()
        {
            return "Crocamedelian's Insects";
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            LoadedModManager.GetMod<CrIMod>().GetSettings<Settings>().Write();
        }
    }
}
