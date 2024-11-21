using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeItemSword : AdobeItemConsumable
{
    public override void Use(AdobeItemUseArguments arguments)
    {
        GameObject instantiated = Instantiate(
            AdobePrefabManager.swordRange,
            arguments.itemUser.transform.position + 2.0f * arguments.direction,
            arguments.rotation);
        instantiated.GetComponent<AdobeAttackRange>().SetDamage(SimpleWeaponSpec.Get(id).damage);

        Consume(this);
    }
}
