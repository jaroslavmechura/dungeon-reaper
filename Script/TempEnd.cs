using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempEnd : MonoBehaviour
{
    public GameObject EndScreen;

    private bool isIn = false;

    private void Start()
    {
        EndScreen.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isIn)
        {
            isIn = true;
            EndScreen.SetActive(true);
        }
    }

    private void Update()
    {
        if ( isIn && Input.GetKeyDown(KeyCode.Space))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("MainMenu");
            isIn = false;
        }
    }
}
