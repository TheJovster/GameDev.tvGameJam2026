using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private PlayerInteraction _playerInteraction;

    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private TextMeshProUGUI _interactPromptText;

    [Header("Victory Panel")]
    [SerializeField] private GameObject _victoryPanel;

    [Header("Buttons")]
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _quitButton;

    private void Start()
    {
        if (_victoryPanel != null)
            _victoryPanel.SetActive(false);

        if (_gameManager != null)
            _gameManager.OnVictory.AddListener(ShowVictory);

        if(_resumeButton != null)
            _resumeButton.onClick.AddListener(UnpauseGame);

        if(_quitButton != null)
            _quitButton.onClick.AddListener(QuitToMainMenu);
    }

    private void Update()
    {
        UpdateProgress();
        UpdateInteractPrompt();
    }

    private void UpdateProgress()
    {
        if (_progressText == null || _gameManager == null) return;
        _progressText.text = $"{_gameManager.GetFilledCount()} / {_gameManager.GetRequiredCount()}";
    }

    private void UpdateInteractPrompt()
    {
        if (_interactPromptText == null || _playerInteraction == null) return;

        string prompt = _playerInteraction.GetCurrentPrompt();
        if (string.IsNullOrEmpty(prompt))
        {
            _interactPromptText.enabled = false;
        }
        else
        {
            _interactPromptText.enabled = true;
            _interactPromptText.text = prompt;
        }
    }

    private void ShowVictory()
    {
        if (_victoryPanel != null)
            _victoryPanel.SetActive(true);
    }

    private void PauseGame() 
    {

    }

    private void UnpauseGame() 
    {

    }

    private void QuitToMainMenu() 
    {

    }
}