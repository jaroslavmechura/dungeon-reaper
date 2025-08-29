using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PlayerDamage
{
    light,
    medium,
    heavy,
    ultra
};

public class PlayerHealth : MonoBehaviour
{
    [Header("--- Health Settings ---")]
    [SerializeField] public float maxHealth;
    [SerializeField] public float currHealth;
    [SerializeField] private TextMeshProUGUI textHealth;
    [SerializeField] private CameraShaker camShaker;

    [Header("--- Armor Settings ---")]
    [SerializeField] public float maxArmor;
    [SerializeField] public float currArmor;
    [SerializeField] private TextMeshProUGUI textArmor;

    private PlayerAudio playerAudio;
    private PlayerPickUpUI playerPickUpUI;

    public GameObject lowHealthText;

    [Header("--- HealthOverlap ---")]
    public Image healthOverlapImage;
    public float maxAlphaValue;

    private void Start()
    {
        //currHealth = maxHealth;
        //currArmor = maxArmor;
        playerAudio = GetComponent<PlayerAudio>();
        playerPickUpUI = GetComponent<PlayerPickUpUI>();
        lowHealthText.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        textHealth.text = currHealth.ToString("F0");
        textArmor.text = currArmor.ToString("F0");
    }
    public void HandleHealth() {
        if (currHealth > maxHealth) currHealth = maxHealth;
        if (currArmor > maxArmor) currArmor = maxArmor;

        lowHealthText.SetActive(currHealth <= maxHealth / 5);

        if (currHealth <= 0f) Death();

        float healthPercentage = currHealth / maxHealth;
        if (healthPercentage > 0.25f)
        {
            SetHealthOverlayAlpha(0);
        }
        else
        {
            float alpha = Mathf.Lerp(0, maxAlphaValue, 1 - (healthPercentage / 0.25f));
            SetHealthOverlayAlpha(alpha);
        }
    }

    public void TakeDamage(float dmg, PlayerDamage damageType) {
        if (currArmor > 0)
        {
            if (currArmor >= dmg)
            {
                currArmor -= dmg;
                dmg = 0;
            }
            else
            {
                dmg -= currArmor;
                currArmor = 0;
            }
        }

        if (dmg > 0)
        {
            currHealth -= dmg;
  
        }

        playerAudio.HurtSound();

        if (damageType == PlayerDamage.light) {
            camShaker.ShakeOnce(5, 2, 0.1f, 0.5f);
        }
        else if (damageType == PlayerDamage.medium)
        {
            camShaker.ShakeOnce(7, 2, 0.1f, 0.7f);
        }
        else if (damageType == PlayerDamage.heavy)
        {
            camShaker.ShakeOnce(9, 2, 0.1f, 0.9f);
        }
        else if (damageType == PlayerDamage.ultra)
        {
            camShaker.ShakeOnce(20, 2, 0.1f, 1.5f);
        }
    }

    public void Death() {
        GetComponent<PlayerInput>().enabled = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool AddHealth(int amount) {
        if (currHealth == maxHealth)
        {
            playerPickUpUI.HealthPickUpText("MAX");
            return false;
        }

        if (currHealth + amount > maxHealth)
        {
            playerPickUpUI.HealthPickUpText("+ " + (maxHealth - currHealth).ToString("F0"));
            currHealth = maxHealth;
        }
        else {
            playerPickUpUI.HealthPickUpText("+ " + amount.ToString("F0"));
            currHealth += amount;
        }
        return true;
    }

    public bool AddArmor(int amount)
    {
        if (currArmor == maxArmor)
        {
            playerPickUpUI.ArmorPickUpText("MAX");
            return false;
        }

        if (currArmor + amount > maxArmor)
        {
            playerPickUpUI.ArmorPickUpText("+ " + (maxArmor - currArmor).ToString("F0"));
            currArmor = maxArmor;
        }
        else
        {
            playerPickUpUI.ArmorPickUpText("+ " + amount.ToString("F0"));
            currArmor += amount;
        }
        return true;
    }

    private void SetHealthOverlayAlpha(float alpha)
    {
        Color color = healthOverlapImage.color;
        color.a = alpha;
        healthOverlapImage.color = color;
    }
}
