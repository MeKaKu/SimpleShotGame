using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask layerMask;//Layer，检测碰撞的layer
    private float speed;
    float damage = 1;

    public void SetSpeed(float _speed){
        speed = _speed;
    }

    private void Update() {
        float moveDistance = Time.deltaTime * speed;
        CheckCollisions(moveDistance);//碰撞检测
        transform.Translate(Vector3.forward * moveDistance);
    }
    void CheckCollisions(float moveDistance){
        Ray ray = new Ray(transform.position,transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,moveDistance,layerMask,QueryTriggerInteraction.Collide)){
            OnHitObject(hit);
        }
    }
    void OnHitObject(RaycastHit hit){
        print(hit.collider.gameObject.name);
        IDamageable damageObject = hit.collider.gameObject.GetComponent<IDamageable>();
        if(damageObject!=null){
            damageObject.TakeHit(damage,hit);
        }
        Destroy(gameObject);
    }
}
