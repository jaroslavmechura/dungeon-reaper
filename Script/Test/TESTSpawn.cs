using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTSpawn : InteractiveObject
{
    public EnemySpawner spawners;

    public override void Interaction()
    {
        spawners.SpawnEnemy();
    }
}
