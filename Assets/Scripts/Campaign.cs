using UnityEngine;

public static class Campaign
{
    public const float FirstPayout = 2400f;
    public const float PayoutDecayPerMission = 0.85f;
    public const float SupplyUseGrowthPerMission = 0.2f;
    public const float TripLengthGrowthPerMission = 0.15f;

    public static int MissionsCompleted { get; private set; }
    public static int MissionNumber => MissionsCompleted + 1;
    public static float CurrentPayout => Mathf.Round(FirstPayout * Mathf.Pow(PayoutDecayPerMission, MissionsCompleted) / 10f) * 10f;
    public static float SupplyUseMultiplier => 1f + SupplyUseGrowthPerMission * MissionsCompleted;
    public static float TripLengthMultiplier => 1f + TripLengthGrowthPerMission * MissionsCompleted;

    public static void StartOver()
    {
        MissionsCompleted = 0;
    }

    public static float PayOutForSuccess()
    {
        float payout = CurrentPayout;
        Resources.money += payout;
        MissionsCompleted++;
        return payout;
    }
}
