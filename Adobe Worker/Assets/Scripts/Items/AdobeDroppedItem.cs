using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeDroppedItem : MonoBehaviour
{
    AdobeItemPack thisItems;

    void GiveItem(AdobeTagActionArguments arguments)
    {
        if (arguments.other == null)
        {
            return;
        }

        AdobeItemPack playerInventory = arguments.other.gameObject.GetComponent<AdobeItemPack>();
        if (playerInventory == null )
        {
            Debug.LogError("�÷��̾�� AdobeItemPack ������Ʈ�� �����ϴ�!");
            return;
        }

        for (int index = 0; index < thisItems.inventory.Count; ++index)
        {
            playerInventory.inventory.Add(thisItems.inventory[index]);
            Debug.Log($"������ ȹ�� : ������ ��ȣ : {thisItems.inventory[index].id}");

            PlayerData.instance.items.Add(thisItems.inventory[index].id);
        }


        gameObject.SetActive(false);
        //Destroy(gameObject);

        
        //PlayerData.instance.items += thisItems.inventory[].id;
    }


    // Start is called before the first frame update
    void Start()
    {
        thisItems = GetComponent<AdobeItemPack>();
        GetComponent<AdobeTagBundle>().AddAction(GiveItem);
    }
}
