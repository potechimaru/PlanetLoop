/// <summary>
/// UI自身がプールに返却されるためのインターフェース。
/// </summary>

public interface IPlayUIReturner
{
    void Return(PooledPlayUIItem item);
}