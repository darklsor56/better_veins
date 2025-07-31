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

    // This defines a Harmony patch for the Generate() method in GenStep_ScatterLumpsMineable.
    // This is the class responsible for spawning minable ore lumps on the map.
    [HarmonyPatch(typeof(GenStep_ScatterLumpsMineable), "Generate")]
    public static class Patch_GenStep_ScatterLumpsMineable_Generate
    {
        // This method runs *before* the original Generate() method.
        // You can modify arguments or skip the original method entirely (with a `return false`).
        public static bool Prefix(Map map)
        {
            // This log confirms the patch is being called during map generation.
            Log.Message("[BetterVeins] Patching GenStep_ScatterLumpsMineable.Generate!");

            // Define the number of things generated
            ThingDef thingDef = ThingDefOf.Steel; // CHANGE LATER

            // Number of lumps (1 lump = 75 steel by default)
            int count = 1;

            // Try to find a suitable cell on the map to place the lump.
            IntVec3 center;
            if (!CellFinder.TryFindRandomCell(map, c => c.Standable(map) && c.GetFirstMineable(map) == null, out center))
            {
                Log.Warning("[BetterVeins] Failed to find a valid cell using fallback CellFinder.");
                return false;
            }

            if (center != IntVec3.Invalid)
            {
                // Create and spawn the mineable lump
                Thing mineable = ThingMaker.MakeThing(thingDef);
                mineable.stackCount = 75 * count; // Set the stack size (75 steel per lump by default)

                GenSpawn.Spawn(mineable, center, map, WipeMode.Vanish);
                Log.Message($"[BetterVeins] Spawned {mineable.stackCount} steel at {center}.");
            }
            else
            {
                Log.Warning("[BetterVeins] Could not find a valid cell to spawn the mineable lump.");
            }

            return false;
        }
    }
}