using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    void Start() {
        animator = GetComponent<Animator>();
        timer = wanderTimer;
    }

    void Update() {
        timer += Time.deltaTime;
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= detectionRadius)
        {
            MoveTowardsTarget(target.position);
            if (distance <= attackRange) {
                animator.Play("Z_Attack");
            } else {
                animator.Play("Z_Run");
            }
        }
        else {
            if (timer >= wanderTimer) {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                if (newPos != Vector3.zero) {
                    wanderTarget = newPos;
                    timer = 0;
                }
            }
            MoveTowardsTarget(wanderTarget);
            animator.Play("Z_Walk1_InPlace");
        }
    }

    void MoveTowardsTarget(Vector3 targetPosition) {
        Vector3 moveDirection = targetPosition - transform.position;
        if (moveDirection != Vector3.zero) {
            moveDirection.Normalize();
            transform.position += moveDirection * speed * Time.deltaTime;
            Quaternion lookRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;

        if (NavMesh.SamplePosition(randDirection, out navHit, dist, layermask)) {
            return navHit.position;
        }
        return Vector3.zero;
    }
}
