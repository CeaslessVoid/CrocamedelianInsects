using LudeonTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CrocamedelianInsects
{
    public static class Debug
    {

        [DebugAction(null, null, false, false, false, false, 0, false, category = "Crocamedelian Insects", name = "Do Shit", requiresRoyalty = false, requiresIdeology = false, requiresBiotech = false, actionType = 0, allowedGameStates = LudeonTK.AllowedGameStates.Playing)]
        private static void DoShit() // Prints current CrE points
        {

        }
        private const string CATEGORY = "Crocamedelian Insects";
    }

}
