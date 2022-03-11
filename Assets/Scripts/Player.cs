using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Player : MonoBehaviour
{
    public static Player instance;
    private Rigidbody2D rb;
    [Separator("Health", true)]
    public float MAX_HEALTH;
    [ReadOnly]public float currentHealth;

    [Separator("Player Properties", true)]
    public float baseMoveSpeed;
    public float baseAttackSpeed;
    public float baseArmor;
    [ReadOnly]public float moveSpeed;
    [ReadOnly]public float attackSpeed;
    [ReadOnly]public float armor;

    [Separator("Projectile", true)]
    public GameObject prefabProjectile;
    public float baseProjectileDamage;
    public float projectileSpeed;
    [ReadOnly]public float projectileDamage;
    [ReadOnly]public bool canMiniStun;
    private float attackCooldown;
    
    
    Vector2 move;
    Vector2 attack;
    

    Animator animator;
    
    
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        if(instance == null){
            instance = this;
        }
    }
    void Start()
    {
        currentHealth = MAX_HEALTH;
        moveSpeed = baseMoveSpeed;
        attackSpeed = baseAttackSpeed;
        projectileDamage = baseProjectileDamage;
        armor = baseArmor;

        attackCooldown = 0;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth <= 0f){
            Destroy(this.gameObject);
        }else if(currentHealth >= MAX_HEALTH){
            currentHealth = MAX_HEALTH;
        }

        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        move.x = Input.GetAxisRaw("Horizontal");
        move.y = Input.GetAxisRaw("Vertical");

        attack.x = Input.GetAxisRaw("FireHorizontal");
        attack.y = Input.GetAxisRaw("FireVertical");

        animator.SetFloat("Horizontal",move.x);
        animator.SetFloat("Vertical", move.y);
        animator.SetFloat("Is Moving", move.sqrMagnitude);
        animator.SetFloat("Fire Horizontal", attack.x);
        animator.SetFloat("Fire Vertical", attack.y);
        animator.SetFloat("Is Firing", attack.sqrMagnitude);
    }
    private void FixedUpdate() {
        rb.MovePosition(rb.position + move * moveSpeed * Time.deltaTime);
        
        if(attack != Vector2.zero && attackCooldown <= 0)
        {
            GameObject projectile = Instantiate(prefabProjectile, rb.position+new Vector2(0.1f,-0.3f), Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            projectileScript.AddForce(attack, projectileSpeed);
            projectileScript.damage = projectileDamage;
            projectileScript.MiniStunSwitch(spellDamageHandler, spellName, canMiniStun);
            attackCooldown = attackSpeed;
        }
    }
    SpellDamageHandler spellDamageHandler;
    string spellName;
    public void MiniStunSwitch(SpellDamageHandler spellHandler, string theSpellName, bool isMiniStun){
        canMiniStun = isMiniStun;
        spellDamageHandler = spellHandler;
        spellName = theSpellName;
    }
}
