using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeItemSword : AdobeItemBase
{
    [SerializeField] GameObject attackRange;

    public override void Use(AdobeItemUseArguments arguments)
    {
        GameObject instantiated = Instantiate(attackRange, arguments.itemUser.transform.position, attackRange.transform.rotation);
    }
}
