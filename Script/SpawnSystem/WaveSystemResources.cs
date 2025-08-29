using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    // Melee - Ground
    Zombie,

    // Ranged - Fly
    FireDemon,

    PurpleCaster,

    Rampage1,

    Rampage2,
}

[Serializable]
public class WaveEnemyTypes
{
    public List<EnemyType> enemies;
}

[Serializable]
public class WaveEnemySpawners
{
    public EnemyType enemyType;
    public List<EnemySpawner> spawners = new List<EnemySpawner>();
}