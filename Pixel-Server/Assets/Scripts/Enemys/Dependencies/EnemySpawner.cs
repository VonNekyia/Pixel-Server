using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using UnityEngine;
using Random = System.Random;

public enum EnemyTypeID : ushort {
    Umbala = 1,
    Wolf = 2,
    Death = 3,
}

public class EnemySpawner : MonoBehaviour, IEnemySpawner {
    /*
    private static EnemySpawner _singleton;
    
    public static EnemySpawner Singleton {
        get => _singleton;
        private set {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value) {
                Debug.Log($"{nameof(EnemySpawner)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }
    */
    public static Dictionary<int, Enemy> list = new Dictionary<int, Enemy>();
    private ushort count = 0;
    private Random random = new Random();
    private bool cooldown;

    [Header("Spawn Settings")]
    [SerializeField] private ushort limit = 5;
    [SerializeField] private ushort interval;
    [SerializeField] private ushort MinLvl;
    [SerializeField] private ushort MaxLvl;
    [SerializeField] private ushort Spawnradius;
    [SerializeField] private EnemyTypeID enemyTypeID = EnemyTypeID.Umbala;
    [SerializeField] public GameObject umbalaPrefab;
    [SerializeField] public GameObject wolfPrefab;
    [SerializeField] public GameObject deathPrefab;
    /*
    public void Awake() {
        Singleton = this;
    }
    */
    private void Update() {
        if (count != limit && !cooldown) {
            
            int Lvl = random.Next(MinLvl,  MaxLvl);
            Vector3 spawnPositionRandomiser = new Vector3(random.Next(-Spawnradius,Spawnradius),random.Next(-Spawnradius,Spawnradius));
            
            GameObject enemyPrefab = enemyTypeID switch {
                EnemyTypeID.Umbala => umbalaPrefab,
                EnemyTypeID.Wolf => wolfPrefab,
                EnemyTypeID.Death => deathPrefab,
                _ => umbalaPrefab
            };

            Enemy enemy = Enemy.Spawn(gameObject.transform.position + spawnPositionRandomiser,enemyPrefab, (ushort)Lvl);

            enemy.Spawner = gameObject;
            enemy.transform.parent = gameObject.transform;
            enemy.type = (int)enemyTypeID;
            StartCoroutine(SpawnCooldown());
            cooldown = true;
            count++;
            
            list.Add(enemy.enemyID, enemy);
            SendSpawnedToAll(enemy);
        }
    }
    
    IEnumerator SpawnCooldown() {
        yield return new WaitForSeconds(interval);
        cooldown = false;
    }
    

    
    private void SendSpawnedToAll(Enemy enemy) {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientID.enemySpawned);
        message.AddInt(enemy.enemyID);
        message.AddInt(enemy.type);
        message.AddVector3(enemy.transform.position);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    public void ReduceCount(ushort number) {
       count -= number;
    }
}





