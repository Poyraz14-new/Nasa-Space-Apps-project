using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuButtons : MonoBehaviour
{
    [SerializeField] private MenuButtons otherTab;
    [SerializeField] private bool startsSelected;
    [SerializeField] private GameObject page;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Color activeTextColor = new Color32(0x26, 0x32, 0x3C, 0xFF);
    [SerializeField] private Color inactiveTextColor = new Color32(0xC0, 0xC9, 0xD0, 0xFF);

    private Image background;
    private TMP_Text label;

    public Button Button { get; private set; }

    private void Awake()
    {
        Button = GetComponent<Button>();
        background = GetComponent<Image>();
        label = GetComponentInChildren<TMP_Text>();
        Button.onClick.AddListener(Select);
        SetSelected(startsSelected);
    }

    public void Connect(MenuButtons other, GameObject tabPage)
    {
        otherTab = other;
        page = tabPage;
    }

    public void Select()
    {
        SetSelected(true);
        if (otherTab != null)
        {
            otherTab.SetSelected(false);
        }
    }

    public void SetSelected(bool selected)
    {
        background.sprite = selected ? activeSprite : inactiveSprite;
        if (label != null)
        {
            label.color = selected ? activeTextColor : inactiveTextColor;
        }
        if (page != null)
        {
            page.SetActive(selected);
        }
    }
}
