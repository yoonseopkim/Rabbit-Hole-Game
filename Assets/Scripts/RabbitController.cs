using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리 매니저 추가
using UnityEngine.UI;

public class RabbitController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 700.0f; // 회전 속도
    public float maxHP = 100f;
    public Slider hpSlider; // HP 바 UI
    public float contactDamage = 1f; // 0.1초당 1의 피해
    public float speedBoostDuration = 5f; // 속도 증가 지속 시간
    public float speedMultiplier = 2f; // 속도 증가 배수

    private Animator animator;
    private float speed = 0.0f;
    private bool isDead = false;
    private bool isSpeedBoosted = false;
    private float originalMoveSpeed;
    private Color originalColor;
    private Camera mainCamera;
    private Rigidbody rb;
    private float currentHP;
    private Renderer rabbitRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        rabbitRenderer = GetComponentInChildren<Renderer>(); // 자식 객체에서 Renderer를 찾음

        originalMoveSpeed = moveSpeed;

        if (rabbitRenderer != null)
        {
            originalColor = rabbitRenderer.material.color;
        }

        currentHP = maxHP;
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
            // Fill Area의 색상 변경
            Image fillImage = hpSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = Color.red;
            }
        }
    }

    void Update()
    {
        if (!isDead)
        {
            float move = Input.GetAxis("Vertical");
            float strafe = Input.GetAxis("Horizontal");

            Vector3 forward = mainCamera.transform.forward;
            Vector3 right = mainCamera.transform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredMoveDirection = forward * move + right * strafe;

            if (desiredMoveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredMoveDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            speed = desiredMoveDirection.magnitude;
            animator.SetFloat("Speed", speed);
        }

        if (hpSlider != null)
        {
            hpSlider.value = currentHP;
        }
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            float move = Input.GetAxis("Vertical");
            float strafe = Input.GetAxis("Horizontal");

            Vector3 forward = mainCamera.transform.forward;
            Vector3 right = mainCamera.transform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredMoveDirection = forward * move + right * strafe;
            Vector3 movement = desiredMoveDirection * moveSpeed * Time.fixedDeltaTime;

            rb.MovePosition(rb.position + movement);

            if (Input.GetKeyDown(KeyCode.Space)) // 가상의 죽음 조건
            {
                Die();
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!isDead && collision.gameObject.CompareTag("Bear"))
        {
            TakeDamage(contactDamage * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDead && other.CompareTag("Food"))
        {
            Heal(20);
            Destroy(other.gameObject); // 먹은 아이템은 파괴
        }

        if (!isDead && other.CompareTag("Carrot"))
        {
            StartCoroutine(SpeedBoost());
            Destroy(other.gameObject); // 먹은 아이템은 파괴
        }

        if (other.CompareTag("Portal"))
        {
            SceneManager.LoadScene("Stage2"); // Stage2 씬으로 이동
        }
    }

    private IEnumerator SpeedBoost()
    {
        isSpeedBoosted = true;
        moveSpeed *= speedMultiplier;

        if (rabbitRenderer != null)
        {
            rabbitRenderer.material.color = new Color(1f, 0.5f, 0f); // 주황색 설정
        }

        yield return new WaitForSeconds(speedBoostDuration);

        moveSpeed = originalMoveSpeed;

        if (rabbitRenderer != null)
        {
            rabbitRenderer.material.color = originalColor;
        }

        isSpeedBoosted = false;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0 && !isDead)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetBool("IsDead", true); // Dead 애니메이션 트리거 설정
        Debug.Log("Rabbit died.");
    }
}
