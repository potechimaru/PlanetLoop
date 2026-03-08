using UnityEngine;

public sealed class EnemyController
{
    private readonly Transform _self;
    private readonly Transform _player;
    private readonly EnemyView _view;
    private readonly EnemyConfig _config;

    public float TelegraphElapsed; // 予告経過
    public float CooldownElapsed;  // クール経過

    // Move用
    public readonly Vector3 Origin;
    public Vector3 WanderTarget;

    public EnemyController(Transform self, Transform player, EnemyView view, EnemyConfig config)
    {
        _self = self;
        _player = player;
        _view = view;
        _config = config;

        Origin = self.position;
        WanderTarget = Origin;
    }

    public Vector3 DirToPlayerNormalized()
    {
        if (_player == null) return Vector3.right;
        var d = (_player.position - _self.position);
        d.z = 0f;
        return d.sqrMagnitude > 0.0001f ? d.normalized : Vector3.right;
    }

    public void StopRotateDecoration()
    {
        if (_view == null) return;
        _view.StopRotateDecoration();

    }

    public void RotateDecoration()
    {
        if (_view == null) return;
        _view.RotateDecoration();
    }

    public void ShowTelegraph(Vector3 dirNormalized)
    {
        if (_view == null) return;
        _view.ShowTelegraph(dirNormalized);
    }

    public void HideTelegraph()
    {
        if (_view == null) return;
        _view.HideTelegraph();
    }

    public void FireBullet(Vector3 dirNormalized)
    {
        if (_view == null) return;
        _view.FireBullet(dirNormalized);
    }

    public Transform Self => _self;
    public Transform Player => _player;

    public float DetectRadius => _config.DetectRadius;

    public float TelegraphTime => _config.TelegraphTime;

    public float CooldownTime => _config.CooldownTime;
    public float MoveSpeed => _config.MoveSpeed;

}