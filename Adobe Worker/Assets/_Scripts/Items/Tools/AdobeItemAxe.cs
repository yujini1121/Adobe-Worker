using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeItemAxe : AdobeItemConsumable
{
    public override void Use(AdobeItemUseArguments arguments)
    {
        GameObject instantiated = Instantiate(
            AdobePrefabManager.axeRange,
            arguments.itemUser.transform.position + 2.0f * arguments.direction,
            arguments.rotation);
        instantiated.GetComponent<AdobeAttackRange>().SetDamage(SimpleWeaponSpec.Get(id).damage);

        Consume(this);
    }
}
