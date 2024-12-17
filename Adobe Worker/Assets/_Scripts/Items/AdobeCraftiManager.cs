using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdobeCraftManager : MonoBehaviour
{
    [Header("Crafting Slots")]
    public List<Image> craftingSlots; // ��� ���� 3��
    public Image resultSlot; // ��� ������ ����

    [Header("Item Recipes")]
    public List<ItemRecipe> itemRecipes;

    private List<AdobeItemBase> currentIngredients = new List<AdobeItemBase>(); // ���� ���õ� ����

    private void Start()
    {
        InitializeCraftingSlots();
    }

    // ��� ���� �ʱ�ȭ
    private void InitializeCraftingSlots()
    {
        foreach (var slot in craftingSlots)
        {
            slot.sprite = null;
        }

        resultSlot.sprite = null;
    }

    // ��� �������� ���Կ� �巡�� �� ���
    public void AddItemToCraftingSlot(AdobeItemBase item, int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < craftingSlots.Count)
        {
            craftingSlots[slotIndex].sprite = item.sprite;
            currentIngredients[slotIndex] = item;
            CheckCraftingRecipe();
        }
    }

    // ��ᰡ �´��� üũ�ϰ�, ��� ������ ����
    private void CheckCraftingRecipe()
    {
        List<string> ingredientNames = new List<string>();

        foreach (var item in currentIngredients)
        {
            if (item != null)
            {
                ingredientNames.Add(item.name); // ������ �̸��� �����ǿ� �°� ��
            }
        }

        foreach (var recipe in itemRecipes)
        {
            if (MatchRecipe(ingredientNames, recipe))
            {
                // �����ǰ� �´ٸ� ��� ������ ����
                resultSlot.sprite = GetResultSprite(recipe.result);
                return;
            }
        }

        resultSlot.sprite = null; // �����ǰ� ���� ������ ��� ������ ���
    }

    // �����ǰ� �´��� Ȯ���ϴ� �Լ�
    private bool MatchRecipe(List<string> ingredientNames, ItemRecipe recipe)
    {
        if (ingredientNames.Count != recipe.ingredients.Count)
            return false;

        foreach (var ingredient in recipe.ingredients)
        {
            if (!ingredientNames.Contains(ingredient))
                return false;
        }

        return true;
    }

    // ��� ������ ��������Ʈ�� �������� �Լ�
    private Sprite GetResultSprite(string resultItem)
    {
        // ��� ������ ��������Ʈ�� ��ȯ�ϴ� ����
        // ����: Resources���� �ε�
        return Resources.Load<Sprite>("Sprites/" + resultItem);
    }
}

[System.Serializable]
public class ItemRecipe
{
    public List<string> ingredients; // ��� ����Ʈ
    public string result; // ��� ������
}
