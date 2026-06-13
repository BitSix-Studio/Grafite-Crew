using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    [Header("UI")]
    public CanvasGroup canvasGroup;
    public Image loadingImage;

    [Header("Slideshow")]
    public Sprite[] images;
    public float imageDuration = 3f;
    public float fadeDuration = 0.5f;

    [Header("Loading")]
    public float minimumLoadingTime = 2f;

    bool isLoadingFinished;
    Coroutine slideshowRoutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        isLoadingFinished = false;

        canvasGroup.alpha = 1;

        slideshowRoutine = StartCoroutine(SlideshowLoop());
    }

    public void Hide()
    {
        if (slideshowRoutine != null)
            StopCoroutine(slideshowRoutine);

        canvasGroup.alpha = 0;

        gameObject.SetActive(false);
    }

    IEnumerator SlideshowLoop()
    {
        int index = 0;

        while (!isLoadingFinished)
        {
            loadingImage.sprite = images[index];

            yield return Fade(0f, 1f);

            yield return new WaitForSeconds(imageDuration);

            yield return Fade(1f, 0f);

            index++;

            if (index >= images.Length)
                index = 0;
        }
    }

    IEnumerator Fade(float start, float end)
    {
        Color color = loadingImage.color;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            color.a = Mathf.Lerp(start, end, timer / fadeDuration);

            loadingImage.color = color;

            yield return null;
        }

        color.a = end;

        loadingImage.color = color;
    }

    public IEnumerator LoadProcess(System.Action loadAction, NetworkManager networkManager)
    {
        Show();

        float startTime = Time.time;

        loadAction?.Invoke();

        while (!networkManager.SceneReady)
            yield return null;

        float elapsed = Time.time - startTime;

        if (elapsed < minimumLoadingTime)
        {
            yield return new WaitForSeconds(minimumLoadingTime - elapsed);
        }

        isLoadingFinished = true;

        yield return FadeCanvas(1, 0);

        Hide();
    }

    IEnumerator FadeCanvas(float start, float end)
    {
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(start, end, timer / fadeDuration);

            yield return null;
        }

        canvasGroup.alpha = end;
    }
}