using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // --- Reference ---
    private PlayerInput playerInput;
    private PlayerMovement playerMovement;
    private PlayerWeapons playerWeapons;
    private PlayerHealth playerHealth;

    [Header("--- Parkour Respawn ---")]
    public float length;
    [SerializeField] private Image respawnBlackScreen;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        playerWeapons = GetComponent<PlayerWeapons>();
        playerHealth = GetComponent<PlayerHealth>();
        respawnBlackScreen.enabled = true;
        respawnBlackScreen.color = new Color(0f, 0f, 0f, 0f);
    }

    private void Update()
    {
        playerInput.HandleInput();
        playerMovement.HandleMovement();
        playerWeapons.HandleWeapons();
        playerHealth.HandleHealth();
    }

    public void FadeOut(float timer)
    {
        StartCoroutine(FadeImage(true, timer));
    }

    public void FadeIn(float timer)
    {
        StartCoroutine(FadeImage(false, timer));
    }

    private IEnumerator FadeImage(bool fadeIn, float timer)
    {
        Color tempColor = respawnBlackScreen.color;
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;
        float elapsedTime = 0f;

        while (elapsedTime < timer)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / timer);
            tempColor.a = newAlpha;
            respawnBlackScreen.color = tempColor;
            yield return null;
        }

        // Ensure the final alpha is set
        tempColor.a = endAlpha;
        respawnBlackScreen.color = tempColor;
    }
}
