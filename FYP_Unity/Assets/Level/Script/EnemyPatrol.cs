using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    public Transform patrolCenter; // ���޶��
    public float patrolRadius = 5f; // ���ޥb�|
    public float patrolSpeed = 2f; // ���޳t��
    public PatrolType patrolType = PatrolType.Circle; // ���������]��ΩΨ�L�^

    private float angle = 0f; // �Ω�p���ιB�ʪ�����

    public enum PatrolType
    {
        Circle,  // ��Ψ���
        Line     // �u�ʨ��ޡ]�i�X�i�^
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

    // ��Ψ���
    void CirclePatrol()
    {
        if (patrolCenter == null)
        {
            Debug.LogError("���޶�ߥ��]�m�I");
            return;
        }

        // �p���e����
        angle += patrolSpeed * Time.deltaTime; // �����H�ɶ��W�[
        if (angle >= 360f) angle -= 360f; // ���׽d�򭭨�b 0 - 360

        // �p��ĤH�s����m
        float x = patrolCenter.position.x + Mathf.Cos(angle) * patrolRadius;
        float z = patrolCenter.position.z + Mathf.Sin(angle) * patrolRadius;

        // ��s�ĤH����m
        transform.position = new Vector3(x, transform.position.y, z);
    }

    // �u�ʨ��ޡ]�ܨҡA�Ω��X�i�^
    void LinePatrol()
    {
        // �w�q��Ө����I
        Vector3 pointA = patrolCenter.position + new Vector3(-patrolRadius, 0, 0);
        Vector3 pointB = patrolCenter.position + new Vector3(patrolRadius, 0, 0);

        // �ϥ� PingPong �p��Ӧ^�B��
        float t = Mathf.PingPong(Time.time * patrolSpeed, 1); // t �b 0 �M 1 �����Ӧ^�ܤ�
        transform.position = Vector3.Lerp(pointA, pointB, t); // �b A �M B �������Ȳ���
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("�o�{���a�A����ިðl���I");
            // ����ިö}�l�l��
            StopPatrolAndChase(other.transform);
        }
    }

    void StopPatrolAndChase(Transform player)
    {
        // �l�ܪ��a���޿�A�Ҧp�ϥ� NavMeshAgent
        GetComponent<NavMeshAgent>().SetDestination(player.position);
        enabled = false; // ���Ψ��ޥ\��
    }
}