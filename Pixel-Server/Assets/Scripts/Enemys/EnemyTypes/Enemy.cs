using System;
using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public int enemyID { get; set; }
    public String enemyName { get; set; }
    public int livePoints { get; set; }
    public GameObject Spawner { get; set; }
    public int type { get; set; }
    private static int _id = 0;
    
    

    private void FixedUpdate() {
        SendMovement(enemyID,gameObject.transform.position);
    }
    
    public void TakeDamage(int damage,Enemy enemy) { 
        livePoints -= damage;
        if (livePoints <= 0) {
            Die(enemy);
        }
        else {
            SendEnemyState(enemy.enemyID, false); 
        }
    }

    private void Die(Enemy enemy) {
        enemy.Spawner.gameObject.GetComponent<EnemySpawner>().ReduceCount(1);
        EnemySpawner.list.Remove(enemy.enemyID);
        SendEnemyState(enemy.enemyID,true);
        Destroy(enemy.gameObject);
    }
    
    public void SendMovement(int enemyID, Vector2 position) {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientID.enemyMovement);
        message.AddInt(enemyID);
        message.AddVector3(position);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    
    public void SendEnemyState(int enemyID, bool state) {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientID.enemyKilled);
        message.AddInt(enemyID);
        message.AddBool(state);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    
    public static Enemy Spawn(Vector2 position, GameObject enemyPrefab, ushort lvl) {
        Enemy enemy = Instantiate(enemyPrefab, position, Quaternion.identity).GetComponent<Enemy>();
        enemy.enemyName = "Umbala";
        enemy.enemyID = _id;
        enemy.livePoints = lvl * 1;
        _id++;
        return enemy;
    }
}
