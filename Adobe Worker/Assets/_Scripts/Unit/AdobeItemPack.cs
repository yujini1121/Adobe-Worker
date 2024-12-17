using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AdobeItemPack : MonoBehaviour
{
    [Tooltip("�ڵ�� �����ؼ� ������ �� ������, AdobeItemBase�� ����ϴ� ������Ʈ�� ����ġ�ϰ�, �ν����� â���� �巡�� �� ������� �ص� �˴ϴ�.")]

    [Header("Panels")]
    public List<Image> quickslotPanels;
    public List<Image> inventoryPanels;

    [Space(20f)]
    [Header("Real Inventory")]
    public List<AdobeItemBase> inventory;

    public int InventoryIndex { get => m_inventoryIndex; }
    private int m_inventoryIndex;

    [SerializeField] float radius;
    const int PLAYER_INVENTORY_MAX_SIZE = 24;   // 6 * 4 ����
    const int QUICK_SLOT_START_INDEX = 0;      // ��ũó�� �� �Ʒ����� �����Կ� �ش��ϴ� ����

    private void Start()
    {
        if (inventory == null)
        {
            inventory = new List<AdobeItemBase>();
        }
        m_inventoryIndex = 0;

        ShowQuickSlot();
        ShowInventory();
    }

    public void ShowInventory()
    {
        UpdateSlots(inventoryPanels, PLAYER_INVENTORY_MAX_SIZE);
    }

    public void ShowQuickSlot()
    {
        for (int i = 0; i < quickslotPanels.Count; i++)
        {
            // �������� ���������� �κ��丮�� 0~5ĭ�̹Ƿ�, 0 + i�� ���
            int inventoryIndex = QUICK_SLOT_START_INDEX + i;

            if (inventoryIndex < inventory.Count && inventory[inventoryIndex] != null)
            {
                quickslotPanels[i].sprite = inventory[inventoryIndex].sprite;
            }
            else
            {
                quickslotPanels[i].sprite = null;
            }
        }
    }

    /// <summary>
    /// ShowInventory()�� ShowQuickSlot()�� ¥�� ���� �������� ��ġ�� �κ��� ���� ���� ���׾��.
    /// </summary>
    /// <param name="slots"></param>
    /// <param name="maxCount"></param>
    private void UpdateSlots(List<Image> slots, int maxCount)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < inventory.Count && i < maxCount && inventory[i] != null)
            {
                slots[i].sprite = inventory[i].sprite;
                slots[i].enabled = true;
            }
            else
            {
                slots[i].sprite = null;
                slots[i].enabled = false;
            }
        }
    }

    public void SwapItems(int fromIndex, int toIndex)
    {
        if (inventory == null || fromIndex < 0 || toIndex < 0 || fromIndex >= inventory.Count || toIndex >= inventory.Count)
        {
            Debug.LogError("Invalid inventory or indices!");
            return;
        }

        AdobeItemBase temp = inventory[fromIndex];
        inventory[fromIndex] = inventory[toIndex];
        inventory[toIndex] = temp;

        ShowInventory();
        ShowQuickSlot();
    }

    #region ����
    public void Use(AdobeItemUseArguments arguments)
    {
        if (inventory.Count > 0)
        {
            inventory[m_inventoryIndex].Use(arguments);
        }
    }

    public void SwitchItem(int delta)
    {
        if (inventory.Count < 1)
        {
            return;
        }

        m_inventoryIndex = Mathf.Clamp(m_inventoryIndex + delta, 0, inventory.Count - 1);
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
            GameObject one = Instantiate(AdobePrefabManager.defaultDropItem,
                pos, AdobePrefabManager.defaultDropItem.transform.rotation);

            one.AddComponent(inventory[index].GetType());
            one.GetComponent<AdobeItemBase>().Paste(inventory[index]);
        }
    }

    public bool IsExist(int id, int count = 1)
    {
        for (int index = 0; index < inventory.Count; ++index)
        {
            if (inventory[index].Id != id) continue;
            count -= inventory[index].amount;
            if (count <= 0) return true;
        }
        return false;
    }

    public bool Remove(AdobeItemBase target)
    {
        for (int index = 0; index < inventory.Count; ++index)
        {
            if (ReferenceEquals(inventory[index], target) == false) continue;

            if (index <= m_inventoryIndex) m_inventoryIndex--;
            if (m_inventoryIndex < 0) m_inventoryIndex = 0;
            inventory.Remove(target);
            return true;
        }
        return false;
    }

    public void Remove(int id, int amount = 1)
    {
        if (IsExist(id, amount) == false)
        {
            return;
        }

        for (int index = inventory.Count - 1; index >= 0; --index)
        {
            bool hasElementRemoved = false;
            if (inventory[index].Id != id) continue;
            if (amount >= inventory[index].amount)
            {
                amount -= inventory[index].amount;
                inventory[index].amount = 0;

                AdobeItemBase m_itemComponent = inventory[index];
                inventory.Remove(inventory[index]);
                Destroy(m_itemComponent);

                hasElementRemoved = true;
            }
            else
            {
                inventory[index].amount -= amount;
                amount = 0;
            }

            if (index <= m_inventoryIndex && hasElementRemoved) m_inventoryIndex--;
            if (m_inventoryIndex < 0) m_inventoryIndex = 0;
            if (amount <= 0) return;
        }
        return;
    }

    public void AddItem(AdobeItemBase target)
    {
        for (int index = 0; index < inventory.Count; ++index)
        {
            if (inventory[index].Id == target.Id)
            {
                inventory[index].amount += target.amount;
                return;
            }
        }
        if (inventory.Count >= PLAYER_INVENTORY_MAX_SIZE)
        {
            return;
        }
        inventory.Add(target);
    }
    #endregion
}
