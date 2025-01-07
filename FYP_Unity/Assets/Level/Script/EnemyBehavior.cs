using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public NavMeshAgent agent; // NavMeshAgent �Ω󲾰�
    public float detectionRange = 10f; // �˴��d��
    public float attackRange = 2f; // �����d��
    public int attackDamage = 10; // �C���������ˮ`
    public float attackCooldown = 1.5f; // �����N�o�ɶ�

    private List<Transform> playersInRange = new List<Transform>(); // �˴��d�򤺪����a
    private Transform currentTarget; // ��e�ؼ�
    private bool isAttacking = false;

    void Update()
    {
        // ��s�ؼСG��ܶZ���̪񪺪��a
        UpdateTarget();

        if (currentTarget == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

        if (distanceToTarget <= detectionRange)
        {
            if (distanceToTarget > attackRange)
            {
                // ���a�b�����d��~�A�l�ܪ��a
                agent.SetDestination(currentTarget.position);
                Debug.Log("�ĤH���b�l�ܪ��a�G" + currentTarget.name);
            }
            else
            {
                // ���a�b�����d�򤺡A����ʨç���
                agent.ResetPath();
                if (!isAttacking)
                {
                    StartCoroutine(AttackPlayer());
                }
            }
        }
        else
        {
            // ���a���b�˴��d��
            agent.ResetPath();
            Debug.Log("���a�w���}�˴��d��A�ĤH����l�ܡC");
        }
    }

    private void UpdateTarget()
    {
        if (playersInRange.Count == 0)
        {
            currentTarget = null;
            return;
        }

        // ��ܶZ���̪񪺪��a
        float closestDistance = float.MaxValue;
        Transform closestPlayer = null;

        foreach (var player in playersInRange)
        {
            if (player == null) continue; // ����a�Q�P�������

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

        Debug.Log("�ĤH���b�������a�I");
        if (currentTarget.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"�ĤH���\�������a�A�y�� {attackDamage} �I�ˮ`�I");
        }

        yield return new WaitForSeconds(attackCooldown); // �����N�o
        isAttacking = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInRange.Add(other.transform); // �N���a�[�J�C��
            Debug.Log("���a�i�J�˴��d��G" + other.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInRange.Remove(other.transform); // �N���a���X�C��
            Debug.Log("���a���}�˴��d��G" + other.name);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // ����˴��d��
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange); // ��ܧ����d��
    }
}