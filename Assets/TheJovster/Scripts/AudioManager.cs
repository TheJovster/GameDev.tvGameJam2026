using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Volume")]
    [SerializeField][Range(0f, 1f)] private float _masterVolume = 1f;
    [SerializeField][Range(0f, 1f)] private float _musicVolume = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _sfxVolume = 0.7f;

    [Header("Music Tracks")]
    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioClip _gameMusic;
    [SerializeField] private AudioClip _victoryMusic;

    [Header("SFX")]
    [SerializeField] private AudioClip _sfxTether;
    [SerializeField] private AudioClip _sfxDisconnect;
    [SerializeField] private AudioClip _sfxPlugIn;
    [SerializeField] private AudioClip _sfxHazardHit;
    [SerializeField] private AudioClip _sfxJump;
    [SerializeField] private AudioClip _sfxVictory;

    private void Awake()
    {
        if (ServiceRegistry.Instance != null)
            ServiceRegistry.Instance.Register(this);

        if (_musicSource == null)
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;
        }

        if (_sfxSource == null)
        {
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.loop = false;
            _sfxSource.playOnAwake = false;
        }

        ApplyVolumes();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        _sfxSource.PlayOneShot(clip, _sfxVolume * _masterVolume);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (_musicSource.clip == clip && _musicSource.isPlaying) return;

        _musicSource.clip = clip;
        _musicSource.Play();
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }

    public void PlayTether() => PlaySFX(_sfxTether);
    public void PlayDisconnect() => PlaySFX(_sfxDisconnect);
    public void PlayPlugIn() => PlaySFX(_sfxPlugIn);
    public void PlayHazardHit() => PlaySFX(_sfxHazardHit);
    public void PlayJump() => PlaySFX(_sfxJump);
    public void PlayVictorySting() => PlaySFX(_sfxVictory);

    public void PlayMenuMusic() => PlayMusic(_menuMusic);
    public void PlayGameMusic() => PlayMusic(_gameMusic);
    public void PlayVictoryMusic() => PlayMusic(_victoryMusic);

    public void SetMasterVolume(float vol)
    {
        _masterVolume = Mathf.Clamp01(vol);
        ApplyVolumes();
    }

    public void SetMusicVolume(float vol)
    {
        _musicVolume = Mathf.Clamp01(vol);
        ApplyVolumes();
    }

    public void SetSFXVolume(float vol)
    {
        _sfxVolume = Mathf.Clamp01(vol);
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        _musicSource.volume = _musicVolume * _masterVolume;
    }
}