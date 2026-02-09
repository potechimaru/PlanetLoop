using System;
using UnityEngine;

internal class PlayerModel
{
    public float CurrentMoveSpeed { get; set; } = 5f;
    public float CurrentJumpspeed { get; set; } = 5f;
    public bool Clockwise { get; set; } = false;
    public bool IsGameOver { get; set; } = false;

    public float CurrentChargeDuaration { get; set; } = 0f; 

    private readonly float _CHARGE_DUARATION_1 = 1f;
    private readonly float _CHARGE_DUARATION_2 = 2f;

    private readonly float _NORMAL_JUMP_SPEED = 5f;

    private readonly float _CHARGE_JUMP_SPEED_1 = 10f;

    private readonly float _CHARGE_JUMP_SPEED_2 = 20f;

    private readonly float _NORMAL_MOVE_SPEED = 5f;
    private readonly float _CHARGE_MOVE_SPEED_1 = 3f;
    private readonly float _CHARGE_MOVE_SPEED_2 = 1f;

    private float GetChargeJumpSpeed ()
    {
        if (CurrentChargeDuaration <= _CHARGE_DUARATION_1) return _NORMAL_JUMP_SPEED;
        else if (CurrentChargeDuaration > _CHARGE_DUARATION_1 && CurrentChargeDuaration <= _CHARGE_DUARATION_2)
            return _CHARGE_JUMP_SPEED_1;
        else return _CHARGE_JUMP_SPEED_2;

    }

    private float GetMoveSpeed()
    {

        if (CurrentChargeDuaration <= _CHARGE_DUARATION_1) return _NORMAL_MOVE_SPEED;
        else if (CurrentChargeDuaration > _CHARGE_DUARATION_1 && CurrentChargeDuaration <= _CHARGE_DUARATION_2)
            return _CHARGE_MOVE_SPEED_1;
        else return _CHARGE_MOVE_SPEED_2;
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

    public bool IsChargeNormal()
    {
        return Mathf.Approximately(CurrentJumpspeed, _NORMAL_JUMP_SPEED);
    }

    public bool IsCharge1()
    {
        return Mathf.Approximately(CurrentJumpspeed, _CHARGE_JUMP_SPEED_1);
    }
}
