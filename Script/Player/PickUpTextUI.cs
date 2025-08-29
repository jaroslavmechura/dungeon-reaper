using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PickUpTextUI : MonoBehaviour
{
    public float duration = 10.0f; // Duration for the text to move and fade out
    public float speed = 20.0f;    // Speed of the text movement on the Y axis

    private TextMeshProUGUI textMeshPro;
    private Color originalTextColor;
    private List<Image> childImages = new List<Image>();
    private List<Color> originalImageColors = new List<Color>();

    private void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        originalTextColor = textMeshPro.color;

        // Get all child images and their original colors
        foreach (Image img in GetComponentsInChildren<Image>())
        {
            childImages.Add(img);
            originalImageColors.Add(img.color);
        }

        StartCoroutine(MoveAndFadeText());
    }

    private IEnumerator MoveAndFadeText()
    {
        float elapsedTime = 0f;
        Vector3 originalPosition = transform.localPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Move the text upwards
            transform.localPosition = originalPosition + new Vector3(0, t * speed, 0);

            // Fade out the text
            textMeshPro.color = new Color(originalTextColor.r, originalTextColor.g, originalTextColor.b, Mathf.Lerp(1, 0, t));

            // Fade out the images
            for (int i = 0; i < childImages.Count; i++)
            {
                Color originalImageColor = originalImageColors[i];
                childImages[i].color = new Color(originalImageColor.r, originalImageColor.g, originalImageColor.b, Mathf.Lerp(1, 0, t));
            }

            yield return null;
        }

        Destroy(gameObject); // Destroy the text object after the animation
    }
}
