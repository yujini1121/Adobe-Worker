using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This is a component that specifies the player.
///     Whatever the player GameObject is, it is based on this.
///     The player and GameObject are independent.
/// </summary>
public class AdobePlayerReference : MonoBehaviour
{
    public static GameObject playerInstance;

    private void Awake()
    {
        playerInstance = gameObject;
    }
}
