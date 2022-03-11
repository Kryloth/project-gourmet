using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Types
{
    public enum SequenceType{
        Points,
        Percentage
    }
    public enum IngredientType{
        Wheat,
        Meat,
        Veggies
    }
    public enum SpellType{
        Offensive,
        Support
    }
    public enum OffensiveSpellType{
        Damage,
        Utility
    }
    public enum SupportSpellType{
        Heal,
        Buff
    }
    public enum SupportBuffType{
        None,
        Player,
        Projectile
    }
    public enum PlayerBuffType{
        AttackSpeed,
        MovementSpeed,
        Armor
    }
    public enum ProjectileBuffType{
        BaseDamage,
        MiniStun
    }
    public enum SpellCommitType{
        Instant,
        Overtime
    }
    public enum OffensiveUtilityType{
        None,
        Stun,
        Slow
    }
    public enum SpellSpawnType{
        FollowSelf,
        Ground,
        Special
    }
    public enum SpecialSpawnType{
        Delay,
        Duplicate
    }
}
