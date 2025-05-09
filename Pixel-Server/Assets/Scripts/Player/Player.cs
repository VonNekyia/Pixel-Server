using System;
using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    

    
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();
    
    public ushort Id { get; private set; }
    public string Username { get; private set; }
    public bool isOnAttackCooldown { get; set; }
    

    public PlayerMovement Movement => movement;
    [SerializeField] private PlayerMovement movement;
    
    public PlayerAttack Attack => attack;
    [SerializeField] private PlayerAttack attack;
    
    

    private void OnDestroy()
    {
        list.Remove(Id);
    }
    
    public static void Spawn(ushort id, string username)
    {
        foreach (Player otherPlayer in list.Values)
        {
            otherPlayer.SendSpawned(id);
        }
        Debug.Log("Spawn");
        Player player = Instantiate(GameLogic.Singleton.PlayerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<Player>();
        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.Username = string.IsNullOrEmpty(username) ? $"Guest {id}" : username;
        SendEnemysOnPlayerJoin(player.Id);
       
        player.SendSpawned();
        list.Add(id, player);
    }
    
    public static void SendEnemysOnPlayerJoin(ushort clientID) {
        foreach (Enemy enemy in EnemySpawner.list.Values) {
            Debug.Log("Sende umbala daten");
            SendSpawned(enemy, clientID);
        }
    }
    
    static void SendSpawned(Enemy enemy, ushort clientID) {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientID.enemySpawned);
        message.AddInt(enemy.EnemyID);
        message.AddInt(enemy.EnemyType);
        message.AddVector3(enemy.transform.position);
        NetworkManager.Singleton.Server.Send(message,clientID);
    }

    #region Messages

    private void SendSpawned()
    {
        NetworkManager.Singleton.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientID.playerSpawned)));
    }

    private void SendSpawned(ushort toClientId)
    {
        NetworkManager.Singleton.Server.Send(AddSpawnData(Message.Create(MessageSendMode.reliable, ServerToClientID.playerSpawned)), toClientId);
    }
    
   
    
    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Id);
        message.AddString(Username);
        message.AddVector3(transform.position);
        return message;
    }
    
    //Übermittlung des Spielernamens
    [MessageHandler((ushort)ClientToServerID.name)]
    private static void Name(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());
    }

    //Übermittlung des Spielermovements
    [MessageHandler((ushort)ClientToServerID.input)]
    private static void Input(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
        {
            player.Movement.SetInput(message.GetBools(5));            
        }
    }
    
    //Übermittlung ob Spieler attackt
    [MessageHandler((ushort)ClientToServerID.attacking)]
    private static void isAttacking(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
        {
            if (!player.isOnAttackCooldown)
            {
                player.Movement.SetIsAttacking(message.GetBool());
                player.attack.Attack(player);
            }
            
        }
        
        
        
    }
        
  
        
    #endregion
    

}
  