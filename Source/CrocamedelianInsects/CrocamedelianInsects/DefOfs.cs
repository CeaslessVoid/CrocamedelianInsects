﻿using HarmonyLib;
using LudeonTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;


namespace CrocamedelianInsects
{
    [DefOf]
    public static class CrIDefOf
    {
        public static readonly JobDef Job_HaulCocoonedPawnToHive        = DefDatabase<JobDef>.GetNamed("Job_HaulCocoonedPawnToHive");

        public static readonly JobDef Job_ConsumeJellyAndBuildHive      = DefDatabase<JobDef>.GetNamed("Job_ConsumeJellyAndBuildHive");

        //public static readonly JobDef Job_ConsumeJellyAndBuildIncubator = DefDatabase<JobDef>.GetNamed("Job_ConsumeJellyAndBuildIncubator");


    }


}
