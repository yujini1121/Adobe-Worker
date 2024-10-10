using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureColliderChild : MonoBehaviour
{
    TagBundle parentTagBunde;

    // Start is called before the first frame update
    void Start()
    {
        parentTagBunde = transform.parent.GetComponent<TagBundle>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        TagBundle otherTag = other.gameObject.GetComponent<TagBundle>();
        if (otherTag == null)
        {
            return;
        }
        AdobeTagActionArguments arguments = new AdobeTagActionArguments();
        arguments.other = other;
        parentTagBunde.WhenChildCollide(otherTag, arguments);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TagBundle otherTag = collision.gameObject.GetComponent<TagBundle>();
        if (otherTag == null)
        {
            return;
        }
        AdobeTagActionArguments arguments = new AdobeTagActionArguments();
        arguments.SetCollision(collision);
        parentTagBunde.WhenChildCollide(otherTag, arguments);
    }
}
