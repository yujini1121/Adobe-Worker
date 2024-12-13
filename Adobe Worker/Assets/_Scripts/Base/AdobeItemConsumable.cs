using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeItemConsumable : AdobeItemBase
{
    public int Duability => duability;

    [SerializeField] protected int duability = 10;

    protected void Consume(AdobeItemBase self)
    {
        duability--;
        if (duability <= 0)
        {
            itemPack.Remove(self);
        }
    }
}
