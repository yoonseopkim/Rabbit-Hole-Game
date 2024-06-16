using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BearController : MonoBehaviour
{
    public Transform rabbit;            // Rabbit의 Transform
    public float detectionRange = 10f;  // Bear가 Rabbit을 감지하는 거리
    public float attackRange = 2f;      // Bear가 Rabbit을 공격하는 거리
    public float speed = 0.5f;          // Bear의 이동 속도
    public float attackDamage = 50f;    // 공격당할 때 피해
    private NavMeshAgent agent;         // NavMeshAgent 컴포넌트
    private Animator animator;          // Animator 컴포넌트
    private Vector3 originalPosition;   // Bear의 원래 위치
    private bool isChasing = false;     // Bear가 Rabbit을 쫓고 있는지 여부
    private bool isAttacking = false;   // Bear가 Rabbit을 공격 중인지 여부
    private RabbitController rabbitController; // RabbitController 참조

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = speed;
        originalPosition = transform.position;
        rabbitController = rabbit.GetComponent<RabbitController>();
    }

    void Update()
    {
        float distanceToRabbit = Vector3.Distance(transform.position, rabbit.position);

        if (distanceToRabbit <= attackRange)
        {
            // 공격 애니메이션 재생
            isAttacking = true;
            agent.isStopped = true;
            animator.SetBool("IsAttacking", true);
            if (rabbitController != null)
            {
                rabbitController.TakeDamage(attackDamage * Time.deltaTime);
            }
        }
        else if (distanceToRabbit <= detectionRange)
        {
            // Rabbit을 쫓음
            isChasing = true;
            isAttacking = false;
            agent.isStopped = false;
            agent.SetDestination(rabbit.position);
            animator.SetBool("IsAttacking", false);
        }
        else
        {
            // Rabbit이 시야에서 사라지면 원래 자리로 돌아감
            if (isChasing)
            {
                isChasing = false;
                isAttacking = false;
                agent.SetDestination(originalPosition);
                animator.SetBool("IsAttacking", false);
            }
            else if (Vector3.Distance(transform.position, originalPosition) < 0.5f)
            {
                agent.isStopped = true;
            }
        }

        // 애니메이션 업데이트
        float currentSpeed = agent.velocity.magnitude;
        animator.SetFloat("Speed", isAttacking ? 0 : currentSpeed);
        animator.SetBool("Run Forward", currentSpeed > 0.1f);
        animator.SetBool("Run Backward", currentSpeed < -0.1f);
    }
}


