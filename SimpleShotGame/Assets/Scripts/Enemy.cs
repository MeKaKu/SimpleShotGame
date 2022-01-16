using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : People
{
    public enum State{
        Idle,Chasing,Attacking
    };
    State currentState;
    NavMeshAgent pathFinder;
    Transform target;//攻击目标
    People targetPeople;//攻击目标的实体
    bool haveTarget;//是否存在攻击目标
    Material skinMaterial;
    Color originColor;

    float attackDistanceThreshold = 1.5f;//攻击范围
    float timeBetweenAttacks = 1;//攻击间隔
    float nextAttackTime;
    float attackSpeed = 3;
    float damage = 1;

    float localCollisionRadius;
    float targetCollisionRadius;

    override protected void Start() {
        base.Start();
        pathFinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        originColor = skinMaterial.color;
        localCollisionRadius = GetComponent<CapsuleCollider>().radius;
        FindTarget();//寻找目标
    }

    private void Update() {
        if(haveTarget){
            if(Time.time > nextAttackTime){
                float sqrDistance = (target.position - transform.position).sqrMagnitude;//距离的平方
                if(sqrDistance < Mathf.Pow(attackDistanceThreshold + localCollisionRadius + targetCollisionRadius,2)){//在攻击范围内
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    //攻击
                    StartCoroutine(Attack());
                }
            }
        }
    }
    void FindTarget(){
        if(GameObject.FindGameObjectWithTag("Player")!=null){
            haveTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;//获取目标
            currentState = State.Chasing;//追击状态
            StartCoroutine(UpdatePath());//开启协程,追击玩家
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
            targetPeople = target.GetComponent<People>();
            targetPeople.OnDeath += OnTargetDeath;
        }
    }
    void OnTargetDeath(){
        currentState = State.Idle;
        haveTarget = false;
    }
    //攻击
    IEnumerator Attack(){
        currentState = State.Attacking;
        pathFinder.enabled = false;
        Vector3 originalPos = transform.position;
        Vector3 dir = (target.position - transform.position).normalized;
        Vector3 targetPos = target.position - dir*(localCollisionRadius/2 + targetCollisionRadius);
        float percent = 0;
        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;
        while(percent <= 1){
            if(percent >= .5f && !hasAppliedDamage){
                hasAppliedDamage = true;
                targetPeople.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float dist = ((-percent*percent)+percent)*4;
            transform.position = Vector3.Lerp(originalPos,targetPos,dist);
            yield return null;
        }
        skinMaterial.color = originColor;
        currentState = State.Chasing;
        pathFinder.enabled = true;
    }
    //追逐目标
    IEnumerator UpdatePath(){//每隔一段时间，更新追踪玩家是路径
        float refreshRate = 0.25f;//刷新频率
        while(haveTarget){
            if(!dead&&currentState==State.Chasing){
                Vector3 dir = (target.position - transform.position).normalized;
                Vector3 targetPos = target.position - dir*(localCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
                pathFinder.SetDestination(targetPos);
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

}
