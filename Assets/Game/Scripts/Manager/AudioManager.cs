using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    private AudioSource musicSource;
    private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip gameOver;
    [SerializeField] private AudioClip takeDamageSound;
    [SerializeField] private AudioClip dieSound;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volumeMultiplier);
    }

    public void PlayShootSound(float volumeMultiplier = 1f)
    {
        sfxSource.PlayOneShot(shootSound, volumeMultiplier);
    }
    public void PlayTakeDamageSound(float volumeMultiplier = 1f)
    {
        sfxSource.PlayOneShot(takeDamageSound, volumeMultiplier);
    }
    public void PlayButtonClickSound(float volumeMultiplier = 1f)
    {
        sfxSource.PlayOneShot(buttonClick, volumeMultiplier);
    }
    public void PlayGameOverSound(float volumeMultiplier = 1f)
    {
        sfxSource.PlayOneShot(gameOver, volumeMultiplier);
    }
    public void PlayDieSound(float volumeMultiplier = 1f)
    {
        sfxSource.PlayOneShot(dieSound, volumeMultiplier);
    }


    public void MuteAll(bool mute)
    {
        musicSource.mute = mute;
        sfxSource.mute = mute;
    }

    public bool IsMusicPlaying() => musicSource.isPlaying;
}
