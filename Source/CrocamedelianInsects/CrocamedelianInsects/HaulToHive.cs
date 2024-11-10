﻿using System;
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
        private const TargetIndex PawnIndex = TargetIndex.A;
        private const TargetIndex HiveIndex = TargetIndex.B;

        private const float MinDistanceFromHive = 5.0f;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(PawnIndex), job) && pawn.Reserve(job.GetTarget(HiveIndex), job);
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

            // Go to the cocooned pawn
            yield return Toils_Goto.GotoThing(PawnIndex, PathEndMode.Touch);

            // Pick up the cocooned pawn
            yield return Toils_Haul.StartCarryThing(PawnIndex);

            // Go to the hive or hive location
            yield return Toils_Goto.GotoThing(HiveIndex, PathEndMode.Touch);

            // Drop the cocooned pawn at the hive
            Toil placePawn = new Toil();
            placePawn.initAction = () =>
            {
                IntVec3 dropCell = FindAdjacentEmptyCell(new TargetInfo(hiveLocation, Map));
                if (dropCell.IsValid)
                {
                    pawn.carryTracker.TryDropCarriedThing(dropCell, ThingPlaceMode.Direct, out _);
                }
            };
            placePawn.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return placePawn;
        }

        private IntVec3 FindAdjacentEmptyCell(TargetInfo hiveLocationTarget)
        {
            // Iterate over all 8 adjacent cells around the hive location
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
            if (xxx.is_insect(pawn))
            {
                Thing hive = FindHiveOrHiveLocation(pawn);
                Pawn cocoonedPawn = FindCocoonedPawn(pawn, hive);

                if (cocoonedPawn == null) return null;
                if (hive == null) return null;

                Job job = JobMaker.MakeJob(CrIDefOf.Job_HaulCocoonedPawnToHive, cocoonedPawn, hive);
                return job;
            }
            return null;
        }

        private Pawn FindCocoonedPawn(Pawn hauler, Thing hive)
        {

            return hauler.Map.mapPawns.AllPawnsSpawned
                .FirstOrDefault(p => p.Downed && p.Position.DistanceTo(hive.Position) > MinDistanceFromHive && p.CurJobDef != CrIDefOf.Job_HaulCocoonedPawnToHive);
        }

        private Thing FindHiveOrHiveLocation(Pawn hauler)
        {
            return hauler.Map.listerThings.ThingsOfDef(ThingDef.Named("Hive")).FirstOrDefault();
        }
    }

}