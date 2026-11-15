using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class ShopSetup
{
    private const string UndoName = "Set Up Shop";
    private const string ToolIconFolder = "Assets/Resources/" + UtilityCatalog.IconFolder;
    private const string SelectedFramePath = ToolIconFolder + "/selected_frame.png";

    private static readonly Color BodyTextColor = new Color32(0xC0, 0xC9, 0xD0, 0xFF);
    private static readonly Color PriceTextColor = new Color32(0xE0, 0xA8, 0x38, 0xFF);

    private static readonly List<string> report = new List<string>();

    [MenuItem("Tools/Set Up Shop")]
    private static void SetUpShop()
    {
        report.Clear();
        Undo.SetCurrentGroupName(UndoName);
        int undoGroup = Undo.GetCurrentGroup();

        Canvas canvas = Object.FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
        if (canvas == null)
        {
            EditorUtility.DisplayDialog(UndoName, "No Canvas found in the open scene. Open Part 1 first.", "OK");
            return;
        }

        Transform partsShelf = FindInChildren(canvas.transform, "PartsShelf");
        if (partsShelf == null)
        {
            EditorUtility.DisplayDialog(UndoName, "No object named PartsShelf under the Canvas.", "OK");
            return;
        }

        AssetDatabase.Refresh();
        PrepareIconImports();
        UseScaleWithScreenSize(canvas);

        GameObject shopManager = FindOrCreateRootObject(canvas.gameObject, "ShopManager");
        ShopPanel shopPanel = FindExistingOrAdd<ShopPanel>(shopManager);
        TravelSystem travelSystem = FindExistingOrAdd<TravelSystem>(shopManager);

        WireShopPanel(shopPanel, travelSystem, canvas.transform);

        Transform utilitiesShelf = FindInChildren(canvas.transform, "UtilitiesShelf");
        if (utilitiesShelf == null)
        {
            utilitiesShelf = CreateUtilitiesShelf(partsShelf);
            FillUtilityButtons(utilitiesShelf, shopPanel);
        }
        else
        {
            ConnectExistingUtilityButtons(utilitiesShelf, shopPanel);
        }

        SetUpPartButtons(partsShelf, shopPanel);
        WireTabs(partsShelf.gameObject, utilitiesShelf.gameObject);
        WireBuyButton(canvas.transform, shopPanel);

        EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
        Undo.CollapseUndoOperations(undoGroup);
        Selection.activeGameObject = shopManager;

        Debug.Log("Set Up Shop finished:\n- " + string.Join("\n- ", report));
        EditorUtility.DisplayDialog(UndoName, "Done. Details are in the Console.\n\nPress Ctrl+S to save the scene.\nCtrl+Z undoes everything this did.", "OK");
    }

    private static void PrepareIconImports()
    {
        if (!AssetDatabase.IsValidFolder(ToolIconFolder))
        {
            report.Add($"WARNING: {ToolIconFolder} is missing, so utility icons and selection frames are skipped.");
            return;
        }

        foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { ToolIconFolder }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                continue;
            }

            Vector4 border = path == SelectedFramePath ? new Vector4(5, 5, 5, 5) : Vector4.zero;
            bool alreadyCorrect = importer.textureType == TextureImporterType.Sprite
                && importer.spriteImportMode == SpriteImportMode.Single
                && importer.filterMode == FilterMode.Point
                && importer.textureCompression == TextureImporterCompression.Uncompressed
                && importer.spriteBorder == border;
            if (alreadyCorrect)
            {
                continue;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.spriteBorder = border;
            importer.SaveAndReimport();
        }
        report.Add("Tool icons imported as crisp pixel-art sprites");
    }

    private static void UseScaleWithScreenSize(Canvas canvas)
    {
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            return;
        }

        Undo.RecordObject(scaler, UndoName);
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        report.Add("Canvas Scaler set to Scale With Screen Size, 1920x1080");
    }

    private static void WireShopPanel(ShopPanel shopPanel, TravelSystem travelSystem, Transform canvas)
    {
        TMP_FontAsset font = FindMonocraftFont();
        SerializedObject panel = new SerializedObject(shopPanel);

        FillIfEmpty(panel, "travelSystem", () => travelSystem);
        FillIfEmpty(panel, "iconImage", () => FindOrCreateIcon(canvas, "ItemIcon", 1413, 156, 72, 72));
        FillIfEmpty(panel, "nameText", () => FindOrCreateText(canvas, "ItemName", 1515, 151, 342, 42, 36, BodyTextColor, font));
        FillIfEmpty(panel, "descriptionText", () => FindOrCreateText(canvas, "ItemDescription", 1401, 258, 456, 140, 27, BodyTextColor, font));
        FillIfEmpty(panel, "statsText", () => FindOrCreateText(canvas, "ItemStats", 1401, 414, 456, 200, 27, BodyTextColor, font));
        FillIfEmpty(panel, "priceText", () => FindOrCreateText(canvas, "ItemPrice", 1401, 705, 456, 34, 27, PriceTextColor, font));
        FillIfEmpty(panel, "messageText", () => FindOrCreateText(canvas, "BuyMessage", 1630, 877, 254, 34, 18, BodyTextColor, font));

        panel.ApplyModifiedProperties();
    }

    private static void FillIfEmpty(SerializedObject owner, string fieldName, System.Func<Object> createValue)
    {
        SerializedProperty field = owner.FindProperty(fieldName);
        if (field.objectReferenceValue != null)
        {
            report.Add($"ShopPanel.{fieldName} kept as you set it");
            return;
        }
        field.objectReferenceValue = createValue();
        report.Add($"ShopPanel.{fieldName} connected");
    }

    private static Transform CreateUtilitiesShelf(Transform partsShelf)
    {
        GameObject copy = Object.Instantiate(partsShelf.gameObject, partsShelf.parent);
        Undo.RegisterCreatedObjectUndo(copy, UndoName);
        copy.name = "UtilitiesShelf";
        copy.transform.SetSiblingIndex(partsShelf.GetSiblingIndex() + 1);
        foreach (PartButton leftover in copy.GetComponentsInChildren<PartButton>(true))
        {
            Object.DestroyImmediate(leftover);
        }
        copy.SetActive(false);
        report.Add("UtilitiesShelf created as a copy of PartsShelf's layout");
        return copy.transform;
    }

    private static void FillUtilityButtons(Transform utilitiesShelf, ShopPanel shopPanel)
    {
        List<Button> buttons = ButtonsInGridOrder(utilitiesShelf);
        for (int index = 0; index < buttons.Count; index++)
        {
            Button button = buttons[index];
            TMP_Text label = LabelFor(button, utilitiesShelf);

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

            Image image = button.GetComponent<Image>();
            Sprite icon = AssetDatabase.LoadAssetAtPath<Sprite>($"{ToolIconFolder}/{item.iconName}.png");
            if (image != null && icon != null)
            {
                image.sprite = icon;
                image.preserveAspect = true;
            }
            if (label != null)
            {
                label.text = item.label;
            }

            UtilityButton utility = button.gameObject.AddComponent<UtilityButton>();
            SerializedObject fields = new SerializedObject(utility);
            fields.FindProperty("shopPanel").objectReferenceValue = shopPanel;
            fields.FindProperty("displayName").stringValue = item.displayName;
            fields.FindProperty("description").stringValue = item.description;
            fields.FindProperty("selectedFrame").objectReferenceValue = EnsureSelectedFrame(button);
            fields.FindProperty("utilityType").enumValueIndex = (int)item.type;
            fields.FindProperty("price").floatValue = item.price;
            fields.FindProperty("amount").floatValue = item.amount;
            fields.FindProperty("storageUsed").floatValue = item.storageUsed;
            fields.FindProperty("toolMassKg").floatValue = item.toolMassKg;
            fields.ApplyModifiedPropertiesWithoutUndo();
            ClearOnClickEntries(button);
        }
        report.Add($"{Mathf.Min(buttons.Count, UtilityCatalog.Items.Length)} utility buttons filled (placeholder prices, change them in the Inspector)");
        if (buttons.Count < UtilityCatalog.Items.Length)
        {
            report.Add($"WARNING: only {buttons.Count} button slots, so {UtilityCatalog.Items.Length - buttons.Count} utility items were not added");
        }
    }

    private static void ConnectExistingUtilityButtons(Transform utilitiesShelf, ShopPanel shopPanel)
    {
        foreach (UtilityButton utility in utilitiesShelf.GetComponentsInChildren<UtilityButton>(true))
        {
            SerializedObject fields = new SerializedObject(utility);
            fields.FindProperty("shopPanel").objectReferenceValue = shopPanel;
            fields.ApplyModifiedProperties();
        }
        report.Add("Existing UtilitiesShelf kept; its utility buttons were connected to ShopPanel");
    }

    private static void SetUpPartButtons(Transform partsShelf, ShopPanel shopPanel)
    {
        int connected = 0;
        foreach (Button button in ButtonsInGridOrder(partsShelf))
        {
            Image image = button.GetComponent<Image>();
            string spriteName = image != null && image.sprite != null ? image.sprite.name.ToLower() : "";
            Match match = Regex.Match(spriteName, @"tier\s*(\d)\s*(cabin|body|thrust)");
            if (!match.Success && button.GetComponent<PartButton>() == null)
            {
                report.Add($"WARNING: skipped '{button.name}', its sprite '{spriteName}' does not look like tierNcabin/body/thrust");
                continue;
            }

            int tier = match.Success ? int.Parse(match.Groups[1].Value) : 1;
            PartButton.PartType type = !match.Success ? PartButton.PartType.Cabin
                : match.Groups[2].Value == "cabin" ? PartButton.PartType.Cabin
                : match.Groups[2].Value == "body" ? PartButton.PartType.Body
                : PartButton.PartType.Thruster;

            PartButton part = button.GetComponent<PartButton>();
            bool isNewPart = part == null;
            if (isNewPart)
            {
                part = Undo.AddComponent<PartButton>(button.gameObject);
            }

            TMP_Text label = LabelFor(button, partsShelf);
            SerializedObject fields = new SerializedObject(part);
            if (fields.FindProperty("shopPanel").objectReferenceValue == null)
            {
                fields.FindProperty("shopPanel").objectReferenceValue = shopPanel;
            }
            if (isNewPart)
            {
                fields.FindProperty("partType").enumValueIndex = (int)type;
                fields.FindProperty("tier").intValue = tier;
            }
            if (string.IsNullOrEmpty(fields.FindProperty("displayName").stringValue))
            {
                fields.FindProperty("displayName").stringValue = label != null ? label.text : $"T-{tier} {type}";
            }
            if (string.IsNullOrEmpty(fields.FindProperty("description").stringValue))
            {
                fields.FindProperty("description").stringValue = PartDescription(type);
            }
            fields.FindProperty("selectedFrame").objectReferenceValue = EnsureSelectedFrame(button);
            fields.ApplyModifiedProperties();
            connected++;
        }
        report.Add($"{connected} part buttons checked (your names, types and tiers were kept; selection frames added)");
    }

    private static void WireTabs(GameObject partsShelf, GameObject utilitiesShelf)
    {
        MenuButtons[] tabs = Object.FindObjectsByType<MenuButtons>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (tabs.Length != 2)
        {
            report.Add($"WARNING: expected 2 tabs with MenuButtons, found {tabs.Length}, so tabs were not wired");
            return;
        }

        MenuButtons partsTab = tabs.FirstOrDefault(tab => tab.name.ToLower().Contains("part")) ?? tabs[0];
        MenuButtons utilitiesTab = tabs.First(tab => tab != partsTab);
        SetUpTab(partsTab, utilitiesTab, true, partsShelf);
        SetUpTab(utilitiesTab, partsTab, false, utilitiesShelf);
        report.Add($"Tabs wired: '{partsTab.name}' shows PartsShelf and starts selected, '{utilitiesTab.name}' shows UtilitiesShelf");
    }

    private static void SetUpTab(MenuButtons tab, MenuButtons otherTab, bool startsSelected, GameObject page)
    {
        SerializedObject fields = new SerializedObject(tab);
        fields.FindProperty("otherTab").objectReferenceValue = otherTab;
        fields.FindProperty("startsSelected").boolValue = startsSelected;
        fields.FindProperty("page").objectReferenceValue = page;
        fields.ApplyModifiedProperties();
        ClearOnClickEntries(tab.GetComponent<Button>());
    }

    private static void WireBuyButton(Transform canvas, ShopPanel shopPanel)
    {
        Button buyButton = canvas.GetComponentsInChildren<Button>(true).FirstOrDefault(IsBuyButton);
        if (buyButton == null)
        {
            report.Add("WARNING: no BUY button found (looked for 'buy' in the name, sprite or label)");
            return;
        }

        for (int index = 0; index < buyButton.onClick.GetPersistentEventCount(); index++)
        {
            if (buyButton.onClick.GetPersistentTarget(index) == shopPanel && buyButton.onClick.GetPersistentMethodName(index) == nameof(ShopPanel.BuySelected))
            {
                report.Add($"'{buyButton.name}' already calls ShopPanel.BuySelected");
                return;
            }
        }

        Undo.RecordObject(buyButton, UndoName);
        UnityEventTools.AddPersistentListener(buyButton.onClick, shopPanel.BuySelected);
        report.Add($"'{buyButton.name}' On Click now calls ShopPanel.BuySelected");
    }

    private static bool IsBuyButton(Button button)
    {
        if (button.name.ToLower().Contains("buy"))
        {
            return true;
        }
        Image image = button.GetComponent<Image>();
        if (image != null && image.sprite != null && image.sprite.name.ToLower().Contains("buy"))
        {
            return true;
        }
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        return label != null && label.text.Trim().ToUpper() == "BUY";
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
        float nearestDistance = 250f;
        foreach (Transform sibling in shelf)
        {
            TMP_Text text = sibling.GetComponent<TMP_Text>();
            if (text == null)
            {
                continue;
            }
            Vector2 offset = sibling.localPosition - button.transform.localPosition;
            if (offset.y > 0f)
            {
                continue;
            }
            if (offset.magnitude < nearestDistance)
            {
                nearestDistance = offset.magnitude;
                nearest = text;
            }
        }
        return nearest;
    }

    private static GameObject EnsureSelectedFrame(Button button)
    {
        Transform existing = button.transform.Find("SelectedFrame");
        if (existing != null)
        {
            return existing.gameObject;
        }

        Sprite frameSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SelectedFramePath);
        if (frameSprite == null)
        {
            return null;
        }

        GameObject frame = new GameObject("SelectedFrame", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(frame, UndoName);
        frame.transform.SetParent(button.transform, false);
        RectTransform rect = (RectTransform)frame.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(-8, -8);
        rect.offsetMax = new Vector2(8, 8);

        Image image = frame.AddComponent<Image>();
        image.sprite = frameSprite;
        image.type = Image.Type.Sliced;
        image.pixelsPerUnitMultiplier = 0.5f;
        image.raycastTarget = false;
        frame.SetActive(false);
        return frame;
    }

    private static void ClearOnClickEntries(Button button)
    {
        if (button == null || button.onClick.GetPersistentEventCount() == 0)
        {
            return;
        }
        Undo.RecordObject(button, UndoName);
        for (int index = button.onClick.GetPersistentEventCount() - 1; index >= 0; index--)
        {
            UnityEventTools.RemovePersistentListener(button.onClick, index);
        }
    }

    private static Image FindOrCreateIcon(Transform canvas, string objectName, float x, float y, float width, float height)
    {
        Transform existing = FindInChildren(canvas, objectName);
        if (existing != null && existing.GetComponent<Image>() != null)
        {
            return existing.GetComponent<Image>();
        }

        GameObject iconObject = CreateUiObject(canvas, objectName, x, y, width, height);
        Image image = iconObject.AddComponent<Image>();
        image.preserveAspect = true;
        image.raycastTarget = false;
        image.enabled = false;
        return image;
    }

    private static TMP_Text FindOrCreateText(Transform canvas, string objectName, float x, float y, float width, float height, float fontSize, Color color, TMP_FontAsset font)
    {
        Transform existing = FindInChildren(canvas, objectName);
        if (existing != null && existing.GetComponent<TMP_Text>() != null)
        {
            return existing.GetComponent<TMP_Text>();
        }

        GameObject textObject = CreateUiObject(canvas, objectName, x, y, width, height);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        if (font != null)
        {
            text.font = font;
        }
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.raycastTarget = false;
        text.text = "";
        return text;
    }

    private static GameObject CreateUiObject(Transform parent, string objectName, float x, float y, float width, float height)
    {
        GameObject uiObject = new GameObject(objectName, typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(uiObject, UndoName);
        uiObject.transform.SetParent(parent, false);
        RectTransform rect = (RectTransform)uiObject.transform;
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, height);
        report.Add($"Created {objectName}");
        return uiObject;
    }

    private static T FindExistingOrAdd<T>(GameObject fallbackOwner) where T : Component
    {
        T existing = Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (existing != null)
        {
            return existing;
        }
        report.Add($"Added {typeof(T).Name} to {fallbackOwner.name}");
        return Undo.AddComponent<T>(fallbackOwner);
    }

    private static GameObject FindOrCreateRootObject(GameObject sceneMember, string objectName)
    {
        GameObject existing = sceneMember.scene.GetRootGameObjects().FirstOrDefault(root => root.name == objectName);
        if (existing != null)
        {
            return existing;
        }

        GameObject created = new GameObject(objectName);
        Undo.RegisterCreatedObjectUndo(created, UndoName);
        report.Add($"Created {objectName}");
        return created;
    }

    private static Transform FindInChildren(Transform root, string objectName)
    {
        return root.GetComponentsInChildren<Transform>(true).FirstOrDefault(child => child.name == objectName);
    }

    private static TMP_FontAsset FindMonocraftFont()
    {
        string guid = AssetDatabase.FindAssets("Monocraft t:TMP_FontAsset").FirstOrDefault();
        return guid == null ? null : AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(guid));
    }

    private static string PartDescription(PartButton.PartType type)
    {
        switch (type)
        {
            case PartButton.PartType.Cabin:
                return "Crew module. Adds storage space and shortens the trip.";
            case PartButton.PartType.Body:
                return "Main hull. Adds storage space and carries fuel.";
            default:
                return "Engine. Adds fuel for a faster trip.";
        }
    }
}
