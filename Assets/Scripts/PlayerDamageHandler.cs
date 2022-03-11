using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerDamageHandler : MonoBehaviour
{
    private Player player;
    List<float> currentStunDuration = new List<float>();
    Dictionary<string, IEnumerator> currentStunCoroutineDict = new Dictionary<string, IEnumerator>();
    public Dictionary<string, IEnumerator> SupportSpellsCoroutineDict = new Dictionary<string, IEnumerator>();
    public Dictionary<string, IEnumerator> OffensiveSpellsCoroutineDict = new Dictionary<string, IEnumerator>();
    void Awake()
    {
        player = GetComponent<Player>();
    }
    public void StartHealOverTimeCoroutine(SpellDamageHandler spellDamageHandler, Types.SequenceType sequenceType, string spellName, float healPerSecond, float duration, float tickInterval = 1f){
        IEnumerator HOTCoroutine = HealOverTimeCoroutine(spellDamageHandler, sequenceType, spellName, healPerSecond, duration, tickInterval);
        SupportSpellsCoroutineDict.Add(spellName, HOTCoroutine);
        StartCoroutine(SupportSpellsCoroutineDict[spellName]);
    }
    public void StartBuffCoroutine(SpellDamageHandler spellDamageHandler, Types.SupportBuffType buffType, PlayerModifier playerModifier, ProjectileModifier projectileModifier, string spellName){
        IEnumerator buffCoroutine = BuffCoroutine(spellDamageHandler, buffType, playerModifier, projectileModifier, spellName);
        SupportSpellsCoroutineDict.Add(spellName, buffCoroutine);
        StartCoroutine(SupportSpellsCoroutineDict[spellName]);
    }

    IEnumerator HealOverTimeCoroutine(SpellDamageHandler spellDamageHandler, Types.SequenceType sequenceType, string spellName, float healPerSecond, float duration, float tickInterval = 1f){
        float healPerTick = 0f;
        float elapsedTime = 0f;

        if(sequenceType == Types.SequenceType.Percentage){
            healPerTick = (healPerSecond / 100) * player.MAX_HEALTH;
        }else{
            healPerTick = healPerSecond;
        }
        if(tickInterval < 1f){
            healPerTick = healPerTick * tickInterval;
        }
        while(elapsedTime < duration){
            player.currentHealth += healPerTick;
            DamagePopup.Create(this.gameObject.transform.position, healPerTick, true);
            elapsedTime += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }
        SupportSpellsCoroutineDict.Remove(spellName);
    }
    IEnumerator BuffCoroutine(SpellDamageHandler spellDamageHandler, Types.SupportBuffType buffType, PlayerModifier playerModifier, ProjectileModifier projectileModifier,string spellName){
        float elapsedTime = 0f;
        float baseMovSpeed = player.moveSpeed;
        float baseAttackSpeed = player.attackSpeed;
        float currentArmor = 0f;
        float currentMovSpeed = 0f;
        float currentAttackSpeed = 0f;
        float currentDamage = 0f;
        switch(buffType){
            case Types.SupportBuffType.Player:
                switch(playerModifier.playerBuff){
                    case Types.PlayerBuffType.AttackSpeed:
                        currentAttackSpeed = ((playerModifier.attackSpeed / 100) * baseAttackSpeed);
                        player.attackSpeed -= currentAttackSpeed;
                        DamagePopup.CreateStatusPopup(Types.SpellSpawnType.FollowSelf, this.gameObject.transform, "ATTACK SPEED UP", true, playerModifier.buffDuration);
                        break;
                    case Types.PlayerBuffType.MovementSpeed:
                        currentMovSpeed = ((playerModifier.movementSpeed / 100) * baseMovSpeed);
                        player.moveSpeed += currentMovSpeed;
                        DamagePopup.CreateStatusPopup(Types.SpellSpawnType.FollowSelf, this.gameObject.transform, "MOVEMENT SPEED UP", true, playerModifier.buffDuration);
                        break;
                    case Types.PlayerBuffType.Armor:
                        currentArmor = playerModifier.tempArmor;;
                        player.armor += currentArmor;
                        DamagePopup.CreateStatusPopup(Types.SpellSpawnType.FollowSelf, this.gameObject.transform, "ARMOR UP", true, playerModifier.buffDuration);
                        break;
                }
                while(elapsedTime < playerModifier.buffDuration){
                    yield return new WaitForSeconds(.5f);
                    elapsedTime += .5f;
                }
                player.attackSpeed += currentAttackSpeed;
                player.moveSpeed -= currentMovSpeed;
                player.armor -= currentArmor;
                break;
            case Types.SupportBuffType.Projectile:
                if(projectileModifier.projectileBuff == Types.ProjectileBuffType.BaseDamage){
                    currentDamage = projectileModifier.baseDamage;
                    player.projectileDamage += currentDamage; 
                    DamagePopup.CreateStatusPopup(Types.SpellSpawnType.FollowSelf, this.gameObject.transform, "DAMAGE UP", true, projectileModifier.buffDuration);
                }else{
                    player.MiniStunSwitch(spellDamageHandler, spellName, true);
                    DamagePopup.CreateStatusPopup(Types.SpellSpawnType.FollowSelf, this.gameObject.transform, "MINI STUN", true, projectileModifier.buffDuration);
                }
                while(elapsedTime < projectileModifier.buffDuration){
                    yield return new WaitForSeconds(.5f);
                    elapsedTime += .5f;
                }
                player.projectileDamage -= currentDamage;
                player.MiniStunSwitch(spellDamageHandler, spellName, false);
                break;
        }
        SupportSpellsCoroutineDict.Remove(spellName);
    }
    IEnumerator DamageOverTimeCoroutine(Types.SequenceType sequenceType, float damagePerSecond, float duration, float tickInterval = 1f){
        float damagePerTick = 0f;
        float elapsedTime = 0f;

        if(sequenceType == Types.SequenceType.Percentage){
            damagePerTick = (damagePerSecond / 100) * player.MAX_HEALTH;
        }else{
            damagePerTick = damagePerSecond;
        }
        if(tickInterval < 1f){
            damagePerTick = damagePerTick * tickInterval;
        }
        while(elapsedTime < duration){
            player.currentHealth -= damagePerTick;
            elapsedTime += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }
    }

    public void InstantDamage(Types.SequenceType sequenceType, float damage)
    {
        float hitDamage;
        float armor = player.armor;
        float armorReduction = 1f - ((0.03f * armor) / (1 + 0.03f * Mathf.Abs(armor)));
        //Debug.Log(armorReduction);
        if (sequenceType == Types.SequenceType.Percentage)
        {
            damage = (damage / 100) * player.MAX_HEALTH;
            damage = damage * armorReduction;
            hitDamage = (float)(Math.Truncate((double) damage * 100.0) / 100.0);
            player.currentHealth -= hitDamage;
        }
        else
        {
            damage = damage * armorReduction;
            hitDamage = (float)(Math.Truncate((double) damage * 100.0) / 100.0);
            player.currentHealth -= hitDamage;
        }
        DamagePopup.Create(this.gameObject.transform.position, hitDamage);
    }

    public void InstantHeal(Types.SequenceType sequenceType, float heal)
    {
        if (sequenceType == Types.SequenceType.Percentage)
        {
            heal = (heal / 100) * player.MAX_HEALTH;
            player.currentHealth += heal;
        }
        else
        {
            player.currentHealth += heal;
        }
        DamagePopup.Create(this.gameObject.transform.position, heal, true);
    }
}
