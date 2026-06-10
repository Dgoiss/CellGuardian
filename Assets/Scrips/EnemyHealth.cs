using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 3;
    [Tooltip("Quantidade de XP que esse inimigo dá ao morrer")]
    public int xpReward = 10; 

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.GainXP(xpReward);
            // ADICIONE ESTA LINHA AQUI:
            GameManager.instance.PlaySomMorteInimigo(); 
        }

        Destroy(gameObject);
    }
}