using UnityEngine;
using UnityEngine.UI; // IMPORTANTE: Dá acesso ao componente Slider (Barra de Vida)

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
            Instantiate(cancerCellPrefab, transform.position, Quaternion.identity);
            Debug.Log("A célula boa sofreu mitose induzida! Um inimigo foi deixado para trás.");
        }

        // Abre o menu de dados (Dice Draft)
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

    // FUNÇÃO QUE ATUALIZA A BARRA DE VIDA VISUALMENTE
    // FUNÇÃO QUE ATUALIZA A BARRA DE VIDA VISUALMENTE
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

            // RETALIAÇÃO DIRETA: Destrói o inimigo que encostou na hora
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(enemyHealth.health);
            }
        }
    }
}