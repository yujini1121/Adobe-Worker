using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeStructureBreakable : MonoBehaviour
{
    [SerializeField] int hp = 3;

    AdobeTagBundle myTags;

    void WhenGetAttacked(AdobeTagActionArguments arguments)
    {
        hp -= 1;

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myTags = GetComponent<AdobeTagBundle>();
        myTags.AddReceiveAction(WhenGetAttacked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
