using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector2 attack;
    private Rigidbody2D rb;
    public float projectileSpeed = 5f;
    
    public float lifeTime = 3f;
    public float damage = 10f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attack = new Vector2(Input.GetAxisRaw("FireHorizontal"), Input.GetAxisRaw("FireVertical"));
        rb.AddForce(attack*projectileSpeed, ForceMode2D.Impulse);
    }

    // Update is called once per frame
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
        }   
        if(other.gameObject.tag != "Spell"){
            Destroy(this.gameObject);
        }
    }
}
