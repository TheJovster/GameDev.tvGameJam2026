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

    private void Awake()
    {
        ServiceRegistry.Instance.Get<GameManager>()?.AssignCentralBattery(this);
    }

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

    private void OnDestroy()
    {
        ServiceRegistry.Instance.Get<GameManager>()?.UnassignCentralBattery();
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
/*        if (_slotRenderer == null) return;

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        _slotRenderer.GetPropertyBlock(mpb);
        Color c = PlugColorHelper.ToColor(_color);
        mpb.SetColor("_BaseColor", c);
        mpb.SetColor("_EmissionColor", c * 2f);
        _slotRenderer.SetPropertyBlock(mpb);*/
    }

    public void ApplyIdleColor()
    {
/*        if (_slotRenderer == null) return;

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        _slotRenderer.GetPropertyBlock(mpb);
        Color c = PlugColorHelper.ToColor(_color);
        c.a = 0.4f;
        mpb.SetColor("_BaseColor", c);
        _slotRenderer.SetPropertyBlock(mpb);*/
    }
}