using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Socket Hubs (assign manually)")]
    [SerializeField] private SocketHub[] _socketHubs;

    [Header("Events")]
    public UnityEvent OnAllPlugged;

    private int _pluggedCount;
    private bool _isComplete;

    private void Start()
    {
        _pluggedCount = 0;
        _isComplete = false;

        for (int i = 0; i < _socketHubs.Length; i++)
        {
            _socketHubs[i].OnObjectPlugged.AddListener(HandlePlugged);
        }
    }

    private void HandlePlugged(GameObject obj)
    {
        _pluggedCount++;

        if (!_isComplete && _pluggedCount >= _socketHubs.Length)
        {
            _isComplete = true;
            Victory();
        }
    }

    private void Victory()
    {
        Debug.Log("ALL HUBS FILLED — VICTORY");
        OnAllPlugged?.Invoke();
    }

    public float GetProgress()
    {
        if (_socketHubs.Length == 0) return 1f;
        return (float)_pluggedCount / _socketHubs.Length;
    }

    public int GetPluggedCount() => _pluggedCount;
    public int GetTotalCount() => _socketHubs.Length;
    public bool IsComplete() => _isComplete;

    public void RestartLevel() 
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1; // Reset time scale in case we were paused on victory.
        SceneManager.LoadSceneAsync(0); //NOTE: This assumes the current level is at index 0 in the build settings. Adjust as necessary.Will need a more robust scene management solution later.
    }
}