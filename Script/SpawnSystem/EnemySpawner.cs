using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemySpawner : MonoBehaviour
{
    [Header("--- Prefabs ---")]
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject effect;

    [Header("--- Spawn Positions ---")]
    [SerializeField] private Transform enemySpawnTransform;
    [SerializeField] private Transform effectSpawnTransform;

    [Header("--- Refs To Destroy ---")]
    [SerializeField] private GameObject[] refObjectToDestroy;

    [Header("--- Settings ---")]
    [SerializeField] private float enemySpawnOffset;

    [Header("--- Spawn Audio ---")]
    [SerializeField] private AudioSource spawnAudioSource;

    private GameObject effectRef;
    private GameObject spawnedEnemy;


    [Header("--- Specials ---")]
    [SerializeField] private bool isCaster = false;
    [SerializeField] private Transform[] teleportPoints;
    

    private void Start()
    {
        foreach (GameObject obj in refObjectToDestroy)
        {
            Destroy(obj);
        }
    }

    public EnemyHealth SpawnEnemy()
    {
        spawnedEnemy = Instantiate(enemy, enemySpawnTransform.position, Quaternion.identity, null);
        spawnedEnemy.SetActive(false);

        if (isCaster) {
            spawnedEnemy.GetComponentInChildren<CasterTeleportState>().teleportPoints = teleportPoints;
        }

        if (effectRef != null)
        {
            Destroy(effectRef);
        }

        effectRef = Instantiate(effect, effectSpawnTransform.position, Quaternion.identity, effectSpawnTransform);

        // Ensure the VFX starts playing immediately
        VisualEffect vfx = effectRef.GetComponentInChildren<VisualEffect>();
        vfx.Play();
        vfx.Reinit();

        spawnAudioSource.Play();


        StartCoroutine(SpawnEnemyRoutine());

        return spawnedEnemy.GetComponent<EnemyHealth>();
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(enemySpawnOffset);
        spawnedEnemy.SetActive(true);

    }
}
