using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.HighDefinition;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private GameObject inventoryItem;
    private Transform itemContent;
    private GameObject selectedCategoryButton;
    private Category selectedCategory = Category.All;
    private GameObject selectedItemButton;

    public Transform ItemContent { get { return itemContent; } }
    public GameObject SelectedCategoryButton { get { return selectedCategoryButton; } set { selectedCategoryButton = value; } }
    public GameObject SelectedItemButton { get { return selectedItemButton; } set { selectedItemButton = value; } }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    public void Add(Item item)
    {
        items.Add(item);
    }

    public void Remove(Item item)
    {
        items.Remove(item);
    }

    public void UpdateSelectedCategoryButton(GameObject newButton)
    {
        selectedCategoryButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255); // white

        selectedCategoryButton = newButton;

        selectedCategoryButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255); // black
    }

    public void UpdateSelectedItemButton(GameObject newButton)
    {
        if (selectedItemButton != null)
            selectedItemButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255); // white

        selectedItemButton = newButton;

        selectedItemButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255); // black
    }

    public void ListItems()
    {
        if (itemContent == null)
            itemContent = GameObject.FindWithTag("InventoryContent").transform;

        foreach (Transform item in itemContent)
            Destroy(item.gameObject);

        selectedItemButton = null;

        if (selectedCategoryButton != null)
        selectedCategory = selectedCategoryButton.GetComponent<ButtonCategory>().Category;

        foreach (Item item in items)
        {
            bool alreadyInList = false;

            if (item.category == selectedCategory || selectedCategory == Category.All) // Check if the item belongs to the selected category, or if the selected category is All Items
            {
                foreach (Transform t in itemContent)
                {
                    TextMeshProUGUI textMesh = t.GetChild(0).GetComponent<TextMeshProUGUI>();
                    if (textMesh.text == item.name)
                    {
                        // increase quantity instead of adding the item to the list
                        alreadyInList = true;
                        break;
                    }
                }

                if (!alreadyInList)
                {
                    GameObject obj = Instantiate(inventoryItem, itemContent);
                    TextMeshProUGUI itemName = obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    itemName.text = item.itemName;
                }
            }
        }
    }
}
