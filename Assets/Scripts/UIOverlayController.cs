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
        FadeImage.enabled = true;

        float startAlpha = fadeOut ? 1f : 0f;
        float endAlpha = fadeOut ? 0f : 1f;

        float elapsed = 0f;

        Color imageColor = FadeImage.color;
        imageColor.a = startAlpha;
        FadeImage.color = imageColor;

        Color textColor = FadeText.color;
        textColor.a = startAlpha;
        FadeText.color = textColor;

        while (elapsed < fadeTimeSeconds)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeTimeSeconds);
            t = t * t;  // easing

            // Update alpha each frame
            imageColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
            textColor.a = Mathf.Lerp(startAlpha, endAlpha, t);

            FadeImage.color = imageColor;
            FadeText.color = textColor;

            yield return null;
        }

        // Ensure final value
        imageColor.a = endAlpha;
        textColor.a = endAlpha;
        FadeImage.color = imageColor;
        FadeText.color = textColor;

        if (fadeOut)
        {
            FadeText.enabled = false;
            FadeImage.enabled = false;
        }
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


