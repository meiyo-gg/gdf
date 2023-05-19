using UnityEngine;
using TMPro;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] private GameObject defaultCategoryButton;
    private void OnEnable()
    {
        GameObject selectedCategoryButton = InventoryManager.Instance.SelectedCategoryButton;

        if (selectedCategoryButton == null)
        {
            InventoryManager.Instance.SelectedCategoryButton = defaultCategoryButton;
            defaultCategoryButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255); // black
        }

  /*      GameObject selectedItemButton = InventoryManager.Instance.SelectedItemButton;

        if (selectedItemButton == null)
        {
            InventoryManager.Instance.SelectedItemButton
        }*/

        InventoryManager.Instance.ListItems();
    }
}
