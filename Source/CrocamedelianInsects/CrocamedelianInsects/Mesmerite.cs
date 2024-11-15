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
    public class Verb_MeleeAttackInject : Verb_MeleeAttackDamage
    {
        protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
        {
            Pawn Mesmerite = this.CasterPawn;

            if (!Mesmerite.health.hediffSet.HasHediff(HediffDef.Named("ParasiteImplantCooldown")))
            {
                if (target.Thing is Pawn targetPawn && xxx.is_human(targetPawn))
                {
                    BodyPartRecord brain = targetPawn.health.hediffSet.GetBrain();

                    if (brain != null && !targetPawn.health.hediffSet.HasHediff(HediffDef.Named("MesmeriteInsect"), brain))
                    {
                        Hediff parasite = HediffMaker.MakeHediff(HediffDef.Named("MesmeriteInsect"), targetPawn, brain);
                        targetPawn.health.AddHediff(parasite);

                        Hediff cooldown = HediffMaker.MakeHediff(HediffDef.Named("ParasiteImplantCooldown"), Mesmerite);
                        Mesmerite.health.AddHediff(cooldown);
                    }
                }
            }

            return base.ApplyMeleeDamageToTarget(target);
        }
    }

}
