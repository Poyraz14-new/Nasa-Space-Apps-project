using TMPro;
using UnityEngine;

public class MoonSupplies : MonoBehaviour
{
    private const float WarningSeconds = 5f;

    private TMP_Text timeText;
    private TMP_Text suppliesText;
    private float daysOnMoon;

    private void Start()
    {
        MissionUI.RememberSceneFont();
        Canvas canvas = MissionUI.CreateCanvas("Moon Supplies", 50);
        RectTransform panel = MissionUI.CreatePanel(canvas.transform, "Supplies Panel");
        MissionUI.PlaceTopRight(panel, 36, 204, 340);
        MissionUI.StackChildren(panel, 20, 4, true);

        MissionUI.CreateText(panel, "SUPPLIES LEFT", 18, MissionUI.Accent);
        timeText = MissionUI.CreateText(panel, "", 27, MissionUI.LightText);
        suppliesText = MissionUI.CreateText(panel, "", 18, MissionUI.MutedText);
        Refresh();
    }

    private void Update()
    {
        if (MissionRun.IsFinished)
        {
            return;
        }

        float daysPassed = Time.deltaTime / CrewNeeds.SecondsPerMoonDay;
        float foodEaten = Mathf.Min(Resources.food, CrewNeeds.FoodPerDayKg * daysPassed);
        float waterDrunk = Mathf.Min(Resources.water, CrewNeeds.WaterPerDayLiters * daysPassed);
        Resources.food -= foodEaten;
        Resources.water -= waterDrunk;
        Resources.mass = Mathf.Max(0f, Resources.mass - foodEaten - waterDrunk);
        Resources.time += daysPassed;
        daysOnMoon += daysPassed;
        Refresh();

        if (Resources.food <= 0f || Resources.water <= 0f)
        {
            string runOut = Resources.food <= 0f && Resources.water <= 0f ? "food and water" : Resources.food <= 0f ? "food" : "water";
            MissionRun.Lose($"Your crew ran out of {runOut} after {daysOnMoon:0.0} days on the Moon, before the mission was done.\nBuy more MRE packs and water next time.");
        }
    }

    private void Refresh()
    {
        float secondsLeft = CrewNeeds.DaysOfSupplies(Resources.food, Resources.water) * CrewNeeds.SecondsPerMoonDay;
        timeText.text = CrewNeeds.FormatSeconds(secondsLeft);
        timeText.color = secondsLeft <= WarningSeconds ? MissionUI.Failure : MissionUI.LightText;
        suppliesText.text = $"Food {Resources.food:0.0} kg  Water {Resources.water:0.0} L";
    }
}
