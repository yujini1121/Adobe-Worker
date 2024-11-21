using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Recipe
{
    public List<string> ingredients;
    public string result;
}

// 모든 조합 레시피
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

    // 아이템 조합 함수
    public string Craft(List<string> inputIngredients)
    {
        // 입력받은 재료를 정렬 (조합 순서 무시)
        inputIngredients.Sort();

        // 레시피 탐색
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
