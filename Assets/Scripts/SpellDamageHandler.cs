using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SpellDamageHandler : MonoBehaviour
{
    private Spell spell;
    List<OffensiveSpellCreator> offensiveSpellList = new List<OffensiveSpellCreator>();
    List<SupportSpellCreator> supportSpellList = new List<SupportSpellCreator>();
    int instanceId;
    string spellName;
    private void Awake() {
        spell = GetComponent<Spell>();

        offensiveSpellList.Clear();
        supportSpellList.Clear();
    }
    void Start()
    {
        instanceId = this.gameObject.GetInstanceID();
        int rand = UnityEngine.Random.Range(1, 3);
        if(rand == 1){
            if(instanceId % 2 == 0){
                instanceId = instanceId * -2;
            }else{
                instanceId = instanceId * 2;
            }
        }else{
            if(instanceId % 2 == 0){
                instanceId = instanceId * -3;
            }else{
                instanceId = instanceId * 3;
            }
        }
        SpellCheck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player"){
            PlayerDamageHandler playerHandler = other.gameObject.GetComponent<PlayerDamageHandler>();
            SupportSpellThrower(playerHandler);
        }else if(other.gameObject.tag == "Enemy"){
            EnemyDamageHandler enemyHandler = other.gameObject.GetComponent<EnemyDamageHandler>();
            OffensiveSpellThrower(enemyHandler);
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag == "Player"){
            PlayerDamageHandler playerHandler = other.gameObject.GetComponent<PlayerDamageHandler>();
            SupportSpellCollector(playerHandler);
        }else if(other.gameObject.tag == "Enemy"){
            EnemyDamageHandler enemyHandler = other.gameObject.GetComponent<EnemyDamageHandler>();
            OffensiveSpellCollector(enemyHandler);
        }
    }
    private void OnTriggerStay2D(Collider2D other) {
        
    }

    void SpellCheck(){
        if(spell == null)
            return;
        
        SpellCreator spellCreator = spell.foogic.foogicSpell;
        SpellList[] spellList = spellCreator.spellList;

        for (int i = 0; i < spellList.Length; i++)
        {
            if(spellList[i].spellType == Types.SpellType.Offensive){
                OffensiveSpellCreator offensiveSpell = spellList[i].offensiveSpellCreator;
                offensiveSpellList.Add(offensiveSpell);
            }else if(spellList[i].spellType == Types.SpellType.Support){
                SupportSpellCreator supportSpell = spellList[i].supportSpellCreator;
                supportSpellList.Add(supportSpell);
            }
        }
    }
    void OffensiveSpellThrower(EnemyDamageHandler enemyHandler){
        for (int i = 0; i < offensiveSpellList.Count; i++){
            spellName = instanceId + " | " + spell.foogic.spellName + " | Offensive Spell | No." + i;
            OffensiveSpellCreator offensiveSpell = offensiveSpellList[i];
            if(enemyHandler.OffensiveSpellsCoroutineDict.ContainsKey(spellName))
                continue;
            if(offensiveSpell.offensiveSpellType == Types.OffensiveSpellType.Damage){
                if(offensiveSpell.commitType == Types.SpellCommitType.Instant){
                    enemyHandler.InstantDamage(offensiveSpell.damageSequence, offensiveSpell.damageAmount);
                }else if(offensiveSpell.commitType == Types.SpellCommitType.Overtime){
                    enemyHandler.StartDamageOverTimeCoroutine(this, offensiveSpell.damageSequence, spellName, offensiveSpell.damageAmount, offensiveSpell.damageDuration, offensiveSpell.tickInverval);
                }
            }else{
                enemyHandler.StartUtilityCoroutine(this, offensiveSpell.utilityType, spellName, offensiveSpell.slowAmount, offensiveSpell.slowDuration, offensiveSpell.stunDuration);
            }
        }
    }
    void OffensiveSpellCollector(EnemyDamageHandler enemyHandler){

        for (int i = 0; i < offensiveSpellList.Count; i++){
            spellName = instanceId + " | " + spell.foogic.spellName + " | Offensive Spell | No." + i;
            OffensiveSpellCreator offensiveSpell = offensiveSpellList[i];
            if(offensiveSpell.commitType == Types.SpellCommitType.Overtime){
                if(offensiveSpell.isStoppable){
                    if(enemyHandler.OffensiveSpellsCoroutineDict.ContainsKey(spellName))
                        enemyHandler.StopCoroutine(enemyHandler.OffensiveSpellsCoroutineDict[spellName]);
                    enemyHandler.OffensiveSpellsCoroutineDict.Remove(spellName);
                }
            }
        }
    }
    void SupportSpellThrower(PlayerDamageHandler playerHandler){
        for (int i = 0; i < supportSpellList.Count; i++){
            spellName = instanceId + " | " + spell.foogic.spellName + " | Support Spell | No." + i;
            SupportSpellCreator supportSpell = supportSpellList[i];
            if(playerHandler.SupportSpellsCoroutineDict.ContainsKey(spellName))
                continue;
            if(supportSpell.supportSpellType == Types.SupportSpellType.Heal){
                if(supportSpell.commitType == Types.SpellCommitType.Instant){
                    playerHandler.InstantHeal(supportSpell.healSequence, supportSpell.healAmount);
                }else if(supportSpell.commitType == Types.SpellCommitType.Overtime){
                    playerHandler.StartHealOverTimeCoroutine(this, supportSpell.healSequence, spellName, supportSpell.healAmount, supportSpell.healDuration, supportSpell.tickInterval);
                }
            }else{
                switch(supportSpell.buffType){
                    case Types.SupportBuffType.Player:
                        for (int t = 0; t < supportSpell.playerModifiers.Length; t++)
                        {
                            spellName = instanceId + " | " + spell.foogic.spellName + " | Support Spell | No." + i + " | Player Buff | No." + t;   
                            playerHandler.StartBuffCoroutine(this, supportSpell.buffType, supportSpell.playerModifiers[t], null, spellName);
                        }
                        break;
                    case Types.SupportBuffType.Projectile:
                        for (int t = 0; t < supportSpell.projectileModifiers.Length; t++)
                        {
                            spellName = instanceId + " | " + spell.foogic.spellName + " | Support Spell | No." + i + " | Projectile Buff | No." + t;
                            playerHandler.StartBuffCoroutine(this, supportSpell.buffType, null, supportSpell.projectileModifiers[t], spellName);
                        }
                        break;
                }
            }
        }
    }
    void SupportSpellCollector(PlayerDamageHandler playerHandler){
        for (int i = 0; i < supportSpellList.Count; i++)
            {
                spellName = instanceId + " | " + spell.foogic.spellName + " | Support Spell | No." + i;
                SupportSpellCreator supportSpell = supportSpellList[i];
                if(supportSpell.commitType == Types.SpellCommitType.Overtime){
                    if(supportSpell.isStoppable){
                        if(playerHandler.SupportSpellsCoroutineDict.ContainsKey(spellName))
                            playerHandler.StopCoroutine(playerHandler.SupportSpellsCoroutineDict[spellName]);
                        playerHandler.SupportSpellsCoroutineDict.Remove(spellName);
                    }
                }
            }
    }
}
