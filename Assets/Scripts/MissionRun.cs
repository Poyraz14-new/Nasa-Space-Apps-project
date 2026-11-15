using UnityEngine;
using UnityEngine.SceneManagement;

public static class MissionRun
{
    public static bool IsFinished { get; private set; }

    private static bool campaignLost = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetBetweenPlaySessions()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        MissionManager.ClearCurrentMission();
        IsFinished = false;
        campaignLost = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ListenForSceneLoads()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;

        if (scene.name == GameScenes.Shop)
        {
            StartNewRunIfNeeded();
            GameObject briefing = new GameObject("Mission Briefing");
            briefing.AddComponent<UtilitiesShelfBuilder>();
            briefing.AddComponent<BuyBar>();
            briefing.AddComponent<MissionBriefing>();
            AddTestShortcut(briefing);
        }
        else if (scene.name == GameScenes.Moon)
        {
            StartNewRunIfNeeded();
            GameObject tracker = new GameObject("Mission Tracker");
            tracker.AddComponent<LandedRocket>();
            tracker.AddComponent<MoonSupplies>();
            tracker.AddComponent<MissionTracker>();
            AddTestShortcut(tracker);
        }
    }

    private static void AddTestShortcut(GameObject owner)
    {
        if (Application.isEditor)
        {
            owner.AddComponent<MissionTestShortcut>();
        }
    }

    private static void StartNewRunIfNeeded()
    {
        if (MissionManager.CurrentMission == null || IsFinished)
        {
            StartNewRun();
        }
    }

    public static void StartNewRun()
    {
        if (campaignLost)
        {
            Campaign.StartOver();
            Resources.money = Resources.StartingMoney;
            campaignLost = false;
        }
        Resources.storage = 0f;
        Resources.time = 0f;
        Resources.water = 0f;
        Resources.food = 0f;
        Resources.oxygen = 0f;
        Resources.fuel = 0f;
        Resources.mass = 0f;
        Resources.research = 0f;
        Resources.tools.Clear();
        RocketBuild.Clear();

        MissionManager.PickRandomMission();
        IsFinished = false;
    }

    public static void Win(string reason)
    {
        if (IsFinished)
        {
            return;
        }

        Mission mission = MissionManager.CurrentMission;
        float payout = Campaign.PayOutForSuccess();
        string details = $"{reason}\n\nPayout: +${payout:0}\nMissions completed: {Campaign.MissionsCompleted}\n"
            + $"Next mission pays ${Campaign.CurrentPayout:0} and the crew uses x{Campaign.SupplyUseMultiplier:0.0} supplies.";
        Finish(true, "MISSION COMPLETE", mission, details, "NEXT MISSION");
    }

    public static void Lose(string reason)
    {
        if (IsFinished)
        {
            return;
        }

        campaignLost = true;
        string details = $"{reason}\n\nCampaign over. Missions completed: {Campaign.MissionsCompleted}";
        Finish(false, "MISSION FAILED", MissionManager.CurrentMission, details, "NEW CAMPAIGN");
    }

    private static void Finish(bool won, string headline, Mission mission, string details, string continueLabel)
    {
        IsFinished = true;
        Time.timeScale = 0f;
        MissionUI.ShowResult(won, headline, mission, details, continueLabel);
    }

    public static void PlayNewMission()
    {
        MissionManager.ClearCurrentMission();
        Time.timeScale = 1f;
        SceneManager.LoadScene(GameScenes.Shop);
    }

    public static void ReturnToMainMenu()
    {
        MissionManager.ClearCurrentMission();
        Time.timeScale = 1f;
        SceneManager.LoadScene(GameScenes.Menu);
    }
}
