using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : People
{
    NavMeshAgent pathFinder;
    Transform target;

    override protected void Start() {
        base.Start();
        pathFinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(UpdatePath());
    }

    private void Update() {
        
    }

    IEnumerator UpdatePath(){
        float refreshRate = 0.25f;//刷新频率
        while(target!=null){
            if(!dead){
                pathFinder.SetDestination(target.position);
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

}
