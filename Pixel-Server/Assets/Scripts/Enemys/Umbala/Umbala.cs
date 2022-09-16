using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Umbala : Enemy
{
    
    public static Dictionary<ushort, Umbala> list = new Dictionary<ushort, Umbala>();
    public static ushort ID = 0;
    
    public static Umbala Spawn(Vector2 position, GameObject umbalaPrefab, ushort lvl)
    {
        Umbala umbala = Instantiate(umbalaPrefab, position, Quaternion.identity).GetComponent<Umbala>();
        umbala.enemyName = "Umbala";
        umbala.enemyID = ID;
        umbala.livePoints = lvl * 1;
            ID++;
        return umbala;
    }
}
