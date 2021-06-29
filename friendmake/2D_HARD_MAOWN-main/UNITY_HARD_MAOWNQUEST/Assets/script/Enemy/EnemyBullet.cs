using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float attack;

    private void Start()
    {
        Physics2D.IgnoreLayerCollision(10, 10, true);
    }
    // 碰撞事件
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Purple>().Hit(attack);

        }
        // 碰撞到任何物件都要刪除
        Destroy(gameObject);
    }
}
