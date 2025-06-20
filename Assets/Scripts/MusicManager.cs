using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
    public static MusicManager Instance { get; private set; }
    private AudioSource audioSource;
    private float volume = 1f;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();

        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, 1f);
        audioSource.volume = volume;
    }
    public void ChangeVolume(float volume)
    {
        this.volume = volume;
        audioSource.volume = this.volume;


        //save music volume data
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
    }

    public float GetVolume()
    {
        return volume;
    }
}
