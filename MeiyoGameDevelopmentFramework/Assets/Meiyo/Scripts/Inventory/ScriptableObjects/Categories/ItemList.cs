using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/ItemList/Create")]
public class ItemList : ScriptableObject
{
    public CategoryType category;
    public List<Item> items;

    /*public ItemList()
    {
        Items = new List<Item>();
    }*/
}
