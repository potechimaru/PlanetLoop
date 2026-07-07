using System.Collections.Generic;
using UnityEngine;

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