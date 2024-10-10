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
            Input.GetAxis("Horizontal") * Time.deltaTime * 3.0f,
            0,
            Input.GetAxis("Vertical") * Time.deltaTime * 3.0f));

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
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            inventory.SwitchItem(-1);
            Debug.Log($"아이템을 바꾸었습니다. 순서 : {inventory.inventoryIndex} {inventory.inventory[inventory.inventoryIndex].id}");
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            inventory.SwitchItem(1);
            Debug.Log($"아이템을 바꾸었습니다. 순서 : {inventory.inventoryIndex} {inventory.inventory[inventory.inventoryIndex].id}");
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
            answer.AppendLine($"[아이템 아이디 {item.id}, 아이템 갯수 {item.amount}]");
        }

        Debug.Log(answer.ToString());
    }
}
