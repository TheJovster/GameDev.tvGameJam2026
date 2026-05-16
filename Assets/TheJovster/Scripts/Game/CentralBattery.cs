using System;
using UnityEngine;
using UnityEngine.Events;

public class CentralBattery : MonoBehaviour, IInteractable
{
    [Header("Battery Slots")]
    [SerializeField] private BatterySlot[] _slots;

    public UnityEvent<PlugColor> OnSlotFilled;
    public UnityEvent OnAllRequiredFilled;

    public string InteractionPrompt => "Connect Cable";

    public bool CanInteract(GameObject interactor)
    {
        PlayerInteraction interaction = interactor.GetComponent<PlayerInteraction>();
        if (interaction == null || !interaction.IsTethered) return false;

        PlugColor carrying = interaction.TetheredColor;
        return HasOpenSlot(carrying);
    }

    public void Interact(GameObject interactor)
    {
        // PlayerInteraction handles calling ConnectPlug
    }

    public bool HasOpenSlot(PlugColor color)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (!_slots[i].IsFilled && _slots[i].Color == color)
                return true;
        }
        return false;
    }

    public Transform ConnectPlug(PlugColor color)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (!_slots[i].IsFilled && _slots[i].Color == color)
            {
                _slots[i].Fill();
                OnSlotFilled?.Invoke(color);

                if (AllRequiredFilled())
                    OnAllRequiredFilled?.Invoke();

                return _slots[i].SnapPoint;
            }
        }
        return null;
    }

    public bool AllRequiredFilled()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].IsRequired && !_slots[i].IsFilled)
                return false;
        }
        return true;
    }

    public int GetRequiredCount()
    {
        int count = 0;
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].IsRequired) count++;
        }
        return count;
    }

    public int GetRequiredFilledCount()
    {
        int count = 0;
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].IsRequired && _slots[i].IsFilled) count++;
        }
        return count;
    }
}

[Serializable]
public class BatterySlot
{
    [SerializeField] private PlugColor _color;
    [SerializeField] private bool _isRequired = true;
    [SerializeField] private Transform _snapPoint;
    [SerializeField] private Renderer _slotRenderer;

    private bool _isFilled = false;

    public PlugColor Color => _color;
    public bool IsRequired => _isRequired;
    public bool IsFilled => _isFilled;
    public Transform SnapPoint => _snapPoint;

    public void Fill()
    {
        _isFilled = true;

        if (_slotRenderer != null)
        {
            Color c = PlugColorHelper.ToColor(_color);
            c.a = 1f;
            _slotRenderer.material.color = c;
            _slotRenderer.material.SetColor("_EmissionColor", c * 2f);
        }
    }

    public void ApplyIdleColor()
    {
        if (_slotRenderer == null) return;
        Color c = PlugColorHelper.ToColor(_color);
        c.a = 0.4f;
        _slotRenderer.material.color = c;
    }
}