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
        TakeDamage(damage);
    }
    public void TakeDamage(float damage){
        health -= damage;
        if(health<=0 && !dead){
            Die();
        }
    }
    void Die(){
        dead = true;
        if(OnDeath!=null){
            OnDeath();
        }
        Destroy(gameObject);
    }
}
