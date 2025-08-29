using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;
using TMPro;

public class Weapon : MonoBehaviour
{
    [Header("--- Weapon Settings ---")]
    [SerializeField] private int weaponID;
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private int bulletsPerShot = 1;
 

    [Header("--- Bullet Settings ---")]
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletForce;

    [Header("--- Spread ---")]
    [SerializeField] private float range;
    [SerializeField] private float currSpread;
    [SerializeField] private float maxSpread;
    [SerializeField] private float minSpread;
    [SerializeField] private float dropRatioSpread;
    [SerializeField] private float growRatioSpread;
    [SerializeField] private float xAxisModifier = 1f;
    [SerializeField] private float yAxisModifier = 1f;

    [Header("--- Ammo ---")]
    public float currAmmo;
    public float maxAmmo;
    public TextMeshProUGUI currAmmoText;
    public bool infiniteAmmo;

    [Header("--- Sound ---")]
    [SerializeField] private AudioClip basicShotClip;
    [SerializeField] private AudioClip[] emptyShotClip;
    private AudioSource audioSource;

    [Header("--- Effects ---")]
    [SerializeField] private GameObject bulletObject;

    [Header("--- Muzzle Flash ---")]
    [SerializeField] private GameObject[] muzzleFlashPool;
    [SerializeField] private float muzzleFlashLength;
    private int muzzleFlashPoolCount;
    private GameObject activeMuzzle1;
    private GameObject activeMuzzle2;
    private int lastMuzzleIndex = -1;

    [Header("--- Impact ---")]
    [SerializeField] private GameObject basicImpact;

    [Header("--- References ---")]
    [SerializeField] private Transform muzzleTransform;
    [SerializeField] private MeshRenderer[] weaponMesh;
    [SerializeField] private SkinnedMeshRenderer armsMesh;
    [SerializeField] private Image crossHair;
    [SerializeField] private Canvas canvas;

    [Header("--- Cross ---")]
    [SerializeField] private float crossGrowRate;
    private float maxCrosshairSize; 
    private float minCrosshairSize;
    private float maxCrosshairAlpha = 1f;
    private float minCrosshairAlpha = 0.1f;
    private float crossSize;
    private float crossAlpha;

    [Header("--- CamShake ---")]
    [SerializeField] private float camShakeMagnitude;
    [SerializeField] private float camShakeRoughness;

    // --- Animation ---
    private Animator animator;

    // --- Timers ---
    private float loadTimer = 0f;

    // --- Aim ---
    private Vector3 aimDirection;
    private Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    private Vector3 endPoint;
    private RaycastHit hit;
    private Camera playerCamera;
    private Vector3 aimUp;
    private Vector3 aimRight;

   // --- special ---
    private WeaponAnimation weaponAnimation;
   
    [Header("--- Specials ---")]
    [SerializeField] private bool isShotgun = false;
    [SerializeField] private bool isPhysical = false;
    [SerializeField] private bool isUpdateCross = true;
    [SerializeField] private LayerMask aimLayerMask;

    private PlayerPickUpUI playerPickUpUI;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        playerCamera = Camera.main;

        minCrosshairSize = crossHair.rectTransform.sizeDelta.x;
        maxCrosshairSize = minCrosshairSize * crossGrowRate;

        muzzleFlashPoolCount = muzzleFlashPool.Length;
        foreach(GameObject flash in muzzleFlashPool) flash.SetActive(false);
    
        weaponAnimation = GetComponent<WeaponAnimation>();

        playerPickUpUI = GetComponentInParent<PlayerPickUpUI>();

        if (infiniteAmmo) currAmmo = maxAmmo;
    }

    private void Update()
    {
        maxCrosshairSize = minCrosshairSize * crossGrowRate;
        CheckAmmo();
        LoadTimer();
        UpdateSpread();
        UpdateUI();
    }

    public void Activate(bool on) 
    {
        foreach (MeshRenderer meshRen in weaponMesh) { meshRen.enabled = on; }
        armsMesh.enabled = on;
        canvas.enabled = on;

        if (on)
        {
            loadTimer = 0;
            currSpread = minSpread;
        }
    }

    public void BasicShot()
    {
        if (loadTimer <= 0 && currAmmo > 0)
        {

            if (weaponAnimation != null)
            {
                weaponAnimation.AddShotSpeed();
            }

            animator.SetTrigger("BasicShot");

            CameraShaker.Instance.ShakeOnce(camShakeMagnitude, camShakeMagnitude, 0.1f, 0.5f);


            for (int i = 0; i < bulletsPerShot; i++)
            {
                if (isPhysical)
                {
                    Ray ray = playerCamera.ScreenPointToRay(screenCenter);

                    // Perform a raycast from the camera
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, aimLayerMask))
                    {
                        Vector3 targetPoint = hit.point;
                        aimDirection = (targetPoint - muzzleTransform.position).normalized;

                        // Instantiate and prepare the projectile
                        Projectile projectile = Instantiate(bulletObject, muzzleTransform.position, Quaternion.LookRotation(aimDirection), null).GetComponent<Projectile>();
                        projectile.PrepareProjectile(aimDirection);
                        projectile.ReleaseProjectile();
                    }
                }
                else
                {
                    aimDirection = playerCamera.ScreenPointToRay(screenCenter).direction;
                    // Calculate right and up vectors relative to aimDirection
                    aimRight = Vector3.Cross(aimDirection, Vector3.up).normalized;
                    aimUp = Vector3.Cross(aimRight, aimDirection).normalized;
                    // Apply random spread
                    aimDirection = Quaternion.AngleAxis(Random.Range(-currSpread, currSpread) * xAxisModifier, aimUp) *
                                   Quaternion.AngleAxis(Random.Range(-currSpread, currSpread) * yAxisModifier, aimRight) *
                                   aimDirection;

                    if (Physics.Raycast(playerCamera.transform.position, aimDirection, out hit, range, aimLayerMask))
                    {
                        GameObject bullet = Instantiate(bulletObject, muzzleTransform.position, muzzleTransform.rotation);
                        StartCoroutine(SpawnBullet(bullet, hit));

                        //Debug.DrawLine(playerCamera.transform.position, hit.point, Color.red, 1f);
                    }
                    else
                    {
                        endPoint = playerCamera.transform.position + aimDirection * range;

                        GameObject bullet = Instantiate(bulletObject, muzzleTransform.position, muzzleTransform.rotation);
                        StartCoroutine(SpawnBullet(bullet, endPoint));

                        //Debug.DrawRay(playerCamera.transform.position, aimDirection * range, Color.red, 1f);
                    }
                }
            }

            if (!infiniteAmmo) currAmmo--;

            audioSource.PlayOneShot(basicShotClip);

            MuzzleFlash();
            AddSpread();

            loadTimer = 1f / fireRate;

        }
        else if (loadTimer <= 0 && currAmmo <= 0) {
            audioSource.PlayOneShot(emptyShotClip[Random.Range(0, emptyShotClip.Length)]);
            loadTimer = 1f / fireRate;
        } 
    }

    private void Hit(RaycastHit hit)
    {
        Collider hitCollider = hit.collider;
        if (hitCollider == null) return;
        if (hitCollider.CompareTag("DeadEnemy"))
        {
            hitCollider.GetComponent<Rigidbody>().AddForce(-hit.normal * bulletForce, ForceMode.Impulse);

            // visual impact
            Instantiate(hitCollider.GetComponentInParent<EnemyController>().impactParticle, hit.point, Quaternion.LookRotation(hit.normal), null);
        }
        else if (hitCollider.CompareTag("Enemy"))
        {
            hitCollider.GetComponentInParent<EnemyHealth>().TakeDamage(damage, isShotgun, true);

            // visual impact
            Instantiate(hitCollider.GetComponentInParent<EnemyController>().impactParticle, hit.point, Quaternion.LookRotation(hit.normal), null);
        }
        else if (hitCollider.CompareTag("DynamicProp"))
        {
            Rigidbody dynamicPropRb = hitCollider.GetComponent<Rigidbody>();
            if (dynamicPropRb == null) {
                dynamicPropRb = hitCollider.GetComponentInParent<Rigidbody>();
            }

            dynamicPropRb.AddForce(-hit.normal * bulletForce, ForceMode.Impulse);
        }
        else if (hitCollider.CompareTag("Interactive")) {
            hitCollider.GetComponent<InteractiveObject>().TakeDamage();
        }
        
        // visual impact
        Instantiate(basicImpact, hit.point, Quaternion.LookRotation(hit.normal), null);
        
    }

    private void CheckAmmo()
    { 
        if (currAmmo > maxAmmo) currAmmo = maxAmmo;
    }

    private void LoadTimer()
    {
        if (loadTimer > 0) loadTimer -= Time.deltaTime * 1f;
    }

    private void UpdateSpread()
    {
        currSpread -= Time.deltaTime * (dropRatioSpread * (currSpread > (maxSpread / 2) ? 1.5f : 1f));
        currSpread = Mathf.Max(currSpread, minSpread);
    }

    private void AddSpread()
    {
        currSpread += maxSpread / growRatioSpread;
        currSpread = Mathf.Min(currSpread, maxSpread);
    }


    private void UpdateUI()
    {
        if (isUpdateCross) {
            crossSize = Mathf.Lerp(minCrosshairSize, maxCrosshairSize, currSpread / maxSpread);
            crossHair.rectTransform.sizeDelta = new Vector2(crossSize, crossSize);

            crossAlpha = Mathf.Lerp(maxCrosshairAlpha, minCrosshairAlpha, currSpread / maxSpread);
            crossHair.color = new Color(crossHair.color.r, crossHair.color.g, crossHair.color.b, crossAlpha);
        }

        if (!infiniteAmmo) currAmmoText.text = currAmmo.ToString("F0");
    }

    private void MuzzleFlash()
    {
        int muzzleIndex;
        do
        {
            muzzleIndex = Random.Range(0, muzzleFlashPoolCount);
        } while (muzzleIndex == lastMuzzleIndex);

        lastMuzzleIndex = muzzleIndex;

        if (activeMuzzle1 == null)
        {
            activeMuzzle1 = muzzleFlashPool[muzzleIndex];
            activeMuzzle1.SetActive(true);
            StartCoroutine(EndMuzzleFlash(activeMuzzle1));
        }
        else
        {
            activeMuzzle2 = muzzleFlashPool[muzzleIndex];
            activeMuzzle2.SetActive(true);
            StartCoroutine(EndMuzzleFlash(activeMuzzle2));
        }
    }

    private IEnumerator SpawnBullet(GameObject bullet, RaycastHit hit)
    {
        float bulletTravelTime = Vector3.Distance(bullet.transform.position, hit.point) / bulletSpeed;

        float time = 0f;
        Vector3 startPosition = bullet.transform.position;

        while (time < bulletTravelTime)
        {
            bullet.transform.position = Vector3.Lerp(startPosition, hit.point, time / bulletTravelTime);
            time += Time.deltaTime;

            yield return null;
        }
        bullet.transform.position = hit.point;

        Hit(hit);

        bullet.GetComponent<Bullet>().TurnOff();
    }

    private IEnumerator SpawnBullet(GameObject bullet, Vector3 hit)
    {
        float bulletTravelTime = Vector3.Distance(bullet.transform.position, hit) / bulletSpeed;

        float time = 0f;
        Vector3 startPosition = bullet.transform.position;

        while (time < bulletTravelTime)
        {
            bullet.transform.position = Vector3.Lerp(startPosition, hit, time / bulletTravelTime);
            time += Time.deltaTime;

            yield return null;
        }
        bullet.transform.position = hit;

        bullet.GetComponent<Bullet>().TurnOff();
    }

    private IEnumerator EndMuzzleFlash(GameObject activeMuzzle)
    {
        yield return new WaitForSeconds(muzzleFlashLength);
        activeMuzzle.SetActive(false);

        if (activeMuzzle == activeMuzzle1)
        {
            activeMuzzle1 = null;
        }
        else if (activeMuzzle == activeMuzzle2)
        {
            activeMuzzle2 = null;
        }
    }

    public bool AddAmmo(int amount) {
        if (currAmmo == maxAmmo)
        {
            playerPickUpUI.WeaponPickUpText(weaponID,"MAX");
            return false;
        }

        if (currAmmo + amount > maxAmmo)
        {
            playerPickUpUI.WeaponPickUpText(weaponID,"+ " + (maxAmmo - currAmmo).ToString("F0"));
            currAmmo = maxAmmo;
        }
        else
        {
            playerPickUpUI.WeaponPickUpText(weaponID, "+ " + amount.ToString("F0"));
            currAmmo += amount;
        }
        return true;
    }
}

