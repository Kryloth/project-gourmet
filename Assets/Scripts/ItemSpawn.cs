using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawn : MonoBehaviour
{
    private SpriteRenderer sprite;
    public ItemObject itemObjectContainer;
    public static ItemSpawn Spawn(Vector2 position){
        Transform item = Instantiate(MainGameController.instance.prefabItem, position, Quaternion.identity).transform;
        ItemSpawn itemSpawn = item.GetComponent<ItemSpawn>();
        itemSpawn.Setup();
        return itemSpawn;
    }
    private void Setup(){
        float randValue = Random.value;
        int i = new int();
        if(randValue <= .33f){
            i = 0;
        }else if(randValue <= .66f){
            i = 1;
        }else if(randValue <= 1f){
            i = 2;
        }

        itemObjectContainer = MainGameController.instance.itemIngredients[i];
        sprite.sprite = itemObjectContainer.sprite;
    }
    private void Awake() {
        sprite = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")){
            switch(itemObjectContainer.ingredient){
                case Types.IngredientType.Wheat:
                    MainGameController.instance.wheatCount += 1;
                    break;
                case Types.IngredientType.Meat:
                    MainGameController.instance.meatCount += 1;
                    break;
                case Types.IngredientType.Veggies:
                    MainGameController.instance.veggiesCount += 1;
                    break;
            }
            Destroy(this.gameObject);
        }
    }
}
