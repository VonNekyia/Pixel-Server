using System;
using RiptideNetworking;
using RiptideNetworking.Utils;
using Unity.VisualScripting;
using UnityEngine;

public enum ServerToClientID : ushort
{
    playerSpawned = 1,
    playerMovement = 2,
    playerMessage = 3,
    playerAttack = 4,
    enemySpawned = 5,
    enemyMovement = 6,
}
public enum ClientToServerID : ushort
{
    name = 1,
    input = 2,
    playerMessageReceiver = 3,
    attacking = 4,
}

public class NetworkManager : MonoBehaviour
{

    
    private static NetworkManager _singleton;

    public static NetworkManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    public Server Server { get; private set; }
 
    [Header("Connection")]
    [SerializeField] private ushort port;

    [SerializeField] private ushort maxClientCount; 
    
    public void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, true);
        
        Server = new Server();
        Server.Start(port, maxClientCount);
        Server.ClientDisconnected += PlayerLeft;
    }

    private void FixedUpdate()
    {
        Server.Tick();
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    { 
        if(Player.list.TryGetValue(e.Id, out Player player)) 
            Destroy(Player.list[e.Id].gameObject);
    }
}
