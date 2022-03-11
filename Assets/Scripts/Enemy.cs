using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using MyBox;

public class Enemy : MonoBehaviour
{
    [HideInInspector]public AIPath aIPath;
    
    [Separator("Health", true)]
    public float MAX_HEALTH;
    [ReadOnly]public float currentHealth;

    [Separator("Knockback", true)]
    public float knockbackForce;
    public float knockbackTime;

    [Separator("Enemy Properties", true)]
    public float baseDamage;
    public float baseArmor;
    public float baseMoveSpeed;
    [ReadOnly]public float damage;
    [ReadOnly]public float armor;

    [Separator("Enemy State", true)]
    [ReadOnly]public bool isSlowed;
    [ReadOnly]public bool isStunned;
    [ReadOnly]public bool isKnocked;

    private Vector2 direction;
    private Rigidbody2D rb;
    private AIDestinationSetter destination;
    private CircleCollider2D enemyChildCollider;

    Animator animator;
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        aIPath = GetComponent<AIPath>();
        destination = GetComponent<AIDestinationSetter>();
        enemyChildCollider = this.gameObject.transform.GetChild(0).GetComponent<CircleCollider2D>();
    }
    void Start()
    {
        currentHealth = MAX_HEALTH;
        damage = baseDamage;
        aIPath.maxSpeed = baseMoveSpeed;
        armor = baseArmor;

        AstarPath.active.logPathResults = PathLog.None;
        destination.target = FindObjectOfType<Player>().transform;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth <= 0f){
            float randValue = UnityEngine.Random.value;
            if(randValue <= .5f){
                ItemSpawn.Spawn(this.gameObject.transform.position);
            }
            MainGameController.instance.CheckRound();
            Destroy(this.gameObject);
        }
        
        direction = aIPath.desiredVelocity;
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);

        if (direction.x > direction.y)
        {
            if (direction.x >= aIPath.maxSpeed-1)
            {
                //Debug.Log("Right");
            }
            if (direction.y <= -1*(aIPath.maxSpeed - 1))
            {
                //Debug.Log("Down");
            }
        }
        else
        {
            if (direction.y >= aIPath.maxSpeed - 1)
            {
                //Debug.Log("Up");
            }
            if (direction.x <= -1*(aIPath.maxSpeed - 1))
            {
                //Debug.Log("Left");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            PlayerDamageHandler playerDamageHandler = collision.gameObject.GetComponent<PlayerDamageHandler>();
            playerDamageHandler.InstantDamage(Types.SequenceType.Points, damage);
            Vector2 dir = (this.gameObject.transform.position - collision.gameObject.transform.position).normalized * knockbackForce;
            if(!isKnocked){
                StartCoroutine(Knockback(dir));
            }
        }
    }

    IEnumerator Knockback(Vector2 dir){
        enemyChildCollider.enabled = false;
        isKnocked = true;
        aIPath.canMove = false;
        rb.AddForce(dir,ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackTime);
        isKnocked = false;
        aIPath.canMove = true;
        enemyChildCollider.enabled = true;
    }
    
}
