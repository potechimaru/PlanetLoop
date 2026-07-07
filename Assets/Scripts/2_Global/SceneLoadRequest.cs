public class SceneLoadRequest
{
    public string NextSceneName { get; private set; }

    public void SetNextScene(string sceneName)
    {
        NextSceneName = sceneName;
    }
}