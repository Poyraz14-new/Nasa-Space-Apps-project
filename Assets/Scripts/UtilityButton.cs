using UnityEngine;

public class UtilityButton : ShopButton
{
    public enum UtilityType { Food, Water, Fuel, Tool }

    [SerializeField] private UtilityType utilityType;
    [SerializeField] private float price;
    [SerializeField] private float amount = 1f;
    [SerializeField] private float storageUsed;
    [SerializeField] private float toolMassKg;

    private const int MaxSupplyPacks = 20;

    public override int MaxQuantity => utilityType == UtilityType.Tool ? 1 : MaxSupplyPacks;
    public override bool AlreadyOwned => utilityType == UtilityType.Tool && Resources.tools.Contains(DisplayName);

    public void SetItem(UtilityCatalog.Item item)
    {
        utilityType = item.type;
        price = item.price;
        amount = item.amount;
        storageUsed = item.storageUsed;
        toolMassKg = item.toolMassKg;
    }

    public override float Price()
    {
        return price;
    }

    public override string StatsText()
    {
        switch (utilityType)
        {
            case UtilityType.Food:
                return $"Food: +{amount} kg\nMass: +{amount} kg\nStorage: -{storageUsed}";
            case UtilityType.Water:
                return $"Water: +{amount} L\nMass: +{amount} kg\nStorage: -{storageUsed}";
            case UtilityType.Fuel:
                return $"Fuel: +{amount}\nStorage: -{storageUsed}";
            default:
                return $"Mass: +{toolMassKg} kg\nStorage: -{storageUsed}";
        }
    }

    public override bool Buy(TravelSystem travelSystem)
    {
        if (utilityType == UtilityType.Tool)
        {
            bool bought = travelSystem.Buy(price, Resources.ResourceType.Research, storageUsed, 1f, DisplayName);
            if (bought)
            {
                Resources.mass += toolMassKg;
            }
            return bought;
        }

        float units = amount > 0f ? amount : 1f;
        return travelSystem.Buy(price / units, ResourceFor(utilityType), storageUsed / units, units);
    }

    private Resources.ResourceType ResourceFor(UtilityType type)
    {
        switch (type)
        {
            case UtilityType.Food:
                return Resources.ResourceType.Food;
            case UtilityType.Water:
                return Resources.ResourceType.Water;
            default:
                return Resources.ResourceType.Fuel;
        }
    }
}
