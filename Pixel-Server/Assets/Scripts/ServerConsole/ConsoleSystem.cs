using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RiptideNetworking;
using RiptideNetworking.Transports;
using UnityEngine;

public class ConsoleSystem : MonoBehaviour
{
    private static ConsoleSystem _singleton;
    public static ConsoleSystem Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(ConsoleSystem)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }
    
    public void Awake()
    {
        Singleton = this;
    }
    
    public void Run(string command)
    {
        String[] args = command.Split(" ");

        if (args[0].Contains("commands"))
        {
            Debug.Log(args.Length);
            Debug.Log("Commands:" +
                      "\nstats" +
                      "\nkick <username>" +
                      "\nstop" +
                      "\nstart [MaxClients] [port]" +
                      "\nrestart [MaxClients] [port]" +
                      "\nplayerlist");
        }
        
        if (args[0].Contains("kick"))
            if (args.Length >= 2)
            {
                foreach (KeyValuePair<ushort, Player> p in Player.list)
                    if (p.Value.Username == args[1])
                    {
                        NetworkManager.Singleton.Server.DisconnectClient(p.Key);
                        Debug.Log($"EXECUTE: Kicked {p.Value.Username} successfully.");
                    } else
                        Debug.Log("ERROR: Spieler konnte nicht gefunden werden.");
            }
            else
                Debug.Log("ERROR: Bitte geben Sie einen Spielernamen an.");
            
        
        if (args[0].Contains("stats"))
        {
            Debug.Log($"Serverstatus: {NetworkManager.Singleton.Server.IsRunning}" +
                      $"\nClients: {NetworkManager.Singleton.Server.ClientCount} / {NetworkManager.Singleton.Server.MaxClientCount}" +
                      $"\nPort: {NetworkManager.Singleton.Server.Port}");
        }
        
        if (args[0].Contains("stop"))
            NetworkManager.Singleton.Server.Stop();
        
        
        if (args[0].Contains("start"))
        {
            ushort port = 30001;
            ushort maxClientCount = NetworkManager.Singleton.Server.MaxClientCount;

#pragma warning disable 0649
            if (args.Length >= 2 && ushort.TryParse(args[1], out maxClientCount))
            {
            } else if(args.Length >= 2) Debug.Log("ERROR: Client-Count wurde falsch agegeben.");

            if (args.Length >= 3 && ushort.TryParse(args[2], out port))
            {
            } else if(args.Length >= 3) Debug.Log("ERROR: Port wurde falsche angegeben.");
#pragma warning restore 0649
            
            
         
        }
        
        if (args[0].Contains("restart"))
        {
            NetworkManager.Singleton.Server.Stop();
            ushort port = NetworkManager.Singleton.Server.Port;
            ushort maxClientCount = NetworkManager.Singleton.Server.MaxClientCount;
            
#pragma warning disable 0649          
            if (args.Length >= 2 && ushort.TryParse(args[1], out maxClientCount))
            {
            } else if(args.Length >= 2) Debug.Log("ERROR: Client-Count wurde falsch agegeben.");

            if (args.Length >= 3 && ushort.TryParse(args[2], out port))
            {
            } else if(args.Length >= 3) Debug.Log("ERROR: Port wurde falsche angegeben.");
#pragma warning restore 0649
            
            StartCoroutine(StartServer(port,maxClientCount));
           
        }
       
        
        if (args[0].Contains("playerlist"))
        {
            foreach (IConnectionInfo clientInfo in NetworkManager.Singleton.Server.Clients)
            {
                foreach (KeyValuePair<ushort, Player> p in Player.list)
                    if (p.Key == clientInfo.Id)
                    {
                        Debug.Log($"{p.Value.Username}\n");
                    }
            }
            
        }
    }
    
    IEnumerator StartServer(ushort port,ushort maxClientCount)
    {
        yield return new WaitForSeconds(2);
        NetworkManager.Singleton.Server.Start(port, maxClientCount);
    }
    
}
