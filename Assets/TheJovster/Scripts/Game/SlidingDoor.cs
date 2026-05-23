using System.Collections;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform _doorTransform;
    [SerializeField] private float _openHeight = 3f;
    [SerializeField] private float _speed = 3f;

    private Vector3 _closedPosition;
    private Vector3 _openPosition;
    private Coroutine _activeRoutine;

    private void Awake()
    {
        if (_doorTransform == null)
            _doorTransform = transform;

        _closedPosition = _doorTransform.localPosition;
        _openPosition = _closedPosition + Vector3.up * _openHeight;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_activeRoutine != null)
            StopCoroutine(_activeRoutine);

        _activeRoutine = StartCoroutine(MoveDoor(_openPosition));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_activeRoutine != null)
            StopCoroutine(_activeRoutine);

        _activeRoutine = StartCoroutine(MoveDoor(_closedPosition));
    }

    private IEnumerator MoveDoor(Vector3 target)
    {
        while (Vector3.Distance(_doorTransform.localPosition, target) > 0.01f)
        {
            _doorTransform.localPosition = Vector3.MoveTowards(_doorTransform.localPosition, target, _speed * Time.deltaTime);
            yield return null;
        }

        _doorTransform.localPosition = target;
        _activeRoutine = null;
    }
}