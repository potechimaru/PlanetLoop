using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameModeの表示データをインスペクタで変更できるようにしている。
/// </summary>
[System.Serializable]
public class GameModeData
{
    [SerializeField] private GameModeType gameModeType;
    [SerializeField] private string displayName;
    [TextArea(3, 6)]
    [SerializeField] private string description;

    public GameModeType GameModeType => gameModeType;

    public string DisplayName => displayName;
    public string Description => description;
}

/// <summary>
/// GameModeSelectで表示するゲームモードのデータベースを管理するクラス。ScriptableObjectとして作成され、ゲームモードの情報を保持する。
/// </summary>
[CreateAssetMenu(menuName = "Game/Mode Database")]
public class GameModeDatabase : ScriptableObject
{
    [SerializeField] private List<GameModeData> modes;

    public IReadOnlyList<GameModeData> Modes => modes;

    public GameModeData GetModeData(GameModeType type)
    {
        return modes.Find(m => m.GameModeType == type);
    }
}