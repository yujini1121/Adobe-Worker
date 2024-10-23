using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeStructureColliderChild : MonoBehaviour
{
	AdobeTagBundle parentTagBunde;

    // Start is called before the first frame update
    void Start()
    {
        parentTagBunde = transform.parent.GetComponent<AdobeTagBundle>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
		AdobeTagBundle otherTag = other.gameObject.GetComponent<AdobeTagBundle>();
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
		AdobeTagBundle otherTag = collision.gameObject.GetComponent<AdobeTagBundle>();
        if (otherTag == null)
        {
            return;
        }
        AdobeTagActionArguments arguments = new AdobeTagActionArguments();
        arguments.SetCollision(collision);
        parentTagBunde.WhenChildCollide(otherTag, arguments);
    }
}
