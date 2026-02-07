internal class PlayerModel
{
    public float MoveSpeed { get; set; } = 5f;
    public float Jumpspeed { get; set; } = 5f;
    public bool Clockwise { get; set; } = false;

    public bool IsJumping { get; set; } = false;
    public bool IsGameOver { get; set; } = false;
}
