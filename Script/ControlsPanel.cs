using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsPanel : MonoBehaviour
{
    public GameObject panel;

    private void Start()
    {
        panel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            panel.SetActive(!panel.activeSelf);
            Time.timeScale = panel.activeSelf ? 0 : 1;
        }
    }
}
