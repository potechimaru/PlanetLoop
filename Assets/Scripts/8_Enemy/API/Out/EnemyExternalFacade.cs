
public interface IEnemyExternalFacade
{
    void PlaySE(SEType sEType);
}

/// <summary>
/// Enemyコンポーネント群が外部のメソッドを呼び出すためのFacadeクラス。
/// このクラスを通じて、ゲームの状態やイベントにアクセスすることができる。
/// </summary>
public class EnemyExternalFacade : IEnemyExternalFacade 
{
    private readonly AudioManager _audioManager;

    public EnemyExternalFacade(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public void PlaySE(SEType sEType)
    {
        _audioManager.PlaySE(sEType);
    }



}

