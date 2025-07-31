using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BetterVeins
{
    public class GenStep_BetterOreLump : GenStep
    {
        public ThingDef forcedDefToScatter = ThingDefOf.MineableSteel;
        public int forcedLumpSize = 30;

        public override int SeedPart => 987654321;

        public override void Generate(Map map, GenStepParams parms)
        {
            IntVec3 center = new IntVec3(map.Size.x / 2, 0, map.Size.z / 2);
            int placed = 0;

            Log.Message($"[BetterVeins] Running BetterOreLump: trying to place {forcedLumpSize} tiles of {forcedDefToScatter.defName}");

            foreach (IntVec3 cell in GenRadial.RadialCellsAround(center, 10f, true))
            {
                if (!cell.InBounds(map)) continue;
                if (!cell.Standable(map)) continue;
                if (cell.GetFirstMineable(map) != null) continue;

                // Optional: you can add terrain or roof restrictions here if needed
                Thing ore = ThingMaker.MakeThing(forcedDefToScatter);
                GenPlace.TryPlaceThing(ore, cell, map, ThingPlaceMode.Direct);
                placed++;

                if (placed >= forcedLumpSize) break;
            }

            Log.Message($"[BetterVeins] Actually placed {placed} {forcedDefToScatter.label}.");
        }
    }
}
