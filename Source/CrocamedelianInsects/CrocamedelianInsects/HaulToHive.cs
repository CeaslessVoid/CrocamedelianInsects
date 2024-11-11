using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using RimWorld;
using rjw;

namespace CrocamedelianInsects
{
    public class JobDriver_HaulCocoonedPawnToHive : JobDriver
    {
        private const TargetIndex PawnIndex     = TargetIndex.A;
        private const TargetIndex HiveIndex     = TargetIndex.B;
        private const float MinDistanceFromHive = 5.0f;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(PawnIndex), job, 1, stackCount: 1, errorOnFailed: errorOnFailed) 
                && pawn.Reserve(job.GetTarget(HiveIndex), job, 1, errorOnFailed: errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(PawnIndex);
            this.FailOnDestroyedOrNull(HiveIndex);

            IntVec3 hiveLocation = job.GetTarget(HiveIndex).Cell;
            IntVec3 pawnLocation = job.GetTarget(PawnIndex).Cell;

            if (pawnLocation.DistanceTo(hiveLocation) <= MinDistanceFromHive)
            {
                EndJobWith(JobCondition.Incompletable);
                yield break;
            }

            yield return Toils_Goto.GotoThing(PawnIndex, PathEndMode.Touch);

            yield return Toils_Haul.StartCarryThing(PawnIndex);

            yield return Toils_Goto.GotoThing(HiveIndex, PathEndMode.Touch);

            Toil placePawn = new Toil();
            placePawn.initAction = () =>
            {
                IntVec3 dropCell = FindAdjacentEmptyCell(new TargetInfo(hiveLocation, Map));
                if (dropCell.IsValid && pawn.CanReserve(dropCell))
                {
                    pawn.Reserve(dropCell, job);
                    pawn.carryTracker.TryDropCarriedThing(dropCell, ThingPlaceMode.Direct, out _);
                }
                else
                {
                    EndJobWith(JobCondition.Incompletable);
                }
            };
            placePawn.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return placePawn;
        }

        private IntVec3 FindAdjacentEmptyCell(TargetInfo hiveLocationTarget)
        {
            foreach (IntVec3 cell in GenAdj.CellsAdjacent8Way(hiveLocationTarget))
            {
                if (cell.InBounds(Map) && cell.Walkable(Map) && cell.GetFirstThing(Map, ThingDefOf.Human) == null)
                {
                    return cell;
                }
            }
            return IntVec3.Invalid;
        }
    }



    public class ThinkNode_HaulCocoonedPawn : ThinkNode_JobGiver
    {
        private const float MinDistanceFromHive = 5.0f;
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.Map == null || !xxx.is_insect(pawn) || pawn.Faction != Faction.OfInsects)
                return null;

            Thing hive = FindHiveOrHiveLocation(pawn);
            Pawn cocoonedPawn = FindCocoonedPawn(pawn);

            if (cocoonedPawn == null || hive == null)
                return null;

            if (cocoonedPawn.Position.DistanceTo(hive.Position) <= MinDistanceFromHive)
                return null;

            if (!pawn.CanReserveAndReach(cocoonedPawn, PathEndMode.ClosestTouch, Danger.Deadly) ||
                !pawn.CanReserveAndReach(hive, PathEndMode.Touch, Danger.Deadly))
            {
                return null;
            }

            Job job = JobMaker.MakeJob(CrIDefOf.Job_HaulCocoonedPawnToHive, cocoonedPawn, hive);
            job.count = 1;
            return job;
        }

        private Pawn FindCocoonedPawn(Pawn hauler)
        {
            return hauler.Map.mapPawns.AllPawnsSpawned
                .Where(p => p.health.hediffSet.HasHediff(HediffDef.Named("RJW_Cocoon"))
                         && IsFarFromAnyHive(p, hauler.Map)
                         && p.CurJobDef != CrIDefOf.Job_HaulCocoonedPawnToHive
                         && hauler.CanReserve(p))
                .OrderBy(p => hauler.Position.DistanceTo(p.Position))
                .FirstOrDefault();
        }

        private bool IsFarFromAnyHive(Pawn cocoonedPawn, Map map)
        {
            foreach (Thing hive in map.listerThings.ThingsOfDef(ThingDef.Named("Hive")))
            {
                if (cocoonedPawn.Position.DistanceTo(hive.Position) <= MinDistanceFromHive)
                {
                    return false;
                }
            }
            return true;
        }

        private Thing FindHiveOrHiveLocation(Pawn hauler)
        {
            return hauler.Map.listerThings.ThingsOfDef(ThingDef.Named("Hive"))
                .OrderBy(hive => hauler.Position.DistanceTo(hive.Position))
                .FirstOrDefault();
        }
    }

}
