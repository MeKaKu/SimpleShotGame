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
    MapGenerator mapGenerator;//地图

    //Player
    People playerPeople;
    Transform playerTransform;
    bool isPlayerAlive;

    public event System.Action<int> OnNewWave;

    //检查玩家是否停留
    float timeBetweenCampChecks = 2;//检查玩家是否停留的时间间隔
    float campThresholdDistance = 1.5f;//是否停留的距离阈值
    Vector3 campPositionOld;//停留的位置
    float nextCampCheckTime;
    bool isCamping;//是否停留


    void Start() {
        //Player
        playerPeople = FindObjectOfType<Player>();
        playerTransform = playerPeople.transform;
        playerPeople.OnDeath += OnPlayerDeath;
        isPlayerAlive = (playerPeople != null);
        campPositionOld = playerTransform.position;
        nextCampCheckTime = Time.time + timeBetweenCampChecks;
        isCamping = false;

        mapGenerator = GameObject.FindObjectOfType<MapGenerator>();
        nextWave();
    }
    void OnPlayerDeath(){
        isPlayerAlive = false;
    }
    void Update() {
        if(!isPlayerAlive){
            return;
        }
        //检查玩家是否停留
        if(Time.time > nextCampCheckTime){
            nextCampCheckTime = Time.time + timeBetweenCampChecks;
            isCamping = (Vector3.Distance(playerTransform.position, campPositionOld) < campThresholdDistance);
            campPositionOld = playerTransform.position;
        }
        //生成敌人
        if(enemyRemainToSpawn > 0 && Time.time > nextSpawnTime){
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawn;
            enemyRemainToSpawn --;
            StartCoroutine(SpawnEnemy());
        }
    }

    IEnumerator SpawnEnemy(){
        //生成敌人的位置
        Transform tile = mapGenerator.GetRandomOpenCoord();
        if(isCamping){//如果玩家在原地停留，则在玩家所在位置生成敌人
            tile = mapGenerator.PosToTile(playerTransform.position);
        }
        Material tileMat = tile.GetComponent<Renderer>().material;
        Color initialColor = tileMat.color;
        Color flashColor = Color.red;

        float spawnDelay = 1;//敌人延迟生成的时间（提醒时间）(s)
        float flashSpeed = 4;//地板闪烁速度
        float spawnTimer = 0;

        while(spawnTimer < spawnDelay){
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer*flashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }
        //生成敌人
        Enemy spawnedEnemy = Instantiate<Enemy>(enemy,tile.position + Vector3.up,Quaternion.identity);
        spawnedEnemy.OnDeath += OnEnemyDeath;//将OnEnemyDeath加入委托事件
        spawnedEnemy.SetCharacteristics(currentWave.enemyHealth, currentWave.moveSpeed, currentWave.attackDamage, currentWave.skinColor);
        yield return null;
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
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawn;
            enemyRemainToSpawn = currentWave.enemyCount;
            enemyRemainAlive = enemyRemainToSpawn;
            if(OnNewWave!=null){
                print("???");
                OnNewWave(currentWaveNumber);
            }
            mapGenerator.mapIndex = currentWaveNumber - 1;
            mapGenerator.GenerateMap();
            ResetPlayerPos();
        }
    }
    //重新设置玩家的位置
    void ResetPlayerPos(){
        playerTransform.position = mapGenerator.PosToTile(Vector3.zero).position + Vector3.up*3;
        //playerTransform.position = Vector3.up*3;
        //print(playerTransform.position);
    }

    [System.Serializable]//在编辑器的属性栏可见
    public class Wave{//每一波敌人的信息
        public int enemyCount;//敌人数目
        public float timeBetweenSpawn;//生成敌人间隔的时间(s)
        public float moveSpeed;//敌人移动速度
        public float attackDamage;//敌人攻击伤害
        public float enemyHealth;//敌人的HP
        public Color skinColor;//敌人的颜色

    }
}
