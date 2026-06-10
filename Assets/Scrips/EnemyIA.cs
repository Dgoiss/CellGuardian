using UnityEngine;
using UnityEngine.AI; // IMPORTANTE: Dá acesso ao motor de inteligência artificial da Unity

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTarget;

    void Start()
    {
        // Pega o componente NavMeshAgent anexado a esta célula cancerígena
        agent = GetComponent<NavMeshAgent>();

        // Procura na cena o objeto que tem a Tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Inimigo não encontrou nenhum objeto com a Tag 'Player' na cena!");
        }
    }

    void Update()
    {
        // Se o Player existir e o jogo não estiver pausado no dado, persegue ele
        if (playerTarget != null && agent != null)
        {
            // Checa se o GameManager existe e se está na tela de dados. Se sim, para a IA.
            if (GameManager.instance != null && GameManager.instance.isDrafting)
            {
                agent.isStopped = true;
                return;
            }

            // Despausa a IA caso o jogo tenha voltado ao normal
            agent.isStopped = false;
            
            // Define o destino do agente como a posição em tempo real do player
            agent.SetDestination(playerTarget.position);
        }
    }
}