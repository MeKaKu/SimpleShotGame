using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class People : MonoBehaviour, IDamageable
{
    public float startingHealth;
    protected float health;
    protected bool dead;

    public event System.Action OnDeath;//事件委托

    protected virtual void Start() {
        health = startingHealth;
    }
    public void TakeHit(float damage,RaycastHit hit){
        health -= damage;
        if(health<=0 && !dead){
            Die(hit);
        }
    }
    void Die(RaycastHit hit){
        dead = true;
        OnDeath();
        Destroy(hit.collider.gameObject);
    }
}
