using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int score{get; private set;}
    float lastKilledTime;//最后一次击杀敌人
    float bonusExpiryTime = 1f;//连击有效时间
    int bonusCount;//连击数目
    private void Start() {
        score = 0;
        Enemy.OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
    }
    void OnEnemyKilled(){
        if(Time.time < lastKilledTime + bonusExpiryTime){
            bonusCount ++;
        }
        else{
            bonusCount = 1;
        }
        lastKilledTime = Time.time;
        score +=  5 + (int)Mathf.Pow(2, bonusCount - 1);
    }

    void OnPlayerDeath(){
        Enemy.OnDeathStatic -= OnEnemyKilled;//取消事件订阅
    }
}
