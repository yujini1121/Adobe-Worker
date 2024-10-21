using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AdobeItemPack : MonoBehaviour
{
    [Tooltip("�ڵ�� �����ؼ� ������ �� ������, AdobeItemBase�� ����ϴ� ������Ʈ�� ����ġ�ϰ�, �ν����� â���� �巡�� �� ������� �ص� �˴ϴ�.")]
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
