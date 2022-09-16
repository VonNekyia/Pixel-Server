using System;
using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using UnityEngine;
using Random = System.Random;

public class EnemySpawner : MonoBehaviour, IEnemySpawner
{
    
    public static Dictionary<ushort, Umbala> list = new Dictionary<ushort, Umbala>();
    public ushort count = 0;
    private ushort limit = 5;
    [Header("Spawn Settings")]
    [SerializeField] private ushort interval;
    [SerializeField] private ushort MinLvl;
    [SerializeField] private ushort MaxLvl;
    private bool cooldown;
    [SerializeField] private GameObject umbalaPrefab;


    
    void Update()
    {
        if (!cooldown && count != 5)
        {
            Random random = new Random();
            int Lvl = random.Next(MinLvl,  MaxLvl);
            Umbala umbala = Umbala.Spawn(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - count), umbalaPrefab, (ushort)Lvl);
            umbala.Spawner = gameObject;
            StartCoroutine(Cooldown());
            cooldown = true;
            count++;
            Umbala.list.Add(umbala.enemyID, umbala);
            SendSpawned(umbala);
        }
        
    }
        
    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(interval);
        cooldown = false;
    }

    private void SendSpawned(Umbala umbala)
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientID.enemySpawned);
        message.AddUShort(umbala.enemyID);
        message.AddVector3(umbala.transform.position);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    
    private void SendMovement(Umbala umbala)
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientID.enemySpawned);
        message.AddVector3(umbala.transform.position);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    

}
