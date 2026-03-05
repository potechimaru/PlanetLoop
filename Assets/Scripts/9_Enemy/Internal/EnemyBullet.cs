using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 8f;
    private Vector3 _vel;

    public void Launch(Vector3 dirNormalized, float speed)
    {
        _vel = dirNormalized * speed;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += _vel * Time.deltaTime;
    }

    // ここでPlayerに当たったら即死、などはプロジェクト側のルールに合わせて実装
}