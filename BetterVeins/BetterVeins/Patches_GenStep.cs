using HarmonyLib;     // Harmony is the library used to patch RimWorld methods
using RimWorld;       // RimWorld-specific types like GenStep_ScatterLumpsMineable
using Verse;          // Core game types like Log, Map, etc.
using UnityEngine;    // Unity types, if needed (not used in this example)

namespace BetterVeins
{
    // This attribute ensures the static constructor runs when the game loads this assembly.
    [StaticConstructorOnStartup]
    public static class HarmonyPatcher
    {
        static HarmonyPatcher()
        {
            // Create a new Harmony instance with a unique ID for your mod.
            // This is how Harmony knows which mod applied which patches.
            var harmony = new Harmony("com.daniel.betterveins");

            // Automatically scan and apply all patches defined with [HarmonyPatch] in this assembly.
            harmony.PatchAll();

            // You could also patch methods manually like:
            // MethodInfo original = AccessTools.Method(typeof(ClassToPatch), "MethodName");
            // MethodInfo prefix = typeof(MyPatchClass).GetMethod("Prefix");
            // harmony.Patch(original, prefix: new HarmonyMethod(prefix));
        }
    }

    public class GenStep_BetterVeins_OreDeposit : GenStep
    {
        public override int SeedPart => 1337420;

        public override void Generate(Map map, GenStepParams parms)
        {
            IntVec3 center = new IntVec3(map.Size.x / 2, 0, map.Size.z / 2);
            int lumpSize = Rand.RangeInclusive(20, 30); // Number of tiles in the ore lump

            int placed = 0;
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(center, 10f, useCenter: true))
            {
                // Find a valid spot for the lump
                if (!cell.InBounds(map)) continue;
                if (!cell.Standable(map)) continue;
                if (cell.GetFirstMineable(map) != null) continue;

                GenSpawn.Spawn(ThingDefOf.MineableSteel, cell, map);
                placed++;

                // Stop when enough is placed.
                if (placed >= lumpSize) break;
            }

            Log.Message($"[BetterVeins] Spawned {placed} compacted steel tiles near center of map.");
        }
    }
   
}