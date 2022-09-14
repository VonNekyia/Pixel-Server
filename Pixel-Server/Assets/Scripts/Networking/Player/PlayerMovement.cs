using System.Collections.Generic;
using System.Numerics;
using RiptideNetworking;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Player player;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float movementSpeed;
    
    [Header("Collission")]
    [SerializeField] public ContactFilter2D movementFilter;
    [SerializeField] private float collisionOffset = 0.05f;
    
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private float moveSpeed;
    private bool[] inputs;
    private RaycastHit2D right;
    private RaycastHit2D left;
    private RaycastHit2D up;
    private RaycastHit2D down;
    public bool isAttacking;

    private void OnValidate()
    {
        if (rb == null) 
            rb = GetComponent<Rigidbody2D>();
        if (player == null) 
            player = GetComponent<Player>();

        Initialize();
    }

    private void Start()
    {
        Initialize();
        inputs = new bool[5];
     
        
    }
    


    private void FixedUpdate()
    {
        Vector2 inputDirection = Vector2.zero;
        if (inputs[0])
            inputDirection.y += 1;
        if (inputs[1])
            inputDirection.y -= 1;
        if (inputs[2])
            inputDirection.x -= 1;
        if (inputs[3])
            inputDirection.x += 1;

        if (inputDirection.x > 0)
        {
            right = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.right), 1f);
            Debug.DrawRay(transform.position + new Vector3(0.05f,0,0), transform.TransformDirection(Vector3.right) * 10f,Color.red);
        }
        if (inputDirection.x < 0)
        {
            left = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.left), 1f);
            Debug.DrawRay(transform.position + new Vector3(-0.05f,0,0), transform.TransformDirection(Vector3.left) * 10f,Color.red);
        }   
        if (inputDirection.y > 0)
        {
            up = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.up), 1f);
            Debug.DrawRay(transform.position + new Vector3(0,-0.18f,0), transform.TransformDirection(Vector3.up) * 10f,Color.green);
        }
        if (inputDirection.y < 0)
        {
            down = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.down), 1f);
            Debug.DrawRay(transform.position + new Vector3(0,-0.18f,0), transform.TransformDirection(Vector3.down) * 10f,Color.green);
        }  
         
        if (CanMove(inputDirection))
            Move(inputDirection, inputs[4]);
        else
        {
            if (CanMove(new Vector2(inputDirection.x, 0)))
                Move(new Vector2(inputDirection.x, 0), inputs[4]);
            if (CanMove(new Vector2(0, inputDirection.y)))
                Move(new Vector2(0, inputDirection.y), inputs[4]);
        }
    }
    
    

    private bool CanMove(Vector2 inputDirection)
    {
       
        int count = rb.Cast(
            inputDirection, // X and Y values between -1 and 1 that represent the direction from the body to look for collisions
            movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
            castCollisions, // List of collisions to store the found collisions into after the Cast is finished
            moveSpeed * Time.fixedDeltaTime +
            collisionOffset); // The amount to cast equal to the movement plus an offset
        if (count == 0)
            return true;
        

        return false;
        
    }
    
    
    private void Initialize()
    {
        moveSpeed = movementSpeed * Time.fixedDeltaTime;
    }

    private void Move(Vector2 inputDirection, bool sprint)
    {
        Vector2 moveDirection =  inputDirection.normalized;
        moveDirection *= moveSpeed;
        
        

        if (sprint)
            moveDirection *= 2f;
        rb.MovePosition(rb.position + moveDirection);
        
        SendMovement();
    }

    public void SetInput(bool[] inputs)
    {
        this.inputs = inputs;
    }
    
    public void SetIsAttacking(bool isAttacking)
    {
        this.isAttacking = isAttacking;
        SendAttack();
    }

    private void SendMovement()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientID.playerMovement);
        message.AddUShort(player.Id);
        message.AddVector3(transform.position);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
    
    private void SendAttack()
    {
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientID.playerAttack);
        message.AddUShort(player.Id);
        message.AddBool(player.Movement.isAttacking);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    
    
}
