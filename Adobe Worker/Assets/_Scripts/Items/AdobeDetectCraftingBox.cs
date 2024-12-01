using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdobeDetectCraftingBox : MonoBehaviour
{
    [Header("Craft")]
    [SerializeField] private bool isNearCraftingBox = false;
    [SerializeField] private LayerMask craftingBoxLayer;
    [SerializeField] private float interactionRange = 3f;

    [Space(10)]
    [Header("Assign")]
    [SerializeField] private AdobeRecipeManager recipeManager;
    [SerializeField] private GameObject craftingUI;
    [SerializeField] private Button craftButton;
    [SerializeField] private Image resultSlot;
    [SerializeField] private Image[] craftingSlots;

    [Space(10)]
    [Header("Ingredients")]
    [SerializeField] private HashSet<int> craftingIngredients = new HashSet<int>();  // HashSet 사용

    private void Update()
    {
        DetectCraftingBox();
    }

    void DetectCraftingBox()
    {
        RaycastHit hit;
        if (Physics.CheckSphere(transform.position, interactionRange, craftingBoxLayer))
        {
            if (!isNearCraftingBox)
            {
                isNearCraftingBox = true;
                craftingUI.SetActive(true);
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

    public void AddIngredientToCraftingBox(int ingredientId, Sprite ingredientSprite)
    {
        if (craftingIngredients.Count < 3)
        {
            craftingIngredients.Add(ingredientId);
            craftingSlots[craftingIngredients.Count - 1].sprite = ingredientSprite;
            craftingSlots[craftingIngredients.Count - 1].enabled = true;  // 이미지 활성화
            CheckCraftingResult();
        }
    }

    private void CheckCraftingResult()
    {
        int resultId = recipeManager.Craft(craftingIngredients);
        if (resultId != -1)
        {
            Sprite resultSprite = LoadItemSprite(resultId);
            resultSlot.sprite = resultSprite;
            resultSlot.enabled = true;
            craftButton.interactable = true;
        }
        else
        {
            resultSlot.sprite = null;
            resultSlot.enabled = false;
            craftButton.interactable = false;
        }
    }

    public void CompleteCrafting()
    {
        if (craftButton.interactable)
        {
            Debug.Log("조합 성공! 결과 아이템 ID: " + recipeManager.Craft(craftingIngredients));
            craftingIngredients.Clear();

            // 슬롯 초기화
            foreach (var slot in craftingSlots)
            {
                slot.sprite = null;
                slot.enabled = false;
            }
            resultSlot.sprite = null;
            resultSlot.enabled = false;
            craftButton.interactable = false;
        }
    }

    private Sprite LoadItemSprite(int itemId)
    {
        return Resources.Load<Sprite>($"Sprites/Items/{itemId}");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
