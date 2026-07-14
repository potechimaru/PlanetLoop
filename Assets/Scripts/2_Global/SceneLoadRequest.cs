/// <summary>
/// シーンロードする際のリクエスト情報を保持するクラス
/// </summary>
public class SceneLoadRequest
{
    // 遷移先のシーン名
    public string NextSceneName { get; private set; }

    // リトライを押して遷移する場合はtrue、通常の遷移の場合はfalse
    public bool IsRetry { get; private set; }

    public void SetNextScene(string sceneName, bool isRetry = false)
    {
        NextSceneName = sceneName;
        IsRetry = isRetry;
    }

    public void Clear()
    {
        NextSceneName = null;
        IsRetry = false;
    }
}