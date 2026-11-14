using UnityEngine;
using UnityEngine.SceneManagement;
using System.Resources;
using System.Collections.Generic;
public class TravelSystem : MonoBehaviour
{
    [SerializeField] private float baseTravelDays = 3f;
    [SerializeField] private float referenceMassKg = 15000f;
    [SerializeField] private float foodPerCrewPerDayKg = 2.39f;
    [SerializeField] private float waterPerCrewPerDayKg = 2.79f;
    [SerializeField] private string moonSceneName = "MoonLanding";
    [SerializeField] private string gameOverSceneName = "GameOver";


    private float travelDaysSaved;

    public bool TryBuy(float cost)
    {
        return Resources.money>cost;
    }

    public bool BuyPart(float cost, float partMassKg, float daysSaved)
    {
        if (TryBuy(cost))
        {
            return false;
        }

        Resources.money -= cost;
        Resources.mass += partMassKg;
        travelDaysSaved += daysSaved;
        return true;
    }
    // use this for buying exepect for parts
    // method expects the cost of the thing
    // what type is it
    // how much storage unit is it
    // amount of it(default=1)
    // if its a tool the tool name
    public void Buy(float cost,Resources.ResourceType resource,float volume,float AMOUNT=1,string tool="")
    {
        // these are all the stuff you can buy
        // TOOL, FOOD/WATER
        switch (resource)
        {
            case Resources.ResourceType.Water:
                if (TryBuy(cost*AMOUNT))
                {
                    Resources.money-=cost*AMOUNT;
                    Resources.storage-=volume;
                }
            break;
            case Resources.ResourceType.Food:
            if (TryBuy(cost*AMOUNT))
                {
                    Resources.money-=cost*AMOUNT;
                    Resources.storage-=volume;
                }
            break;

            default:
            if (TryBuy(cost))
                {
                    Resources.money-=cost;
                    Resources.storage-=volume;
                    Resources.tools.Add(tool);
                }
            break;
        }
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
        return waterPerCrewPerDayKg  * CalculateTravelDays();
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