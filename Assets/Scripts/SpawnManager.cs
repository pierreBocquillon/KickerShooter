using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public MapManager mapManager;

    public Enemy ennemyPrefab;
    public Enemy ennemyGunPrefab;
    public Enemy ennemyHealPrefab;
    public Enemy ennemySpeedPrefab;

    MapManager.Map currentMap;
    Transform[] spawnPoints;

    Queue<Enemy> spawnQueue;

    LivingEntity playerEntity;
    bool playerIsDead = false;
    
    float nextSpawnTime;

    public float timeBetweenMaps;
    public int enemiesRemainingAlive;
    bool mapFinished = false;
    float nextMapTimer;

    void Start(){
        mapManager.OnNewMap += OnNewMap;

        playerEntity = FindObjectOfType<Player>();
        playerEntity.OnDeath += OnPlayerDeath;

        StartWave();
    }

    void Update(){
        if(!playerIsDead && Time.timeScale > 0 && spawnQueue.Count > 0 && Time.time > nextSpawnTime){
            SpawnEnnemy(spawnQueue.Dequeue());
        }

        if(!currentMap.bossMap && !mapFinished && enemiesRemainingAlive <= 0){
            mapFinished = true;
            nextMapTimer = Time.time + timeBetweenMaps;
        }

        if(currentMap.bossMap && !mapFinished && currentMap.victoryCondition && enemiesRemainingAlive <= 0){
            mapFinished = true;
            nextMapTimer = Time.time + timeBetweenMaps;
        }

        if(!playerIsDead && Time.timeScale > 0 && mapFinished && Time.time > nextMapTimer){
            mapManager.NextMap();
        }
    }

    public void StartWave(){
        mapFinished = false;
        spawnQueue = new Queue<Enemy>();

        currentMap = mapManager.maps[mapManager.currentMapIndex];
        spawnPoints = new Transform[currentMap.gameObject.transform.Find("SpawnPoints").childCount];
        
        for(int i = 0; i < currentMap.gameObject.transform.Find("SpawnPoints").childCount; i++){
            spawnPoints[i] = currentMap.gameObject.transform.Find("SpawnPoints").GetChild(i);
        }

        enemiesRemainingAlive = currentMap.enemyCount + currentMap.enemyGunCount + currentMap.enemyHealCount + currentMap.enemySpeedCount;

        Enemy[] tmpEnemyArray = new Enemy[enemiesRemainingAlive];

        int tmpEnemyIndex = 0;

        for(int i = 0; i < currentMap.enemyCount; i++ ){
            tmpEnemyArray[tmpEnemyIndex] = ennemyPrefab;
            tmpEnemyIndex ++;
        }
        for(int i = 0; i < currentMap.enemyGunCount; i++ ){
            tmpEnemyArray[tmpEnemyIndex] = ennemyGunPrefab;
            tmpEnemyIndex ++;
        }
        for(int i = 0; i < currentMap.enemyHealCount; i++ ){
            tmpEnemyArray[tmpEnemyIndex] = ennemyHealPrefab;
            tmpEnemyIndex ++;
        }
        for(int i = 0; i < currentMap.enemySpeedCount; i++ ){
            tmpEnemyArray[tmpEnemyIndex] = ennemySpeedPrefab;
            tmpEnemyIndex ++;
        }

        for(int i = 0; i < tmpEnemyArray.Length; i++ ){
            int rnd = Random.Range(i, tmpEnemyArray.Length);
            Enemy tmpEnemy = tmpEnemyArray[rnd];
            tmpEnemyArray[rnd] = tmpEnemyArray[i];
            tmpEnemyArray[i] = tmpEnemy;
        }

        for(int i = 0; i < tmpEnemyArray.Length; i++ ){
            spawnQueue.Enqueue(tmpEnemyArray[i]);
        }

        nextSpawnTime = Time.time + currentMap.timeBetweenSpawn;
    }

    public void SpawnEnnemy(Enemy enemyPrefab){
        Transform spawnPoint = spawnPoints[Random.Range(0,spawnPoints.Length)];

        Enemy spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;

        nextSpawnTime = Time.time + currentMap.timeBetweenSpawn;
    }

    void OnNewMap(){
        StartWave();
    }

    void OnPlayerDeath(){
        playerIsDead = true;
    }

    void OnEnemyDeath(){
        enemiesRemainingAlive --;
    }

    public int GetWaveNumber(){
        return mapManager.currentMapIndex + 1;
    }
}
