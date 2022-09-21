using System;
using RiptideNetworking;
using UnityEngine;

    public class Enemy : MonoBehaviour {
        
        public int EnemyID { get; set; }
        public String EnemyName { get; set; }
        public int LivePoints { get; set; }
        public GameObject Spawner { get; set; }
        public int EnemyType { get; set; }
        
        private void FixedUpdate() {
            SendMovement(EnemyID,gameObject.transform.position);
        }
        
        public void DamageEnemy(Enemy enemy, int damage) { 
            LivePoints -= damage;
            if (LivePoints <= 0) KillEnemy(enemy);
            else SendEnemyState(enemy.EnemyID, false);
        }
    
        private void KillEnemy(Enemy enemy) {
            enemy.Spawner.gameObject.GetComponent<EnemySpawner>().ReduceCount(1);
            EnemySpawner.list.Remove(enemy.EnemyID);
            SendEnemyState(enemy.EnemyID,true);
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
    }


