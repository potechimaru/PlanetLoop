public class SceneLoadRequest
{
    public string NextSceneName { get; private set; }

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