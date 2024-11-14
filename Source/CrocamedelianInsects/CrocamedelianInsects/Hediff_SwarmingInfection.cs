using RimWorld;
using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CrocamedelianInsects
{
    public class Hediff_SwarmingInfection : HediffWithComps
    {
        private const float SpreadThresholdSeverity = 0.5f;
        private const float SeverityReductionOnSpread = 0.2f;
        private const float SpreadChance = 0.2f;
        private const int   SpreadTicks = 60000;
        private const float SeverityThreshold = 0.4f;
        private const float ChanceToBroken = 0.2f;

        private static readonly List<string> TargetBodyParts = new List<string>
        {
            "Liver",
            "Stomach",
            "Brain",
            "Genitals",
            "Anus",
            "Chest"
        };


        public override void Tick()
        {
            base.Tick();
            if (this.pawn.IsHashIntervalTick(1000))
            {

            }

            if (this.pawn.IsHashIntervalTick(SpreadTicks) && this.Severity >= SpreadThresholdSeverity)
            {
                TrySpreadToNewBodyPart();
            }

            if (this.pawn.IsHashIntervalTick(60000) && this.Severity >= 0.33)
            {
                ApplyAphrodisiacHediff();
                ApplySwarmInfectionThought();
                SpawnMaggots();

                if (this.Part.def.defName == "Genitals" && pawn.gender == Gender.Female && Rand.Chance(ChanceToBroken) && this.Severity >= SeverityThreshold)
                {
                    BreakWomb();
                }
            }
        }

        private void TrySpreadToNewBodyPart()
        {
            if (Rand.Chance(SpreadChance))
            {
                List<BodyPartRecord> availableParts = new List<BodyPartRecord>();

                foreach (var part in pawn.health.hediffSet.GetNotMissingParts())
                {
                    if (TargetBodyParts.Contains(part.def.defName))
                    {
                        bool alreadyInfected = false;
                        foreach (var hediff in pawn.health.hediffSet.hediffs)
                        {
                            if (hediff.def == this.def && hediff.Part == part)
                            {
                                alreadyInfected = true;
                                break;
                            }
                        }

                        if (!alreadyInfected)
                        {
                            availableParts.Add(part);
                        }
                    }
                }

                if (availableParts.Count > 0)
                {

                    BodyPartRecord newPart = availableParts.RandomElement();
                    Hediff_SwarmingInfection newInfection = (Hediff_SwarmingInfection)HediffMaker.MakeHediff(this.def, pawn, newPart);

                    newInfection.Severity = Rand.Range(0.12f, 0.19f);

                    pawn.health.AddHediff(newInfection, newPart);

                    this.Severity -= SeverityReductionOnSpread;
                }
            }
        }

        private void ApplyAphrodisiacHediff()
        {
            HediffDef Aphrodisiac = HediffDef.Named("InsectAphrodisiac");

            Hediff existingAphrodisiac = pawn.health.hediffSet.GetFirstHediffOfDef(Aphrodisiac);
            if (existingAphrodisiac != null)
            {
                existingAphrodisiac.Severity = 1.0f;
            }
            else
            {
                Hediff newUnhealthy = HediffMaker.MakeHediff(Aphrodisiac, pawn, null);
                newUnhealthy.Severity = 1.0f;
                pawn.health.AddHediff(newUnhealthy);
            }

        }

        private void ApplySwarmInfectionThought()
        {

            
            if (xxx.is_zoophile(pawn))
            {
                ThoughtDef thoughtDef = ThoughtDef.Named("ZoophileSwarmInfection");
                pawn.needs.mood.thoughts.memories.TryGainMemory(thoughtDef);
            }
            else
            {
                ThoughtDef thoughtDef = ThoughtDef.Named("SwarmInfection");
                pawn.needs.mood.thoughts.memories.TryGainMemory(thoughtDef);
            }
            
        }

        private void SpawnMaggots()
        {
            if (pawn == null || pawn.Map == null) return;

            int maggotCount = Rand.RangeInclusive(1, 2);

            for (int i = 0; i < maggotCount; i++)
            {
                Thing maggot = ThingMaker.MakeThing(ThingDef.Named("Maggot"));
                GenPlace.TryPlaceThing(maggot, pawn.Position, pawn.Map, ThingPlaceMode.Near);
            }

            Thing cum = ThingMaker.MakeThing(ThingDef.Named("FilthInsectCum"));
            GenPlace.TryPlaceThing(cum, pawn.Position, pawn.Map, ThingPlaceMode.Near);
        }

        private void BreakWomb()
        {
            HediffDef insectBrokenDef = HediffDef.Named("InsectBroken");

            if (insectBrokenDef != null && !this.pawn.health.hediffSet.HasHediff(insectBrokenDef))
            {
                Hediff newHediff = HediffMaker.MakeHediff(insectBrokenDef, this.pawn);
                this.pawn.health.AddHediff(newHediff);
            }
        }
    }

}
