using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask layerMask;//Layer，检测碰撞的layer
    private float speed;
    float damage = 1;
    float lifeTime = 3;//子弹存在时间
    float objectMoveDistance = .1f;//目标向子弹移动的距离
    public void SetSpeed(float _speed){
        speed = _speed;
    }
    private void Start() {
        Destroy(gameObject,lifeTime);//在lifeTime（s）时间后摧毁子弹

        //检测子弹是否在产生时就在碰撞体内，
        Collider[] colliders = Physics.OverlapSphere(transform.position,.1f,layerMask);//在半径为.1f为半径的球内否覆盖的碰撞体
        if(colliders.Length>0){//在碰撞体内
            OnHitObject(colliders[0]);
        }
    }

    private void Update() {
        float moveDistance = Time.deltaTime * speed;//子弹移动的距离
        CheckCollisions(moveDistance + objectMoveDistance);//碰撞检测
        transform.Translate(Vector3.forward * moveDistance);
    }
    void CheckCollisions(float moveDistance){
        Ray ray = new Ray(transform.position,transform.forward); //子弹向前投射射线
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,moveDistance,layerMask,QueryTriggerInteraction.Collide)){//如果在移动距离内碰到了物体
            OnHitObject(hit);
        }
    }
    void OnHitObject(RaycastHit hit){
        //print(hit.collider.gameObject.name);
        IDamageable damageObject = hit.collider.gameObject.GetComponent<IDamageable>();
        if(damageObject!=null){
            damageObject.TakeHit(damage,hit);
        }
        Destroy(gameObject);
    }
    void OnHitObject(Collider collider){
        //print(hit.collider.gameObject.name);
        IDamageable damageObject = collider.gameObject.GetComponent<IDamageable>();
        if(damageObject!=null){
            damageObject.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
