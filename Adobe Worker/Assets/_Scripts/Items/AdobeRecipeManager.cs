using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    public Item[] input;
    public Item output;
}

[System.Serializable]
public class Item
{
    public int id;
    public int amount;
}



public class AdobeRecipeManager : MonoBehaviour
{

}
