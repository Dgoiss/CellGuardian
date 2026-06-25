using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float initialSpawnDelay = 3f; // Tempo inicial entre os inimigos
    public float minimumSpawnDelay = 0.5f; // O limite máximo de velocidade de spawn
    public float difficultyIncreaseRate = 0.05f; // Quanto diminui o tempo a cada segundo
    public float spawnRadius = 15f; // Distância do centro onde os inimigos vão nascer

    private float currentSpawnDelay;
    private float timer;

    void Start()
    {
        currentSpawnDelay = initialSpawnDelay;
        timer = currentSpawnDelay;
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isBossPhase)
        {
            return; 
        }
        // Se o jogo estiver pausado no Dice Drafting, não conta tempo
        if (GameManager.instance != null && GameManager.instance.isDrafting) return;

        // Faz o jogo ficar progressivamente mais difícil diminuindo o tempo de espera
        if (currentSpawnDelay > minimumSpawnDelay)
        {
            currentSpawnDelay -= difficultyIncreaseRate * Time.deltaTime;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnEnemy();
            timer = currentSpawnDelay; // Reseta o cronômetro com o novo tempo mais rápido
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        // Escolhe um ângulo aleatório para criar um círculo ao redor do centro do mapa
        float angle = Random.Range(0f, Mathf.PI * 2f);
        // Se ela for bem pequena, tente deixar o Y em 0.1f ou 0.2f para ela nascer rente à malha azul
        Vector3 spawnPosition = new Vector3(Mathf.Cos(angle), 0.1f, Mathf.Sin(angle)) * spawnRadius;

        // Instancia o inimigo na borda do mapa
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}