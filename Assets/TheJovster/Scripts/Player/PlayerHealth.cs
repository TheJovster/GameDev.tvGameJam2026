using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int _currentHealth;
    [SerializeField] private int _maxHealth = 5;
    [SerializeField] private bool _canTakeDamage = true;

    [Header("Components")]
    [SerializeField] private Knockbackable _knockbackable = null;
    [SerializeField] private FlashComponent _flashComponent = null;
    [SerializeField] private PlayerInteraction _playerInteraction = null;
     private void Awake()
    {
        _currentHealth = _maxHealth;

        if(_knockbackable == null)
        {
            _knockbackable = GetComponent<Knockbackable>();
        }
        if(_flashComponent == null)
        {
            _flashComponent = GetComponent<FlashComponent>();
        }
        if(_playerInteraction == null)
        {
            _playerInteraction = GetComponent<PlayerInteraction>();
        }
    }

    public void TakeDamage() 
    {
        _knockbackable.ApplyKnockback(-transform.forward, 2.0f);
        _playerInteraction.Disconnect();
        _flashComponent.TriggerFlash();
    }

    public void TeleportPlayer(Vector3 teleportPoint)
    {
        //add some teleportation effects here
        transform.position = teleportPoint;
    }

    public void SetCanTakeDamage(bool value) 
    {
        _canTakeDamage = value;
    }
}
