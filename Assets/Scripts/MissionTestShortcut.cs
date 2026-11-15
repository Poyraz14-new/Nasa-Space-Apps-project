using UnityEngine;
using UnityEngine.InputSystem;

public class MissionTestShortcut : MonoBehaviour
{
    private const float TestSupplies = 50f;

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.f9Key.wasPressedThisFrame)
        {
            return;
        }

        Mission mission = MissionManager.CurrentMission;
        if (mission == null)
        {
            return;
        }

        foreach (string tool in mission.MissingTools())
        {
            Resources.tools.Add(tool);
        }
        Resources.food += TestSupplies;
        Resources.water += TestSupplies;
        Debug.Log($"Test shortcut (F9): gave the tools for '{mission.Title}' plus {TestSupplies} kg food and {TestSupplies} L water.");
    }
}
