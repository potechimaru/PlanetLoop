using UnityEngine;

public class AttachEvent
{
    private PlayerView _playerView;
    private PlayerModel _playerModel;
    private IPlayerExternalFacade _playerExternalFacade;

    private float _newOrbitPointUIPosOffsetY = 1.5f;


    internal AttachEvent(PlayerView playerView, PlayerModel playerModel, IPlayerExternalFacade playerExternalFacade)
    {
        _playerModel = playerModel;
        _playerView = playerView;
        _playerExternalFacade = playerExternalFacade;

    }

    internal void NewOrbitAttached(ClosedSplineLine currentSpline, float distance, Vector3 playerWorldPos)
    {

        if (currentSpline.IsNewOrbit)
        {
            currentSpline.FlashLandingMaterial();
            _playerExternalFacade.AddScore(ScoreRuleType.NewOrbit);
            _playerExternalFacade.SpawnNewOrbitPointUI(new Vector3 (playerWorldPos.x, playerWorldPos.y + _newOrbitPointUIPosOffsetY, playerWorldPos.z));
        }

        _playerView.PlaySplineAttachFx(currentSpline, distance, playerWorldPos);

        currentSpline.IsNewOrbit = false;

    }


}