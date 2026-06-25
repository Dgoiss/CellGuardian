using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("LÓGICA ORIGINAL RESTAURADA")]
    public int maxHitsBeforeSplit = 3;  // Quantos hits aguenta antes da mitose
    public int currentHits = 0;        // Quantos hits já tomou
    public int totalLives = 3;          // Total de vidas/chances mecânicas
    
    public GameObject cancerCellPrefab; 

    [Header("INTERFACE VISUAL (BARRA DE VIDA)")]
    [Tooltip("Arraste o Slider da sua UI aqui no Inspector")]
    public Slider barraVidaSlider;

    void Start()
    {
        currentHits = 0;
        AtualizarInterfaceBarra();
    }

    public void TakeDamage()
    {
        currentHits++;
        Debug.Log($"O Player tomou um hit! ({currentHits}/{maxHitsBeforeSplit})");

        if (GameManager.instance != null)
        {
            GameManager.instance.PlaySomDanoPlayer(GameManager.instance.somDanoPlayer);
        }

        AtualizarInterfaceBarra();

        if (currentHits >= maxHitsBeforeSplit)
        {
            SplitCell();
        }
    }

    void SplitCell()
    {
        totalLives--;
        currentHits = 0; // Reseta os hits para a nova vida

        AtualizarInterfaceBarra();

        if (cancerCellPrefab != null)
        {
            // Sorteia um ângulo aleatório ao redor do jogador
            float anguloAleatorio = Random.Range(0f, Mathf.PI * 2f);
            
            // Define uma distância segura (ex: 12 metros, que joga a célula para um canto da tela)
            float distanciaSegura = 12f; 

            // Calcula a posição de spawn afastada do jogador
            Vector3 posicaoSpawnInimigo = new Vector3(
                transform.position.x + Mathf.Cos(anguloAleatorio) * distanciaSegura,
                1f, // Altura padrão do chão para não enterrar na malha
                transform.position.z + Mathf.Sin(anguloAleatorio) * distanciaSegura
                );

            // Instancia a nova ameaça cancerígena longe do jogador
            Instantiate(cancerCellPrefab, posicaoSpawnInimigo, Quaternion.identity);
            
            Debug.Log("SPLIT CELL: O acúmulo de danos causou uma mitose! Uma nova célula inimiga surgiu nos limites da tela!");
        }
        else
        {
            Debug.LogWarning("PlayerHealth: Nenhum prefab de célula cancerígena foi arrastado para o campo 'Cancer Cell Prefab'!");
        }

        // Abre o menu de dados (Upgrade)
        if (GameManager.instance != null)
        {
            GameManager.instance.TriggerDiceDraft();
        }

        if (totalLives <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over! O câncer venceu.");
        if (GameManager.instance != null) 
        {
            GameManager.instance.PlaySomMortePlayer();
            GameManager.instance.IniciarGameOver();
        }
        Destroy(gameObject);
    }

    public void AtualizarInterfaceBarra()
    {
        if (barraVidaSlider != null)
        {
            //Garante que o limite máximo do Slider mude em tempo real com o Upgrade!
            barraVidaSlider.maxValue = maxHitsBeforeSplit;
            
            // A barra começa cheia e vai esvaziando conforme toma hits
            barraVidaSlider.value = maxHitsBeforeSplit - currentHits;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // O Player toma o dano da colisão
            TakeDamage();

            //Destrói o inimigo que encostou na hora
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(enemyHealth.health);
            }
        }
    }
}