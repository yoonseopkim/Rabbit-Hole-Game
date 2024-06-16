using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingItem : MonoBehaviour
{
    public float rotationSpeed = 50f; // 회전 속도
    public float floatingHeight = 1f; // 떠 있는 높이

    void Start()
    {
        // 초기 위치를 약간 떠 있는 상태로 설정
        transform.position = new Vector3(transform.position.x, transform.position.y + floatingHeight, transform.position.z);

        // 45도 기울임
        transform.rotation = Quaternion.Euler(45, 0, 0);
    }

    void Update()
    {
        // 반시계 방향으로 회전
        transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime, Space.World);
    }
}
