using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rjw;
using HarmonyLib;
using Verse;


namespace CrocamedelianInsects
{

    [HarmonyPatch(typeof(Cocoon), "HealWounds")]
    public static class Patch_Cocoon_HealWounds
        // Increase the healing as Pawns died to quickly
    {
        public static bool Prefix(Cocoon __instance)
        {
            IEnumerable<Hediff> untreatedWounds = __instance.pawn.health.hediffSet.hediffs
                .Where(hd => !hd.IsTended() && hd.TendableNow());


            if (CrIGameComponent.Settings.CrIDebugLogging)
            {
                Util.Msg("Tried Healing Pawn");
            }

            foreach (Hediff wound in untreatedWounds)
            {
                HediffWithComps woundComp = wound as HediffWithComps;

                if (woundComp != null)
                {
                    if (woundComp.Bleeding)
                    {
                        woundComp.Heal(1.5f);
                    }
                    else if ((!woundComp.def.chronic && woundComp.def.lethalSeverity > 0f) ||
                             (woundComp.CurStage?.lifeThreatening ?? false))
                    {
                        HediffComp_TendDuration tendComp = HediffUtility.TryGetComp<HediffComp_TendDuration>(woundComp);
                        if (tendComp != null)
                        {
                            tendComp.tendQuality = 1f;
                            tendComp.tendTicksLeft = 10000;
                            __instance.pawn.health.Notify_HediffChanged(wound);
                        }
                    }
                }
            }

            return false;
        }
    }
}
