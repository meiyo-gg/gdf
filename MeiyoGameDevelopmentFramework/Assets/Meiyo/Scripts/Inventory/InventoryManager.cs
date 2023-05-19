using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.HighDefinition;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private List<ItemList> categoryList = new List<ItemList>();
    [SerializeField] private GameObject itemButton;
    private Transform itemContent;
    private GameObject selectedCategoryButton;
    private CategoryType selectedCategory = CategoryType.All;
    private GameObject selectedItemButton;
    private Item selectedItem;

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
        // Loop through the list to find the category that matches the item to be added
        foreach(ItemList list in categoryList)
        {
            if (list.category == item.category)
            {
                if (item.quantityInInventory > 0)
                {
                    // Item exists in inventory, so increase quantityInInventory
                    item.quantityInInventory++;
                }
                else
                {
                    // Item does not exist in inventory, so add it and increase quantityInInventory
                    list.items.Add(item);
                    item.quantityInInventory++;
                }

                break;
            }
        }
    }

    public void Remove()
    {
        if (selectedItem != null)
        {
            foreach (ItemList list in categoryList)
            {
                if (list.category == selectedItem.category)
                {
                    if (selectedItem.quantityInInventory > 1)
                    {
                        // Multiple of this item type exists in inventory, so decrease quantityInInventory
                        selectedItem.quantityInInventory--;
                        AdjustButtonTextQuantity();
                    }
                    else
                    {
                        // There is only one of this item type in inventory, so remove it and set quantityInInventory to 0
                        list.items.Remove(selectedItem);
                        selectedItem.quantityInInventory = 0;
                        RemoveItemButton();
                    }

                    break;
                }
            }
        }
    }

    private void AdjustButtonTextQuantity()
    {
        selectedItemButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = selectedItem.itemName + " (" + selectedItem.quantityInInventory + ")";
    }

    private void RemoveItemButton()
    {
        Destroy(selectedItemButton);
        selectedItemButton = null;
        selectedItem = null;
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
        selectedItem = newButton.GetComponent<ButtonItem>().item;

        selectedItemButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255); // black
    }

    public void ListItems()
    {
        // Get the itemContent transform of the Inventory UI
        if (itemContent == null)
            itemContent = GameObject.FindWithTag("InventoryContent").transform;

        // Destroy all children of itemContent
        foreach (Transform item in itemContent)
            Destroy(item.gameObject);
        
        // No item buttons should be selected
        selectedItemButton = null;

        // Identify the category that the player has selected
        if (selectedCategoryButton != null)
            selectedCategory = selectedCategoryButton.GetComponent<ButtonCategory>().Category;

        bool categoriesMatch = false;

        // Check each category in the category list to find if it matches the selected category
        foreach (ItemList list in categoryList)
        {
            if (list.category == selectedCategory)
            {
                // If the categories match, list all items of that category and display the quantityInInventory for each item
                foreach (Item item in list.items)
                {
                    InstantiateItemButtons(item);
                }

                categoriesMatch = true;

                break;
            }
        }

        // The selected category is All, hence we will not have a match
        if (!categoriesMatch)
        {
            // Loop through all lists and instantiate all items that are in inventory
            foreach (ItemList list in categoryList)
            {
                foreach (Item item in list.items)
                {
                    InstantiateItemButtons(item);
                }
            }
        }
    }

    private void InstantiateItemButtons(Item item)
    {
        GameObject obj = Instantiate(itemButton, itemContent);
        obj.GetComponent<ButtonItem>().item = item;
        TextMeshProUGUI itemName = obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        itemName.text = item.itemName + " (" + item.quantityInInventory + ")";
    }
}
