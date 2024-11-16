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
            Pawn Stillicid = this.CasterPawn;

            if (!Stillicid.health.hediffSet.HasHediff(HediffDef.Named("ParasiteImplantCooldown")))
            {
                if (target.Thing is Pawn targetPawn && xxx.is_human(targetPawn))
                {
                    BodyPartRecord brain = targetPawn.health.hediffSet.GetBrain();

                    if (brain != null && !targetPawn.health.hediffSet.HasHediff(HediffDef.Named("StillicidInsect"), brain))
                    {
                        Hediff parasite = HediffMaker.MakeHediff(HediffDef.Named("StillicidInsect"), targetPawn, brain);
                        targetPawn.health.AddHediff(parasite);

                        Hediff cooldown = HediffMaker.MakeHediff(HediffDef.Named("ParasiteImplantCooldown"), Stillicid);
                        Stillicid.health.AddHediff(cooldown);
                    }
                }
            }

            return base.ApplyMeleeDamageToTarget(target);
        }
    }

}
