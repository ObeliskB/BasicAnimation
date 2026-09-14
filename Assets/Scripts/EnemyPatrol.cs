using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Waypoints Setup")]
    [Tooltip("ใส่จุด Waypoint (เช่น Point A และ Point B)")]
    [SerializeField] private List<Transform> waypoints = new List<Transform>();

    [Header("Idle & Turn Settings")]
    [Tooltip("ระยะเวลาที่หยุดยืน Idle (วินาที)")]
    [SerializeField] private float idleDuration = 2.0f;
    [Tooltip("ความเร็วในการค่อยๆ หันหน้าไปหาจุดถัดไประหว่างยืน Idle")]
    [SerializeField] private float turnSpeed = 5.0f;

    [Header("Animation Setup")]
    [SerializeField] private Animator animator;
    [Tooltip("ชื่อพารามิเตอร์ Float ใน Animator (เช่น Speed)")]
    [SerializeField] private string speedParam = "Speed";

    private NavMeshAgent agent;
    private int currentTargetIndex = 0;
    private bool isIdling = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        if (waypoints == null || waypoints.Count < 2)
        {
            Debug.LogWarning("กรุณาใส่จุด Waypoint อย่างน้อย 2 จุดใน Inspector", this);
            return;
        }

        // เริ่มต้นเดินไปจุดแรก
        SetDestinationToCurrentWaypoint();
    }

    private void Update()
    {
        // อัปเดตค่าความเร็วส่งให้ Animator
        UpdateAnimator();

        if (isIdling || waypoints.Count == 0) return;

        // เช็คว่าตัวละครเดินถึงจุดหมายแล้วหรือยัง
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(IdleAtWaypointRoutine());
        }
    }

    private void SetDestinationToCurrentWaypoint()
    {
        agent.isStopped = false;
        agent.SetDestination(waypoints[currentTargetIndex].position);
    }

    private IEnumerator IdleAtWaypointRoutine()
    {
        isIdling = true;

        // 1. สั่งให้ NavMeshAgent หยุดเดินสนิท
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        // 2. คำนวณหาจุดถัดไปเพื่อเตรียมหันหน้า
        currentTargetIndex = (currentTargetIndex + 1) % waypoints.Count;
        Transform nextTarget = waypoints[currentTargetIndex];

        // คำนวณทิศทางที่ต้องหันไป (ตัดแกน Y ออกเพื่อไม่ให้ตัวละครก้มเงย)
        Vector3 directionToNext = (nextTarget.position - transform.position).normalized;
        directionToNext.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToNext);

        // 3. ยืน Idle ค้างไว้ตามเวลาที่กำหนด (idleDuration = 2 วิ)
        float timer = 0f;
        while (timer < idleDuration)
        {
            timer += Time.deltaTime;

            // ค่อยๆ หันตัวอย่างนุ่มนวลไปยังจุดหมายถัดไปในขณะที่กำลังยืน Idle
            if (directionToNext != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
            }

            yield return null;
        }

        // หันหน้าให้ตรงเป๊ะก่อนออกเดิน
        transform.rotation = targetRotation;

        // 4. สั่งให้ออกเดินต่อ
        SetDestinationToCurrentWaypoint();
        isIdling = false;
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        // ถ้ากำลังยืน Idle อยู่ ให้ส่งค่า 0 ทันทีเพื่อให้เล่นท่า Idle
        if (isIdling)
        {
            animator.SetFloat(speedParam, 0f);
        }
        else
        {
            // ถ้ากำลังเดิน ให้ส่งค่าความเร็วจริงของ Agent ไปขับเคลื่อน Blend Tree (Idle <-> Walk)
            float currentSpeed = agent.velocity.magnitude;
            animator.SetFloat(speedParam, currentSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);

            int next = (i + 1) % waypoints.Count;
            if (waypoints[next] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[next].position);
            }
        }
    }
}