using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item/Create")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public Category category;
    public int value;
}
