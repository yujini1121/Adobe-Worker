using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TempPlayerControllerAlpha : MonoBehaviour
{
    AdobeItemPack inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<AdobeItemPack>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(
            Input.GetAxis("Horizontal") * Time.deltaTime * 7.0f,
            0,
            Input.GetAxis("Vertical") * Time.deltaTime * 7.0f));

        UseItems();
        SwitchItems();
        ShowInventory();
    }

    void UseItems()
    {
        if (Input.GetKeyDown(KeyCode.U) == false)
        {
            return;
        }

        AdobeItemUseArguments args = new AdobeItemUseArguments();
        args.itemUser = gameObject;
        inventory.Use(args);
    }

    void SwitchItems()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            inventory.SwitchItem(-1);
            Debug.Log($"�������� �ٲپ����ϴ�. ���� : {inventory.InventoryIndex} {inventory.inventory[inventory.InventoryIndex].Id}");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            inventory.SwitchItem(1);
            Debug.Log($"�������� �ٲپ����ϴ�. ���� : {inventory.InventoryIndex} {inventory.inventory[inventory.InventoryIndex].Id}");
        }

    }

    void ShowInventory()
    {
        if (Input.GetKeyDown(KeyCode.I) == false)
        {
            return;
        }

        StringBuilder answer = new StringBuilder();
        foreach (AdobeItemBase item in inventory.inventory)
        {
            answer.AppendLine($"[������ ���̵� {item.Id}, ������ ���� {item.amount}]");
        }

        Debug.Log(answer.ToString());
    }
}
