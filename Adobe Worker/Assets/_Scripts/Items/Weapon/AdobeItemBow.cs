using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeItemBow : AdobeItemConsumable
{
    private void Awake()
    {
        m_id = 207;
    }

    public override void Use(AdobeItemUseArguments arguments)
    {
        if (itemPack == null)
        {
            Debug.Log($"<!>AdobeItemBow.Use(AdobeItemUseArguments arguments) : {gameObject.name}의 아이템 팩이 존재하지 않음!");
            return;
        }

        int arrowId = -1;
        if (itemPack.IsExist(SimpleWeaponSpec.ITEM_ID_IRON_ARROW))
        {
            arrowId = SimpleWeaponSpec.ITEM_ID_IRON_ARROW;
        }
        else if (itemPack.IsExist(SimpleWeaponSpec.ITEM_ID_STONE_ARROW))
        {
            arrowId = SimpleWeaponSpec.ITEM_ID_STONE_ARROW;
        }
        else
        {
            Debug.Log($"<!>AdobeItemBow.Use(AdobeItemUseArguments arguments) : {gameObject.name}의 아이템 팩에 화살이 없음!");
            return;
        }

        itemPack.Remove(arrowId);
            
        GameObject instantiated = Instantiate(
            AdobePrefabManager.arrowRange,
            arguments.itemUser.transform.position + 2.0f * arguments.direction,
            arguments.rotation);
        instantiated.GetComponent<AdobeAttackRange>().SetDamage(SimpleWeaponSpec.Get(arrowId).damage);

        Consume(this);
    }
}
