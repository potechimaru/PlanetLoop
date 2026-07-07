using UnityEngine;

/// <summary>
/// アタッチされたGameObjectをシーン遷移時に破棄しないようにするコンポーネント。
/// 同じオブジェクトが複数生成されることは考慮しない。
/// </summary>
public class DontDestroyOnLoadObject : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}