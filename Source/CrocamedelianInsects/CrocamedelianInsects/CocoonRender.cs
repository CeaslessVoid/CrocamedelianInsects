using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CrocamedelianInsects
{
    public class PawnRenderNode_Cocoon : PawnRenderNode
    {
        public PawnRenderNode_Cocoon(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {

        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            Shader shader = ShaderFor(pawn);
            String path = "Hediff/";
            String bodyType = pawn.story.bodyType.defName;
            String pathString;

            pathString = path + "Cocoon_" + bodyType;
            if (ContentFinder<Texture2D>.Get(pathString + "_south", false) != null)
            {
                return GraphicDatabase.Get<Graphic_Multi>(pathString, shader);
            }
            return null;
        }
    }

}
