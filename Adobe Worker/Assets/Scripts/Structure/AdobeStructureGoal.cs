using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeStructureGoal : MonoBehaviour
{
    AdobeTagBundle myTags;

    void DoWhenReachedGoal(AdobeTagActionArguments arguments)
    {
        Debug.Log("�̰��� �����߽��ϴ�!");
    }


    // Start is called before the first frame update
    void Start()
    {
        myTags = GetComponent<AdobeTagBundle>();

        myTags.AddAction(DoWhenReachedGoal);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
