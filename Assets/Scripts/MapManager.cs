using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Map[] maps;

    public int currentMapIndex;

    public float minObstacleHeight;
    public float maxObstacleHeight;

    GameUI gameUI;
    Player player;

    public event System.Action OnNewMap;

    void Start() {
        player = FindObjectOfType<Player>();
        gameUI = FindObjectOfType<GameUI>();

        PreProcessMaps();
        LoadMap(currentMapIndex);
    }

    public void NextMap(){
        LoadMap(currentMapIndex + 1);

        if(OnNewMap != null){
            OnNewMap();
        }
    }

    public void LoadMap(int mapIndex){
        if(mapIndex < maps.Length){
            Pickup[] remainingPickups = FindObjectsOfType<Pickup>();
            foreach(Pickup pickup in remainingPickups){
                Destroy(pickup.gameObject);
            }

            player.transform.position = new Vector3(0, 1, 0);
            currentMapIndex = mapIndex;
            foreach(Map map in maps){
                map.gameObject.SetActive(false);
            }
            maps[currentMapIndex].gameObject.SetActive(true);
        }else{
            gameUI.OnGameEnd();
        }
    }

    public void PreProcessMaps(){
        foreach(Map map in maps){
            foreach(Transform obstacle in map.gameObject.transform.Find("Obstacles").transform){
                Material obstacleMat = obstacle.GetComponent<Renderer>().material;
                obstacleMat.color = map.obstaclesColor;

                obstacle.localScale = new Vector3(obstacle.localScale.x, obstacle.localScale.y, Random.Range(minObstacleHeight, maxObstacleHeight));
                obstacle.position = new Vector3(obstacle.position.x, obstacle.localScale.z/2, obstacle.position.z);
            }
        }
    }

    [System.Serializable]
    public class Map{
        public GameObject gameObject;
        public Color obstaclesColor;
        public int enemyCount;
        public int enemyHealCount;
        public int enemyGunCount;
        public int enemySpeedCount;
        public float timeBetweenSpawn;
        public bool bossMap = false;
        public bool victoryCondition = false;
    }
}
