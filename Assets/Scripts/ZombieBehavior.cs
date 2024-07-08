using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ZombieBehavior : MonoBehaviour
{
    public Transform target;
    public float detectionRadius = 20f;
    public float attackRange = 2f;
    public float speed = 1f;
    public float rotationSpeed = 5f;
    public float wanderRadius = 20f;
    public float wanderTimer = 5f;

    private Animator animator;
    private float timer;
    private Vector3 wanderTarget = Vector3.zero;

    // Health bar related
    public GameObject healthBarPrefab;
    private GameObject healthBar;
    public float maxHealth = 100;
    private float currentHealth = 100;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        timer = wanderTimer;
        InitializeHealthBar();
    }

    void Update()
    {
        timer += Time.deltaTime;
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= detectionRadius)
        {
            MoveTowardsTarget(target.position);
            if (distance <= attackRange)
            {
                if (animator.HasState(0, Animator.StringToHash("Z_Attack")))
                {
                    animator.Play("Z_Attack");
                }
            }
            else
            {
                if (animator.HasState(0, Animator.StringToHash("Z_Run")))
                {
                    animator.Play("Z_Run");
                }
            }
        }
        else
        {
            if (timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                if (newPos != Vector3.zero)
                {
                    wanderTarget = newPos;
                    timer = 0;
                }
            }
            MoveTowardsTarget(wanderTarget);
            if (animator.HasState(0, Animator.StringToHash("Z_Walk1_InPlace")))
            {
                animator.Play("Z_Walk1_InPlace");
            }
        }

        if (healthBar != null)
        {
            healthBar.transform.position = transform.position + new Vector3(0, 2.5f, 0);
            healthBar.transform.LookAt(Camera.main.transform);
        }
    }

    void InitializeHealthBar()
    {
        if (healthBarPrefab)
        {
            healthBar = Instantiate(healthBarPrefab, transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity, transform);
            Slider healthSlider = healthBar.GetComponentInChildren<Slider>();
            if (healthSlider != null)
            {
                healthSlider.value = CalculateHealthPercentage();
                healthBar.transform.LookAt(Camera.main.transform);
            }
            else
            {
                Debug.LogError("Slider component not found on health bar prefab");
            }
        }
        else
        {
            Debug.LogError("Health bar prefab is not assigned");
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (healthBar != null)
        {
            Slider healthSlider = healthBar.GetComponentInChildren<Slider>();
            if (healthSlider != null)
            {
                healthSlider.value = CalculateHealthPercentage();
            }
            else
            {
                Debug.LogError("Failed to find Slider on health bar when taking damage.");
            }
        }
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        animator.Play("Z_FallingBack");
        Destroy(gameObject, 3);
    }

    float CalculateHealthPercentage()
    {
        return currentHealth;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;

        if (NavMesh.SamplePosition(randDirection, out navHit, dist, layermask))
        {
            return navHit.position;
        }
        return Vector3.zero;
    }

    void MoveTowardsTarget(Vector3 targetPosition)
    {
        Vector3 moveDirection = targetPosition - transform.position;
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
            transform.position += moveDirection * speed * Time.deltaTime;
            Quaternion lookRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
