using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BetterVeins
{
    public class GenStep_BetterOreLump : GenStep
    {
        public ThingDef oreDef;
        public IntRange oreTileCountRange = new IntRange(300, 450);

        public override int SeedPart => 987654321;

        public override void Generate(Map map, GenStepParams parms)
        {
            if (oreDef == null)
            {
                Log.Error("[BetterVeins] MineableSteel is null! Check ThingDefOf initialization.");
                return;
            }

            int tileCount = oreTileCountRange.RandomInRange;
            IntVec3 center = new IntVec3(map.Size.x / 2, 0, map.Size.z / 2);
            int placed = 0;

            Log.Message($"[BetterVeins] Running BetterOreLump: trying to place {tileCount} tiles of {oreDef.defName}");

            // Create a natural organic looking blob of ore
            IEnumerable<IntVec3> lumpCells = GridShapeMaker.IrregularLump(center, map, tileCount, c => (c.InBounds(map) && !c.Fogged(map)));

            foreach (IntVec3 cell in lumpCells)
            {
                // Clear existing mineables and vegetation
                Thing mineable = cell.GetFirstMineable(map);
                mineable?.Destroy();

                foreach (Thing thing in cell.GetThingList(map).ToList())
                {
                    if (thing.def.category == ThingCategory.Plant || thing.def.destroyable)
                    {
                        thing.Destroy();
                    }
                }

                //Spawn the ore!
                GenSpawn.Spawn(oreDef, cell, map, WipeMode.Vanish);
                placed++;
            }

            Log.Message($"[BetterVeins] Actually placed {placed} {oreDef.label}.");
        }
    }
}
