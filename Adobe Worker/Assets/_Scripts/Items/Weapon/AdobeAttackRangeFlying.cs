using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeAttackRangeFlying : MonoBehaviour
{
    [SerializeField] float time;
    [SerializeField] float range;
    float startTime;
    Vector3 startPos;
    Vector3 endPos; // caching

    [Header("Debug")]
    [SerializeField] bool debugField;

    public void End(AdobeTagActionArguments arguments)
    {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (time <= 0)
        {
            Debug.LogWarning("AdobeAttackRangeFlying.Start() : time 값이 너무 작습니다!");
            time = 1.0f;
        }
        startTime = Time.time;
        startPos = transform.position;
        endPos = transform.position + range * transform.forward;

        if (debugField)
        {
            Debug.Log($"AdobeAttackRangeFlying.Start() : {startTime} {startPos} {endPos}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (debugField)
        {
            Debug.Log($"AdobeAttackRangeFlying.Update() : {(Time.time - startTime) / time} {Mathf.Clamp((Time.time - startTime) / time, 0, 1)}");
        }

        transform.position = Vector3.Lerp(startPos, endPos, Mathf.Clamp((Time.time - startTime) / time, 0, 1));
    }
}
