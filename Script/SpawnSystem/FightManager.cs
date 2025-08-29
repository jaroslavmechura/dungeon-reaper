using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    [Header("--- Fight Settings ---")]
    [SerializeField] private List<WaveEnemyTypes> wawes;
    [SerializeField] private float enemiesLeftFromPreviousWaweToSpawnNextWave = 1f;
    [Header("--- References ---")]
    [SerializeField] private List<WaveEnemySpawners> spawners;

    private RoomManager roomManager;
    public List<EnemyHealth> activeEnemies = new List<EnemyHealth>();
    private int currentWaveIndex = 0;
    private bool isInFight = false;

    public void StartFight(RoomManager managerRef)
    {
        roomManager = managerRef;
        isInFight = true;

        if (wawes.Count > 0)
        {
            SpawnEnemiesFromWave(wawes[0]);
        }
    }

    private void SpawnEnemiesFromWave(WaveEnemyTypes wave)
    {
        print("SpawnWawe");
        foreach (EnemyType enemyType in wave.enemies)
        {
            foreach (WaveEnemySpawners spawnerGroup in spawners)
            {
                if (spawnerGroup.enemyType == enemyType)
                {
                    foreach (EnemySpawner spawner in spawnerGroup.spawners)
                    {
                        activeEnemies.Add(spawner.SpawnEnemy());
                    }
                }
            }
        }
    }

    void Update()
    {
        // --- Update active enemies ---
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i].isDead)
            {
                activeEnemies.RemoveAt(i);
            }
        }

        // Check if conditions for spawning another wave or ending the room are met
        if (activeEnemies.Count <= enemiesLeftFromPreviousWaweToSpawnNextWave && isInFight)
        {
            // Check if there are more waves to spawn
            if (currentWaveIndex < wawes.Count - 1)
            {
                SpawnEnemiesFromWave(wawes[++currentWaveIndex]);
            }
            else if (activeEnemies.Count == 0)
            {
                isInFight = false;
                // Notify the room manager that the fight is over
                roomManager.EndRoom();
            }
        }
    }
}
