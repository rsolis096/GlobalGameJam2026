using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cashier : MonoBehaviour
{
    bool win = false;

    bool playedEnding = false;

    List<ItemStand> itemStands = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemStands = GetComponentsInChildren<ItemStand>().ToList();
    }

    // Update is called once per frame
    void Update()
    {

        if (win) return;

        win = true;
        foreach (ItemStand stand in itemStands)
        {
            if (!stand.placedItem) win = false;
        }

        if (win)
        {
            OnGameEnd();
        }
    }

    private void OnGameEnd()
    {
        if (Level.LevelInstance && !playedEnding)
        {
            Level.LevelInstance.PlayWinAudio();
            UIOverlayController.Instance.StartCoroutine(UIOverlayController.Instance.FadeRoutine(5f, fadeOut: false));
            UIOverlayController.Instance.FadeText.text = "You Win!";
        }
        playedEnding = true;
    }
}
