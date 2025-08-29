using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerTrigger : MonoBehaviour
{
    [SerializeField] private EnemySpawner[] enemySpawnersToTrigger;
    [SerializeField] private bool rigHealthForWeaponShowcase;

    private EnemyHealth enemyHealth;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            foreach (EnemySpawner enemySpawner in enemySpawnersToTrigger) 
            {
               enemyHealth =  enemySpawner.SpawnEnemy();
                if (rigHealthForWeaponShowcase) enemyHealth.RiggHealth();
            }
            Destroy(gameObject);
        }
    }
}
