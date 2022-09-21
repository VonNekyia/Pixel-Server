using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using UnityEngine;
using UnityEngine.Serialization;

public enum EnemyTypeID : ushort {
    GoldenUmbala = 1,
    Wolf = 2,
    Death = 3,
    GreenUmbala = 4,
}

public class EnemySpawner : MonoBehaviour, IEnemySpawner {

    public static Dictionary<int, Enemy> list = new Dictionary<int, Enemy>();
    
    private ushort enemySpawnCount;
    private bool isOnSpawnCooldown;
    private static int _id;
    
    [Header("Spawn Settings")]
    [SerializeField] private EnemyTypeID enemyTypeID = EnemyTypeID.GoldenUmbala;
    [SerializeField] private ushort minLvl;
    [SerializeField] private ushort maxLvl;
    [SerializeField] private float spawnRadius;
    [SerializeField] private ushort enemySpawnLimit = 5;
    [SerializeField] private ushort spawnInterval;

    [Header("Prefabs")]
    [SerializeField] public GameObject goldenUmbalaPrefab;
    [SerializeField] public GameObject wolfPrefab;
    [SerializeField] public GameObject deathPrefab;
    [SerializeField] public GameObject greenUmbalaPrefab;

    private void Update() {
        
        if (enemySpawnCount != enemySpawnLimit && !isOnSpawnCooldown) {
            float lvl = Random.Range(minLvl, maxLvl);;
            Vector3 spawnPositionRandomiser = new Vector3(Random.Range(-spawnRadius,spawnRadius),Random.Range(-spawnRadius,spawnRadius));
            GameObject enemyPrefab = enemyTypeID switch {
                EnemyTypeID.GoldenUmbala => goldenUmbalaPrefab,
                EnemyTypeID.Wolf => wolfPrefab,
                EnemyTypeID.Death => deathPrefab,
                EnemyTypeID.GreenUmbala => greenUmbalaPrefab,
                _ => goldenUmbalaPrefab
            };

            Enemy enemy = Spawn(gameObject.transform.position + spawnPositionRandomiser,enemyPrefab, (ushort)lvl);
            enemy.Spawner = gameObject;
            enemy.transform.parent = gameObject.transform;
            enemy.EnemyType = (int)enemyTypeID;
            list.Add(enemy.EnemyID, enemy);
            
            enemySpawnCount++;
            StartCoroutine(SpawnCooldown());
            isOnSpawnCooldown = true;
            
            SendSpawnedToAll(enemy);
        }
    }
    
    IEnumerator SpawnCooldown() {
        yield return new WaitForSeconds(spawnInterval);
        isOnSpawnCooldown = false;
    }

    private static Enemy Spawn(Vector2 position, GameObject enemyPrefab, ushort lvl) {
        Enemy enemy = Instantiate(enemyPrefab, position, Quaternion.identity).GetComponent<Enemy>();
        enemy.EnemyName = "Enemy";
        enemy.EnemyID = _id;
        enemy.LivePoints = lvl * 1;
        _id++;
        return enemy;
        
    }

    private void SendSpawnedToAll(Enemy enemy) {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientID.enemySpawned);
        message.AddInt(enemy.EnemyID);
        message.AddInt(enemy.EnemyType);
        message.AddVector3(enemy.transform.position);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    public void ReduceCount(ushort number) {
       enemySpawnCount -= number;
    }
}





