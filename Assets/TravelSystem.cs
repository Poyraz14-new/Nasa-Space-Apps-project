using UnityEngine;
using UnityEngine.SceneManagement;

public class TravelSystem : MonoBehaviour
{
    [SerializeField] private float baseTravelDays = 3f;
    [SerializeField] private float referenceMassKg = 15000f;
    [SerializeField] private float foodPerCrewPerDayKg = 2.39f;
    [SerializeField] private float waterPerCrewPerDayKg = 2.79f;
    [SerializeField] private string moonSceneName = "MoonLanding";
    [SerializeField] private string gameOverSceneName = "GameOver";

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

            default:
                Resources.tools.Add(toolName);
                break;
        }

        return true;
    }

    public float CalculateTravelDays()
    {
        float massFactor = Mathf.Sqrt(Resources.mass / referenceMassKg);
        return Mathf.Max(1f, baseTravelDays * massFactor - travelDaysSaved);
    }

    public float FoodNeededKg()
    {
        return foodPerCrewPerDayKg * CalculateTravelDays();
    }

    public float WaterNeededKg()
    {
        return waterPerCrewPerDayKg * CalculateTravelDays();
    }

    public bool HasEnoughSupplies()
    {
        return Resources.food >= FoodNeededKg() && Resources.water >= WaterNeededKg();
    }

    public void TravelToMoon()
    {
        if (!HasEnoughSupplies())
        {
            SceneManager.LoadScene(gameOverSceneName);
            return;
        }

        float travelDays = CalculateTravelDays();
        float foodUsed = FoodNeededKg();
        float waterUsed = WaterNeededKg();

        Resources.food -= foodUsed;
        Resources.water -= waterUsed;
        Resources.mass -= foodUsed + waterUsed;
        Resources.time += travelDays;

        SceneManager.LoadScene(moonSceneName);
    }
}