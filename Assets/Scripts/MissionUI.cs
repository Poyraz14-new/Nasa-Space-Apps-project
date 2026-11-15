using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public static class MissionUI
{
    public static readonly Color Frame = new Color32(0x1A, 0x22, 0x29, 0xFF);
    public static readonly Color PanelFill = new Color32(0x26, 0x32, 0x3C, 0xFF);
    public static readonly Color PanelInner = new Color32(0x3F, 0x50, 0x5E, 0xFF);
    public static readonly Color Slot = new Color32(0x1D, 0x26, 0x2E, 0xFF);
    public static readonly Color Accent = new Color32(0xE0, 0xA8, 0x38, 0xFF);
    public static readonly Color AccentShade = new Color32(0x9E, 0x6A, 0x1E, 0xFF);
    public static readonly Color HazardDark = new Color32(0x22, 0x2B, 0x33, 0xFF);
    public static readonly Color DarkPlateBorder = new Color32(0x3B, 0x4B, 0x58, 0xFF);
    public static readonly Color DarkPlateFill = new Color32(0x2B, 0x37, 0x42, 0xFF);
    public static readonly Color LightText = new Color32(0xC0, 0xC9, 0xD0, 0xFF);
    public static readonly Color MutedText = new Color32(0x8F, 0xA0, 0xAD, 0xFF);
    public static readonly Color DarkText = new Color32(0x26, 0x32, 0x3C, 0xFF);
    public static readonly Color Success = new Color32(0x6C, 0xC0, 0x70, 0xFF);
    public static readonly Color Failure = new Color32(0xD9, 0x57, 0x4A, 0xFF);
    public static readonly Color LedCyan = new Color32(0x6E, 0xC4, 0xD8, 0xFF);
    public static readonly Color LedYellow = new Color32(0xE0, 0xB0, 0x48, 0xFF);
    public static readonly Color LedOrange = new Color32(0xD8, 0x78, 0x44, 0xFF);
    public static readonly Color Dim = new Color32(0x1D, 0x26, 0x2E, 0xD9);

    private const int PanelBorder = 8;
    private const int PlateScrewStart = 14;
    private const int PlateScrewSize = 6;
    private const float LedSize = 15f;

    private static TMP_FontAsset gameFont;
    private static Sprite panelSprite;
    private static Sprite orangePlateSprite;
    private static Sprite darkPlateSprite;
    private static Sprite hazardSprite;
    private static Sprite compassDialSprite;
    private static Sprite compassArrowSprite;

    private static readonly string[] CompassArrowPixels =
    {
        "....#....",
        "...#A#...",
        "..#AAA#..",
        ".#AAAAA#.",
        "#AAAAAAA#",
        "###AAA###",
        "..#AAA#..",
        "..#AAA#..",
        "..#AAA#..",
        "..#AAA#..",
        "..#AAA#..",
        "..#####..",
    };

    public class Compass
    {
        public RectTransform Arrow;
        public TMP_Text Label;
    }

    public static void RememberSceneFont()
    {
        foreach (TMP_Text text in Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (text.font != null && text.font.name.Contains("Monocraft"))
            {
                gameFont = text.font;
                return;
            }
        }
    }

    public static Canvas CreateCanvas(string canvasName, int sortingOrder)
    {
        EnsureEventSystem();

        GameObject canvasObject = new GameObject(canvasName, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        eventSystemObject.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
    }

    public static RectTransform CreateDim(Transform parent)
    {
        RectTransform dim = CreateImage(parent, "Dim", Dim);
        Stretch(dim);
        return dim;
    }

    public static RectTransform CreatePanel(Transform parent, string panelName)
    {
        RectTransform panel = CreateImage(parent, panelName, Color.white);
        Image image = panel.GetComponent<Image>();
        image.sprite = PanelSprite();
        image.type = Image.Type.Sliced;
        return panel;
    }

    public static RectTransform CreateImage(Transform parent, string objectName, Color color)
    {
        GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(parent, false);
        imageObject.GetComponent<Image>().color = color;
        return (RectTransform)imageObject.transform;
    }

    public static void StackChildren(RectTransform panel, int padding, float spacing, bool fitHeight)
    {
        VerticalLayoutGroup column = panel.gameObject.AddComponent<VerticalLayoutGroup>();
        column.padding = new RectOffset(padding, padding, padding, padding);
        column.spacing = spacing;
        column.childControlWidth = true;
        column.childControlHeight = true;
        column.childForceExpandWidth = true;
        column.childForceExpandHeight = false;

        if (fitHeight)
        {
            ContentSizeFitter fitter = panel.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    public static void PlaceTopLeft(RectTransform rect, float x, float y, float width, float height)
    {
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, height);
    }

    public static void PlaceTopRight(RectTransform rect, float rightMargin, float y, float width)
    {
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(-rightMargin, -y);
        rect.sizeDelta = new Vector2(width, 0);
    }

    public static void PlaceTopCenter(RectTransform rect, float y, float width)
    {
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.anchoredPosition = new Vector2(0, -y);
        rect.sizeDelta = new Vector2(width, 0);
    }

    public static void PlaceCenter(RectTransform rect, float width)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(width, 0);
    }

    public static TMP_Text CreateText(Transform parent, string content, float fontSize, Color color)
    {
        GameObject textObject = new GameObject("Text", typeof(RectTransform));
        textObject.transform.SetParent(parent, false);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        if (gameFont != null)
        {
            text.font = gameFont;
        }
        text.fontSize = fontSize;
        text.color = color;
        text.text = content;
        text.raycastTarget = false;
        return text;
    }

    public static void CreateHeader(Transform parent, string title, Color titleColor)
    {
        RectTransform row = CreateRow(parent, "Header", 0, 8);
        TMP_Text label = CreateText(row, title, 18, titleColor);
        label.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
        foreach (Color ledColor in new[] { LedOrange, LedYellow, LedCyan })
        {
            CreateLed(row, ledColor);
        }
    }

    public static void CreateHazardStripe(Transform parent)
    {
        RectTransform stripe = CreateImage(parent, "Hazard Stripe", Color.white);
        Image image = stripe.GetComponent<Image>();
        image.sprite = HazardSprite();
        image.type = Image.Type.Tiled;
        image.raycastTarget = false;
        SetHeight(stripe, 21);
    }

    public static void CreateTextBlock(Transform parent, string content, float fontSize)
    {
        RectTransform block = CreateImage(parent, "Text Block", Slot);
        block.GetComponent<Image>().raycastTarget = false;
        VerticalLayoutGroup column = block.gameObject.AddComponent<VerticalLayoutGroup>();
        column.padding = new RectOffset(16, 16, 12, 14);
        column.childControlWidth = true;
        column.childControlHeight = true;
        column.childForceExpandWidth = true;
        column.childForceExpandHeight = false;
        CreateText(block, content, fontSize, LightText);
    }

    public static TMP_Text CreateSlotRow(Transform parent, string content, float height, float fontSize)
    {
        RectTransform row = CreateSlotRowFrame(parent, height);
        TMP_Text label = CreateText(row, content, fontSize, LightText);
        label.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
        return label;
    }

    public static Button CreateButton(Transform parent, string label, float height, UnityAction onClick, bool primary)
    {
        RectTransform buttonRect = CreateImage(parent, label + " Button", Color.white);
        Image image = buttonRect.GetComponent<Image>();
        image.sprite = primary ? OrangePlateSprite() : DarkPlateSprite();
        image.type = Image.Type.Sliced;

        Button button = buttonRect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);
        SetHeight(buttonRect, height);

        TMP_Text text = CreateText(buttonRect, label, 27, primary ? DarkText : LightText);
        text.alignment = TextAlignmentOptions.Center;
        Stretch((RectTransform)text.transform);
        return button;
    }

    public static RectTransform CreateButtonRow(Transform parent, float height)
    {
        RectTransform row = CreateRow(parent, "Buttons", 0, 24);
        HorizontalLayoutGroup layout = row.GetComponent<HorizontalLayoutGroup>();
        layout.childForceExpandWidth = true;
        SetHeight(row, height);
        return row;
    }

    public static Compass CreateCompass(Transform parent, float dialSize)
    {
        RectTransform dial = CreateImage(parent, "Compass", Color.white);
        Image dialImage = dial.GetComponent<Image>();
        dialImage.sprite = CompassDialSprite();
        dialImage.raycastTarget = false;
        dial.anchorMin = new Vector2(1, 1);
        dial.anchorMax = new Vector2(1, 1);
        dial.pivot = new Vector2(1, 1);
        dial.anchoredPosition = new Vector2(-36, -36);
        dial.sizeDelta = new Vector2(dialSize, dialSize);

        float pixelScale = dialSize / 40f;
        RectTransform arrow = CreateImage(dial, "Arrow", Color.white);
        Image arrowImage = arrow.GetComponent<Image>();
        arrowImage.sprite = CompassArrowSprite();
        arrowImage.raycastTarget = false;
        arrow.anchorMin = new Vector2(0.5f, 0.5f);
        arrow.anchorMax = new Vector2(0.5f, 0.5f);
        arrow.pivot = new Vector2(0.5f, 0.5f);
        arrow.anchoredPosition = Vector2.zero;
        arrow.sizeDelta = new Vector2(9 * pixelScale, 12 * pixelScale);

        TMP_Text label = CreateText(dial, "", 18, LightText);
        label.alignment = TextAlignmentOptions.Center;
        RectTransform labelRect = (RectTransform)label.transform;
        labelRect.anchorMin = new Vector2(0.5f, 0);
        labelRect.anchorMax = new Vector2(0.5f, 0);
        labelRect.pivot = new Vector2(0.5f, 1);
        labelRect.anchoredPosition = new Vector2(0, -8);
        labelRect.sizeDelta = new Vector2(220, 24);

        return new Compass { Arrow = arrow, Label = label };
    }

    public static void ShowConfirm(string message, string confirmLabel, UnityAction onConfirm, string cancelLabel)
    {
        Canvas canvas = CreateCanvas("Confirm", 150);
        CreateDim(canvas.transform);
        RectTransform panel = CreatePanel(canvas.transform, "Confirm Panel");
        PlaceCenter(panel, 960);
        StackChildren(panel, PanelBorder + 28, 16, true);

        CreateHeader(panel, "WARNING", Failure);
        CreateHazardStripe(panel);
        CreateTextBlock(panel, message, 27);

        RectTransform buttons = CreateButtonRow(panel, 78);
        GameObject dialog = canvas.gameObject;
        CreateButton(buttons, cancelLabel, 78, () => Object.Destroy(dialog), false);
        CreateButton(buttons, confirmLabel, 78, () =>
        {
            Object.Destroy(dialog);
            onConfirm();
        }, true);
    }

    public static void ShowResult(bool won, string headline, Mission mission, string details, string continueLabel)
    {
        Canvas canvas = CreateCanvas("Mission Result", 200);
        CreateDim(canvas.transform);
        RectTransform panel = CreatePanel(canvas.transform, "Result Panel");
        PlaceCenter(panel, 1040);
        StackChildren(panel, PanelBorder + 28, 16, true);

        CreateHeader(panel, "MISSION REPORT", Accent);
        CreateHazardStripe(panel);
        CreateText(panel, headline, 45, won ? Success : Failure);
        if (mission != null)
        {
            CreateText(panel, $"{mission.Title}  ({mission.Inspiration})", 18, MutedText);
        }
        CreateTextBlock(panel, details, 27);

        RectTransform buttons = CreateButtonRow(panel, 78);
        CreateButton(buttons, "MAIN MENU", 78, MissionRun.ReturnToMainMenu, false);
        CreateButton(buttons, continueLabel, 78, MissionRun.PlayNewMission, true);
    }

    private static RectTransform CreateRow(Transform parent, string rowName, int sidePadding, float spacing)
    {
        GameObject rowObject = new GameObject(rowName, typeof(RectTransform));
        rowObject.transform.SetParent(parent, false);
        HorizontalLayoutGroup row = rowObject.AddComponent<HorizontalLayoutGroup>();
        row.padding = new RectOffset(sidePadding, sidePadding, 0, 0);
        row.spacing = spacing;
        row.childAlignment = TextAnchor.MiddleLeft;
        row.childControlWidth = true;
        row.childControlHeight = true;
        row.childForceExpandWidth = false;
        row.childForceExpandHeight = false;
        return (RectTransform)rowObject.transform;
    }

    private static RectTransform CreateSlotRowFrame(Transform parent, float height)
    {
        RectTransform row = CreateRow(parent, "Slot Row", 14, 14);
        Image background = row.gameObject.AddComponent<Image>();
        background.color = Slot;
        background.raycastTarget = false;
        SetHeight(row, height);
        return row;
    }

    private static Image CreateLed(Transform parent, Color color)
    {
        RectTransform led = CreateImage(parent, "LED", color);
        LayoutElement size = led.gameObject.AddComponent<LayoutElement>();
        size.minWidth = LedSize;
        size.preferredWidth = LedSize;
        size.minHeight = LedSize;
        size.preferredHeight = LedSize;
        Image image = led.GetComponent<Image>();
        image.raycastTarget = false;
        return image;
    }

    private static void SetHeight(RectTransform rect, float height)
    {
        LayoutElement size = rect.GetComponent<LayoutElement>();
        if (size == null)
        {
            size = rect.gameObject.AddComponent<LayoutElement>();
        }
        size.minHeight = height;
        size.preferredHeight = height;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static Sprite PanelSprite()
    {
        if (panelSprite == null)
        {
            panelSprite = BuildFramedSprite(24, 5, 3, Frame, PanelInner, PanelFill, false);
        }
        return panelSprite;
    }

    private static Sprite DarkPlateSprite()
    {
        if (darkPlateSprite == null)
        {
            darkPlateSprite = BuildFramedSprite(24, 5, 3, Frame, DarkPlateBorder, DarkPlateFill, false);
        }
        return darkPlateSprite;
    }

    private static Sprite OrangePlateSprite()
    {
        if (orangePlateSprite == null)
        {
            orangePlateSprite = BuildFramedSprite(48, 4, 5, Frame, AccentShade, Accent, true);
        }
        return orangePlateSprite;
    }

    private static Sprite HazardSprite()
    {
        if (hazardSprite != null)
        {
            return hazardSprite;
        }

        const int width = 30;
        const int height = 21;
        Texture2D texture = NewPixelTexture(width, height);
        texture.wrapMode = TextureWrapMode.Repeat;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool amberBand = (x + (height - 1 - y)) % 30 < 15;
                texture.SetPixel(x, y, amberBand ? Accent : HazardDark);
            }
        }
        texture.Apply();
        hazardSprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
        return hazardSprite;
    }

    private static Sprite CompassDialSprite()
    {
        if (compassDialSprite != null)
        {
            return compassDialSprite;
        }

        const int size = 40;
        Texture2D texture = NewPixelTexture(size, size);
        float center = (size - 1) / 2f;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Mathf.Sqrt((x - center) * (x - center) + (y - center) * (y - center));
                Color color = distance > 19.5f ? Color.clear : distance > 17.5f ? Frame : distance > 15.5f ? PanelInner : PanelFill;
                bool northTick = x >= 19 && x <= 20 && y >= 30 && y <= 33;
                bool sideTick = (y >= 19 && y <= 20 && (x <= 9 && x >= 6 || x >= 30 && x <= 33)) || (x >= 19 && x <= 20 && y >= 6 && y <= 9);
                if (northTick)
                {
                    color = Accent;
                }
                else if (sideTick)
                {
                    color = MutedText;
                }
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        compassDialSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        return compassDialSprite;
    }

    private static Sprite CompassArrowSprite()
    {
        if (compassArrowSprite != null)
        {
            return compassArrowSprite;
        }

        int width = CompassArrowPixels[0].Length;
        int height = CompassArrowPixels.Length;
        Texture2D texture = NewPixelTexture(width, height);
        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                char pixel = CompassArrowPixels[row][column];
                Color color = pixel == '#' ? Frame : pixel == 'A' ? Accent : Color.clear;
                texture.SetPixel(column, height - 1 - row, color);
            }
        }
        texture.Apply();
        compassArrowSprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
        return compassArrowSprite;
    }

    private static Sprite BuildFramedSprite(int size, int outlineWidth, int borderWidth, Color outline, Color border, Color fill, bool withScrews)
    {
        Texture2D texture = NewPixelTexture(size, size);
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int edgeDistance = Mathf.Min(Mathf.Min(x, y), Mathf.Min(size - 1 - x, size - 1 - y));
                Color color = edgeDistance < outlineWidth ? outline : edgeDistance < outlineWidth + borderWidth ? border : fill;
                if (withScrews && IsScrewPixel(x, size) && IsScrewPixel(y, size))
                {
                    color = border;
                }
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();

        float slice = withScrews ? PlateScrewStart + PlateScrewSize : outlineWidth + borderWidth;
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(slice, slice, slice, slice));
    }

    private static bool IsScrewPixel(int position, int size)
    {
        bool nearStart = position >= PlateScrewStart && position < PlateScrewStart + PlateScrewSize;
        bool nearEnd = position <= size - 1 - PlateScrewStart && position > size - 1 - PlateScrewStart - PlateScrewSize;
        return nearStart || nearEnd;
    }

    private static Texture2D NewPixelTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        return texture;
    }
}
