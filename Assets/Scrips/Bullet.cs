using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f; // Tempo para o tiro sumir se não acertar nada

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 1. Se acertar o inimigo, dá dano
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }
            Destroy(gameObject); 
        }
        // 2. SE COLIDIR COM O PLAYER OU COM O DRONE, NÃO FAZ NADA! (Ignora)
        else if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Drone"))
        {
            return; // Sai da função sem destruir a bala
        }
        // 3. Destrói o tiro se bater em paredes/obstáculos, mas ignora o chão
        else if (collision.gameObject.CompareTag("Chao") == false)
        {
            Destroy(gameObject);
        }
    }
}