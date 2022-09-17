using System;
using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using UnityEngine;
using Random = System.Random;

public class EnemySpawner : MonoBehaviour, IEnemySpawner
{
    private static EnemySpawner _singleton;

    public static EnemySpawner Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(EnemySpawner)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }
    
    
    public static Dictionary<ushort, Umbala> list = new Dictionary<ushort, Umbala>();
    public ushort count = 0;
    private ushort limit = 5;
    [Header("Spawn Settings")]
    [SerializeField] private ushort interval;
    [SerializeField] private ushort MinLvl;
    [SerializeField] private ushort MaxLvl;
    private bool cooldown;
    [SerializeField] private GameObject umbalaPrefab;

    public void Awake()
    {
        Singleton = this;
    }
    
    void Update()
    {
        if (!cooldown && count != 5)
        {
            Random random = new Random();
            int Lvl = random.Next(MinLvl,  MaxLvl);
            Umbala umbala = Umbala.Spawn(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - count), umbalaPrefab, (ushort)Lvl);
            umbala.Spawner = gameObject;
            umbala.transform.parent = gameObject.transform;
            StartCoroutine(Cooldown());
            cooldown = true;
            count++;
            Umbala.list.Add(umbala.enemyID, umbala);
            list.Add(umbala.enemyID, umbala);
            SendSpawnedToAll(umbala);
        }
        
    }

    public void SpawnEnemys(ushort clientID)
    {
        
        foreach (Umbala umbala in list.Values)
        {
            Debug.Log("Sende umbala daten");
            SendSpawned(umbala, clientID);
        }
    }
    
    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(interval);
        cooldown = false;
    }

    private void SendSpawnedToAll(Umbala umbala)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientID.enemySpawned);
        message.AddUShort(umbala.enemyID);
        message.AddVector3(umbala.transform.position);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    
    private void SendSpawned(Umbala umbala, ushort clientID)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientID.enemySpawned);
        message.AddUShort(umbala.enemyID);
        message.AddVector3(umbala.transform.position);
        NetworkManager.Singleton.Server.Send(message,clientID);
    }

}
