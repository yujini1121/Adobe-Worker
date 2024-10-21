using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AdobeIDamageable
{
    /// <summary>
    ///     It called when gameobject get hurted by attack action
    /// </summary>
    void DoWhenDamaged(float damage);
}