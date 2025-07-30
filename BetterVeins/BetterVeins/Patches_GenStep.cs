using HarmonyLib;     // Harmony is the library used to patch RimWorld methods
using RimWorld;       // RimWorld-specific types like GenStep_ScatterLumpsMineable
using Verse;          // Core game types like Log, Map, etc.

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
        public static void Prefix(Map map)
        {
            // This log confirms the patch is being called during map generation.
            Log.Message("[BetterVeins] Patching GenStep_ScatterLumpsMineable.Generate!");

            // You can add logic here to affect the map before ore generation.
            // For example: remove zones, alter terrain, or replace ore defs.
        }
    }
}