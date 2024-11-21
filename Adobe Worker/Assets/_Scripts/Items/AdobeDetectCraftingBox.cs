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
            craftingSlots[craftingIngredients.Count - 1].enabled = true; // �̹��� Ȱ��ȭ
            CheckCraftingResult();
        }
    }

    private void CheckCraftingResult()
    {
        string result = recipeManager.Craft(craftingIngredients);
        if (result != "Invalid")
        {
            // ��� ������ �̹����� �ε��� ǥ��
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
            Debug.Log("���� ����! ��� ������: " + recipeManager.Craft(craftingIngredients));
            craftingIngredients.Clear();

            // ���� �ʱ�ȭ
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
        // Resources �������� ��� ������ ��������Ʈ�� �ε�
        return Resources.Load<Sprite>($"Items/{itemName}");
    }

    void OnDrawGizmosSelected()
    {
        // ���õ� ��ü �ֺ��� Sphere�� �׷��ݴϴ�.
        Gizmos.color = Color.red;  // Sphere�� ����
        Gizmos.DrawWireSphere(transform.position, interactionRange); // Sphere ���� �׸���
    }
}