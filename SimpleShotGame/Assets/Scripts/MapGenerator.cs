using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;//地板的预制体
    public Transform obstaclePrefab;
    public Vector2 mapSize;//地图大小（n*m）
    [Range(0,1)]
    public float outlinePercent;//地板外轮廓大小
    [Range(0,1)]
    public float obstaclePercent;
    public int seed = 10;
    List<Coord> tileCoords;
    Queue<Coord> randomTileCoords;
    Coord birthCoord;
    private void Start() {
        GenerateMap();
    }
    public void GenerateMap(){
        string mapHolderName = "Generated Map";//地图存放的文件夹的名字
        if(transform.Find(mapHolderName)){//已经存在地图
            DestroyImmediate(transform.Find(mapHolderName).gameObject);//删除旧的地图
        }

        Transform mapHolder = new GameObject(mapHolderName).transform;//新建地图存放的文件夹
        mapHolder.parent = transform;

        for(int x=0;x<mapSize.x;x++){
            for(int y=0;y<mapSize.y;y++){
                Vector3 tilePosition = CoordToPos(x,y);//地板位置
                Transform newTile = Instantiate(tilePrefab,tilePosition,Quaternion.Euler(Vector3.right*90)) as Transform;
                newTile.localScale = Vector3.one*(1-outlinePercent);//地板大小
                newTile.parent = mapHolder;
            }
        }

        //生成障碍物
        birthCoord = new Coord((int)mapSize.x/2,(int)mapSize.y/2);//出生点
        tileCoords = new List<Coord>();//坐标列表
        for(int x = 0; x < mapSize.x; x++){
            for(int y = 0; y < mapSize.y; y++){
                tileCoords.Add(new Coord(x,y));
            }
        }
        randomTileCoords = new Queue<Coord>(Utility.ShuffleArray<Coord>(tileCoords.ToArray(), seed));//打乱的坐标列表

        bool[,] obstacles = new bool[(int)mapSize.x,(int)mapSize.y];//标记每一个位置是否放置了障碍物
        int obstaclePlaced = 0;
        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);//墙（障碍物）数目
        for(int i = 0;i < obstacleCount;i++){
            Coord randomCoord = GetRandomCoord();
            if(randomCoord!=birthCoord){
                obstacles[randomCoord.x,randomCoord.y] = true;//在该位置试放障碍物
                obstaclePlaced++;
                if(MapIsOneConnected(obstacles, obstaclePlaced)){//地图是单联通的
                    //放置障碍物
                    Vector3 obstaclePos = CoordToPos(randomCoord.x, randomCoord.y) + Vector3.up * .5f;
                    Transform newObstacle = Instantiate(obstaclePrefab,obstaclePos,Quaternion.identity) as Transform;
                    newObstacle.parent = mapHolder;
                }
                else{
                    obstacles[randomCoord.x,randomCoord.y] = false;
                    obstaclePlaced--;
                }
            }
        }
    }
    //判断一个网格图是否是单联通的
    bool MapIsOneConnected(bool[,] obstacles, int obstaclePlaced){
        bool[,] vis = new bool[obstacles.GetLength(0),obstacles.GetLength(1)];
        //bfs
        int[] dx = new int[4]{-1,0,0,1};
        int[] dy = new int[4]{0,-1,1,0};
        int connectedSize = 1;
        Queue<Coord> q = new Queue<Coord>();
        q.Enqueue(birthCoord);
        vis[birthCoord.x,birthCoord.y] = true;
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
        return connectedSize + obstaclePlaced == mapSize.x * mapSize.y;
    }
    //判断一个坐标是否在网格图内
    bool InGrid(int x,int y){
        return x >=0 && y >= 0 && x < mapSize.x && y < mapSize.y;
    }
    Coord GetRandomCoord(){//获取随机坐标
        Coord randomCoord = randomTileCoords.Dequeue();//从随机序列里面获取
        randomTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }
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

    Vector3 CoordToPos(int x,int y){
        return new Vector3(-mapSize.x/2 + x + (mapSize.x%2==0?.4f:0), 0, -mapSize.y/2 + y + (mapSize.y%2==0?.5f:0));
    }
}
