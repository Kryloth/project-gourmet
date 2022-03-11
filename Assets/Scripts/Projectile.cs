using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool canMiniStun;
    public float lifeTime = 3f;
    public float damage;

    Animator animator;
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }

        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out EnemyDamageHandler enemyDamageHandler))
        {
            enemyDamageHandler.InstantDamage(Types.SequenceType.Points, damage);
            if(canMiniStun){
                enemyDamageHandler.StartUtilityCoroutine(spellDamageHandler, Types.OffensiveUtilityType.Stun, spellName, 0f, 0f, 0.2f);
            }
        }
        Destroy(this.gameObject);
    }
    SpellDamageHandler spellDamageHandler;
    string spellName;
    public void MiniStunSwitch(SpellDamageHandler spellHandler, string theSpellName, bool isMiniStun){
        canMiniStun = isMiniStun;
        spellDamageHandler = spellHandler;
        spellName = theSpellName;
    }
    public void AddForce(Vector2 attackDirection, float projectileSpeed){
        animator.SetFloat("Horizontal", attackDirection.x);
        animator.SetFloat("Vertical", attackDirection.y);
        rb.AddForce(attackDirection * projectileSpeed, ForceMode2D.Impulse);
    }
}
