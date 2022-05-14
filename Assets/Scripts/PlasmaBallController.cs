using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaBallController : MonoBehaviour
{
    private int damage = 4;
    private bool didSingleHit = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(!this.didSingleHit && collision.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            enemy.TakeDamage(this.damage);
            this.didSingleHit = true;
            OnHitEffect();
        }
    }

    private void OnHitEffect()
    {
        Destroy(this.gameObject);
    }
}
