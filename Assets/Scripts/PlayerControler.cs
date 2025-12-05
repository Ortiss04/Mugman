using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpAmount = 10f;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab; // Kéo Prefab đạn vào đây
    public Transform firePoint;
    public Transform firePointDiag;// Kéo FirePoint vào đây
    public float fireRate = 0.2f;   // Tốc độ bắn (0.2s một viên)

    [Header("Physics & Ground Check")]
    public Transform groundcheck;
    public LayerMask GroundMask;
    public float radius = 0.2f;

    // Các biến nội bộ
    private float nextFireTime = 0f;
    private float inputMovement;
    private bool jumpRequest = false;
    private bool isOnGround;
    private bool right = true; // Biến theo dõi hướng mặt (Phải/Trái)

    private Rigidbody2D rb;
    public Animator anim; // Kéo PlayerSprite vào đây

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Tự tìm Animator nếu quên kéo
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // 1. Nhận Input
        inputMovement = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical"); // Lấy tín hiệu lên/xuống

        // 2. Xử lý NHẢY
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequest = true;
        }

        // 3. Xử lý Animation CHẠY
        if (isOnGround && inputMovement != 0)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        // --- CẬP NHẬT ANIMATOR HƯỚNG NHÌN (MỚI) ---
        // Nếu bấm LÊN (verticalInput > 0), bật biến IsLookingUp
        bool lookingUp = verticalInput > 0;
        anim.SetBool("IsLookingUp", lookingUp);
        // ------------------------------------------

        // 4. Xử lý BẮN SÚNG (Giữ nút + Tốc độ bắn)
        if (Input.GetButton("Fire1") && Time.time > nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }

        // 5. Xử lý Animation BẮN
        if (Input.GetButton("Fire1"))
        {
            anim.SetBool("IsShooting", true);
        }
        else
        {
            anim.SetBool("IsShooting", false);
        }
    }

    void Shoot()
    {
        float verticalInput = Input.GetAxis("Vertical");
        Quaternion bulletRotation = Quaternion.identity;

        // Tạo một biến tạm để chứa điểm bắn sẽ dùng
        Transform currentFirePoint = firePoint; // Mặc định là bắn ngang

        // --- LOGIC CHỌN GÓC VÀ VỊ TRÍ ---
        if (verticalInput > 0) // Đang giữ nút LÊN
        {
            // 1. Đổi sang điểm bắn chéo
            currentFirePoint = firePointDiag;

            // 2. Tính góc xoay
            if (right)
                bulletRotation = Quaternion.Euler(0, 0, 45);
            else
                bulletRotation = Quaternion.Euler(0, 0, 135);
        }
        else // Bắn ngang bình thường
        {
            // 1. Giữ nguyên điểm bắn ngang (firePoint)
            currentFirePoint = firePoint;

            // 2. Tính góc xoay
            if (right)
                bulletRotation = Quaternion.Euler(0, 0, 0);
            else
                bulletRotation = Quaternion.Euler(0, 0, 180);
        }

        // --- QUAN TRỌNG: Dùng 'currentFirePoint.position' thay vì 'firePoint.position' ---
        Instantiate(bulletPrefab, currentFirePoint.position, bulletRotation);
    }

    void FlipSprite()
    {
        right = !right;
        Vector3 scale = anim.transform.localScale;
        scale.x *= -1;
        anim.transform.localScale = scale;
    }

    void FixedUpdate()
    {
        // Kiểm tra chạm đất
        isOnGround = Physics2D.OverlapCircle(groundcheck.position, radius, GroundMask);

        // Cập nhật biến cho Animator (nếu có dùng)
        anim.SetBool("isGrounded", isOnGround);

        // Di chuyển nhân vật
        // LƯU Ý: Unity 6 dùng linearVelocity, bản cũ dùng velocity
        rb.linearVelocity = new Vector2(inputMovement * speed, rb.linearVelocity.y);

        // Xử lý vật lý nhảy
        if (jumpRequest && isOnGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpAmount);
            jumpRequest = false;
        }

        // Lật nhân vật theo hướng đi
        if (!right && inputMovement > 0) FlipSprite();
        else if (right && inputMovement < 0) FlipSprite();
    }

    private void OnDrawGizmosSelected()
    {
        if (groundcheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundcheck.position, radius);
        }
    }
}