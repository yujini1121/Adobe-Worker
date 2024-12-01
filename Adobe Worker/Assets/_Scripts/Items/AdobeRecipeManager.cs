using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    public HashSet<int> ingredients;
    public int result;
}

[System.Serializable]
public class RecipeList
{
    public List<RecipeData> ItemRecipe;
}

[System.Serializable]
public class RecipeData
{
    public List<int> ingredients;
    public int result;
}

public class AdobeRecipeManager : MonoBehaviour
{
    private List<Recipe> recipeBook = new List<Recipe>();

    void Start()
    {
        LoadRecipeData();
    }

    private void LoadRecipeData()
    {
        TextAsset recipeData = Resources.Load<TextAsset>("Json Files/ItemRecipe");
        var recipes = JsonUtility.FromJson<RecipeList>(recipeData.text);

        foreach (var r in recipes.ItemRecipe)
        {
            recipeBook.Add(new Recipe
            {
                ingredients = new HashSet<int>(r.ingredients),
                result = r.result
            });
        }
    }

    public int Craft(HashSet<int> inputIngredients)
    {
        foreach (var recipe in recipeBook)
        {
            if (recipe.ingredients.SetEquals(inputIngredients))  // HashSet ��
            {
                return recipe.result;  // ��� ������ ID ��ȯ
            }
        }
        return -1;  // ��ȿ���� ���� ����
    }
}