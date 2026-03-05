using UnityEngine;

public sealed class EnemyContext
{
    public readonly Transform Self;
    public readonly Transform Player;
    public readonly EnemyViewCommon View;
    public readonly EnemyCommonConfig Config;

    public float TelegraphElapsed; // 予告経過
    public float CooldownElapsed;  // クール経過

    // Move用
    public readonly Vector3 Origin;
    public Vector3 WanderTarget;

    public EnemyContext(Transform self, Transform player, EnemyViewCommon view, EnemyCommonConfig config)
    {
        Self = self;
        Player = player;
        View = view;
        Config = config;

        Origin = self.position;
        WanderTarget = Origin;
    }

    public Vector3 DirToPlayerNormalized()
    {
        if (Player == null) return Vector3.right;
        var d = (Player.position - Self.position);
        d.z = 0f;
        return d.sqrMagnitude > 0.0001f ? d.normalized : Vector3.right;
    }
}