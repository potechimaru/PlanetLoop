/// <summary>
/// HUDのカウント表示を管理するインターフェース。現在のカウントと最大カウントを設定するためのメソッドを定義する。
/// </summary>
public interface IHUDCountView
{
    void SetCount(int current, int max);
}