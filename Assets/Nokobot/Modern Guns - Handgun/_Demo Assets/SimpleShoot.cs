using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Location References")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destroy the casing object")][SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")][SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")][SerializeField] private float ejectPower = 150f;
    public AudioSource source;
    public AudioClip fireSound;

    void Start()
    {
        // Ensure all references are set up correctly
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();
    }

    public void PullTheTrigger()
    {
        // Trigger the shooting animation
        gunAnimator.SetTrigger("Fire");

        // Call Shoot function
        Shoot();

        // Optionally call CasingRelease if it needs to be synchronized with shooting
        CasingRelease();
    }

    // This function handles the creation and behavior of the bullet
    void Shoot()
    {
        // Play gun shot sound
        source.PlayOneShot(fireSound);

        // Handle muzzle flash
        if (muzzleFlashPrefab)
        {
            GameObject tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);
            Destroy(tempFlash, destroyTimer);
        }

        // Ensure there is a bullet prefab to instantiate
        if (!bulletPrefab) return;

        // Create the bullet and apply force
        GameObject bullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);

        // Perform raycast to check for hits
        RaycastHit hit;
        if (Physics.Raycast(barrelLocation.position, barrelLocation.forward, out hit, 100))
        {
            if (hit.transform.CompareTag("Zombie"))
            {
                ZombieBehavior zombie = hit.transform.GetComponent<ZombieBehavior>();
                if (zombie != null)
                {
                    zombie.TakeDamage(25);
                }
            }
            else
            {
                Debug.Log("Hit something that is not a zombie");
            }
        }
        else
        {
            Debug.Log("No hit detected");
        }
    }

    // This function handles the casing ejection
    void CasingRelease()
    {
        // Check if casingExitLocation and casingPrefab are set
        if (!casingExitLocation || !casingPrefab) return;

        // Create the casing at the ejection slot
        GameObject tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation);
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(ejectPower, (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        // Destroy the casing after a delay
        Destroy(tempCasing, destroyTimer);
    }
}
