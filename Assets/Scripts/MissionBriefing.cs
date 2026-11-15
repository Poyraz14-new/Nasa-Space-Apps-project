using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionBriefing : MonoBehaviour
{
    private const float RefreshSeconds = 0.25f;

    private Mission mission;
    private TravelSystem travelSystem;
    private Canvas canvas;
    private GameObject briefOverlay;
    private readonly List<TMP_Text> toolRows = new List<TMP_Text>();
    private TMP_Text suppliesRow;
    private TMP_Text moonTimeRow;
    private float nextRefreshTime;

    private void Start()
    {
        MissionUI.RememberSceneFont();
        mission = MissionManager.CurrentMission;
        travelSystem = FindFirstObjectByType<TravelSystem>();
        if (travelSystem == null)
        {
            travelSystem = gameObject.AddComponent<TravelSystem>();
        }

        canvas = MissionUI.CreateCanvas("Mission UI", 50);
        BuildMissionPanel();
        BuildLaunchButton();
        ShowBrief();
    }

    private void Update()
    {
        if (Time.unscaledTime < nextRefreshTime)
        {
            return;
        }
        nextRefreshTime = Time.unscaledTime + RefreshSeconds;
        RefreshMissionPanel();
    }

    private void BuildMissionPanel()
    {
        RectTransform panel = MissionUI.CreatePanel(canvas.transform, "Mission Panel");
        MissionUI.PlaceTopLeft(panel, 36, 762, 405, 0);
        MissionUI.StackChildren(panel, 20, 6, true);

        Button reopenBrief = panel.gameObject.AddComponent<Button>();
        reopenBrief.targetGraphic = panel.GetComponent<Image>();
        reopenBrief.onClick.AddListener(ShowBrief);

        MissionUI.CreateHeader(panel, $"MISSION {Campaign.MissionNumber} - VIEW BRIEF", MissionUI.Accent);
        MissionUI.CreateText(panel, mission.Title, 27, MissionUI.LightText);
        foreach (string tool in mission.RequiredTools)
        {
            toolRows.Add(MissionUI.CreateSlotRow(panel, tool, 26, 18));
        }
        suppliesRow = MissionUI.CreateSlotRow(panel, "", 26, 18);
        moonTimeRow = MissionUI.CreateSlotRow(panel, "", 26, 18);
        RefreshMissionPanel();
    }

    private void BuildLaunchButton()
    {
        Button launch = MissionUI.CreateButton(canvas.transform, "LAUNCH", 120, OnLaunchPressed, true);
        MissionUI.PlaceTopLeft((RectTransform)launch.transform, 1374, 924, 510, 120);
        launch.GetComponentInChildren<TMP_Text>().fontSize = 45;
    }

    private void RefreshMissionPanel()
    {
        if (suppliesRow == null)
        {
            return;
        }

        for (int index = 0; index < toolRows.Count; index++)
        {
            string tool = mission.RequiredTools[index];
            bool owned = Resources.tools.Contains(tool);
            toolRows[index].text = owned ? $"<s>{tool}</s>" : tool;
            toolRows[index].color = owned ? MissionUI.MutedText : MissionUI.LightText;
        }

        suppliesRow.text = $"Food {Resources.food:0.0}/{travelSystem.FoodNeededKg():0.0}  Water {Resources.water:0.0}/{travelSystem.WaterNeededKg():0.0}";
        moonTimeRow.text = $"Moon time: {CrewNeeds.FormatSeconds(travelSystem.MoonStaySeconds())}";
    }

    private void ShowBrief()
    {
        if (briefOverlay != null)
        {
            return;
        }

        Canvas briefCanvas = MissionUI.CreateCanvas("Mission Brief", 100);
        briefOverlay = briefCanvas.gameObject;
        MissionUI.CreateDim(briefCanvas.transform);

        RectTransform panel = MissionUI.CreatePanel(briefCanvas.transform, "Brief Panel");
        MissionUI.PlaceCenter(panel, 1080);
        MissionUI.StackChildren(panel, 36, 14, true);

        MissionUI.CreateHeader(panel, $"MISSION {Campaign.MissionNumber} BRIEF", MissionUI.Accent);
        MissionUI.CreateHazardStripe(panel);
        MissionUI.CreateText(panel, mission.Title, 45, MissionUI.LightText);
        MissionUI.CreateText(panel, $"Inspired by: {mission.Inspiration}", 18, MissionUI.MutedText);
        MissionUI.CreateText(panel, $"Payout: ${Campaign.CurrentPayout:0}    Crew supply use: x{Campaign.SupplyUseMultiplier:0.0}", 18, MissionUI.Accent);
        MissionUI.CreateTextBlock(panel, mission.Objective, 27);
        MissionUI.CreateText(panel, "TOOLS TO BUY", 18, MissionUI.Accent);
        foreach (string tool in mission.RequiredTools)
        {
            MissionUI.CreateSlotRow(panel, tool, 42, 27);
        }
        MissionUI.CreateButton(panel, "TO THE MARKET", 78, CloseBrief, true);
    }

    private void CloseBrief()
    {
        Destroy(briefOverlay);
        briefOverlay = null;
    }

    private void OnLaunchPressed()
    {
        if (MissionRun.IsFinished)
        {
            return;
        }

        List<string> problems = new List<string>();
        List<string> missingTools = mission.MissingTools();
        if (missingTools.Count > 0)
        {
            problems.Add($"Missing tools: {string.Join(", ", missingTools)}. The mission will fail without them.");
        }
        if (!travelSystem.HasEnoughSupplies())
        {
            problems.Add($"Not enough food or water for the {travelSystem.CalculateTravelDays():0.0}-day trip. The crew will not make it.");
        }

        if (problems.Count == 0)
        {
            Launch();
            return;
        }
        MissionUI.ShowConfirm(string.Join("\n\n", problems), "LAUNCH ANYWAY", Launch, "BACK TO MARKET");
    }

    private void Launch()
    {
        travelSystem.TravelToMoon();
    }
}
