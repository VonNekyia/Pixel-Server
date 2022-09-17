using System;
using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using UnityEngine;

public class Enemy : MonoBehaviour, IEnemy
{
    public ushort enemyID { get; set; }
    public String enemyName { get; set; }
    public int livePoints { get; set; }
    public GameObject Spawner { get; set; }

    public void TakeDamage(int damage,Enemy enemy)
    {
        livePoints -= damage;
        if (livePoints <= 0)
        {
            Die(enemy);
        }
    }

    private void Die(Enemy enemy)
    {
        enemy.Spawner.gameObject.GetComponent<EnemySpawner>().count -= 1;
        Umbala.list.Remove(enemy.enemyID);
        EnemySpawner.list.Remove(enemy.enemyID);
        SendKill(enemy.enemyID);
        Destroy(enemy.gameObject);
        
        
    }
    
    public void SendMovement(ushort enemyID, Vector2 position)
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientID.enemyMovement);
        message.AddUShort(enemyID);
        message.AddVector3(position);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    
    public void SendKill(ushort enemyID)
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientID.enemyKilled);
        message.AddUShort(enemyID);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
}
