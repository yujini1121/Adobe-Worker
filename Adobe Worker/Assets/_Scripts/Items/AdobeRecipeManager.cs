using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Recipe
{
    public List<string> ingredients;
    public string result;
}

// ��� ���� ������
[System.Serializable]
public class RecipeBook
{
    public List<Recipe> recipes;
}


public class AdobeRecipeManager : MonoBehaviour
{
    [SerializeField] private TextAsset recipeData; 
    private RecipeBook recipeBook;

    void Start()
    {
        recipeBook = JsonUtility.FromJson<RecipeBook>(recipeData.text);
    }

    // ������ ���� �Լ�
    public string Craft(List<string> inputIngredients)
    {
        // �Է¹��� ��Ḧ ���� (���� ���� ����)
        inputIngredients.Sort();

        // ������ Ž��
        foreach (var recipe in recipeBook.recipes)
        {
            var sortedRecipeIngredients = new List<string>(recipe.ingredients);
            sortedRecipeIngredients.Sort();

            if (sortedRecipeIngredients.SequenceEqual(inputIngredients))
            {
                return recipe.result;
            }
        }

        return "Invalid";
    }
}
