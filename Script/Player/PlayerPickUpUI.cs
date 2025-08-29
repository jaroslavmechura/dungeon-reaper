using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerPickUpUI : MonoBehaviour
{
    public Transform canvas;

    public GameObject healthText;
    public GameObject armorText;

    public GameObject pistolText;
    public GameObject assaultRifleText;
    public GameObject shotgunText;
    public GameObject rocketLauncherText;

    public void HealthPickUpText(string text) {
        TextMeshProUGUI textMeshPro = Instantiate(healthText, canvas).GetComponent<TextMeshProUGUI>();
        textMeshPro.text = text;
    }

    public void ArmorPickUpText(string text)
    {
        TextMeshProUGUI textMeshPro = Instantiate(armorText, canvas).GetComponent<TextMeshProUGUI>();
        textMeshPro.text = text;
    }

    public void WeaponPickUpText(int id, string text) {
        if (id == 0)
        {
            TextMeshProUGUI textMeshPro = Instantiate(pistolText, canvas).GetComponent<TextMeshProUGUI>();
            textMeshPro.text = text;
        }
        else if (id == 1) 
        {
            TextMeshProUGUI textMeshPro = Instantiate(assaultRifleText, canvas).GetComponent<TextMeshProUGUI>();
            textMeshPro.text = text;
        }
        else if (id == 2) 
        {
            TextMeshProUGUI textMeshPro = Instantiate(shotgunText, canvas).GetComponent<TextMeshProUGUI>();
            textMeshPro.text = text;
        }
        else if (id == 3) 
        {
            TextMeshProUGUI textMeshPro = Instantiate(rocketLauncherText, canvas).GetComponent<TextMeshProUGUI>();
            textMeshPro.text = text;
        }
    }
}
