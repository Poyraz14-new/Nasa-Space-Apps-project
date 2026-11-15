using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class ShopButton : MonoBehaviour
{
    [SerializeField] private ShopPanel shopPanel;
    [SerializeField] private string displayName;
    [SerializeField] [TextArea] private string description;
    [SerializeField] private GameObject selectedFrame;

    public string DisplayName => displayName;
    public bool HasSelectedFrame => selectedFrame != null;
    public string Description => description;
    public Sprite Icon
    {
        get
        {
            Image image = GetComponent<Image>();
            return image != null ? image.sprite : null;
        }
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(SelectThisItem);
        SetHighlighted(false);
    }

    private void SelectThisItem()
    {
        shopPanel.Select(this);
    }

    public void Connect(ShopPanel panel, string itemName, string itemDescription)
    {
        shopPanel = panel;
        displayName = itemName;
        description = itemDescription;
    }

    public void SetSelectedFrame(GameObject frame)
    {
        selectedFrame = frame;
        SetHighlighted(false);
    }

    public void SetHighlighted(bool highlighted)
    {
        if (selectedFrame != null)
        {
            selectedFrame.SetActive(highlighted);
        }
    }

    public virtual int MaxQuantity => 1;
    public virtual bool AlreadyOwned => false;

    public abstract float Price();
    public abstract string StatsText();
    public abstract bool Buy(TravelSystem travelSystem);
}
