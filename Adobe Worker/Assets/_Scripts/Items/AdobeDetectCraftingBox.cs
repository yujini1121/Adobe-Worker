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
    [SerializeField] private AdobeRecipeManager recipeManager;
    [SerializeField] private GameObject craftingUI;
    [SerializeField] private Button craftButton; 
    [SerializeField] private Image resultSlot; 
    [SerializeField] private Image[] craftingSlots; 

    [Space(10)] [Header("Ingredients")]
    [SerializeField] private List<string> craftingIngredients = new List<string>();


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

    public void AddIngredientToCraftingBox(string ingredient, Sprite ingredientSprite)
    {
        if (craftingIngredients.Count < 3)
        {
            craftingIngredients.Add(ingredient);
            craftingSlots[craftingIngredients.Count - 1].sprite = ingredientSprite;
            craftingSlots[craftingIngredients.Count - 1].enabled = true; // 이미지 활성화
            CheckCraftingResult();
        }
    }

    private void CheckCraftingResult()
    {
        string result = recipeManager.Craft(craftingIngredients);
        if (result != "Invalid")
        {
            // 결과 아이템 이미지를 로드해 표시
            Sprite resultSprite = LoadItemSprite(result);
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
            Debug.Log("조합 성공! 결과 아이템: " + recipeManager.Craft(craftingIngredients));
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

    private Sprite LoadItemSprite(string itemName)
    {
        // Resources 폴더에서 결과 아이템 스프라이트를 로드
        return Resources.Load<Sprite>($"Items/{itemName}");
    }

    void OnDrawGizmosSelected()
    {
        // 선택된 객체 주변에 Sphere를 그려줍니다.
        Gizmos.color = Color.red;  // Sphere의 색상
        Gizmos.DrawWireSphere(transform.position, interactionRange); // Sphere 범위 그리기
    }
}