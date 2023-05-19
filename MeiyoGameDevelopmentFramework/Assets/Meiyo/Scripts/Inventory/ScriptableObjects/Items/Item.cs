using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item/Create")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public CategoryType category;
    public int value;
    public int quantityInInventory;
}
