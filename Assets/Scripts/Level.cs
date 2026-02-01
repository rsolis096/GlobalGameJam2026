using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level LevelInstance = null;

    public AudioClip LevelMusicKlip;
    public AudioClip PickupSoundKlip;
    public AudioClip PurchaseSoundKlip;
    public AudioClip WinSoundKlip;
    public AudioSource LevelMusicAudioSource;
    public AudioSource LevelSoundsAudioSource;

    private void Awake()
    {
        LevelInstance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if(LevelMusicAudioSource)
        {
            if (LevelMusicKlip)
            {
                LevelMusicAudioSource.clip = LevelMusicKlip;
                LevelMusicAudioSource.loop = true;
                LevelMusicAudioSource.Play();
            }
        }


    }

    public void PlayPurchaseAudio()
    {
        if (LevelSoundsAudioSource)
        {
            if (PurchaseSoundKlip)
            {
                LevelSoundsAudioSource.loop = false;
                LevelSoundsAudioSource.PlayOneShot(PurchaseSoundKlip);
            }
        }
    }

    public void PlayWinAudio()
    {
        if (LevelSoundsAudioSource)
        {
            if (WinSoundKlip)
            {
                LevelSoundsAudioSource.loop = false;
                LevelSoundsAudioSource.PlayOneShot(WinSoundKlip);
            }
        }
    }

    public void PlayPickupAudio()
    {
        if (LevelSoundsAudioSource)
        {
            if (PickupSoundKlip)
            {
                LevelSoundsAudioSource.loop = false;
                LevelSoundsAudioSource.PlayOneShot(PickupSoundKlip);
            }
        }
    }
}
