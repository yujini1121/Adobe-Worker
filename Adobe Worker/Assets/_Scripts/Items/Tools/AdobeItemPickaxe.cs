using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeItemPickaxe : AdobeItemConsumable
{
    public override void Use(AdobeItemUseArguments arguments)
    {
        GameObject instantiated = Instantiate(
            AdobePrefabManager.pickaxeRange,
            arguments.itemUser.transform.position + 2.0f * arguments.direction,
            arguments.rotation);

        Consume(this);
    }
}
