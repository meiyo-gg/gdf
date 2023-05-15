using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public Item item;

    public void Pickup()
    {
        InventoryManager.Instance.Add(item);
        Destroy(gameObject);

        if (InventoryManager.Instance.ItemContent != null)
            InventoryManager.Instance.ListItems();
    }



}
