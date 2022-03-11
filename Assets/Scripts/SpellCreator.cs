using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "Foogic Spell", menuName = "Items/Foogic Spell")]
public class SpellCreator : ScriptableObject{
    public SpellList[] spellList;
}
[System.Serializable]
public class SpellList{
    public Types.SpellType spellType;
    [ConditionalField(nameof(spellType), false, Types.SpellType.Offensive)]public OffensiveSpellCreator offensiveSpellCreator;
    [ConditionalField(nameof(spellType), false, Types.SpellType.Support)]public SupportSpellCreator supportSpellCreator;
}
[System.Serializable]
public class OffensiveSpellCreator{
    public Types.OffensiveSpellType offensiveSpellType;
    [Separator("Spell Properties", true)]
    [ConditionalField(nameof(offensiveSpellType), false, Types.OffensiveSpellType.Damage)]public Types.SpellCommitType commitType;
    [ConditionalField(nameof(offensiveSpellType), false, Types.OffensiveSpellType.Utility)]public Types.OffensiveUtilityType utilityType;
    [Header("Damage Sequence | Points or Percentage")]
    [ConditionalField(nameof(offensiveSpellType), false, Types.OffensiveSpellType.Damage)]public Types.SequenceType damageSequence ;
    [Header("Can it be stop at runtime ?")]
    [ConditionalField(nameof(commitType), false, Types.SpellCommitType.Overtime)]public bool isStoppable;
    [Header("Damage amount in an instant or per seconds if using tickType")]
    [ConditionalField(nameof(offensiveSpellType), false, Types.OffensiveSpellType.Damage)]public float damageAmount;
    [Header("Damage duration in seconds if using tickType")]
    [ConditionalField(nameof(commitType), false, Types.SpellCommitType.Overtime)]public float damageDuration;
    [Header("The interval between tick damage in seconds (Default = 1f)")]
    [ConditionalField(nameof(commitType), false, Types.SpellCommitType.Overtime)]public float tickInverval = 1f;
    [Header("Slow enemy movement amount in %")]
    [ConditionalField(nameof(utilityType), false, Types.OffensiveUtilityType.Slow)]public float slowAmount;
    [ConditionalField(nameof(utilityType), false, Types.OffensiveUtilityType.Slow)]public float slowDuration;
    [Header("Stun duration in seconds")]
    [ConditionalField(nameof(utilityType), false, Types.OffensiveUtilityType.Stun)]public float stunDuration;
}
[System.Serializable]
public class SupportSpellCreator{
    public Types.SupportSpellType supportSpellType;
    [ConditionalField(nameof(supportSpellType), false, Types.SupportSpellType.Heal)]public Types.SpellCommitType commitType;
    [ConditionalField(nameof(supportSpellType), false, Types.SupportSpellType.Buff)]public Types.SupportBuffType buffType;
    [ConditionalField(nameof(supportSpellType), false, Types.SupportSpellType.Heal)]public Types.SequenceType healSequence;
    [ConditionalField(nameof(commitType), false, Types.SpellCommitType.Overtime)]public bool isStoppable;
    [ConditionalField(nameof(supportSpellType), false, Types.SupportSpellType.Heal)]public float healAmount;
    [ConditionalField(nameof(commitType), false, Types.SpellCommitType.Overtime)]public float healDuration;
    [ConditionalField(nameof(commitType), false, Types.SpellCommitType.Overtime)]public float tickInterval = 1f;
    [Separator("Modifiers is only for buffs", true)]
    [ConditionalField(nameof(buffType), false, Types.SupportBuffType.Player)]public PlayerModifier[] playerModifiers;
    [ConditionalField(nameof(buffType), false, Types.SupportBuffType.Projectile)]public ProjectileModifier[] projectileModifiers;
}
[System.Serializable]
public class PlayerModifier{
    public Types.PlayerBuffType playerBuff;
    [ConditionalField(nameof(playerBuff), false, Types.PlayerBuffType.AttackSpeed)]public float attackSpeed;
    [ConditionalField(nameof(playerBuff), false, Types.PlayerBuffType.MovementSpeed)]public float movementSpeed;
    [ConditionalField(nameof(playerBuff), false, Types.PlayerBuffType.Armor)]public float tempArmor;
    public float buffDuration;
}
[System.Serializable]
public class ProjectileModifier{
    public Types.ProjectileBuffType projectileBuff;
    [ConditionalField(nameof(projectileBuff), false, Types.ProjectileBuffType.BaseDamage)]public float baseDamage;
    [ConditionalField(nameof(projectileBuff), false, Types.ProjectileBuffType.MiniStun)]public bool canMiniStun;
    public float buffDuration;
}
