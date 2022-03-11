using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyDamageHandler : MonoBehaviour
{
    [HideInInspector]public Enemy enemy;
    List<float> currentStunDuration = new List<float>();
    Dictionary<string, IEnumerator> currentStunCoroutineDict = new Dictionary<string, IEnumerator>();
    public Dictionary<string, IEnumerator> SupportSpellsCoroutineDict = new Dictionary<string, IEnumerator>();
    public Dictionary<string, IEnumerator> OffensiveSpellsCoroutineDict = new Dictionary<string, IEnumerator>();
    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }
    private void Update() {
    }
    void StunCheck(){
        if(currentStunCoroutineDict.Count > 0){
            enemy.isStunned = true;
            enemy.aIPath.canMove = false;
        }else{
            enemy.isStunned = false;
            enemy.aIPath.canMove = true;
        }
    }
    public void InstantDamage(Types.SequenceType sequenceType, float damage)
    {
        int hitDamage;
        float armor = enemy.armor;
        float armorReduction = 1f - ((0.03f * armor) / (1 + 0.03f * Mathf.Abs(armor)));
        if (sequenceType == Types.SequenceType.Percentage)
        {
            damage = (damage / 100) * enemy.MAX_HEALTH;
            damage = damage * armorReduction;
            hitDamage = Mathf.FloorToInt(damage);
            enemy.currentHealth -= hitDamage;
        }
        else
        {
            damage = damage * armorReduction;
            hitDamage = Mathf.FloorToInt(damage);
            enemy.currentHealth -= hitDamage;
        }
        DamagePopup.Create(this.gameObject.transform.position, hitDamage);
    }
    public void StartDamageOverTimeCoroutine(SpellDamageHandler spellDamageHandler, Types.SequenceType sequenceType, string spellName, float damagePerSecond, float duration, float tickInterval = 1f){
        IEnumerator DOTCoroutine = DamageOverTimeCoroutine(spellDamageHandler, sequenceType, spellName, damagePerSecond, duration, tickInterval);
        OffensiveSpellsCoroutineDict.Add(spellName, DOTCoroutine);
        StartCoroutine(OffensiveSpellsCoroutineDict[spellName]);
    }
    public void StartUtilityCoroutine(SpellDamageHandler spellDamageHandler, Types.OffensiveUtilityType utilityType, string spellName, float slowAmount, float slowDuration, float stunDuration){
        if(utilityType == Types.OffensiveUtilityType.Slow){
            IEnumerator slowCoroutine = SlowCoroutine(spellDamageHandler, spellName, slowAmount, slowDuration);
            OffensiveSpellsCoroutineDict.Add(spellName, slowCoroutine);
            StartCoroutine(OffensiveSpellsCoroutineDict[spellName]);
        }else if(utilityType == Types.OffensiveUtilityType.Stun){
            string name = currentStunCoroutineDict.Count.ToString();
            IEnumerator stunCoroutine = StunCoroutine(name, stunDuration);
            currentStunCoroutineDict.Add(name, stunCoroutine);
            StartCoroutine(currentStunCoroutineDict[name]);
        }
    }
    IEnumerator DamageOverTimeCoroutine(SpellDamageHandler spellDamageHandler, Types.SequenceType sequenceType, string spellName, float damagePerSecond, float duration, float tickInterval = 1f){
        float damagePerTick = 0f;
        float elapsedTime = 0f;

        if(sequenceType == Types.SequenceType.Percentage){
            damagePerTick = (damagePerSecond / 100) * enemy.MAX_HEALTH;
        }else{
            damagePerTick = damagePerSecond;
        }
        if(tickInterval < 1f)
            damagePerTick = damagePerTick * tickInterval;

        while(elapsedTime < duration){
            enemy.currentHealth -= damagePerTick;
            DamagePopup.Create(this.gameObject.transform.position, Mathf.RoundToInt(damagePerTick));
            elapsedTime += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }
        OffensiveSpellsCoroutineDict.Remove(spellName);
    }
    IEnumerator SlowCoroutine(SpellDamageHandler spellDamageHandler, string spellName, float slowAmount, float slowDuration){
        float elapsedTime = 0f;
        float baseMaxSpeed = 0f;
        float enemySpeed = 0f;

        baseMaxSpeed = enemy.aIPath.maxSpeed;
        enemySpeed = ((slowAmount / 100) * baseMaxSpeed);
        enemy.isSlowed = true;
        enemy.aIPath.maxSpeed -= enemySpeed;
        DamagePopup.CreateStatusPopup(Types.SpellSpawnType.FollowSelf, this.gameObject.transform, "SLOWED", false, slowDuration);
        while(elapsedTime < slowDuration){
            yield return new WaitForSeconds(.5f);
            elapsedTime += .5f;
        }
        enemy.isSlowed = false;
        enemy.aIPath.maxSpeed += enemySpeed;
        OffensiveSpellsCoroutineDict.Remove(spellName);
    }
    IEnumerator StunCoroutine(string stunNo,float stunDuration){
        DamagePopup.CreateStatusPopup(Types.SpellSpawnType.FollowSelf, this.gameObject.transform, "STUNNED", false, stunDuration);
        StunCheck();
        yield return new WaitForSeconds(stunDuration);
        currentStunCoroutineDict.Remove(stunNo);
        StunCheck();
    }
}
