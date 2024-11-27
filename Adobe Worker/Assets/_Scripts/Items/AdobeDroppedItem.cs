using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class AdobeDroppedItem : MonoBehaviour
{
    AdobeItemPack thisItems;
    bool itemExists = false;

    /// <summary>
    /// Combine Nearby Items
    /// </summary>
    public AnimationCurve combineCurve;
    public float combineDistance = 1.5f;


	void Start()
	{
		thisItems = GetComponent<AdobeItemPack>();
		GetComponent<AdobeTagBundle>().AddReceiveAction(GiveItem);
	}

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
			AdobeItemBase curItem = thisItems.inventory[index];

			playerInventory.inventory.Add(curItem);
            Debug.Log($"������ ȹ�� - ������ ��ȣ : {curItem.Id}, ���� : {curItem.amount}");

			foreach (ItemEntry entry in PlayerData.instance.items)
			{
                if (entry.id == curItem.Id)
                {
					entry.amount += curItem.amount;
					itemExists = true;
					break;
				}
            }
            if (!itemExists)
            {
                // �������� ������ ���� �߰�
                PlayerData.instance.items.Add(new ItemEntry(curItem.Id, curItem.amount));
            }
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// �Ȱ��� �������� ���� ������ ���� �� ����Ͽ� �ϳ��� ��ġ�� ���
    /// </summary>
    void CombineNearbyItem()
    {
        
	}

	void OnDrawGizmos()
	{
        Gizmos.color = Color.red;

        //Physics.SphereCast()
	}

}
