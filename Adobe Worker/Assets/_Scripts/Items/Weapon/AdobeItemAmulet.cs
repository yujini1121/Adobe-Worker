using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeItemAmulet : AdobeItemBase
{
    public override void Use(AdobeItemUseArguments arguments)
    {
        GameObject instantiated = Instantiate(
            AdobePrefabManager.amuletRange,
            arguments.itemUser.transform.position + 2.0f * arguments.direction,
            arguments.rotation);
    }
}
