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
            Scribe_Values.Look<bool>(ref this.CrIDebugLogging,      "CrIDebugLogging",      this.CrIDebugLogging,      true);


            Scribe_Values.Look<int>(ref this.CrIMinHiveDistance,    "CrIMinHiveDistance",   this.CrIMinHiveDistance,   true);
            Scribe_Values.Look<int>(ref this.CrIMaxHiveDistance,    "CrIMaxHiveDistance",   this.CrIMaxHiveDistance,   true);
            Scribe_Values.Look<int>(ref this.CrINewHiveCost,        "CrINewHiveCost",       this.CrINewHiveCost,       true);
            Scribe_Values.Look<int>(ref this.CrINewHiveCostPlayer,  "CrINewHiveCostPlayer", this.CrINewHiveCostPlayer, true);


        }

        // Debug Logging
        public bool CrIDebugLogging      = true;

        // Hive Expansion
        public int  CrIMinHiveDistance   = 8;
        public int  CrIMaxHiveDistance   = 14;

        public int  CrINewHiveCost       = 20;
        public int  CrINewHiveCostPlayer = 200;
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
