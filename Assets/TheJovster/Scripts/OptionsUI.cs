using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    [Header("Panel")]
    [SerializeField] private GameObject _optionsPanel;

    private AudioManager _audio;

    private void Start()
    {
        _audio = ServiceRegistry.Instance.Get<AudioManager>();

        if (_audio == null) return;

        _masterSlider.value = _audio.MasterVolume;
        _musicSlider.value = _audio.MusicVolume;
        _sfxSlider.value = _audio.SFXVolume;

        _masterSlider.onValueChanged.AddListener(OnMasterChanged);
        _musicSlider.onValueChanged.AddListener(OnMusicChanged);
        _sfxSlider.onValueChanged.AddListener(OnSFXChanged);
    }

    private void OnMasterChanged(float val) => _audio?.SetMasterVolume(val);
    private void OnMusicChanged(float val) => _audio?.SetMusicVolume(val);
    private void OnSFXChanged(float val) => _audio?.SetSFXVolume(val);

    public void Show()
    {
        if (_optionsPanel != null)
            _optionsPanel.SetActive(true);
    }

    public void Hide()
    {
        if (_optionsPanel != null)
            _optionsPanel.SetActive(false);
    }
}