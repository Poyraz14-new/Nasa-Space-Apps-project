using UnityEngine;

public static class CrewNeeds
{
    public const float BaseFoodPerDayKg = 2.39f;
    public const float BaseWaterPerDayLiters = 2.79f;
    public const float SecondsPerMoonDay = 4f;

    public static float FoodPerDayKg => BaseFoodPerDayKg * Campaign.SupplyUseMultiplier;
    public static float WaterPerDayLiters => BaseWaterPerDayLiters * Campaign.SupplyUseMultiplier;

    public static float DaysOfSupplies(float foodKg, float waterLiters)
    {
        return Mathf.Max(0f, Mathf.Min(foodKg / FoodPerDayKg, waterLiters / WaterPerDayLiters));
    }

    public static string FormatSeconds(float seconds)
    {
        int wholeSeconds = Mathf.CeilToInt(Mathf.Max(0f, seconds));
        return $"{wholeSeconds / 60:00}:{wholeSeconds % 60:00}";
    }
}
