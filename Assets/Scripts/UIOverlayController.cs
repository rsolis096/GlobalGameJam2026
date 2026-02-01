using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOverlayController : MonoBehaviour
{
    public static UIOverlayController Instance { get; private set; }

    public TMP_Text InteractText;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);


    }

    private void Start()
    {

        InteractText.gameObject.SetActive(false);
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


