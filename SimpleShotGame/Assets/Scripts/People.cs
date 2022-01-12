using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class People : MonoBehaviour, IDamageable
{
    public float startingHealth;
    protected float health;
    protected bool dead;

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
        Destroy(hit.collider.gameObject);
    }
}
