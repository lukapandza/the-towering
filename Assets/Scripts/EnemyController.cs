using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    protected int health;
    protected GameManager gameManager;

    protected virtual void Awake()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void TakeDamage(int damage)
    {
        this.health -= damage;
        OnDamageEffect();
        if(this.health <= 0)
        {
            this.gameManager.EnemyKilled();
        } Die();
    }

    private void Die()
    {
        this.gameManager.RemoveEnemy(this.gameObject);
        Destroy(this.gameObject, .5f);
    }

    protected virtual void OnDamageEffect(){}
}
