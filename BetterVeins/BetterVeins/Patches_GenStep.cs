using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BetterVeins
{
    public class GenStep_BetterOreLump : GenStep
    {
        public string oreDef;
        public IntRange oreTileCountRange = new IntRange(300, 450);

        public override int SeedPart => 987654321;

        public override void Generate(Map map, GenStepParams parms)
        {
            ThingDef oreDef = DefDatabase<ThingDef>.GetNamedSilentFail(this.oreDef);
            if (oreDef == null)
            {
                Log.Error("[BetterVeins] MineableSteel is null! Check ThingDefOf initialization.");
                return;
            }

            IntRange range = GetRangeForOre(oreDef.defName);
            int tileCount = range.RandomInRange;
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

        private IntRange GetRangeForOre(string oreDefName)
        {
            var s = BetterVeinsMod.settings;

            if (oreDefName == "MineableSteel")
                return new IntRange(s.steelMin, s.steelMax);
            else if (oreDefName == "MineableSilver")
                return new IntRange(s.silverMin, s.silverMax);
            else if (oreDefName == "MineableGold")
                return new IntRange(s.goldMin, s.goldMax);
            else if (oreDefName == "MineableJade")
                return new IntRange(s.jadeMin, s.jadeMax);
            else if (oreDefName == "MineablePlasteel")
                return new IntRange(s.plasteelMin, s.plasteelMax);
            else if (oreDefName == "MineableUranium")
                return new IntRange(s.uraniumMin, s.uraniumMax);
            else if (oreDefName == "MineableComponentsIndustrial")
                return new IntRange(s.componentsMin, s.componentsMax);
            else
                return oreTileCountRange; // Default range if not specified
        }
    }

    public class  BetterVeinsSettings : ModSettings
    {
        // components
        public int componentsMin = 60;
        public int componentsMax = 80;

        // gold
        public int goldMin = 35;
        public int goldMax = 60;

        // jade
        public int jadeMin = 35;
        public int jadeMax = 50;

        // plassteel
        public int plasteelMin = 63;
        public int plasteelMax = 88;

        // silver
        public int silverMin = 350;
        public int silverMax = 500;

        // steel
        public int steelMin = 350;
        public int steelMax = 450;

        // uranium
        public int uraniumMin = 65;
        public int uraniumMax = 100;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref componentsMin, "componentsMin", componentsMin);
            Scribe_Values.Look(ref componentsMax, "componentsMax", componentsMax);

            Scribe_Values.Look(ref goldMin, "goldMin", goldMin);
            Scribe_Values.Look(ref goldMax, "goldMax", goldMax);

            Scribe_Values.Look(ref jadeMin, "jadeMin", jadeMin);
            Scribe_Values.Look(ref jadeMax, "jadeMax", jadeMax);

            Scribe_Values.Look(ref plasteelMin, "plasteelMin", plasteelMin);
            Scribe_Values.Look(ref plasteelMax, "plasteelMax", plasteelMax);

            Scribe_Values.Look(ref silverMin, "silverMin", silverMin);
            Scribe_Values.Look(ref silverMax, "silverMax", silverMax);

            Scribe_Values.Look(ref steelMin, "steelMin", steelMin);
            Scribe_Values.Look(ref steelMax, "steelMax", steelMax);

            Scribe_Values.Look(ref uraniumMin, "uraniumMin", uraniumMin);
            Scribe_Values.Look(ref uraniumMax, "uraniumMax", uraniumMax);
        }

        public void ResetToDefaults()
        {
            componentsMin = 60;
            componentsMax = 80;

            goldMin = 35;
            goldMax = 60;

            jadeMin = 35;
            jadeMax = 50;

            plasteelMin = 63;
            plasteelMax = 88;

            silverMin = 350;
            silverMax = 500;

            steelMin = 350;
            steelMax = 450;

            uraniumMin = 65;
            uraniumMax = 100;
        }

    }

    public class BetterVeinsMod : Mod
    {
        public static BetterVeinsSettings settings;
        private Vector2 scrollPosition = Vector2.zero; // Add this field

        public BetterVeinsMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<BetterVeinsSettings>();
        }
        public override string SettingsCategory() => "Better Veins";
        public override void DoSettingsWindowContents(Rect inRect)
        {
            float viewHeight = 700f; // Adjust as needed for your content
            Rect outRect = inRect;
            Rect viewRect = new Rect(0, 0, inRect.width - 16f, viewHeight);

            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            // Make the min/max sliders for each resource
            void DrawRange(string label, ref int min, ref int max, int maxSlider = 1000)
            {
                listing.Label($"{label} Range: {min} - {max}");
                min = (int)listing.Slider(min, 0, maxSlider);
                max = (int)listing.Slider(max, 0, maxSlider);

                if (min > max)
                {
                    (min, max) = (max, min); // Swap if min > max
                }
            }

            DrawRange("Components", ref settings.componentsMin, ref settings.componentsMax);
            DrawRange("Gold", ref settings.goldMin, ref settings.goldMax);
            DrawRange("Jade", ref settings.jadeMin, ref settings.jadeMax);
            DrawRange("Plasteel", ref settings.plasteelMin, ref settings.plasteelMax);
            DrawRange("Silver", ref settings.silverMin, ref settings.silverMax);
            DrawRange("Steel", ref settings.steelMin, ref settings.steelMax);
            DrawRange("Uranium", ref settings.uraniumMin, ref settings.uraniumMax);

            listing.GapLine();
            if (listing.ButtonText("Reset to Default Values"))
            {
                settings.ResetToDefaults();
            }

            listing.End();
            Widgets.EndScrollView();
            base.DoSettingsWindowContents(inRect);
        }
    }
}
