using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    Transform player;
    float time = 0.1f;
    float radious = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        player = AdobePlayerReference.playerInstance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + new Vector3(Mathf.Sin(Time.time / time), 0, Mathf.Cos(Time.time / time)) * radious;
    }
}
