using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private CentralBattery _battery;

    [Header("Events")]
    public UnityEvent OnVictory;

    private bool _isComplete;

    private void Start()
    {
        _isComplete = false;

        if (_battery != null)
        {
            _battery.OnAllRequiredFilled.AddListener(HandleVictory);
        }
    }

    private void HandleVictory()
    {
        if (_isComplete) return;
        _isComplete = true;

        Debug.Log("ALL REQUIRED SLOTS FILLED — VICTORY");
        OnVictory?.Invoke();
        Time.timeScale = 0; //pause the game
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public float GetProgress()
    {
        if (_battery == null) return 0f;
        int total = _battery.GetRequiredCount();
        if (total == 0) return 1f;
        return (float)_battery.GetRequiredFilledCount() / total;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; //unpause the game
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadSceneAsync(0); // Assuming the main level is at index 0 - need a more robust scene management strategy for later
    }

    public int GetFilledCount() => _battery != null ? _battery.GetRequiredFilledCount() : 0;
    public int GetRequiredCount() => _battery != null ? _battery.GetRequiredCount() : 0;
    public bool IsComplete() => _isComplete;
}