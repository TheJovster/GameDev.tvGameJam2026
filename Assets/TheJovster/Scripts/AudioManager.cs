using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

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
    [SerializeField] private AudioClip[] _footsteps;
    [SerializeField] private AudioClip _sfxTether;
    [SerializeField] private AudioClip _sfxDisconnect;
    [SerializeField] private AudioClip _sfxPlugIn;
    [SerializeField] private AudioClip _sfxHazardHit;
    [SerializeField] private AudioClip _sfxJump;
    [SerializeField] private AudioClip _sfxVictory;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

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

        LoadVolumePrefs();
        ApplyVolumes();
    }

    private void Start()
    {
        if (ServiceRegistry.Instance != null)
            ServiceRegistry.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        _sfxSource.pitch = Random.Range(0.95f, 1.05f);
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

    public void PlayFootstep() => PlaySFX(_footsteps[Random.Range(0, _footsteps.Length)]);
    public void PlayTether() => PlaySFX(_sfxTether);
    public void PlayDisconnect() => PlaySFX(_sfxDisconnect);
    public void PlayPlugIn() => PlaySFX(_sfxPlugIn);
    public void PlayHazardHit() => PlaySFX(_sfxHazardHit);
    public void PlayJump() => PlaySFX(_sfxJump);
    public void PlayVictorySting() => PlaySFX(_sfxVictory);

    public void PlayMenuMusic() => PlayMusic(_menuMusic);
    public void PlayGameMusic() => PlayMusic(_gameMusic);
    public void PlayVictoryMusic() => PlayMusic(_victoryMusic);

    public float MasterVolume => _masterVolume;
    public float MusicVolume => _musicVolume;
    public float SFXVolume => _sfxVolume;

    public void SetMasterVolume(float vol)
    {
        _masterVolume = Mathf.Clamp01(vol);
        ApplyVolumes();
        SaveVolumePrefs();
    }

    public void SetMusicVolume(float vol)
    {
        _musicVolume = Mathf.Clamp01(vol);
        ApplyVolumes();
        SaveVolumePrefs();
    }

    public void SetSFXVolume(float vol)
    {
        _sfxVolume = Mathf.Clamp01(vol);
        ApplyVolumes();
        SaveVolumePrefs();
    }

    private void ApplyVolumes()
    {
        _musicSource.volume = _musicVolume * _masterVolume;
    }

    private void LoadVolumePrefs()
    {
        _masterVolume = PlayerPrefs.GetFloat("vol_master", 1f);
        _musicVolume = PlayerPrefs.GetFloat("vol_music", 0.5f);
        _sfxVolume = PlayerPrefs.GetFloat("vol_sfx", 0.7f);
    }

    private void SaveVolumePrefs()
    {
        PlayerPrefs.SetFloat("vol_master", _masterVolume);
        PlayerPrefs.SetFloat("vol_music", _musicVolume);
        PlayerPrefs.SetFloat("vol_sfx", _sfxVolume);
        PlayerPrefs.Save();
    }
}