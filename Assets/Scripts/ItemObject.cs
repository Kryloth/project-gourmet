using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemObject", menuName = "Items/Ingredients")]
public class ItemObject : ScriptableObject {
    [Header("Ingredient Sprite")]
    public Sprite sprite;
    [Header("Ingredient Type")]
    public Types.IngredientType ingredient;
}