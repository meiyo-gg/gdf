using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveItem : MonoBehaviour
{
    public void RemoveItemFromInventory()
    {
        InventoryManager.Instance.Remove();
    }
}
