using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOverlayController : MonoBehaviour
{
    public static UIOverlayController Instance { get; private set; }

    public TMP_Text InteractText;
    public Image FadeImage;
    public TMP_Text FadeText;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        StartCoroutine(FadeRoutine(5f, fadeOut: true));
    }
    private void Start()
    {
        InteractText.gameObject.SetActive(false);
    }

    public IEnumerator FadeRoutine(float fadeTimeSeconds, bool fadeOut)
    {
        if (!FadeImage) yield break;

        FadeText.enabled = true;

        float startAlpha = fadeOut ? 1f : 0f;
        float endAlpha = fadeOut ? 0f : 1f;

        float elapsed = 0f;

        Color c = FadeImage.color;
        c.a = startAlpha;
        FadeImage.color = c;

        while (elapsed < fadeTimeSeconds)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeTimeSeconds);
            t = t * t;  

            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            FadeImage.color = c;

            yield return null;
        }

        c.a = endAlpha;
        FadeImage.color = c;

        if (fadeOut) FadeText.enabled = false;
    }

    public void ShowInteractText(bool show, string text)
    {
        InteractText.SetText(text);
        InteractText.gameObject.SetActive(show);
    }

    public void HideAllUI()
    {
        InteractText.gameObject.SetActive(false);
    }
}


