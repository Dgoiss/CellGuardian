using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("ATRIBUTOS DO CHEFÃO")]
    public int maxHealth = 50;        // Vida total do chefe (ajuste como quiser)
    public int currentHealth;
    public float moveSpeed = 3.5f;    // Velocidade de perseguição do chefe

    private Transform playerTarget;
    private UnityEngine.AI.NavMeshAgent agent;

    void Start()
    {
        currentHealth = maxHealth;

        // Pega o componente NavMeshAgent para ele saber andar na malha do mapa
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }

        // Procura pelo jogador na cena usando a tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    void Update()
    {
        // COMPORTAMENTO DE IA: Persegue o jogador constantemente se o jogo não estiver pausado
        if (playerTarget != null && agent != null)
        {
            // Se o GameManager estiver na tela de escolha de dados, congela o chefe
            if (GameManager.instance != null && GameManager.instance.isDrafting)
            {
                agent.isStopped = true;
                return;
            }

            agent.isStopped = false;
            agent.SetDestination(playerTarget.position);
        }
    }

    // Função que recebe o dano vindo dos projéteis
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Chefão recebeu dano! Vida atual: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("O Chefão foi derrotado!");

        // Comunica ao GameManager que o jogador ganhou a partida!
        if (GameManager.instance != null)
        {
            GameManager.instance.MarcarVitoria();
        }

        // Destrói o objeto do chefe da cena
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Se o Chefão encostar fisicamente no jogador, causa dano nele
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth pHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (pHealth != null)
            {
                pHealth.TakeDamage();
            }
        }
    }
}