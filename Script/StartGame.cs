using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartGame : MonoBehaviour
{
    public GameObject fadeImage;
    public List<GameObject> text;
    public GameObject textToProceed;
    public float textDelay = 0.5f;
    public float textFadeDuration = 1f; 

    public Slider loadingSlider; 

    [SerializeField] private string sceneToLoad;

    private bool isLoading = false;
    private bool sceneReady = false;
    private bool textsFinished = false;
    private AsyncOperation operation;

    private void Start()
    {
        if (fadeImage != null)
            fadeImage.SetActive(false);

        if (textToProceed != null)
            textToProceed.SetActive(false);

        foreach (var t in text)
            t.SetActive(false);

        if (loadingSlider != null)
            loadingSlider.gameObject.SetActive(false);
    }

    public void LoadSceneByName()
    {
        if (isLoading) return;

        Debug.Log("Button clicked! Trying to load scene: " + sceneToLoad);

        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneToLoad)
            {
                sceneExists = true;
                break;
            }
        }

        if (!sceneExists)
        {
            Debug.LogError("Scene \"" + sceneToLoad + "\" not found in Build Settings!");
            return;
        }

        isLoading = true;

        if (fadeImage != null)
            fadeImage.SetActive(true);

        if (loadingSlider != null)
        {
            loadingSlider.gameObject.SetActive(true);
            loadingSlider.value = 0f;
        }

        StartCoroutine(ShowTexts());
        StartCoroutine(LoadSceneAsync());
    }

    public void QuitGame()
    {
        Debug.Log("Quit button clicked!");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator LoadSceneAsync()
    {
        operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
 
            if (loadingSlider != null)
                loadingSlider.value = Mathf.Clamp01(operation.progress / 0.9f);

            if (operation.progress >= 0.9f)
            {
                sceneReady = true;

                if (textsFinished && textToProceed != null)
                    textToProceed.SetActive(true);
            }
            yield return null;
        }
    }

    private IEnumerator ShowTexts()
    {
        foreach (var t in text)
        {
            t.SetActive(true);

            TextMeshProUGUI tmp = t.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
                StartCoroutine(FadeInText(tmp));

            yield return new WaitForSeconds(textDelay);
        }

        textsFinished = true;

        if (sceneReady && textToProceed != null)
            textToProceed.SetActive(true);
    }


    private IEnumerator FadeInText(TextMeshProUGUI tmp)
    {
        Color c = tmp.color;
        c.a = 0f;
        tmp.color = c;

        float elapsed = 0f;
        while (elapsed < textFadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / textFadeDuration);
            tmp.color = c;
            yield return null;
        }
    }

    private void Update()
    {
        if (sceneReady && textsFinished && textToProceed.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                operation.allowSceneActivation = true;
            }
        }
    }
}
