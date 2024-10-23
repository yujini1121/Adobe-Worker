using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AdobeItemPack : MonoBehaviour
{
    [Tooltip("코드로 접근해서 수정할 수 있으나, AdobeItemBase를 상속하는 컴포넌트를 어태치하고, 인스펙터 창에서 드래그 앤 드롭으로 해도 됩니다.")]
    public List<AdobeItemBase> inventory;
    public int inventoryIndex;
    [SerializeField] GameObject itemsPrefab;
    [SerializeField] float radius;

    private void Start()
    {
        if (inventory == null)
        {
            inventory = new List<AdobeItemBase>();
        }
        inventoryIndex = 0;
    }

    public void Use(AdobeItemUseArguments arguments)
    {
        if (inventory.Count > 0)
        {
            inventory[inventoryIndex].Use(arguments);
        }
    }
    
    public void SwitchItem(int delta)
    {
        if (inventory.Count < 1)
        {
            return;
        }

        inventoryIndex = Mathf.Clamp(inventoryIndex + delta, 0, inventory.Count - 1);
    }

    public void DropItemsAll()
    {
        for (int index = 0; index < inventory.Count; index++)
        {
            Vector3 pos = transform.position +
                new Vector3(
                    Random.Range(-radius, radius),
                    0, Random.Range(-radius, radius));
            pos.y = 0.2f;
            GameObject one = Instantiate(itemsPrefab,
                pos, itemsPrefab.transform.rotation);

            one.AddComponent(inventory[index].GetType());
            one.GetComponent<AdobeItemBase>().Paste(inventory[index]);
        }
    }
}
