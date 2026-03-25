using UnityEngine;

public class SplineBurstEmitter : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private ClosedSplineLine spline;

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem psLocal;   // ’…’n‹ß‚­
    [SerializeField] private ParticleSystem psGlobal;  // Spline‘S‘Ì

    [Header("Local Burst")]
    [SerializeField] private int localCount = 48;
    [SerializeField] private float sampleRangeDistance = 0.6f;
    [SerializeField] private float spawnOffsetLocal = 0.05f;
    [SerializeField] private float speedMinLocal = 1.5f;
    [SerializeField] private float speedMaxLocal = 4.0f;
    [SerializeField, Range(0f, 1f)] private float spreadLocal = 0.25f;
    [SerializeField] private float sizeMinLocal = 0.03f;
    [SerializeField] private float sizeMaxLocal = 0.12f;
    [SerializeField] private float lifeMinLocal = 0.25f;
    [SerializeField] private float lifeMaxLocal = 0.6f;

    [Header("Global Scatter")]
    [SerializeField] private int globalCount = 24;
    [SerializeField] private float spawnOffsetGlobal = 0.08f;
    [SerializeField] private float speedMinGlobal = 0.2f;
    [SerializeField] private float speedMaxGlobal = 1.0f;
    [SerializeField, Range(0f, 1f)] private float spreadGlobal = 0.4f;
    [SerializeField] private float sizeMinGlobal = 0.02f;
    [SerializeField] private float sizeMaxGlobal = 0.08f;
    [SerializeField] private float lifeMinGlobal = 0.6f;
    [SerializeField] private float lifeMaxGlobal = 1.4f;

    private void Reset()
    {
        spline = GetComponent<ClosedSplineLine>();
    }

    public void BurstLocalAtDistance(float d0)
        => EmitNearDistance(psLocal, d0, localCount,
            sampleRangeDistance, spawnOffsetLocal,
            speedMinLocal, speedMaxLocal, spreadLocal,
            sizeMinLocal, sizeMaxLocal, lifeMinLocal, lifeMaxLocal,
            bothSides: false);

    public void ScatterGlobal()
        => EmitWhole(psGlobal, globalCount,
            spawnOffsetGlobal,
            speedMinGlobal, speedMaxGlobal, spreadGlobal,
            sizeMinGlobal, sizeMaxGlobal, lifeMinGlobal, lifeMaxGlobal,
            bothSides: true); // ‘S‘Ì‚Í—¼‘¤‚ÉŽU‚ç‚·

    private void EmitNearDistance(
        ParticleSystem ps,
        float d0,
        int count,
        float range,
        float offset,
        float speedMin, float speedMax,
        float spread,
        float sizeMin, float sizeMax,
        float lifeMin, float lifeMax,
        bool bothSides)
    {
        if (spline == null || ps == null) return;

        var ep = new ParticleSystem.EmitParams();

        for (int i = 0; i < count; i++)
        {
            float d = d0 + Random.Range(-range, range);

            Vector3 pos = spline.EvaluateByDistance(d);
            Vector3 n = spline.EvaluateNormalByDistance(d);

            float side = bothSides ? (Random.value < 0.5f ? -1f : 1f) : 1f;

            pos += n * side * Random.Range(0f, offset);

            Vector3 rand = (Vector3)Random.insideUnitCircle * spread;
            Vector3 dir = (n * side + rand).normalized;

            float spd = Random.Range(speedMin, speedMax);

            ep.position = pos;
            ep.velocity = dir * spd;
            ep.startSize = Random.Range(sizeMin, sizeMax);
            ep.startLifetime = Random.Range(lifeMin, lifeMax);

            ps.Emit(ep, 1);
        }
    }

    private void EmitWhole(
        ParticleSystem ps,
        int count,
        float offset,
        float speedMin, float speedMax,
        float spread,
        float sizeMin, float sizeMax,
        float lifeMin, float lifeMax,
        bool bothSides)
    {
        if (spline == null || ps == null) return;

        float total = spline.GetTotalLength();
        if (total <= 1e-6f) return;

        var ep = new ParticleSystem.EmitParams();

        for (int i = 0; i < count; i++)
        {
            float d = Random.Range(0f, total);

            Vector3 pos = spline.EvaluateByDistance(d);
            Vector3 n = spline.EvaluateNormalByDistance(d);

            float side = bothSides ? (Random.value < 0.5f ? -1f : 1f) : 1f;

            pos += n * side * Random.Range(0f, offset);

            Vector3 rand = (Vector3)Random.insideUnitCircle * spread;
            Vector3 dir = (n * side + rand).normalized;

            float spd = Random.Range(speedMin, speedMax);

            ep.position = pos;
            ep.velocity = dir * spd;
            ep.startSize = Random.Range(sizeMin, sizeMax);
            ep.startLifetime = Random.Range(lifeMin, lifeMax);

            ps.Emit(ep, 1);
        }
    }
}