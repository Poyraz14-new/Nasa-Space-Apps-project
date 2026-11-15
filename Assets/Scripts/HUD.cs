using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text storageText;
    [SerializeField] private TMP_Text massText;
    [SerializeField] private TMP_Text fuelText;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text waterText;



    private void Update()
    {
        moneyText.text = $"Money: {Resources.money:0} $";
        storageText.text = $"Storage: {Resources.storage:0} L";
        massText.text = $"Mass: {Resources.mass:0} kg";
        fuelText.text= $"Fuel: {Resources.fuel:0} L";
        foodText.text= $"Food: {Resources.food:0} kg";
        waterText.text= $"Water: {Resources.water:0} L";
    }
}