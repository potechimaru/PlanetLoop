using UnityEngine;

internal class PlayerSplineMover
{
    private const float AttachRadius = 0.1f;
    private const float AttachDuration = 0.03f;

    private readonly PlayerView _view;
    private readonly PlayerModel _model;
    private readonly IPlayerExternalFacade _playerExternalFacade;
    private readonly AttachEvent _attachEvent;

    private ClosedSplineLine _currentSpline;

    private float _totalLen;
    private float _distance;

    private bool _isAttaching;
    private float _attachT;
    private Vector3 _attachFrom;
    private Vector3 _attachTo;

    private bool _isJumping;
    private Vector3 _jumpDir;
    private float _jumpSpeed;
    private Vector3 _jumpPos;

    private Vector3 _jumpStartPos;

    internal PlayerSplineMover(
        PlayerView view,
        PlayerModel model,
        AttachEvent attachEvent,
        IPlayerExternalFacade playerExternalFacade)
    {
        _view = view;
        _model = model;
        _attachEvent = attachEvent;
        _playerExternalFacade = playerExternalFacade;

        _currentSpline = _view.Spline;
    }

    public void Initialize()
    {
        RebuildTable();

        if (_totalLen <= 0.0001f)
        {
            Debug.LogError("[PlayerSplineMover] Initialize failed: spline samples are empty");
        }
    }

    public void Tick(float deltaTime)
    {
        if (_model.IsGameOver) return;
        if (_totalLen <= 0.0001f) return;
        if (_isJumping) return;

        if (_isAttaching)
        {
            TickAttach(deltaTime);
            return;
        }

        TickSplineMove(deltaTime);
    }

    public void StartJump()
    {
        if (_model.IsGameOver) return;

        _isJumping = true;
        _jumpPos = _view.transform.position;
        _jumpStartPos = _jumpPos;
        _jumpDir = GetOuterNormal().normalized;
        _jumpSpeed = _model.CurrentJumpspeed;
    }

    public bool TickJump(float dt)
    {
        if (_model.IsGameOver) return false;
        if (!_isJumping) return false;

        BendJumpDirection(dt);

        Vector3 prevPos = _jumpPos;
        _jumpPos += _jumpDir * _jumpSpeed * dt;
        _view.SetPosition(_jumpPos);

        if (!_playerExternalFacade.TryFindTouchedSpline(
                prevPos,
                _jumpPos,
                AttachRadius,
                _currentSpline,
                out var touchedSpline,
                out float hitDistanceOnSpline,
                out Vector3 hitPointOnSpline))
        {
            return false;
        }

        float jumpDistance = Vector3.Distance(_jumpStartPos, hitPointOnSpline);

        AttachToSpline(touchedSpline, hitDistanceOnSpline, hitPointOnSpline);
        _attachEvent.CheckLongJumped(jumpDistance);

        _isJumping = false;
        return true;
    }


    private void TickAttach(float deltaTime)
    {
        _attachT += deltaTime / AttachDuration;
        float t = Mathf.SmoothStep(0f, 1f, _attachT);

        Vector3 pos = Vector3.Lerp(_attachFrom, _attachTo, t);
        _view.SetPosition(pos);

        if (_attachT >= 1f)
        {
            _isAttaching = false;
        }
    }

    private void TickSplineMove(float deltaTime)
    {
        float dir = _model.Clockwise ? -1f : 1f;
        _distance = Mathf.Repeat(
            _distance + dir * _model.CurrentMoveSpeed * deltaTime,
            _totalLen);

        ApplyPosition();
    }

    private void BendJumpDirection(float dt)
    {
        if (_playerExternalFacade == null) return;
        _jumpDir = _playerExternalFacade.BendDirection(_jumpPos, _jumpDir, dt);
    }

    private void AttachToSpline(
    ClosedSplineLine newSpline,
    float hitDistanceOnSpline,
    Vector3 hitPointWorld)
    {
        _view.SetSpline(newSpline);
        _currentSpline = newSpline;

        _distance = hitDistanceOnSpline;
        _totalLen = _currentSpline.GetTotalLength();

        _attachFrom = _view.transform.position;
        _attachTo = _currentSpline.EvaluateByDistance(_distance);

        _attachT = 0f;
        _isAttaching = true;

        _attachEvent.OnSplineAttached(_currentSpline, _distance, hitPointWorld);
    }

    private void RebuildTable()
    {
        if (_currentSpline == null)
        {
            _totalLen = 0f;
            return;
        }

        _totalLen = _currentSpline.GetTotalLength();
    }

    private void ApplyPosition()
    {
        Vector3 pos = _currentSpline.EvaluateByDistance(_distance);
        _view.SetPosition(pos);
    }

    public Vector3 GetOuterNormal()
    {
        return _currentSpline.EvaluateNormalByDistance(_distance);
    }

    public bool IsJumping => _isJumping;
}