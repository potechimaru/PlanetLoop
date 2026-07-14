
/// <summary>
/// Enemyの種類に応じて数値的特徴を設定するためのクラス。インスペクタで設定可能なパラメータを持つ。
/// </summary>
[System.Serializable]
public class EnemyConfig
{
    public float DetectRadius = 6f;
    public float TelegraphTime = 1.0f;
    public float CooldownTime = 1.2f;

    // Move用
    public float MoveSpeed = 1.2f;
}
