using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private CentralBattery _battery = null;

    [Header("Events")]
    public UnityEvent OnVictory;

    private bool _isComplete;
    private bool _isPaused = false;

    public bool IsPaused => _isPaused;

    private void Awake()
    {
        ServiceRegistry.Instance.Register(this);
    }

    private void Start()
    {
        _isComplete = false;
        ServiceRegistry.Instance.Get<SceneLoader>().OnSceneLoadFinished += HandleSceneLoadFinished;
    }

    private void HandleVictory()
    {
        if (_isComplete) return;
        _isComplete = true;

        Debug.Log("ALL REQUIRED SLOTS FILLED — VICTORY");
        OnVictory?.Invoke();
        PauseGame();
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
        UnpauseGame();
        ServiceRegistry.Instance.Get<SceneLoader>().ReloadCurrentScene(); // Assuming the main level is at index 0 - need a more robust scene management strategy for later
    }

    public int GetFilledCount() => _battery != null ? _battery.GetRequiredFilledCount() : 0;
    public int GetRequiredCount() => _battery != null ? _battery.GetRequiredCount() : 0;
    public bool IsComplete() => _isComplete;

    public void PauseGame() 
    {
        Time.timeScale = 0f;
        _isPaused = true;
    }

    public void UnpauseGame() 
    {
        Time.timeScale = 1f;
        _isPaused = false;
    }

    public void SetCursorState(bool value) 
    {
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void AssignCentralBattery(CentralBattery newBattery)
    {
        _battery = newBattery;
        _battery.OnAllRequiredFilled.AddListener(HandleVictory);
    }

    public void UnassignCentralBattery() 
    {
        if (_battery != null) { return; } 
        _battery.OnAllRequiredFilled.RemoveListener(HandleVictory);
        _battery = null;
    }

    private void HandleSceneLoadFinished()
    {
        _isComplete = false;
        if(_battery != null) 
        {
            UnassignCentralBattery();
        }
    }
}