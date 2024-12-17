using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdobeCraftManager : MonoBehaviour
{
    [Header("Crafting Slots")]
    public List<Image> craftingSlots; // 재료 슬롯 3개
    public Image resultSlot; // 결과 아이템 슬롯

    [Header("Item Recipes")]
    public List<ItemRecipe> itemRecipes;

    private List<AdobeItemBase> currentIngredients = new List<AdobeItemBase>(); // 현재 선택된 재료들

    private void Start()
    {
        InitializeCraftingSlots();
    }

    // 재료 슬롯 초기화
    private void InitializeCraftingSlots()
    {
        foreach (var slot in craftingSlots)
        {
            slot.sprite = null;
        }

        resultSlot.sprite = null;
    }

    // 재료 아이템을 슬롯에 드래그 앤 드롭
    public void AddItemToCraftingSlot(AdobeItemBase item, int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < craftingSlots.Count)
        {
            craftingSlots[slotIndex].sprite = item.sprite;
            currentIngredients[slotIndex] = item;
            CheckCraftingRecipe();
        }
    }

    // 재료가 맞는지 체크하고, 결과 아이템 생성
    private void CheckCraftingRecipe()
    {
        List<string> ingredientNames = new List<string>();

        foreach (var item in currentIngredients)
        {
            if (item != null)
            {
                ingredientNames.Add(item.name); // 아이템 이름을 레시피에 맞게 비교
            }
        }

        foreach (var recipe in itemRecipes)
        {
            if (MatchRecipe(ingredientNames, recipe))
            {
                // 레시피가 맞다면 결과 아이템 생성
                resultSlot.sprite = GetResultSprite(recipe.result);
                return;
            }
        }

        resultSlot.sprite = null; // 레시피가 맞지 않으면 결과 슬롯을 비움
    }

    // 레시피가 맞는지 확인하는 함수
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

    // 결과 아이템 스프라이트를 가져오는 함수
    private Sprite GetResultSprite(string resultItem)
    {
        // 결과 아이템 스프라이트를 반환하는 로직
        // 예시: Resources에서 로드
        return Resources.Load<Sprite>("Sprites/" + resultItem);
    }
}

[System.Serializable]
public class ItemRecipe
{
    public List<string> ingredients; // 재료 리스트
    public string result; // 결과 아이템
}
