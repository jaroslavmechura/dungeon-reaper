using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressLock : MonoBehaviour
{
    public List<GameObject> ToLock;

    private void Start()
    {
        foreach (GameObject obj in ToLock)
        {
            obj.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            foreach (GameObject obj in ToLock)
            {
                obj.SetActive(true);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject obj in ToLock)
            {
                obj.SetActive(true);
            }
            Destroy(gameObject);
        }
    }
}

