using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

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
            Debug.LogError("플레이어에게 AdobeItemPack 컴포넌트가 없습니다!");
            return;
        }

        for (int index = 0; index < thisItems.inventory.Count; ++index)
        {
			AdobeItemBase curItem = thisItems.inventory[index];

			playerInventory.inventory.Add(curItem);
            Debug.Log($"아이템 획득 - 아이템 번호 : {curItem.id}, 수량 : {curItem.amount}");

            bool itemExists = false;
			foreach (ItemEntry entry in PlayerData.instance.items)
			{
                if (entry.id == curItem.id)
                {
					entry.amount = curItem.amount;
					itemExists = true;
					break;
				}
            }
            if (!itemExists)
            {
                // 아이템이 없으면 새로 추가
                PlayerData.instance.items.Add(new ItemEntry(curItem.id, curItem.amount));
            }
        }
        gameObject.SetActive(false);
    }


    void Start()
    {
        thisItems = GetComponent<AdobeItemPack>();
        GetComponent<AdobeTagBundle>().AddReceiveAction(GiveItem);
    }
}
