using System;
using UnityEngine;
public interface IPlayerExternalFacade
{
    void MoveSubscribe();
    void JumpSubscribe(Action jump);

}

public class PlayerExternalFacade : IPlayerExternalFacade
{
    private readonly IInputFacade _inputFacade;
    public PlayerExternalFacade(IInputFacade inputFacade)
    {
        _inputFacade = inputFacade;
    }
    public void MoveSubscribe()
    {
        _inputFacade.MoveSubscribe();
    }
    public void JumpSubscribe(Action jump)
    {
        _inputFacade.JumpSubscribe(jump);
    }

}
