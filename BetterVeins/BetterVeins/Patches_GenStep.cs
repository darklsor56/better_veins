using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BetterVeins
{
    public class GenStep_BetterOreLump : GenStep
    {
        public int forcedLumpSize = 30;

        public override int SeedPart => 987654321;

        public override void Generate(Map map, GenStepParams parms)
        {
            // ✅ Safely resolve the ore def *at runtime*
            ThingDef oreDef = ThingDefOf.MineableSteel;
            if (oreDef == null)
            {
                Log.Error("[BetterVeins] MineableSteel is null! Check ThingDefOf initialization.");
                return;
            }

            IntVec3 center = new IntVec3(map.Size.x / 2, 0, map.Size.z / 2);
            int placed = 0;

            Log.Message($"[BetterVeins] Running BetterOreLump: trying to place {forcedLumpSize} tiles of {oreDef.defName}");

            foreach (IntVec3 cell in GenRadial.RadialCellsAround(center, 10f, true))
            {
                if (!cell.InBounds(map)) continue;
                if (!cell.Standable(map)) continue;
                if (cell.GetFirstMineable(map) != null) continue;

                Thing ore = ThingMaker.MakeThing(oreDef);
                GenPlace.TryPlaceThing(ore, cell, map, ThingPlaceMode.Direct);
                placed++;

                if (placed >= forcedLumpSize) break;
            }

            Log.Message($"[BetterVeins] Actually placed {placed} {oreDef.label}.");
        }
    }
}
