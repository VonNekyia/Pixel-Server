using System;
using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using UnityEngine;

public class Chat : MonoBehaviour
{
    [MessageHandler((ushort)ClientToServerID.playerMessageReceiver)]
    private static void getMessage(ushort fromClientId, Message message)
    {

        if (Player.list.TryGetValue(fromClientId, out Player player))
        {
            String stringMessage = message.GetString();
            Debug.Log(player.Username + ": " + stringMessage);
            sendMessage(player, stringMessage);
        }
            
        
    }
    
    private static void sendMessage( Player player,String messageInput)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientID.playerMessage);
        message.AddUShort(player.Id);
        message.AddString(messageInput);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
}
