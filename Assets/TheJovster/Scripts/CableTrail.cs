using System.Collections.Generic;
using UnityEngine;

public class CableTrail : MonoBehaviour
{
    [Header("Waypoint Settings")]
    [SerializeField] private float _dropDistance = 3f;
    [SerializeField] private LayerMask _groundMask = ~0;
    [SerializeField] private float _groundRayHeight = 5f;
    [SerializeField] private float _groundOffset = 0.05f;

    [Header("Cable Prefab")]
    [SerializeField] private CableLine _segmentPrefab;

    private Transform _playerAttach;
    private Transform _sourceAnchor;
    private PlugColor _color;
    private bool _isActive;

    private List<Transform> _waypoints = new List<Transform>();
    private List<CableLine> _staticSegments = new List<CableLine>();
    private CableLine _activeSegment;
    private Transform _lastWaypoint;

    public bool IsActive => _isActive;

    public void Activate(Transform sourceAnchor, Transform playerAttach, PlugColor color)
    {
        _sourceAnchor = sourceAnchor;
        _playerAttach = playerAttach;
        _color = color;
        _isActive = true;

        // First waypoint is the source device anchor
        _lastWaypoint = _sourceAnchor;

        // Spawn the active segment from source to player
        _activeSegment = Instantiate(_segmentPrefab, transform);
        _activeSegment.Activate(_lastWaypoint, _playerAttach, _color);
    }

    public void Deactivate()
    {
        _isActive = false;

        // Destroy active segment
        if (_activeSegment != null)
        {
            _activeSegment.Deactivate();
            Destroy(_activeSegment.gameObject);
            _activeSegment = null;
        }

        // Destroy all static segments
        for (int i = _staticSegments.Count - 1; i >= 0; i--)
        {
            if (_staticSegments[i] != null)
                Destroy(_staticSegments[i].gameObject);
        }
        _staticSegments.Clear();

        // Destroy all waypoint transforms
        for (int i = _waypoints.Count - 1; i >= 0; i--)
        {
            if (_waypoints[i] != null)
                Destroy(_waypoints[i].gameObject);
        }
        _waypoints.Clear();

        _lastWaypoint = null;
    }

    /// <summary>
    /// Finalizes the trail — converts active segment to static, returns all segments.
    /// Call when plugging into the battery.
    /// </summary>
    public void Finalize(Transform batterySnapPoint)
    {
        if (!_isActive) return;

        // Drop a final waypoint at the battery
        _activeSegment.Deactivate();
        Destroy(_activeSegment.gameObject);

        CableLine finalSegment = Instantiate(_segmentPrefab, transform);
        finalSegment.Activate(_lastWaypoint, batterySnapPoint, _color);
        _staticSegments.Add(finalSegment);

        _activeSegment = null;
        _isActive = false;
    }

    private void Update()
    {
        if (!_isActive || _playerAttach == null || _lastWaypoint == null) return;

        float dist = Vector3.Distance(_playerAttach.position, _lastWaypoint.position);

        if (dist >= _dropDistance)
        {
            DropWaypoint();
        }
    }

    private void DropWaypoint()
    {
        // Find ground position at player's current location
        Vector3 dropPos = _playerAttach.position;

        Vector3 rayOrigin = dropPos + Vector3.up * _groundRayHeight;
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, _groundRayHeight * 2f, _groundMask))
        {
            dropPos = hit.point + Vector3.up * _groundOffset;
        }

        // Create waypoint transform
        GameObject waypointObj = new GameObject($"CableWaypoint_{_waypoints.Count}");
        waypointObj.transform.SetParent(transform);
        waypointObj.transform.position = dropPos;
        Transform waypoint = waypointObj.transform;
        _waypoints.Add(waypoint);

        // Convert current active segment into a static one (last waypoint → new waypoint)
        _activeSegment.Deactivate();
        Destroy(_activeSegment.gameObject);

        CableLine staticSeg = Instantiate(_segmentPrefab, transform);
        staticSeg.Activate(_lastWaypoint, waypoint, _color);
        _staticSegments.Add(staticSeg);

        // Spawn new active segment from new waypoint to player
        _lastWaypoint = waypoint;
        _activeSegment = Instantiate(_segmentPrefab, transform);
        _activeSegment.Activate(_lastWaypoint, _playerAttach, _color);
    }
}