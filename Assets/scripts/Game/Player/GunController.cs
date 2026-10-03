using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public enum GunState
    {
        Idle,
        Taking,
        Shooting,
        Replenishing
    }
    [SerializeField]
    public FirstPersonCtrl ctrl;

    public GunState currentState = GunState.Idle;

    [Header("枪械设置")]
    public float takeInterval = 0.5f;
    public float takeTimer = 0;
    public string takeTrigger = "Take";
    public int MagazineSize = 30;
    private int currentAmmo = 30;

    [Header("射击设置")]
    public Animator gunAni;
    public GameObject bulletPrefab;
    public SimpleSpawner BulletSpawner;
    public Transform firePoint;
    public float fireInterval = 0.1f;
    public float fireTimer = 0;

    [Header("后坐力设置")]
    public float recoilAngle = -30f;
    public float speed = 10f;
    public float returnSpeed = 5f;
    public float targetRotation = 0f;
    public float currentRotation = 0f;

    void Start()
    {
        Reset();
    }
    public void Reset()
    {
        if(gunAni == null)
        {
            gunAni = GetComponent<Animator>();
        }
        if (ctrl == null)
        {
            ctrl = GetComponentInParent<FirstPersonCtrl>();
        }
        if(firePoint == null) 
            firePoint = transform.GetChild(0).GetChild(0);
        //独自生成一个BulletSpawner，避免与EnmeySpawner冲突
        if(BulletSpawner == null)
            SetBulletSpawner();
        currentState = GunState.Idle;
        takeTimer = 0;
        fireTimer = 0;
        currentAmmo = MagazineSize;
        targetRotation = 0;
        currentRotation = 0;
        gunAni.SetBool(takeTrigger, false);
    }

    void Update()
    {
        Recoil();
        takeTimer += Time.deltaTime;
        fireTimer += Time.deltaTime;
        if (takeTimer > takeInterval && Input.GetKey(KeyCode.Q))
        {
            takeTimer = 0;
            if(currentState == GunState.Idle)
            {
                currentState = GunState.Taking;
                gunAni.SetBool(takeTrigger, true);
            }
            else if(currentState == GunState.Taking)
            {
                currentState = GunState.Idle;
                gunAni.SetBool(takeTrigger, false);
                currentState = GunState.Idle;
            }
        }
        if (currentState == GunState.Taking)
        {
            if(currentAmmo <= 0 || Input.GetKey(KeyCode.Z))
            {
                ReplenishAmmo();
                return;
            }
            if(fireTimer > fireInterval && Input.GetMouseButton(1))
            {
                fireTimer = 0;
                Fire();
            }
        }
    }
    void Recoil()
    {
        //目标旋转角逐渐恢复至0
        targetRotation = Mathf.Lerp(targetRotation, 0, returnSpeed * Time.deltaTime);
        //当前旋转角逐渐接近目标旋转角
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, speed * Time.deltaTime);
        //应用旋转到gun
        gunAni.transform.localRotation = Quaternion.Euler(currentRotation, 0, 0);
    }
    void Fire()
    {
        currentAmmo--;
        if (BulletSpawner == null)
        {
            Debug.LogError("BulletSpawner is not set!");
        }
        BulletSpawner?.SpawnOne(firePoint.position, firePoint.rotation);
        targetRotation += recoilAngle;
    }
    public void ReplenishAmmo()
    {
        currentState = GunState.Replenishing;
        gunAni.SetTrigger("Replenish");       
    }

    public void FinishReplenish()
    {  
        currentState = GunState.Taking;
        currentAmmo = MagazineSize;
    }

    private void SetBulletSpawner()
    {
        if (BulletSpawner == null)
        {
            BulletSpawner = gameObject.AddComponent<SimpleSpawner>();
            BulletSpawner.objectPool = PoolManager.Instance.GetPool(bulletPrefab, 20);
            BulletSpawner.autoSpawn = false;
            BulletSpawner.autoStart = false;
        }
    }
}
