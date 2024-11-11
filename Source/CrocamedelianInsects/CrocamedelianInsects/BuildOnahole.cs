//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Verse.AI;
//using Verse;
//using RimWorld;
//using rjw;

//namespace CrocamedelianInsects
//{
//    public class JobDriver_ConsumeJellyAndBuildIncubator : JobDriver
//    {
//        private const TargetIndex JellyIndex = TargetIndex.A;
//        private const TargetIndex IncubatorLocationIndex = TargetIndex.B;

//        public override bool TryMakePreToilReservations(bool errorOnFailed)
//        {
//            return pawn.Reserve(job.GetTarget(JellyIndex), job, 1, stackCount: 1, errorOnFailed: errorOnFailed)
//                && pawn.Reserve(job.GetTarget(IncubatorLocationIndex), job, 1, errorOnFailed: errorOnFailed);
//        }

//        protected override IEnumerable<Toil> MakeNewToils()
//        {
//            this.FailOnDestroyedOrNull(JellyIndex);

//            yield return Toils_Goto.GotoThing(JellyIndex, PathEndMode.ClosestTouch);

//            yield return Toils_Haul.StartCarryThing(JellyIndex, putRemainderInQueue: false, subtractNumTakenFromJobCount: true);

//            yield return Toils_Goto.GotoCell(IncubatorLocationIndex, PathEndMode.OnCell);

//            Toil buildIncubator = new Toil();
//            buildIncubator.initAction = () =>
//            {
//                if (pawn.carryTracker.CarriedThing != null && pawn.carryTracker.CarriedThing.def == ThingDefOf.InsectJelly)
//                {
//                    pawn.carryTracker.DestroyCarriedThing();
//                    GenSpawn.Spawn(ThingDef.Named("OnaholeInsect"), job.GetTarget(IncubatorLocationIndex).Cell, Map);
//                }
//            };
//            buildIncubator.defaultCompleteMode = ToilCompleteMode.Instant;
//            yield return buildIncubator;
//        }
//    }

//    public class ThinkNode_BuildIncubator : ThinkNode_JobGiver
//    {
//        private const int MaxDistanceFromHive = 4;
//        private const int RequiredJellyCount = 1;
//        private const int MinDistanceFromIncubators = 3;

//        protected override Job TryGiveJob(Pawn pawn)
//        {
//            if (pawn.Map == null || !xxx.is_insect(pawn) || pawn.Faction != Faction.OfInsects)
//                return null;

//            if (pawn.CurJob != null)
//                return null;

//            if (!pawn.Map.listerThings.ThingsOfDef(ThingDefOf.InsectJelly).Any())
//                return null;

//            Thing jelly = FindClosestJelly(pawn);
//            if (jelly == null)
//                return null;

//            Thing hive = FindClosestHive(pawn);
//            if (hive == null)
//                return null;

//            IntVec3 incubatorLocation = FindIncubatorLocationNearHive(hive, pawn.Map, pawn);
//            if (!incubatorLocation.IsValid)
//                return null;

//            Job job = JobMaker.MakeJob(CrIDefOf.Job_ConsumeJellyAndBuildIncubator, jelly, incubatorLocation);
//            job.count = 1;

//            return job;
//        }

//        private Thing FindClosestJelly(Pawn pawn)
//        {
//            return pawn.Map.listerThings.ThingsOfDef(ThingDefOf.InsectJelly)
//                .Where(jelly => pawn.CanReserveAndReach(jelly, PathEndMode.ClosestTouch, Danger.Deadly))
//                .OrderBy(jelly => jelly.Position.DistanceTo(pawn.Position))
//                .FirstOrDefault();
//        }

//        private Thing FindClosestHive(Pawn pawn)
//        {
//            return pawn.Map.listerThings.ThingsOfDef(ThingDef.Named("Hive"))
//                .Where(hive => pawn.CanReserveAndReach(hive, PathEndMode.ClosestTouch, Danger.Deadly))
//                .OrderBy(hive => hive.Position.DistanceTo(pawn.Position))
//                .FirstOrDefault();
//        }

//        private IntVec3 FindIncubatorLocationNearHive(Thing hive, Map map, Pawn pawn)
//        {
//            foreach (IntVec3 cell in GenRadial.RadialCellsAround(hive.Position, MaxDistanceFromHive, true))
//            {
//                if (cell.InBounds(map) && cell.Standable(map) &&
//                    IsFarEnoughFromOtherIncubatorsAndReservable(cell, map, pawn))
//                {
//                    return cell;
//                }
//            }
//            return IntVec3.Invalid;
//        }

//        private bool IsFarEnoughFromOtherIncubatorsAndReservable(IntVec3 cell, Map map, Pawn pawn)
//        {
//            bool farFromOtherIncubators = !map.listerThings.ThingsOfDef(ThingDef.Named("OnaholeInsect"))
//                .Any(incubator => incubator.Position.DistanceTo(cell) < MinDistanceFromIncubators);

//            bool reservable = pawn.CanReserveAndReach(cell, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, false);

//            return farFromOtherIncubators && reservable;
//        }
//    }

//}
