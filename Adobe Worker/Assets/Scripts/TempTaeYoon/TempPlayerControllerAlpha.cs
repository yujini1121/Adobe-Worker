using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayerControllerAlpha : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(
            Input.GetAxis("Horizontal") * Time.deltaTime * 3.0f,
            0,
            Input.GetAxis("Vertical") * Time.deltaTime * 3.0f));


    }
}
