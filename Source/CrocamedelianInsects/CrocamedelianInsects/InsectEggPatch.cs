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
    [HarmonyPatch(typeof(PregnancyHelper), "DoEgg")]
    public static class Patch_PregnancyHelper_DoEgg
    {
        private const float InfectionChance = 1.0f;
        private const float ChanceToBroken = 0.2f;
        static bool Prefix(SexProps props)
        {

            if (Rand.Chance(InfectionChance) && !xxx.is_insect(props.partner))
            { 
                BodyPartRecord targetBodyPart       = Genital_Helper.get_genitalsBPR(props.pawn);
                Predicate<Hediff> filterByGenitalia = hediff => hediff.Part == targetBodyPart;
                BodyPartRecord partnerGenitals      = null;
                HediffDef swarmingInfectionDef      = HediffDef.Named("SwarmingInfection");
                HediffDef insectBrokenDef           = HediffDef.Named("InsectBroken");

                if (props.sexType == xxx.rjwSextype.Anal)
                    partnerGenitals = Genital_Helper.get_anusBPR(props.partner);
                else if (props.sexType == xxx.rjwSextype.Oral)
                    partnerGenitals = Genital_Helper.get_stomachBPR(props.partner);
                else if (props.sexType == xxx.rjwSextype.DoublePenetration && Rand.Value > 0.5f && RJWPregnancySettings.insect_anal_pregnancy_enabled)
                    partnerGenitals = Genital_Helper.get_anusBPR(props.partner);
                else
                    partnerGenitals = Genital_Helper.get_genitalsBPR(props.partner);

                Hediff existingInfection = props.partner.health.hediffSet.GetFirstHediffOfDef(swarmingInfectionDef);
                if (existingInfection == null)
                {
                    Hediff newInfection = HediffMaker.MakeHediff(swarmingInfectionDef, props.partner, partnerGenitals);
                    props.partner.health.AddHediff(newInfection);
                }

                if (props.sexType == xxx.rjwSextype.Vaginal && Rand.Chance(ChanceToBroken) && !props.partner.health.hediffSet.HasHediff(insectBrokenDef))
                {
                    Hediff newHediff = HediffMaker.MakeHediff(insectBrokenDef, props.partner);
                    props.partner.health.AddHediff(newHediff);
                }
            }

            return true;
        }
    }

}
