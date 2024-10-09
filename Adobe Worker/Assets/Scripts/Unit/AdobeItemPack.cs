using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeItemPack : MonoBehaviour
{
    public List<AdobeItemBase> inventory;
    public int inventoryIndex;

    private void Start()
    {
        inventory = new List<AdobeItemBase>();
        inventoryIndex = 0;
    }

    public void Use(AdobeItemUseArguments arguments)
    {
        if (inventory.Count > 0)
        {
            inventory[inventoryIndex].Use(arguments);
        }
    }
    
    public void SwitchItem(int delta)
    {
        if (inventory.Count < 1)
        {
            return;
        }

        inventoryIndex = Mathf.Clamp(inventoryIndex + delta, 0, inventory.Count - 1);
    }
}
