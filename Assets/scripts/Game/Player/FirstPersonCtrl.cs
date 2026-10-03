using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCtrl : MonoBehaviour
{
    public Rigidbody rb;
    public GunController gunController;
    [Header("Move")]
    public float speed = 3;
    public Vector3 velocity;
    public Vector3 startPos = new Vector3(0, 1, 0);
    public float jumpForce = 5;
    public static float speedWithGun = 2f;
    public static float speedWithoutGun = 5;
    [Header("Camera")]
    public Camera main_camera;
    public float sensitivity = 5.0f;
    public float xRotation = 0f;

    private bool takingGun()
    {
        return gunController != null && gunController.currentState != GunController.GunState.Idle;
    }
    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if (gunController == null)
            gunController = GetComponentInChildren<GunController>();
        if (main_camera == null)
            main_camera = GetComponentInChildren<Camera>();

        // 简要告警，便于快速定位缺失组件
        if (rb == null)
            Debug.LogWarning($"{name}: Rigidbody not found. Movement will not work until a Rigidbody is attached.");
        if (main_camera == null)
            Debug.LogWarning($"{name}: Camera not found as child. Camera-related logic may fail.");
        if (gunController == null)
            Debug.LogWarning($"{name}: GunController not found in children. Gun interaction will be disabled.");
    }

    void Start()
    {
        Reset();
    }
    public void Reset()
    {
        // 设置位置与朝向
        transform.SetPositionAndRotation(startPos, Quaternion.identity);

        if (main_camera != null)
        {
            // 使用 localPosition/localRotation 更通用
            main_camera.transform.localPosition = new Vector3(0, 0.5f, 0);
            main_camera.transform.localRotation = Quaternion.identity;
        }

        // 清除速度
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
        velocity = Vector3.zero;

        // 重置枪的状态（有空检查）
        if (gunController != null)
        {
            try
            {
                gunController.Reset();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"{name}: Exception when resetting GunController: {ex.Message}");
            }
        }
    }

    void Update()
    {
        TurnHead();
        Move();
        Jump();
    }
    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.velocity = velocity;
        }
    }
    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 dir = (transform.forward * vertical + transform.right * horizontal).normalized;
        speed = takingGun() ? speedWithGun : speedWithoutGun;
        velocity = dir * speed;

        // 保持竖直速度（如果没有 Rigidbody 则置 0）
        velocity.y = rb != null ? rb.velocity.y : 0f;
    }

    void TurnHead()
    {
        // 注意：当前实现需要按住鼠标左键才可视角旋转
        if (Input.GetMouseButton(0) && main_camera != null)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -40f, 40f);
            main_camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
    }
    private bool isGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.3f);
    }
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded() && rb != null)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
