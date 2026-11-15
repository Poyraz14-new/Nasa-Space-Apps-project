using UnityEngine;
using UnityEngine.SceneManagement;

public class TravelSystem : MonoBehaviour
{
    public const float FoodPerDayKg = 2.39f;
    public const float WaterPerDayLiters = 2.79f;

    [SerializeField] private float baseTravelDays = 3f;
    [SerializeField] private float referenceMassKg = 15000f;
    [SerializeField] private float fuelToDoubleSpeed = 500f;
    [SerializeField] private float minimumTravelDays = 1f;

    private float travelDaysSaved;

    public bool CanAfford(float cost)
    {
        return Resources.money >= cost;
    }

    public bool HasStorageFor(float storageNeeded)
    {
        return Resources.storage >= storageNeeded;
    }

    public bool BuyPart(SpaceCraftParts.Cabin cabin)
    {
        return PayForPart(cabin.money, cabin.mass, 0f, cabin.daysSaved, cabin.storage);
    }

    public bool BuyPart(SpaceCraftParts.Body body)
    {
        return PayForPart(body.money, body.mass, body.fuel, 0f, body.storage);
    }

    public bool BuyPart(SpaceCraftParts.Thruster thruster)
    {
        return PayForPart(thruster.money, thruster.mass, thruster.fuel, 0f, 0f);
    }

    private bool PayForPart(float cost, float partMassKg, float fuelAdded, float daysSaved, float storageAdded)
    {
        if (!CanAfford(cost))
        {
            return false;
        }

        Resources.money -= cost;
        Resources.mass += partMassKg;
        Resources.fuel += fuelAdded;
        Resources.storage += storageAdded;
        travelDaysSaved += daysSaved;
        return true;
    }

    public bool Buy(float costPerUnit, Resources.ResourceType resource, float storagePerUnit, float amount = 1f, string toolName = "")
    {
        float totalCost = costPerUnit * amount;
        float totalStorage = storagePerUnit * amount;

        if (!CanAfford(totalCost) || !HasStorageFor(totalStorage))
        {
            return false;
        }

        Resources.money -= totalCost;
        Resources.storage -= totalStorage;

        switch (resource)
        {
            case Resources.ResourceType.Food:
                Resources.food += amount;
                Resources.mass += amount;
                break;

            case Resources.ResourceType.Water:
                Resources.water += amount;
                Resources.mass += amount;
                break;

            case Resources.ResourceType.Fuel:
                Resources.fuel += amount;
                break;

            default:
                Resources.tools.Add(toolName);
                break;
        }

        return true;
    }

    public float CalculateTravelDays()
    {
        float massFactor = Mathf.Sqrt(Resources.mass / referenceMassKg);
        float speedFactor = 1f + Resources.fuel / fuelToDoubleSpeed;
        float travelDays = baseTravelDays * Campaign.TripLengthMultiplier * massFactor / speedFactor - travelDaysSaved;
        return Mathf.Max(minimumTravelDays, travelDays);
    }

    public float FoodNeededKg()
    {
        return FoodPerDayKg * CalculateTravelDays();
    }

    public float WaterNeededKg()
    {
        return WaterPerDayLiters * CalculateTravelDays();
    }

    public float MoonStaySeconds()
    {
        float foodLeft = Resources.food - FoodNeededKg();
        float waterLeft = Resources.water - WaterNeededKg();
        return CrewNeeds.DaysOfSupplies(foodLeft, waterLeft) * CrewNeeds.SecondsPerMoonDay;
    }

    public bool HasEnoughSupplies()
    {
        return Resources.food >= FoodNeededKg() && Resources.water >= WaterNeededKg();
    }

    public void TravelToMoon()
    {
        if (!HasEnoughSupplies())
        {
            MissionRun.Lose($"Not enough supplies for the {CalculateTravelDays():0.0}-day trip. "
                + $"The crew needed {FoodNeededKg():0.0} kg of food and {WaterNeededKg():0.0} L of water "
                + $"but had {Resources.food:0.0} kg and {Resources.water:0.0} L.");
            return;
        }

        float travelDays = CalculateTravelDays();
        float foodUsed = FoodNeededKg();
        float waterUsed = WaterNeededKg();

        Resources.food -= foodUsed;
        Resources.water -= waterUsed;
        Resources.mass -= foodUsed + waterUsed;
        Resources.fuel = 0f;
        Resources.time += travelDays;

        SceneManager.LoadScene(GameScenes.Moon);
    }
}