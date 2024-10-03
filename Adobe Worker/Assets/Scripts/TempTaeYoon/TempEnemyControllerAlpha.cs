using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemyControllerAlpha : MonoBehaviour
{
    TagBundle myTagBundle;

    void CollideHandler()
    {
        Debug.Log("I'm attacked!");
    }

    // Start is called before the first frame update
    void Start()
    {
        myTagBundle = GetComponent<TagBundle>();
        myTagBundle.AddAction(CollideHandler);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
