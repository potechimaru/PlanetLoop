using UnityEngine;

internal enum ChargeLevel
{
    Normal,
    Charge1,
    Charge2
}

internal class PlayerModel
{
    public float CurrentMoveSpeed { get; set; } = 5f;
    public float CurrentJumpspeed { get; set; } = 5f;
    public bool Clockwise { get; set; } = false;
    public float CurrentChargeDuaration { get; set; } = 0f;

    public float LongJumpDistanceThreshold => _LONG_JUMP_DISTANCE;

    public ChargeLevel CurrentChargeLevel
    {
        get
        {
            if (CurrentChargeDuaration <= _CHARGE_DUARATION_1) return ChargeLevel.Normal;
            if (CurrentChargeDuaration <= _CHARGE_DUARATION_2) return ChargeLevel.Charge1;
            return ChargeLevel.Charge2;
        }
    }

    private readonly float _CHARGE_DUARATION_1 = 1f;
    private readonly float _CHARGE_DUARATION_2 = 2f;

    private readonly float _NORMAL_JUMP_SPEED = 5f;
    private readonly float _CHARGE_JUMP_SPEED_1 = 10f;
    private readonly float _CHARGE_JUMP_SPEED_2 = 20f;

    private readonly float _NORMAL_MOVE_SPEED = 5f;
    private readonly float _CHARGE_MOVE_SPEED_1 = 3.5f;
    private readonly float _CHARGE_MOVE_SPEED_2 = 2f;

    private readonly float _LONG_JUMP_DISTANCE = 1f;

    private float GetChargeJumpSpeed()
    {
        if (CurrentChargeDuaration <= _CHARGE_DUARATION_1) return _NORMAL_JUMP_SPEED;
        if (CurrentChargeDuaration <= _CHARGE_DUARATION_2) return _CHARGE_JUMP_SPEED_1;
        return _CHARGE_JUMP_SPEED_2;
    }

    private float GetMoveSpeed()
    {
        if (CurrentChargeDuaration <= _CHARGE_DUARATION_1) return _NORMAL_MOVE_SPEED;
        if (CurrentChargeDuaration <= _CHARGE_DUARATION_2) return _CHARGE_MOVE_SPEED_1;
        return _CHARGE_MOVE_SPEED_2;
    }

    public void ApplyChargeJumpSpeed()
    {
        CurrentJumpspeed = GetChargeJumpSpeed();
    }

    public void ApplyChargeMoveSpeed()
    {
        CurrentMoveSpeed = GetMoveSpeed();
    }

    public void InitializeMoveSpeed()
    {
        CurrentMoveSpeed = _NORMAL_MOVE_SPEED;
    }

    public void InitializeJumpSpeed()
    {
        CurrentJumpspeed = _NORMAL_JUMP_SPEED;
    }
}