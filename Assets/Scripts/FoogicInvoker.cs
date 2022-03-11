using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoogicInvoker : MonoBehaviour
{
    public static FoogicInvoker instance;
    public Foogic[] foogicSpell = new Foogic[2];
    private void Awake() {
        if(instance == null){
            instance = this;
        }
    }
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
         if(Input.GetKeyDown(KeyCode.Space)){
            // InvokeSpell();
        }
    }
    public void InvokeSpell(){
        for (int i = 0; i < foogicSpell.Length; i++)
        {
            if(foogicSpell[i] != null && MainGameController.instance.currentSpell == i){
                InstantiateSpell(foogicSpell[i]);
                foogicSpell[i] = null;
            }
        }
    }
    void InstantiateSpell(Foogic foogic){
        GameObject spellGameObject = null;
        Spell spellScript = null;
        switch(foogic.spawnType){
            case Types.SpellSpawnType.FollowSelf:
                Transform parent = Player.instance.gameObject.transform;
                spellGameObject = Instantiate(foogic.spellPrefab, parent);
                break;
            case Types.SpellSpawnType.Ground:
                if(foogic.name == "Meteor Meatball"){
                    Vector3 pos = new Vector3(Player.instance.transform.position.x, Player.instance.transform.position.y + 2, Player.instance.transform.position.z);
                    spellGameObject = Instantiate(foogic.spellPrefab, pos, Quaternion.identity);
                    break;
                }
                spellGameObject = Instantiate(foogic.spellPrefab, Player.instance.gameObject.transform.position, Quaternion.identity);
                break;
            case Types.SpellSpawnType.Special:
                if(foogic.specialSpawnType == Types.SpecialSpawnType.Delay){
                    Transform parentt = Player.instance.gameObject.transform;
                    spellGameObject = Instantiate(foogic.spellPrefab, Player.instance.gameObject.transform.position, Quaternion.identity);
                    spellScript = spellGameObject.GetComponent<Spell>();
                    spellScript.spellDuration = foogic.spellDuration;
                    spellScript.SetFoogic(foogic);
                    spellScript.SetDelay(foogic.spawnDelay);
                    if(foogic.spellAudio != null){
                        spellScript.SetAudio(foogic.spellAudio, foogic.audioVolume);
                    }
                }else if(foogic.specialSpawnType == Types.SpecialSpawnType.Duplicate){
                    spellGameObject = Instantiate(foogic.spellPrefab, Player.instance.gameObject.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
                    spellScript = spellGameObject.GetComponent<Spell>();
                    spellScript.spellDuration = foogic.spellDuration;
                    spellScript.SetFoogic(foogic);
                    spellScript.rigidBody.AddForce(new Vector2(0f, 17f), ForceMode2D.Impulse);

                    spellGameObject = Instantiate(foogic.spellPrefab, Player.instance.gameObject.transform.position, Quaternion.Euler(new Vector3(0, 0, 90)));
                    spellScript = spellGameObject.GetComponent<Spell>();
                    spellScript.spellDuration = foogic.spellDuration;
                    spellScript.SetFoogic(foogic);
                    spellScript.rigidBody.AddForce(new Vector2(-17f, 0f), ForceMode2D.Impulse);

                    spellGameObject = Instantiate(foogic.spellPrefab, Player.instance.gameObject.transform.position, Quaternion.Euler(new Vector3(0, 0, 180)));
                    spellScript = spellGameObject.GetComponent<Spell>();
                    spellScript.spellDuration = foogic.spellDuration;
                    spellScript.SetFoogic(foogic);
                    spellScript.rigidBody.AddForce(new Vector2(0f, -17f), ForceMode2D.Impulse);

                    spellGameObject = Instantiate(foogic.spellPrefab, Player.instance.gameObject.transform.position, Quaternion.Euler(new Vector3(0, 0, 270)));
                    spellScript = spellGameObject.GetComponent<Spell>();
                    spellScript.spellDuration = foogic.spellDuration;
                    spellScript.SetFoogic(foogic);
                    spellScript.rigidBody.AddForce(new Vector2(17f, 0f), ForceMode2D.Impulse);
                }
                return;
        }
        spellScript = spellGameObject.GetComponent<Spell>();
        spellScript.SetFoogic(foogic);
        spellScript.spellDuration = foogic.spellDuration;

        if(foogic.spellAudio != null){
            spellScript.SetAudio(foogic.spellAudio, foogic.audioVolume);
        }

        if(foogic.spellRadius > 0){
            spellScript.SetRadius(foogic.spellRadius);
        }
        if(foogic.name == "Meteor Meatball"){
            spellScript.SetDelay(.8f);
            spellScript.isMeteor(MainGameController.instance.meteorFall);
        }else if(foogic.name == "Full Course Meal"){
            Player.instance.MAX_HEALTH += 5f;
            Player.instance.currentHealth += 5f;
            Player.instance.projectileDamage += 2f;
            Player.instance.armor += .5f;
            DamagePopup.CreateStatusPopup(Types.SpellSpawnType.Ground, Player.instance.transform, "MAX HP UP", true, 2f);
            DamagePopup.CreateStatusPopup(Types.SpellSpawnType.Ground, Player.instance.transform, "MAX DAMAGE UP", true, 2f);
            DamagePopup.CreateStatusPopup(Types.SpellSpawnType.Ground, Player.instance.transform, "MAX ARMOR UP", true, 2f);
        }
    }
    IEnumerator cookingSpell(){
        yield return new WaitForSeconds(1f);
    }
}
