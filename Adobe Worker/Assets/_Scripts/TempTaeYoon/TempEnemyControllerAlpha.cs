using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemyControllerAlpha : MonoBehaviour
{
    AdobeTagBundle myTagBundle;

    void CollideHandler(AdobeTagActionArguments arguments)
    {
        Debug.Log("I'm attacked!");
    }

    // Start is called before the first frame update
    void Start()
    {
        myTagBundle = GetComponent<AdobeTagBundle>();
        myTagBundle.AddReceiveAction(CollideHandler);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
