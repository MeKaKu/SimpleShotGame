using System.Security.Claims;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;//地板的预制体
    public Transform obstaclePrefab;
    public float scale = 1; //地图缩放比例
    [Range(0,1)]
    public float outlinePercent;//地板外轮廓大小
    public Transform navmeshFloor;//导航网格
    public Vector2 navSize;//导航区域的大小
    //public Transform navmeshMasker;//空气墙
    List<Coord> tileCoords;//所有的坐标
    Queue<Coord> randomTileCoords;//打乱的坐标
    Queue<Coord> randomOpenTileCoords;//打乱的空格子的坐标
    Transform[,] tiles;//地板
    public Map[] maps;//地图数组
    public int mapIndex;
    Map currentMap;
    Spawner spawner;
    private void Start() {
        //GenerateMap();
        //spawner = FindObjectOfType<Spawner>();
        //spawner.OnNewWave += OnNewWave;
    }
    // void OnNewWave(int newWaveIndex){
    //     mapIndex = newWaveIndex - 1;
    //     print(newWaveIndex);
    //     GenerateMap();
    // }
    public void GenerateMap(){
        currentMap = maps[mapIndex];//当前生成的地图
        System.Random rand = new System.Random(currentMap.seed);//获取一个随机数序列
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * scale, .05f, currentMap.mapSize.y * scale);//设置地图碰撞盒的大小
        tiles = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];


        string mapHolderName = "Generated Map";//地图存放的文件夹的名字
        if(transform.Find(mapHolderName)){//已经存在地图
            DestroyImmediate(transform.Find(mapHolderName).gameObject);//删除旧的地图
        }

        Transform mapHolder = new GameObject(mapHolderName).transform;//新建地图存放的文件夹
        mapHolder.parent = transform;

        //生成地图地板
        for(int x=0;x<currentMap.mapSize.x;x++){
            for(int y=0;y<currentMap.mapSize.y;y++){
                Vector3 tilePosition = CoordToPos(x,y);//地板位置
                Transform newTile = Instantiate(tilePrefab,tilePosition,Quaternion.Euler(Vector3.right*90)) as Transform;
                newTile.localScale = Vector3.one*(1-outlinePercent) * scale;//地板大小
                newTile.parent = mapHolder;
                tiles[x,y] = newTile;
            }
        }

        //生成障碍物
        tileCoords = new List<Coord>();//坐标列表
        for(int x = 0; x < currentMap.mapSize.x; x++){
            for(int y = 0; y < currentMap.mapSize.y; y++){
                tileCoords.Add(new Coord(x,y));
            }
        }
        randomTileCoords = new Queue<Coord>(Utility.ShuffleArray<Coord>(tileCoords.ToArray(), currentMap.seed));//打乱的坐标列表

        bool[,] obstacles = new bool[(int)currentMap.mapSize.x,(int)currentMap.mapSize.y];//标记每一个位置是否放置了障碍物
        int obstaclePlaced = 0;
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);//墙（障碍物）数目
        for(int i = 0;i < obstacleCount;i++){
            Coord randomCoord = GetRandomCoord();
            if(randomCoord!=currentMap.birthCoord){
                obstacles[randomCoord.x,randomCoord.y] = true;//在该位置试放障碍物
                obstaclePlaced++;
                if(MapIsOneConnected(obstacles, obstaclePlaced)){//地图是单联通的
                    //放置障碍物
                    float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight,currentMap.maxObstacleHeight,(float)rand.NextDouble());//障碍物的高度
                    Vector3 obstaclePos = CoordToPos(randomCoord.x, randomCoord.y) + Vector3.up * obstacleHeight/2;
                    Transform newObstacle = Instantiate(obstaclePrefab,obstaclePos,Quaternion.identity) as Transform;
                    newObstacle.localScale = new Vector3((1-outlinePercent) * scale, obstacleHeight, (1-outlinePercent) * scale);//障碍物大小
                    newObstacle.parent = mapHolder;

                    Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                    Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                    float colorPercent = randomCoord.y / (float)currentMap.mapSize.y;
                    obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor,currentMap.backgroundColor,colorPercent);
                    obstacleRenderer.sharedMaterial = obstacleMaterial;
                }
                else{
                    obstacles[randomCoord.x,randomCoord.y] = false;
                    obstaclePlaced--;
                }
            }
        }

        //没有障碍物的格子
        List<Coord> openTileCoords = new List<Coord>();
        for(int i = 0; i < currentMap.mapSize.x; i++){
            for(int j = 0; j < currentMap.mapSize.y; j++){
                if(!obstacles[i, j]){
                    openTileCoords.Add(new Coord(i, j));
                }
            }
        }
        randomOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray<Coord>(openTileCoords.ToArray(),currentMap.seed));
        //print(randomOpenTileCoords.Count);
        //导航网格
        navSize = new Vector2(currentMap.mapSize.x, currentMap.mapSize.y);
        navmeshFloor.localScale = new Vector3(navSize.x,navSize.y) * scale;
    }
    //判断一个网格图是否是单联通的
    bool MapIsOneConnected(bool[,] obstacles, int obstaclePlaced){
        bool[,] vis = new bool[obstacles.GetLength(0),obstacles.GetLength(1)];
        //bfs
        int[] dx = new int[4]{-1,0,0,1};
        int[] dy = new int[4]{0,-1,1,0};
        int connectedSize = 1;
        Queue<Coord> q = new Queue<Coord>();
        q.Enqueue(currentMap.birthCoord);
        vis[currentMap.birthCoord.x,currentMap.birthCoord.y] = true;
        while(q.Count>0){
            Coord u = q.Dequeue();
            for(int i=0;i<4;i++){
                int x = u.x + dx[i];
                int y = u.y + dy[i];
                if(InGrid(x,y)&&!vis[x,y]&&!obstacles[x,y]){
                    vis[x,y] = true;
                    q.Enqueue(new Coord(x,y));
                    connectedSize++;
                }
            }
        }
        return connectedSize + obstaclePlaced == currentMap.mapSize.x * currentMap.mapSize.y;
    }
    //判断一个坐标是否在网格图内
    bool InGrid(int x,int y){
        return x >=0 && y >= 0 && x < currentMap.mapSize.x && y < currentMap.mapSize.y;
    }
    //获取随机坐标
    Coord GetRandomCoord(){
        Coord randomCoord = randomTileCoords.Dequeue();//从随机序列里面获取
        randomTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }
    //随机获取空格子
    public Transform GetRandomOpenCoord(){
        Coord randomCoord = randomOpenTileCoords.Dequeue();//从随机序列里面获取
        randomOpenTileCoords.Enqueue(randomCoord);
        return tiles[randomCoord.x, randomCoord.y];
    }
    //根据坐标获取位置
    Vector3 CoordToPos(int x,int y){
        return new Vector3(-currentMap.mapSize.x/2 + x + (currentMap.mapSize.x%2==0?.5f:0), 0, -currentMap.mapSize.y/2 + y + (currentMap.mapSize.y%2==0?.5f:0)) * scale;
    }
    //根据位置获取格子
    public Transform PosToTile(Vector3 pos){
        int x = Mathf.RoundToInt(currentMap.mapSize.x/2 + pos.x/scale - (currentMap.mapSize.x%2==0?.4f:0));
        int y = Mathf.RoundToInt(currentMap.mapSize.y/2 + pos.z/scale - (currentMap.mapSize.y%2==0?.4f:0));
        x = Mathf.Clamp(x, 0, tiles.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, tiles.GetLength(1) - 1);
        return tiles[x, y];
    }
    [System.Serializable]
    public struct Coord{//坐标结构体
        public int x,y;
        public Coord(int _x,int _y){
            x = _x;
            y = _y;
        }
        public static bool operator != (Coord a,Coord b){
            return a.x!=b.x || a.y!=b.y;
        }
        public static bool operator == (Coord a,Coord b){
            return a.x==b.x && a.y==b.y;
        }
    }

    //地图类，存放地图的属性
    [System.Serializable]
    public class Map{
        public Coord mapSize;//地图大小（n*m）
        [Range(0,1)]
        public float obstaclePercent;//障碍物数目
        public int seed;//随机种子
        public float minObstacleHeight;//障碍物最小高度
        public float maxObstacleHeight;//障碍物最大高度
        public Color foregroundColor;//前景颜色
        public Color backgroundColor;//后景颜色

        public Coord birthCoord {
            get{
                return new Coord(mapSize.x/2, mapSize.y/2);
            }
        }
    }
}
