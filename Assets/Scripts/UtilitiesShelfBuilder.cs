using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UtilitiesShelfBuilder : MonoBehaviour
{
    private const float LabelSearchRadius = 250f;
    private const float FrameThickness = 4f;
    private const float FrameMargin = 8f;

    private void Start()
    {
        GameObject partsShelf = FindInScene("PartsShelf");
        ShopPanel shopPanel = FindFirstObjectByType<ShopPanel>();
        if (partsShelf == null || shopPanel == null)
        {
            Debug.LogWarning("UtilitiesShelfBuilder: Part 1 needs an object named PartsShelf and a ShopPanel.");
            return;
        }

        GameObject utilitiesShelf = FindInScene("UtilitiesShelf");
        if (utilitiesShelf == null)
        {
            utilitiesShelf = BuildUtilitiesShelf(partsShelf, shopPanel);
        }

        AddSelectionFrames(partsShelf);
        AddSelectionFrames(utilitiesShelf);
        ConnectTabs(partsShelf, utilitiesShelf);
    }

    private static GameObject BuildUtilitiesShelf(GameObject partsShelf, ShopPanel shopPanel)
    {
        GameObject shelf = Instantiate(partsShelf, partsShelf.transform.parent);
        shelf.name = "UtilitiesShelf";
        shelf.transform.SetSiblingIndex(partsShelf.transform.GetSiblingIndex() + 1);

        foreach (Button copiedButton in shelf.GetComponentsInChildren<Button>(true))
        {
            if (copiedButton.GetComponent<ShopButton>() == null)
            {
                Destroy(copiedButton.gameObject);
            }
        }

        List<Button> buttons = ButtonsInGridOrder(shelf.transform)
            .Where(button => button.GetComponent<ShopButton>() != null)
            .ToList();
        for (int index = 0; index < buttons.Count; index++)
        {
            Button button = buttons[index];
            TMP_Text label = LabelFor(button, shelf.transform);
            button.onClick.RemoveAllListeners();
            foreach (ShopButton copiedItem in button.GetComponents<ShopButton>())
            {
                DestroyImmediate(copiedItem);
            }

            if (index >= UtilityCatalog.Items.Length)
            {
                button.gameObject.SetActive(false);
                if (label != null)
                {
                    label.gameObject.SetActive(false);
                }
                continue;
            }

            UtilityCatalog.Item item = UtilityCatalog.Items[index];
            button.name = item.label;
            if (label != null)
            {
                label.text = item.label;
            }

            Sprite icon = UnityEngine.Resources.Load<Sprite>($"{UtilityCatalog.IconFolder}/{item.iconName}");
            Image image = button.GetComponent<Image>();
            if (icon != null && image != null)
            {
                image.sprite = icon;
                image.preserveAspect = true;
            }
            else if (icon == null)
            {
                Debug.LogWarning($"UtilitiesShelfBuilder: no sprite at Resources/{UtilityCatalog.IconFolder}/{item.iconName}");
            }

            UtilityButton utility = button.gameObject.AddComponent<UtilityButton>();
            utility.Connect(shopPanel, item.displayName, item.description);
            utility.SetItem(item);
        }

        shelf.SetActive(false);
        return shelf;
    }

    private static void ConnectTabs(GameObject partsShelf, GameObject utilitiesShelf)
    {
        MenuButtons[] tabs = FindObjectsByType<MenuButtons>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (tabs.Length != 2)
        {
            Debug.LogWarning($"UtilitiesShelfBuilder: expected 2 tabs with MenuButtons, found {tabs.Length}.");
            return;
        }

        MenuButtons partsTab = tabs.FirstOrDefault(tab => tab.name.ToLower().Contains("part"));
        if (partsTab == null)
        {
            partsTab = tabs[0];
        }
        MenuButtons utilitiesTab = tabs.First(tab => tab != partsTab);

        partsTab.Connect(utilitiesTab, partsShelf);
        utilitiesTab.Connect(partsTab, utilitiesShelf);
        partsTab.Select();
    }

    private static void AddSelectionFrames(GameObject shelf)
    {
        foreach (ShopButton item in shelf.GetComponentsInChildren<ShopButton>(true))
        {
            if (!item.HasSelectedFrame)
            {
                item.SetSelectedFrame(CreateFrame(item.transform));
            }
        }
    }

    private static GameObject CreateFrame(Transform button)
    {
        Transform existing = button.Find("SelectedFrame");
        if (existing != null)
        {
            return existing.gameObject;
        }

        GameObject frame = new GameObject("SelectedFrame", typeof(RectTransform));
        frame.transform.SetParent(button, false);
        RectTransform rect = (RectTransform)frame.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(-FrameMargin, -FrameMargin);
        rect.offsetMax = new Vector2(FrameMargin, FrameMargin);

        AddFrameBar(frame.transform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -FrameThickness), Vector2.zero);
        AddFrameBar(frame.transform, new Vector2(0, 0), new Vector2(1, 0), Vector2.zero, new Vector2(0, FrameThickness));
        AddFrameBar(frame.transform, new Vector2(0, 0), new Vector2(0, 1), Vector2.zero, new Vector2(FrameThickness, 0));
        AddFrameBar(frame.transform, new Vector2(1, 0), new Vector2(1, 1), new Vector2(-FrameThickness, 0), Vector2.zero);
        return frame;
    }

    private static void AddFrameBar(Transform frame, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        RectTransform bar = MissionUI.CreateImage(frame, "Bar", MissionUI.Accent);
        bar.anchorMin = anchorMin;
        bar.anchorMax = anchorMax;
        bar.offsetMin = offsetMin;
        bar.offsetMax = offsetMax;
        bar.GetComponent<Image>().raycastTarget = false;
    }

    private static GameObject FindInScene(string objectName)
    {
        Transform match = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .FirstOrDefault(candidate => candidate.name == objectName);
        return match != null ? match.gameObject : null;
    }

    private static List<Button> ButtonsInGridOrder(Transform shelf)
    {
        return shelf.Cast<Transform>()
            .Select(child => child.GetComponent<Button>())
            .Where(button => button != null)
            .OrderByDescending(button => Mathf.Round(button.transform.localPosition.y / 100f))
            .ThenBy(button => button.transform.localPosition.x)
            .ToList();
    }

    private static TMP_Text LabelFor(Button button, Transform shelf)
    {
        TMP_Text childLabel = button.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault(text => !string.IsNullOrWhiteSpace(text.text));
        if (childLabel != null)
        {
            return childLabel;
        }

        TMP_Text nearest = null;
        float nearestDistance = LabelSearchRadius;
        foreach (Transform sibling in shelf)
        {
            TMP_Text text = sibling.GetComponent<TMP_Text>();
            if (text == null)
            {
                continue;
            }
            Vector2 offset = sibling.localPosition - button.transform.localPosition;
            if (offset.y > 0f || offset.magnitude >= nearestDistance)
            {
                continue;
            }
            nearestDistance = offset.magnitude;
            nearest = text;
        }
        return nearest;
    }
}
