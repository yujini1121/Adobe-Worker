using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureBreakable : MonoBehaviour
{
    [SerializeField] int hp = 3;

    TagBundle myTags;

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
        myTags = GetComponent<TagBundle>();
        myTags.AddAction(WhenGetAttacked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
