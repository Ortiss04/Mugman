using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f; // Thời gian tồn tại (giây)
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // --- QUAN TRỌNG: BAY THEO GÓC XOAY ---
        // transform.right: Tự động hiểu là hướng "mũi" của viên đạn.
        // Nếu đạn xoay 45 độ, nó sẽ bay chéo 45 độ.

        // LƯU Ý: Nếu bạn dùng Unity 2022 trở về trước mà báo lỗi dòng dưới, 
        // hãy đổi 'linearVelocity' thành 'velocity'
        rb.linearVelocity = transform.right * speed;

        // Tự hủy sau lifeTime giây
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 1. Không tự bắn vào bản thân
        if (hitInfo.CompareTag("Player")) return;

        // 2. Trúng kẻ địch
        if (hitInfo.CompareTag("Enemy"))
        {
            // Code trừ máu Enemy viết ở đây...
            Destroy(gameObject);
        }

        // 3. Trúng tường/đất
        else if (hitInfo.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}