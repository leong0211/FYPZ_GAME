using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    public Transform patrolCenter; // 巡邏圓心
    public float patrolRadius = 5f; // 巡邏半徑
    public float patrolSpeed = 2f; // 巡邏速度
    public PatrolType patrolType = PatrolType.Circle; // 巡邏類型（圓形或其他）

    private float angle = 0f; // 用於計算圓形運動的角度

    public enum PatrolType
    {
        Circle,  // 圓形巡邏
        Line     // 線性巡邏（可擴展）
    }

    void Update()
    {
        if (patrolType == PatrolType.Circle)
        {
            CirclePatrol();
        }
        else if (patrolType == PatrolType.Line)
        {
            LinePatrol();
        }
    }

    // 圓形巡邏
    void CirclePatrol()
    {
        if (patrolCenter == null)
        {
            Debug.LogError("巡邏圓心未設置！");
            return;
        }

        // 計算當前角度
        angle += patrolSpeed * Time.deltaTime; // 角度隨時間增加
        if (angle >= 360f) angle -= 360f; // 角度範圍限制在 0 - 360

        // 計算敵人新的位置
        float x = patrolCenter.position.x + Mathf.Cos(angle) * patrolRadius;
        float z = patrolCenter.position.z + Mathf.Sin(angle) * patrolRadius;

        // 更新敵人的位置
        transform.position = new Vector3(x, transform.position.y, z);
    }

    // 線性巡邏（示例，用於擴展）
    void LinePatrol()
    {
        // 定義兩個巡邏點
        Vector3 pointA = patrolCenter.position + new Vector3(-patrolRadius, 0, 0);
        Vector3 pointB = patrolCenter.position + new Vector3(patrolRadius, 0, 0);

        // 使用 PingPong 計算來回運動
        float t = Mathf.PingPong(Time.time * patrolSpeed, 1); // t 在 0 和 1 之間來回變化
        transform.position = Vector3.Lerp(pointA, pointB, t); // 在 A 和 B 之間插值移動
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("發現玩家，停止巡邏並追擊！");
            // 停止巡邏並開始追擊
            StopPatrolAndChase(other.transform);
        }
    }

    void StopPatrolAndChase(Transform player)
    {
        // 追蹤玩家的邏輯，例如使用 NavMeshAgent
        GetComponent<NavMeshAgent>().SetDestination(player.position);
        enabled = false; // 停用巡邏功能
    }
}