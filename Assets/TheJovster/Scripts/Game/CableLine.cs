using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CableLine : MonoBehaviour
{
    [Header("Cable Appearance")]
    [SerializeField] private float _startWidth = 0.08f;
    [SerializeField] private float _endWidth = 0.08f;
    [SerializeField] private Material _baseCableMaterial;

    [Header("Sag / Droop")]
    [SerializeField] private int _segmentCount = 20;
    [SerializeField] private float _sagAmount = 0.5f;

    [Header("Collision")]
    [SerializeField] private LayerMask _collisionMask = ~0;
    [SerializeField] private float _raycastHeight = 5f;
    [SerializeField] private float _surfaceOffset = 0.05f;
    [SerializeField] private float _smoothSpeed = 10f;

    private LineRenderer _lr;
    private Transform _pointA;
    private Transform _pointB;
    private bool _isActive;
    private Vector3[] _currentPositions;
    private Vector3[] _targetPositions;

    private bool _grounded;

    private void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = 0;
        _lr.startWidth = _startWidth;
        _lr.endWidth = _endWidth;
        _lr.useWorldSpace = true;
        _isActive = false;
    }

    public void Activate(Transform from, Transform to, PlugColor color, bool grounded = false)
    {
        _pointA = from;
        _pointB = to;
        _isActive = true;
        _grounded = grounded;

        _lr.positionCount = _segmentCount;
        _currentPositions = new Vector3[_segmentCount];
        _targetPositions = new Vector3[_segmentCount];

        // Init positions so the cable doesn't lerp from origin
        for (int i = 0; i < _segmentCount; i++)
        {
            float t = (float)i / (_segmentCount - 1);
            _currentPositions[i] = Vector3.Lerp(from.position, to.position, t);
            _targetPositions[i] = _currentPositions[i];
        }

        if (_baseCableMaterial != null)
        {
            Material mat = new Material(_baseCableMaterial);
            Color c = PlugColorHelper.ToColor(color);
            mat.color = c;

            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", c * 0.5f);
            }

            _lr.material = mat;
        }
        else
        {
            _lr.startColor = PlugColorHelper.ToColor(color);
            _lr.endColor = PlugColorHelper.ToColor(color);
        }
    }

    public void Deactivate()
    {
        _isActive = false;
        _pointA = null;
        _pointB = null;
        _lr.positionCount = 0;
    }

    private void LateUpdate()
    {
        if (!_isActive || _pointA == null || _pointB == null) return;

        Vector3 start = _pointA.position;
        Vector3 end = _pointB.position;
        float distance = Vector3.Distance(start, end);

        for (int i = 0; i < _segmentCount; i++)
        {
            float t = (float)i / (_segmentCount - 1);
            Vector3 point = Vector3.Lerp(start, end, t);

            if (!_grounded)
            {
                float sag = _sagAmount * distance * 0.1f;
                float sagOffset = 4f * sag * t * (1f - t);
                point.y -= sagOffset;
            }

            Vector3 rayOrigin = point + Vector3.up * _raycastHeight;

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, _raycastHeight * 2f, _collisionMask))
            {
                float surfaceY = hit.point.y + _surfaceOffset;
                if (_grounded || point.y < surfaceY)
                {
                    point.y = surfaceY;
                }
            }

            _targetPositions[i] = point;

            _currentPositions[i] = Vector3.Lerp(_currentPositions[i], _targetPositions[i], _smoothSpeed * Time.deltaTime);
            _lr.SetPosition(i, _currentPositions[i]);
        }
    }
}