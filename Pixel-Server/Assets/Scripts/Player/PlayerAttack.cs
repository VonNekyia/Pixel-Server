using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    
    /*private static PlayerAttack _singleton;

    public static PlayerAttack Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(PlayerAttack)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }
*/

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    private int attackDamage = 1;
    /*
    public void Awake()
    {
        Singleton = this;
    }
    */
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack(Player player)
    {
        StartCoroutine(AttackCooldown(player));
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position,attackRange,enemyLayers);
        foreach (Collider2D hittedObject in hitEnemies)
        {
            Enemy enemy = hittedObject.GetComponent<Enemy>();
            enemy.DamageEnemy(enemy, attackDamage);
        }
    }
    
    static IEnumerator AttackCooldown(Player player)
    {
        player.isOnAttackCooldown = true;
        yield return new WaitForSeconds(1);
        player.isOnAttackCooldown = false;
    } 

    
    
}
