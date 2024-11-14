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
    }

}
