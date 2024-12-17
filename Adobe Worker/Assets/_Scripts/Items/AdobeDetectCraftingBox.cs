using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdobeDetectCraftingBox : MonoBehaviour
{
    [Header("Craft")]
    [SerializeField] private bool isNearCraftingBox = false;
    [SerializeField] private LayerMask craftingBoxLayer;
    [SerializeField] private float interactionRange = 3f;

    [Space(10)] [Header("Assign")]
    [SerializeField] private GameObject craftingUI;

    private void Update()
    {
        DetectCraftingBox();
    }

    void DetectCraftingBox()
    {
        RaycastHit hit;
        bool isInRange = Physics.CheckSphere(transform.position, interactionRange, craftingBoxLayer);

        if (isInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                isNearCraftingBox = !isNearCraftingBox;
                craftingUI.SetActive(isNearCraftingBox);
            }
        }
        else
        {
            if (isNearCraftingBox)
            {
                isNearCraftingBox = false;
                craftingUI.SetActive(false);
            }
        }
    }
}