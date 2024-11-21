using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeItemAmulet : AdobeItemBase
{
    public override void Use(AdobeItemUseArguments arguments)
    {
        AdobePlayerController player = arguments.itemUser.GetComponent<AdobePlayerController>();

        if (player == null)
        {
            return;
        }
        if (player.DemandStamina(SimpleWeaponSpec.STAMINA_FOR_MAGIC) == false)
        {
            return;
        }

        GameObject instantiated = Instantiate(
            AdobePrefabManager.amuletRange,
            arguments.itemUser.transform.position + 2.0f * arguments.direction,
            arguments.rotation);
        instantiated.GetComponent<AdobeAttackRange>().SetDamage(SimpleWeaponSpec.Get(id).damage);
    }
}
