using UnityEngine;

public class PlayerSpawnOverlapResolver
{
    private readonly ContactFilter2D _filter;
    private readonly Collider2D[] _results = new Collider2D[16];

    public PlayerSpawnOverlapResolver(LayerMask blockingLayerMask)
    {
        _filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = blockingLayerMask,
            useTriggers = true
        };
    }

    public bool IsBlocked(Vector2 position, float radius)
    {
        int hitCount = Physics2D.OverlapCircle(
            position,
            radius,
            _filter,
            _results
        );

        return hitCount > 0;
    }
}
