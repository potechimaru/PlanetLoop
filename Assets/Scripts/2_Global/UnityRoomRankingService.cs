using UnityEngine;
using unityroom.Api;

public class UnityroomRankingService
{
    private const int HighScoreBoardNo = 1;
    private const int VisitedSplineBoardNo = 2;
    private const int DefeatedEnemyBoardNo = 3;

    public void SendResults(
        int highScore,
        int visitedSplineCount,
        int defeatedEnemyCount)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        UnityroomApiClient.Instance.SendScore(
            HighScoreBoardNo,
            highScore,
            ScoreboardWriteMode.HighScoreDesc);

        UnityroomApiClient.Instance.SendScore(
            VisitedSplineBoardNo,
            visitedSplineCount,
            ScoreboardWriteMode.HighScoreDesc);

        UnityroomApiClient.Instance.SendScore(
            DefeatedEnemyBoardNo,
            defeatedEnemyCount,
            ScoreboardWriteMode.HighScoreDesc);
#else
        //Debug.Log(
        //    $"[UnityroomRanking] EditorÇ≈ÇÕëóêMÇµÇ‹ÇπÇÒÅB " +
        //    $"Score={highScore}, " +
        //    $"Visited={visitedSplineCount}, " +
        //    $"Defeated={defeatedEnemyCount}");
#endif
    }
}