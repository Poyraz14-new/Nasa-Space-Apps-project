using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyBar : MonoBehaviour
{
    private const string BuyMethodName = nameof(ShopPanel.BuySelected);

    private ShopPanel shopPanel;
    private TMP_Text quantityText;
    private TMP_Text totalText;

    private void Start()
    {
        shopPanel = FindFirstObjectByType<ShopPanel>();
        if (shopPanel == null)
        {
            Debug.LogWarning("BuyBar: no ShopPanel in this scene.");
            enabled = false;
            return;
        }

        HideOldBuyButtons();

        Canvas canvas = MissionUI.CreateCanvas("Buy Bar", 45);
        Button buy = MissionUI.CreateButton(canvas.transform, "BUY", 78, shopPanel.BuySelected, true);
        MissionUI.PlaceTopLeft((RectTransform)buy.transform, 1374, 787, 231, 78);

        Button minus = MissionUI.CreateButton(canvas.transform, "-", 78, shopPanel.DecreaseQuantity, false);
        MissionUI.PlaceTopLeft((RectTransform)minus.transform, 1620, 787, 78, 78);

        RectTransform quantityBox = MissionUI.CreateImage(canvas.transform, "Quantity", MissionUI.Slot);
        MissionUI.PlaceTopLeft(quantityBox, 1713, 787, 78, 78);
        quantityText = MissionUI.CreateText(quantityBox, "1", 27, MissionUI.LightText);
        quantityText.alignment = TextAlignmentOptions.Center;
        StretchToParent((RectTransform)quantityText.transform);

        Button plus = MissionUI.CreateButton(canvas.transform, "+", 78, shopPanel.IncreaseQuantity, false);
        MissionUI.PlaceTopLeft((RectTransform)plus.transform, 1806, 787, 78, 78);

        totalText = MissionUI.CreateText(canvas.transform, "", 18, MissionUI.Accent);
        MissionUI.PlaceTopLeft((RectTransform)totalText.transform, 1374, 877, 240, 34);
    }

    private void Update()
    {
        quantityText.text = shopPanel.Quantity.ToString();
        totalText.text = shopPanel.HasSelection ? $"Total: ${shopPanel.TotalPrice:0}" : "";
    }

    private static void HideOldBuyButtons()
    {
        foreach (Button button in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            for (int index = 0; index < button.onClick.GetPersistentEventCount(); index++)
            {
                if (button.onClick.GetPersistentMethodName(index) == BuyMethodName)
                {
                    button.gameObject.SetActive(false);
                    break;
                }
            }
        }
    }

    private static void StretchToParent(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
