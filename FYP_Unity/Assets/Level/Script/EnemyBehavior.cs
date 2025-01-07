using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public NavMeshAgent agent; // NavMeshAgent 用於移動
    public float detectionRange = 10f; // 檢測範圍
    public float attackRange = 2f; // 攻擊範圍
    public int attackDamage = 10; // 每次攻擊的傷害
    public float attackCooldown = 1.5f; // 攻擊冷卻時間

    private List<Transform> playersInRange = new List<Transform>(); // 檢測範圍內的玩家
    private Transform currentTarget; // 當前目標
    private bool isAttacking = false;

    void Update()
    {
        // 更新目標：選擇距離最近的玩家
        UpdateTarget();

        if (currentTarget == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

        if (distanceToTarget <= detectionRange)
        {
            if (distanceToTarget > attackRange)
            {
                // 玩家在攻擊範圍外，追蹤玩家
                agent.SetDestination(currentTarget.position);
                Debug.Log("敵人正在追蹤玩家：" + currentTarget.name);
            }
            else
            {
                // 玩家在攻擊範圍內，停止移動並攻擊
                agent.ResetPath();
                if (!isAttacking)
                {
                    StartCoroutine(AttackPlayer());
                }
            }
        }
        else
        {
            // 玩家不在檢測範圍內
            agent.ResetPath();
            Debug.Log("玩家已離開檢測範圍，敵人停止追蹤。");
        }
    }

    private void UpdateTarget()
    {
        if (playersInRange.Count == 0)
        {
            currentTarget = null;
            return;
        }

        // 選擇距離最近的玩家
        float closestDistance = float.MaxValue;
        Transform closestPlayer = null;

        foreach (var player in playersInRange)
        {
            if (player == null) continue; // 防止玩家被銷毀後報錯

            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        currentTarget = closestPlayer;
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;

        Debug.Log("敵人正在攻擊玩家！");
        if (currentTarget.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"敵人成功攻擊玩家，造成 {attackDamage} 點傷害！");
        }

        yield return new WaitForSeconds(attackCooldown); // 攻擊冷卻
        isAttacking = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInRange.Add(other.transform); // 將玩家加入列表
            Debug.Log("玩家進入檢測範圍：" + other.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInRange.Remove(other.transform); // 將玩家移出列表
            Debug.Log("玩家離開檢測範圍：" + other.name);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // 顯示檢測範圍
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange); // 顯示攻擊範圍
    }
}