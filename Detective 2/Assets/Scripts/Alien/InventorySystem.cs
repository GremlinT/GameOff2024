using System.Collections.Generic;

using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    private List<PickableItem> itemsInInventory = new List<PickableItem>();

    public void AddItemToInventory(PickableItem item)
    {
        itemsInInventory.Add(item);
    }

    public bool IsItemInInventory(int id)
    {
        for (int i = 0; i < itemsInInventory.Count; i++)
        {
            if (itemsInInventory[i].id == id) return true;
        }
        return false;
    }
}
