using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // 最大血量
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth; // 初始化血量
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("玩家受到攻擊，剩餘血量: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die(); // 玩家死亡
        }
    }

    void Die()
    {
        Debug.Log("玩家死亡");
        Destroy(gameObject); // 摧毀玩家角色
    }
}