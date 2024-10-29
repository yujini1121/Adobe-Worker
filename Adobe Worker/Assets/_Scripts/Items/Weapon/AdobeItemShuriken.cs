using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeItemShuriken : AdobeItemBase
{
    public override void Use(AdobeItemUseArguments arguments)
    {
        GameObject instantiated = Instantiate(
            AdobePrefabManager.shurikenRange,
            arguments.itemUser.transform.position + 2.0f * arguments.direction,
            arguments.rotation);
    }
}
