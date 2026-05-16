using UnityEngine;
using TMPro;
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
    [SerializeField] private Button _restartButton;

    private void Start()
    {
        if (_victoryPanel != null)
            _victoryPanel.SetActive(false);

        if (_gameManager != null)
            _gameManager.OnAllPlugged.AddListener(ShowVictory);
    }

    private void Update()
    {
        UpdateProgress();
        UpdateInteractPrompt();
    }

    private void UpdateProgress()
    {
        if (_progressText == null || _gameManager == null) return;
        _progressText.text = $"{_gameManager.GetPluggedCount()} / {_gameManager.GetTotalCount()}";
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
            _interactPromptText.text = $"[F] {prompt}";
        }
    }

    private void ShowVictory()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (_victoryPanel != null)
            _victoryPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}