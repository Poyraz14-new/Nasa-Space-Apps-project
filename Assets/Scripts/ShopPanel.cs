using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour
{
    [SerializeField] private TravelSystem travelSystem;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text messageText;

    private ShopButton selectedItem;
    private int quantity = 1;

    public int Quantity => quantity;
    public bool HasSelection => selectedItem != null;
    public float TotalPrice => selectedItem != null ? selectedItem.Price() * quantity : 0f;

    private void Awake()
    {
        ArrangeDescription();
        if (iconImage != null)
        {
            iconImage.enabled = false;
            iconImage.preserveAspect = true;
        }
    }

    private void ArrangeDescription()
    {
        PlaceOnScreen(iconImage, 1413, 156, 72, 72);
        PlaceText(nameText, 1515, 151, 342, 84, 27);
        PlaceText(descriptionText, 1401, 258, 456, 140, 18);
        PlaceText(statsText, 1401, 414, 456, 200, 18);
        PlaceText(priceText, 1401, 705, 456, 34, 27);
        PlaceText(messageText, 1630, 877, 254, 44, 18);
    }

    private static void PlaceText(TMP_Text text, float x, float y, float width, float height, float fontSize)
    {
        if (text == null)
        {
            return;
        }
        PlaceOnScreen(text, x, y, width, height);
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.TopLeft;
    }

    private static void PlaceOnScreen(Graphic graphic, float x, float y, float width, float height)
    {
        if (graphic == null || graphic.canvas == null)
        {
            return;
        }

        RectTransform rect = graphic.rectTransform;
        rect.SetParent(graphic.canvas.rootCanvas.transform, false);
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, height);
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
        rect.SetAsLastSibling();
    }

    public void Select(ShopButton item)
    {
        if (selectedItem != null)
        {
            selectedItem.SetHighlighted(false);
        }
        selectedItem = item;
        selectedItem.SetHighlighted(true);
        quantity = 1;

        if (iconImage != null)
        {
            iconImage.sprite = item.Icon;
            iconImage.enabled = true;
        }
        SetText(nameText, item.DisplayName);
        SetText(descriptionText, item.Description);
        SetText(statsText, item.StatsText());
        SetText(priceText, $"${item.Price()}");
        SetText(messageText, "");
    }

    public void IncreaseQuantity()
    {
        if (selectedItem != null)
        {
            quantity = Mathf.Min(quantity + 1, selectedItem.MaxQuantity);
        }
    }

    public void DecreaseQuantity()
    {
        quantity = Mathf.Max(1, quantity - 1);
    }

    public void BuySelected()
    {
        if (selectedItem == null)
        {
            SetText(messageText, "Pick an item first");
            return;
        }
        if (selectedItem.AlreadyOwned)
        {
            SetText(messageText, $"You already have the {selectedItem.DisplayName}");
            return;
        }

        int bought = 0;
        while (bought < quantity && selectedItem.Buy(travelSystem))
        {
            bought++;
        }

        if (bought == quantity)
        {
            SetText(messageText, quantity == 1 ? $"Bought {selectedItem.DisplayName}" : $"Bought {quantity} x {selectedItem.DisplayName}");
            return;
        }

        bool storageIsFull = travelSystem.CanAfford(selectedItem.Price());
        if (bought > 0)
        {
            SetText(messageText, $"Bought {bought} of {quantity}: {(storageIsFull ? "storage full" : "out of money")}");
        }
        else if (storageIsFull)
        {
            SetText(messageText, "Storage full: buy a Cabin or Body first");
        }
        else
        {
            SetText(messageText, $"Costs ${selectedItem.Price():0}, you have ${Resources.money:0}");
        }
    }

    private void SetText(TMP_Text field, string value)
    {
        if (field != null)
        {
            field.text = value;
        }
    }
}
