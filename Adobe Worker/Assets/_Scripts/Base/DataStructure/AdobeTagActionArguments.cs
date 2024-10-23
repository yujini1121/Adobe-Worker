using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeTagActionArguments
{
    public Collider other;
    public Collision collision;

    public void SetCollision(Collision collision)
    {
        this.collision = collision;
        this.other = collision.collider;
    }
}
