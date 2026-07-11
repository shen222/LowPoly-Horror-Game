using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    
    [Header("Gun Settings")]
    public float fireRate = 0.4f;
    public int magSize = 12;
    public int reserveAmmo = 48;
    public float reloadTime = 1.2f;

    [Header("References")]
    public Transform firePoint;
    public GameObject shootVFXPrefabA;
    public GameObject shootVFXPrefabB;
    public GameObject projectilePrefab;
    public Transform magazineEjectPoint;
    public GameObject magazinePrefab;
    public Animator characterAnimator;
    public Animator gunAnimator;
    public GameObject shellPrefab;
    public Transform shellEjectPoint;

    private float nextFireTime;
    private int currentAmmo;
    private bool isReloading = false;
    
    void Start()
    {
        currentAmmo = magSize;
    }

    public void TryShoot()
    {
        if (isReloading) return;

        if (!CanShoot()) return;

        Shoot();
    }

    private bool CanShoot()
    {
        if (Time.time < nextFireTime)
            return false;

        if (currentAmmo <= 0)
        {
            //PlayOutOfAmmoSFX();
            return false;
        }

        return true;
    }

    private void Shoot()
    {
        characterAnimator.SetTrigger("Shoot");
        gunAnimator.SetTrigger("Shoot");
        
        Instantiate(shellPrefab, shellEjectPoint.position, shellEjectPoint.rotation);
        
        nextFireTime = Time.time + fireRate;
        
        currentAmmo--;
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

GameObject selectedVFX = Random.value > 0.5f ? shootVFXPrefabA : shootVFXPrefabB;

if (selectedVFX != null && firePoint != null)
{
    GameObject vfx = Instantiate(selectedVFX, firePoint.position, firePoint.rotation);

    // Randomly adjust the scale
    float randomScale = 1f + Random.Range(-0.1f, 0.1f);
    vfx.transform.localScale *= randomScale;

    Destroy(vfx, 2f);
}
        //PlayShootSFX();
    }
    
    public void TryReload()
    {
        if (isReloading) return;
        if (currentAmmo == magSize) return;
        if (reserveAmmo <= 0) return;

        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        characterAnimator.SetTrigger("Reload");
        gunAnimator.SetTrigger("Reload");
        isReloading = true;
        
        Instantiate(magazinePrefab, magazineEjectPoint.position, magazineEjectPoint.rotation);
        //PlayReloadAnimation();
        //PlayReloadSFX();

        yield return new WaitForSeconds(reloadTime);

        // Calculate actual ammo to load
        int needed = magSize - currentAmmo;
        int ammoToLoad = Mathf.Min(needed, reserveAmmo);

        currentAmmo += ammoToLoad;
        reserveAmmo -= ammoToLoad;

        isReloading = false;
    }
    
    public void AddReserveAmmo(int amount)
    {
        reserveAmmo += amount;
    }
    
    //private void PlayShootVFX() { }
    //private void PlayShootSFX() { }
    //private void PlayOutOfAmmoSFX() { }
    //private void PlayReloadAnimation() { }
    //private void PlayReloadSFX() { }
}
