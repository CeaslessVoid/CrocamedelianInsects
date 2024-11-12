using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CrocamedelianInsects
{
    internal class CrIGameComponent : GameComponent
    {
        // Load Settings
        public static Settings Settings { get; private set; }
        public CrIGameComponent(Game game)
        {
            Settings = LoadedModManager.GetMod<CrIMod>().GetSettings<Settings>();
        }

    }


}
