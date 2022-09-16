using System;
using System.Collections;
using System.Collections.Generic;
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
        Destroy(enemy.gameObject);
    }
    
}
