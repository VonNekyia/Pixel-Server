using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IEnemy 
{
    static ushort enemyID { get; set; }
    String enemyName { get; set; }
    int livePoints { get; set; }
}


