using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceRegistry : MonoBehaviour
{
    private static ServiceRegistry _instance = null;
    public static ServiceRegistry Instance => _instance;

    [Header("Scene References (refreshed on scene load)")]
    [SerializeField] private Camera _mainCamera = null;
    [SerializeField] private Transform _player = null;

    public Camera MainCamera => _mainCamera;
    public Transform Player => _player;

    private Dictionary<Type, object> _services = new Dictionary<Type, object>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public void SetSceneReferences(Camera cam, Transform player)
    {
        if (cam != null) _mainCamera = cam;
        if (player != null) _player = player;
    }

    public void ClearPlayerReference()
    {
        _player = null;
    }

    public void Register<T>(T service) where T : class
    {
        _services[typeof(T)] = service;
    }

    public void Unregister<T>() where T : class
    {
        _services.Remove(typeof(T));
    }

    public T Get<T>() where T : class
    {
        if (_services.TryGetValue(typeof(T), out object service))
        {
            return service as T;
        }

        return null;
    }

    public bool Has<T>() where T : class
    {
        return _services.ContainsKey(typeof(T));
    }
}