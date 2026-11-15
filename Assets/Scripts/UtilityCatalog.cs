public static class UtilityCatalog
{
    public const string IconFolder = "ToolIcons";

    public struct Item
    {
        public string label;
        public string displayName;
        public UtilityButton.UtilityType type;
        public float price;
        public float amount;
        public float storageUsed;
        public float toolMassKg;
        public string iconName;
        public string description;

        public Item(string label, string displayName, UtilityButton.UtilityType type, float price, float amount, float storageUsed, float toolMassKg, string iconName, string description)
        {
            this.label = label;
            this.displayName = displayName;
            this.type = type;
            this.price = price;
            this.amount = amount;
            this.storageUsed = storageUsed;
            this.toolMassKg = toolMassKg;
            this.iconName = iconName;
            this.description = description;
        }
    }

    public static readonly Item[] Items =
    {
        new Item("MRE Pack", "MRE Pack", UtilityButton.UtilityType.Food, 50f, 10f, 10f, 0f, "MRE", "Ready-to-eat meals. The crew eats about 2.4 kg of food per day of travel."),
        new Item("Water", "Water Bottle", UtilityButton.UtilityType.Water, 20f, 10f, 10f, 0f, "WaterBottle", "Drinking water. The crew drinks about 2.8 L per day of travel."),
        new Item("Fuel", "Fuel Can", UtilityButton.UtilityType.Fuel, 10f, 10f, 10f, 0f, "Fuel", "More fuel means a faster trip, so less food and water is used."),
        new Item("Hammer", MissionManager.GeologicalHammer, UtilityButton.UtilityType.Tool, 150f, 1f, 5f, 2f, "GeologicalHammer", "Breaks lunar rocks open to expose the minerals inside."),
        new Item("Scoop", MissionManager.LunarSamplingScoop, UtilityButton.UtilityType.Tool, 100f, 1f, 3f, 1f, "LunarSamplingScoop", "Collects loose surface regolith and soil."),
        new Item("Sample Pouch", MissionManager.SamplePouch, UtilityButton.UtilityType.Tool, 50f, 1f, 2f, 0.5f, "SamplePouch", "Seals collected samples so they are not contaminated."),
        new Item("Spectrometer", MissionManager.PortableSpectrometer, UtilityButton.UtilityType.Tool, 250f, 1f, 6f, 3f, "PortableSpectrometer", "Analyses the chemical and mineral makeup of samples."),
        new Item("Repair Kit", MissionManager.EvaRepairKit, UtilityButton.UtilityType.Tool, 120f, 1f, 4f, 2f, "EVARepairKit", "Fixes minor spacesuit and equipment damage."),
    };
}
