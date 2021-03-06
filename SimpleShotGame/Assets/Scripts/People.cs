using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class People : MonoBehaviour, IDamageable
{
    public float startingHealth;
    public float health {get; private set;}
    protected bool dead;

    public event System.Action OnDeath;//事件委托

    protected virtual void Start() {
        health = startingHealth;
    }
    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection){
        TakeDamage(damage);
    }
    public virtual void TakeDamage(float damage){
        health -= damage;
        if(health<=0 && !dead){
            Die();
        }
    }
    [ContextMenu("Commit Suicide")]
    public virtual void Die(){
        dead = true;
        if(OnDeath!=null){
            OnDeath();
        }
        Destroy(gameObject);
    }
}
