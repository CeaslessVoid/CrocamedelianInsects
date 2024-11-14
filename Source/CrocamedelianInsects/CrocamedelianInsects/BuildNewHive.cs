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
    public class JobDriver_ConsumeJellyAndBuildHive : JobDriver
    {
        private const TargetIndex JellyIndex = TargetIndex.A;
        private const TargetIndex HiveLocationIndex = TargetIndex.B;
        //private int   RequiredJellyCount            = CrIGameComponent.Settings.CrINewHiveCost;

        public static bool jobInProgress = false;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(JellyIndex), job, 1, stackCount: 1, errorOnFailed: errorOnFailed)
                && pawn.Reserve(job.GetTarget(HiveLocationIndex), job, 1, errorOnFailed: errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(JellyIndex);

            yield return Toils_Goto.GotoThing(JellyIndex, PathEndMode.ClosestTouch);

            yield return Toils_Haul.StartCarryThing(JellyIndex, putRemainderInQueue: false, subtractNumTakenFromJobCount: false);

            yield return Toils_Goto.GotoCell(HiveLocationIndex, PathEndMode.OnCell);

            Toil buildHive = new Toil();
            buildHive.initAction = () =>
            {
                if (pawn.carryTracker.CarriedThing != null && pawn.carryTracker.CarriedThing.def == ThingDefOf.InsectJelly)
                {
                    pawn.carryTracker.DestroyCarriedThing();
                    GenSpawn.Spawn(ThingDef.Named("Hive"), job.GetTarget(HiveLocationIndex).Cell, Map);
                }
            };
            buildHive.defaultCompleteMode = ToilCompleteMode.Instant;

            buildHive.AddFinishAction(() => jobInProgress = false);
            yield return buildHive;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.Saving || Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                jobInProgress = false;
            }
        }
    }



    public class ThinkNode_BuildNewHive : ThinkNode_JobGiver
    {
        //private int MinDistanceFromOtherHives = CrIGameComponent.Settings.CrIMinHiveDistance;
        //private int MaxDistanceFromOtherHives = CrIGameComponent.Settings.CrIMaxHiveDistance;
        //private int RequiredJellyCount        = CrIGameComponent.Settings.CrINewHiveCost;

        private int MinDistanceFromOtherHives = 8;
        private int MaxDistanceFromOtherHives = 14;

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.Map == null || !xxx.is_insect(pawn) || pawn.Faction != Faction.OfInsects)
                return null;

            if (!pawn.Map.listerThings.ThingsOfDef(ThingDefOf.InsectJelly).Any())
                return null;

            IntVec3 hiveLocation = FindHiveLocation(pawn);
            if (!hiveLocation.IsValid || !pawn.CanReserve(hiveLocation))
                return null;

            if (JobDriver_ConsumeJellyAndBuildHive.jobInProgress)
                return null;

            Thing jelly = FindClosestJelly(pawn);
            if (jelly == null || !pawn.CanReserve(jelly))
                return null;

            int cocoonedPawnCount = pawn.Map.mapPawns.AllPawnsSpawned
                .Count(p => p.health?.hediffSet?.HasHediff(HediffDef.Named("RJW_Cocoon")) ?? false);
            int hiveCount = pawn.Map.listerThings.ThingsOfDef(ThingDef.Named("Hive")).Count();

            if (cocoonedPawnCount + 2 <= hiveCount)
                return null;

            Job job = JobMaker.MakeJob(CrIDefOf.Job_ConsumeJellyAndBuildHive, jelly, hiveLocation);
            job.count = 1;

            JobDriver_ConsumeJellyAndBuildHive.jobInProgress = true;

            return job;
            //return null; // TEMP
        }

        private Thing FindClosestJelly(Pawn pawn)
        {
            return pawn.Map.listerThings.ThingsOfDef(ThingDefOf.InsectJelly) //TEMP ITEM
                .Where(jelly => pawn.CanReserveAndReach(jelly, PathEndMode.ClosestTouch, Danger.Deadly))
                .OrderBy(jelly => jelly.Position.DistanceTo(pawn.Position))
                .FirstOrDefault();
        }

        private IntVec3 FindHiveLocation(Pawn pawn)
        {
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(pawn.Position, MaxDistanceFromOtherHives, true))
            {
                if (cell.InBounds(pawn.Map) && cell.Standable(pawn.Map)
                    && cell.DistanceTo(pawn.Position) >= MinDistanceFromOtherHives
                    && IsFarEnoughFromOtherHives(cell, pawn.Map))
                {
                    return cell;
                }
            }
            return IntVec3.Invalid;
        }

        private bool IsFarEnoughFromOtherHives(IntVec3 cell, Map map)
        {
            return !map.listerThings.ThingsOfDef(ThingDef.Named("Hive"))
                .Any(hive => hive.Position.DistanceTo(cell) < MinDistanceFromOtherHives);
        }
    }

}