using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : People
{
    public enum State{
        Idel,Chasing,Attacking
    };
    State currentState;
    NavMeshAgent pathFinder;
    Transform target;//攻击目标
    Material skinMaterial;
    Color originColor;

    float attackDistanceThreshold = 1.5f;//攻击范围
    float timeBetweenAttacks = 1;//攻击间隔
    float nextAttackTime;
    float attackSpeed = 3;

    float localCollisionRadius;
    float targetCollisionRadius;

    override protected void Start() {
        base.Start();
        pathFinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        originColor = skinMaterial.color;
        currentState = State.Chasing;
        target = GameObject.FindGameObjectWithTag("Player").transform;//获取玩家对象
        localCollisionRadius = GetComponent<CapsuleCollider>().radius;
        targetCollisionRadius = GetComponent<CapsuleCollider>().radius;
        StartCoroutine(UpdatePath());//开启协程,追击玩家
    }

    private void Update() {
        if(Time.time > nextAttackTime){
            float sqrDistance = (target.position - transform.position).sqrMagnitude;//距离的平方
            if(sqrDistance < Mathf.Pow(attackDistanceThreshold + localCollisionRadius + targetCollisionRadius,2)){//在攻击范围内
                nextAttackTime = Time.time + timeBetweenAttacks;
                //攻击
                StartCoroutine(Attack());
            }
        }
    }
    IEnumerator Attack(){
        currentState = State.Attacking;
        pathFinder.enabled = false;
        Vector3 originalPos = transform.position;
        Vector3 dir = (target.position - transform.position).normalized;
        Vector3 targetPos = target.position - dir*(localCollisionRadius/2 + targetCollisionRadius);
        float percent = 0;
        skinMaterial.color = Color.red;
        while(percent <= 1){
            percent += Time.deltaTime * attackSpeed;
            float dist = ((-percent*percent)+percent)*4;
            transform.position = Vector3.Lerp(originalPos,targetPos,dist);
            yield return null;
        }
        skinMaterial.color = originColor;
        currentState = State.Chasing;
        pathFinder.enabled = true;
    }
    IEnumerator UpdatePath(){//每隔一段时间，更新追踪玩家是路径
        float refreshRate = 0.25f;//刷新频率
        while(target!=null){
            if(!dead&&currentState==State.Chasing){
                Vector3 dir = (target.position - transform.position).normalized;
                Vector3 targetPos = target.position - dir*(localCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
                pathFinder.SetDestination(targetPos);
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

}
