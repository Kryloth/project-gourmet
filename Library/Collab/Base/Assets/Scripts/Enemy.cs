using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public float MAX_HEALTH;
    public float currentHealth;
    private AIPath aIPath;
    private Vector2 direction;
    private Rigidbody2D rb;
    void Start()
    {
        currentHealth = MAX_HEALTH;
        aIPath = GetComponent<AIPath>();
        AstarPath.active.logPathResults = PathLog.None;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth <= 0f){
            Destroy(this.gameObject);
        }

        direction = aIPath.desiredVelocity;
        
        if(direction.x > direction.y)
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
        if(collision.gameObject.tag == "Player")
        {
            PlayerDamageHandler playerDamageHandler = collision.gameObject.GetComponent<PlayerDamageHandler>();
            playerDamageHandler.InstantDamage(Types.SequenceType.Points,10);
            Vector2 dir = (this.gameObject.transform.position-collision.gameObject.transform.position).normalized*50f;
            
            rb.AddForce(dir,ForceMode2D.Impulse);
        }
    }
}
