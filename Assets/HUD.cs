using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text storageText;
    [SerializeField] private TMP_Text massText;

    private void Update()
    {
        moneyText.text = $"Money: {Resources.money:0}";
        storageText.text = $"Storage: {Resources.storage:0}";
        massText.text = $"Mass: {Resources.mass:0} kg";
    }
}