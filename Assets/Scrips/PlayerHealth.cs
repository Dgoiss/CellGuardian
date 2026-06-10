using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHitsBeforeSplit = 3;
    private int currentHits = 0;
    
    public int totalLives = 3; 
    public GameObject cancerCellPrefab; 

    public void TakeDamage()
    {
        currentHits++;
        Debug.Log($"O Player tomou um hit! ({currentHits}/{maxHitsBeforeSplit})");

        if (currentHits >= maxHitsBeforeSplit)
        {
            SplitCell();
        }
    }

    void SplitCell()
    {
        totalLives--;
        currentHits = 0;

        if (cancerCellPrefab != null)
        {
            // LEAVE SOMETHING BEHIND: Instancia o inimigo exatamente onde o player estava
            Instantiate(cancerCellPrefab, transform.position, Quaternion.identity);
            Debug.Log("A célula boa sofreu mitose induzida! Um inimigo foi deixado para trás.");
        }

        // CHAMA O DICE DRAFTING: Dá um upgrade para o jogador tentar reagir à dificuldade
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
        
        // 1. Toca o som de morte do jogador se o GameManager existir
        if (GameManager.instance != null) 
        {
            GameManager.instance.PlaySomMortePlayer();
            
            // 2. ATIVA A TELA DE GAME OVER E TRAVA O JOGO
            GameManager.instance.IniciarGameOver();
        }

        // 3. Destrói o objeto do player
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se o objeto que encostou no Player tem a tag "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // 1. O Player toma o hit normalmente (Logica original)
            TakeDamage();

            // 2. RETALIAÇÃO: Faz o inimigo que encostou também tomar dano
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // Aqui você define quanto de dano o inimigo toma ao encostar no Player.
                // Como os cubos pretos têm 3 de vida por padrão (no EnemyHealth.cs), 
                // dar 1 de dano fará com que eles precisem encostar 3 vezes para morrer,
                // ou você pode colocar 'enemyHealth.health' para explodir eles direto!
                enemyHealth.TakeDamage(enemyHealth.health);
                
                Debug.Log("A célula cancerígena sofreu retaliação por contato!");
            }
        }
    }
}