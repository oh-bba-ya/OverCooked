using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RecipeList Data", menuName = "Scriptable Object/RecipeList Data", order = int.MaxValue)]
public class RecipeListSO : ScriptableObject
{
    public List<RecipeSO> recipeSOList;
}
