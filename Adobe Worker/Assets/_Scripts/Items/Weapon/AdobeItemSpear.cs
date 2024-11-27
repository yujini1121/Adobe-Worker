using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeItemSpear : AdobeItemConsumable
{
    public override void Use(AdobeItemUseArguments arguments)
    {
        GameObject instantiated = Instantiate(
            AdobePrefabManager.spearRange,
            arguments.itemUser.transform.position + 2.0f * arguments.direction,
            arguments.rotation);
        instantiated.GetComponent<AdobeAttackRange>().SetDamage(SimpleWeaponSpec.Get(Id).damage);

        Consume(this);
    }
}
