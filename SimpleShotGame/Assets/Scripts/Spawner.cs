using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//敌人生产系统
public class Spawner : MonoBehaviour
{
    public Wave[] waves;//每波敌人的信息

    public Enemy enemy;

    int enemyRemainToSpawn;//当前波剩余的待生成敌人数目
    int enemyRemainAlive;//当前波剩余的未被消灭的敌人
    int currentWaveNumber;//当前是第几波
    Wave currentWave;
    float nextSpawnTime;//下一个敌人生成时间

    void Start() {
        nextWave();
    }
    void Update() {
        if(enemyRemainToSpawn > 0 && Time.time > nextSpawnTime){
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawn;
            enemyRemainToSpawn --;
            //生成敌人
            Enemy spawnedEnemy = Instantiate<Enemy>(enemy,Vector3.zero,Quaternion.identity);
            spawnedEnemy.OnDeath += OnEnemyDeath;//将OnEnemyDeath加入委托事件
        }
    }

    void OnEnemyDeath(){//敌人死亡时
        enemyRemainAlive --;
        if(enemyRemainAlive == 0){
            nextWave();
        }
    }

    void nextWave(){ //设置下一波敌人信息
        currentWaveNumber ++;//当前波数加一
        //print("Waves : " + currentWaveNumber);
        if(currentWaveNumber <= waves.Length){
            currentWave = waves[currentWaveNumber-1];
            enemyRemainToSpawn = currentWave.enemyCount;
            enemyRemainAlive = enemyRemainToSpawn;
        }
    }

    [System.Serializable]//在编辑器的属性栏可见
    public class Wave{//每一波敌人的信息
        public int enemyCount;//敌人数目
        public float timeBetweenSpawn;//生成敌人间隔的时间(s)
    }
}
