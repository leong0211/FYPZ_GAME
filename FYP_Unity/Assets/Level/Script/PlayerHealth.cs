using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // �̤j��q
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth; // ��l�Ʀ�q
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("���a��������A�Ѿl��q: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die(); // ���a���`
        }
    }

    void Die()
    {
        Debug.Log("���a���`");
        Destroy(gameObject); // �R�����a����
    }
}