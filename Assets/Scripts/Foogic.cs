using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "Foogic", menuName = "Items/Foogic")]
public class Foogic : ScriptableObject {

    [Separator("Foogic Spell", true)]
    public SpellCreator foogicSpell;
    [Separator("Foogic", true)]
    public GameObject spellPrefab;
    public Sprite spellSprite;
    public AudioClip spellAudio;
    public float audioVolume;
    public string spellName;
    public ItemObject[] recipes = new ItemObject[3];
    [Separator("Foogic Properties", true)]
    public float spellDuration;
    public float spellRadius;
    public float cookingTime;
    public Types.SpellSpawnType spawnType;
    [ConditionalField(nameof(spawnType), false, Types.SpellSpawnType.Special)]public Types.SpecialSpawnType specialSpawnType;
    [ConditionalField(nameof(specialSpawnType), false, Types.SpecialSpawnType.Delay)]public float spawnDelay;
    
}