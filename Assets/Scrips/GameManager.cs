using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isDrafting = false;

    [Header("SISTEMA DE XP E NÍVEL")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 50; 
    public float xpMultiplier = 1.5f; 

    [Header("SISTEMA DE ÁUDIO")]
    private AudioSource efeitosAudioSource;
    public AudioClip somTiro;
    public AudioClip somMorteInimigo;
    public AudioClip somMortePlayer;
    public AudioClip somUpgrade;

    // Guarda os 3 números sorteados para a rodada atual de upgrades
    private int[] dadosSorteados = new int[3];

    [Header("ESTADO JOGO")]
    public bool isGameOver = false; // Controla se a partida acabou

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        efeitosAudioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // Tecla de teste caso queira forçar a abertura do menu
        if (Input.GetKeyDown(KeyCode.G) && !isDrafting)
        {
            TriggerDiceDraft();
        }
    }

    public void PlaySomTiro() { if (somTiro != null) efeitosAudioSource.PlayOneShot(somTiro); }
    public void PlaySomMorteInimigo() { if (somMorteInimigo != null) efeitosAudioSource.PlayOneShot(somMorteInimigo); }
    public void PlaySomMortePlayer() { if (somMortePlayer != null) efeitosAudioSource.PlayOneShot(somMortePlayer); }
    
    public void PlaySomUpgrade()
    {
        if (somUpgrade != null)
        {
            efeitosAudioSource.ignoreListenerPause = true;
            efeitosAudioSource.PlayOneShot(somUpgrade);
        }
    }

    public void GainXP(int amount)
    {
        if (isDrafting) return; 

        currentXP += amount;
        Debug.Log($"XP: {currentXP}/{xpToNextLevel}");

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentLevel++;
        currentXP -= xpToNextLevel; 
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpMultiplier);
        TriggerDiceDraft();
    }

    public void TriggerDiceDraft()
    {
        isDrafting = true;
        Time.timeScale = 0f; // Congela a física do jogo
        AudioListener.pause = true; // Pausa sons do ambiente

        // Rola e salva as 3 opções de dados (valores de 1 a 6)
        dadosSorteados[0] = Random.Range(1, 7);
        dadosSorteados[1] = Random.Range(1, 7);
        dadosSorteados[2] = Random.Range(1, 7);
    }

    // Retorna o Nome e a Descrição Científica do Upgrade baseado no dado rolado
    (string nome, string descricao) GetUpgradeDetails(int value)
    {
        switch (value)
        {
            case 1: 
                return ("Anticorpos Avançados", "Estimula a produção de proteínas de defesa.\nEfeito: +Velocidade de Ataque");
            case 2: 
                return ("Quimioterapia de Impacto", "Bloqueia o crescimento celular desordenado.\nEfeito: +Dano em Área");
            case 3: 
                return ("Imunoterapia Ativa", "Treina o sistema imunológico a reconhecer ameaças.\nEfeito: +1 Vida Máxima");
            case 4: 
                return ("Células T Aliadas", "Recruta linfócitos auxiliares para o combate.\nEfeito: Spawn de Drones de suporte");
            case 5: 
                return ("Radioterapia Direcionada", "Destrói o DNA mutado com feixes de energia.\nEfeito: Tiro Penetrante");
            case 6: 
                return ("Memória Imune", "Garante uma resposta celular secundária mais rápida.\nEfeito: +Velocidade de Movimento");
            default: 
                return ("Upgrade Genérico", "Melhoria biológica básica nos atributos.");
        }
    }

    void OnGUI()
    {
        // ====== CONFIGURAÇÃO DE ESTILO ADAPTATIVO ======
        GUIStyle estiloTexto = new GUIStyle(GUI.skin.label);
        estiloTexto.fontSize = Mathf.RoundToInt(Screen.height * 0.02f); // Tamanho padrão (aprox. 20px em 1080p)
        estiloTexto.alignment = TextAnchor.MiddleLeft;

        GUIStyle estiloTitulo = new GUIStyle(GUI.skin.box);
        estiloTitulo.fontSize = Mathf.RoundToInt(Screen.height * 0.025f);
        estiloTitulo.fontStyle = FontStyle.Bold;
        estiloTitulo.alignment = TextAnchor.UpperCenter;

        GUIStyle estiloBotao = new GUIStyle(GUI.skin.button);
        estiloBotao.fontSize = Mathf.RoundToInt(Screen.height * 0.018f);
        estiloBotao.alignment = TextAnchor.MiddleCenter;

        // ====== NOVO ESTILO EXCLUSIVO PARA A VIDA (TEXTO MAIOR) ======
        GUIStyle estiloVidaGrande = new GUIStyle(GUI.skin.label);
        // Aumentado significativamente: Multiplica por 0.035 (Fica cerca de 75% maior que o texto padrão)
        estiloVidaGrande.fontSize = Mathf.RoundToInt(Screen.height * 0.035f); 
        estiloVidaGrande.fontStyle = FontStyle.Bold;
        estiloVidaGrande.alignment = TextAnchor.MiddleRight; // Alinha à direita para o canto da tela
        estiloVidaGrande.normal.textColor = new Color(0.3f, 1f, 0.4f); // Verde claro bio/saudável

        // ================================================

        if (isDrafting)
        {
            // 1. Janela Central Dinâmica
            float menuWidth = Screen.width * 0.5f;
            float menuHeight = Screen.height * 0.6f;
            
            if (menuWidth < 400f) menuWidth = 400f;
            if (menuHeight < 350f) menuHeight = 350f;

            float posX = (Screen.width - menuWidth) / 2f;
            float posY = (Screen.height - menuHeight) / 2f;

            GUI.Box(new Rect(posX, posY, menuWidth, menuHeight), $"--- SELEÇÃO DE TRATAMENTO - NÍVEL {currentLevel} ---", estiloTitulo);

            float margemBorda = 20f;
            GUI.Label(new Rect(posX + margemBorda, posY + 45f, menuWidth - (margemBorda * 2), 40f), 
                "Sua célula evoluiu! Escolha uma estratégia biológica para conter o avanço do tumor:", estiloTexto);

            float botaoWidth = menuWidth - (margemBorda * 2);
            float botaoHeight = (menuHeight - 110f) / 3.5f; 
            float espacamento = botaoHeight + 10f; 

            for (int i = 0; i < 3; i++)
            {
                int valorDoDado = dadosSorteados[i];
                var (nome, descricao) = GetUpgradeDetails(valorDoDado);

                string textoBotão = $"[DADO {valorDoDado}] {nome}\n{descricao}";
                float botaoY = posY + 95f + (i * espacamento);

                if (GUI.Button(new Rect(posX + margemBorda, botaoY, botaoWidth, botaoHeight), textoBotão, estiloBotao))
                {
                    ApplyUpgrade(valorDoDado);
                }
            }
        }
        else
        {
            // 2. HUD DE XP ORIGINAL (Voltou ao tamanho padrão de 55px de altura no canto esquerdo)
            float hudWidth = Screen.width * 0.2f; 
            if (hudWidth < 180f) hudWidth = 180f; 
            
            float hudHeight = 55f;
            float hudX = 15f; 
            float hudY = 15f;

            GUI.Box(new Rect(hudX, hudY, hudWidth, hudHeight), "Célula de Defesa", estiloTitulo);
            GUI.Label(new Rect(hudX + 10f, hudY + 30f, hudWidth - 20f, 20f), $"Nível: {currentLevel}  |  XP: {currentXP} / {xpToNextLevel}", estiloTexto);

            // =============================================================
            // 3. NOVA HUD DE VIDA INDEPENDENTE (Canto Superior Direito)
            // =============================================================
            PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
            string textoVida = "Vidas: 0";
            if (playerHealth != null)
            {
                textoVida = $"Vidas: {playerHealth.totalLives}";
            }

            // Define as dimensões da caixa de texto da Vida
            float vidaWidth = 300f;
            float vidaHeight = 50f;
            
            // Posição X calculada a partir da largura total da tela (Margem de 20px da borda direita)
            float vidaX = Screen.width - vidaWidth - 20f;
            float vidaY = 15f; // Alinhado na mesma altura da HUD de XP

            // Desenha o texto da vida diretamente na tela com o estiloGrande e alinhado à direita
            GUI.Label(new Rect(vidaX, vidaY, vidaWidth, vidaHeight), textoVida, estiloVidaGrande);
        }

        // ... [Mantenha todo o seu código de upgrades e HUD aqui em cima] ...

        // NOVO BLOCO: Tela de Game Over
        if (isGameOver)
        {
            // Define as dimensões da janela de derrota (50% da largura, 40% da altura da tela)
            float goWidth = Screen.width * 0.5f;
            float goHeight = Screen.height * 0.4f;
            if (goWidth < 400f) goWidth = 400f;
            if (goHeight < 250f) goHeight = 250f;

            float posX = (Screen.width - goWidth) / 2f;
            float posY = (Screen.height - goHeight) / 2f;

            // Estilo personalizado para o título de Game Over em vermelho
            GUIStyle estiloGameOver = new GUIStyle(GUI.skin.box);
            estiloGameOver.fontSize = Mathf.RoundToInt(Screen.height * 0.035f);
            estiloGameOver.fontStyle = FontStyle.Bold;
            estiloGameOver.alignment = TextAnchor.UpperCenter;
            estiloGameOver.normal.textColor = Color.red;

            // Desenha a caixa de fundo da derrota
            GUI.Box(new Rect(posX, posY, goWidth, goHeight), "--- FIM DE PARTIDA ---", estiloGameOver);

            // Mensagem explicativa
            GUIStyle estiloTextoGO = new GUIStyle(GUI.skin.label);
            estiloTextoGO.fontSize = Mathf.RoundToInt(Screen.height * 0.022f);
            estiloTextoGO.alignment = TextAnchor.MiddleCenter;
            
            GUI.Label(new Rect(posX + 20f, posY + 60f, goWidth - 40f, 40f), 
                "O tumor se espalhou e as defesas falharam. O câncer venceu.", estiloTextoGO);

            // Botão para Reiniciar a Partida
            float btnWidth = goWidth * 0.6f;
            float btnHeight = 45f;
            float btnX = posX + (goWidth - btnWidth) / 2f;
            float btnY = posY + goHeight - 70f;

            GUIStyle estiloBtnGO = new GUIStyle(GUI.skin.button);
            estiloBtnGO.fontSize = Mathf.RoundToInt(Screen.height * 0.02f);

            if (GUI.Button(new Rect(btnX, btnY, btnWidth, btnHeight), "Tentar Novamente (Reiniciar)", estiloBtnGO))
            {
                // Descongela o tempo antes de recarregar a cena para o jogo não começar travado!
                Time.timeScale = 1f; 
                
                // Recarrega a cena atual de forma limpa
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    [Header("PREFABS DE UPGRADE")]
    public GameObject dronePrefab; // Arraste seu prefab de drone aqui no GameManager

    void ApplyUpgrade(int upgradeEscolhido)
    {
        Debug.Log($"Aplicando efeitos reais do Dado número {upgradeEscolhido}...");

        // Procura os componentes do jogador na cena para alterar seus atributos
        PlayerController playerCtrl = FindObjectOfType<PlayerController>();
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerCtrl != null)
        {
            switch (upgradeEscolhido)
            {
                case 1: // Anticorpos Avançados
                    playerCtrl.fireCooldown *= 0.7f; // Reduz o tempo de espera do tiro em 30% (Atira mais rápido!)
                    Debug.Log("Upgrade aplicado: Velocidade de ataque aumentada!");
                    break;

                case 2: // Quimioterapia de Impacto
                    playerCtrl.bulletForce *= 1.4f; // Aumenta a velocidade/impacto do projétil em 40%
                    // Dica: Se quiser dano em área real, você pode aumentar a escala física do Prefab da bala temporariamente:
                    playerCtrl.bulletPrefab.transform.localScale *= 1.3f;
                    break;

                case 3: // Imunoterapia Ativa
                    if (playerHealth != null)
                    {
                        playerHealth.totalLives += 1; // Cura ou adiciona +1 vida máxima para resistir à mitose
                        Debug.Log("Upgrade aplicado: +1 Vida de imunidade concedida!");
                    }
                    break;

                case 4: // Células T Aliadas (Spawn de Drone)
                    if (dronePrefab != null)
                    {
                        Transform pontoOrbita = playerCtrl.droneOrbitPoint != null ? playerCtrl.droneOrbitPoint : playerCtrl.transform;
                        
                        // CORREÇÃO: Forçamos o Y a subir 1.5 ou 2 unidades para ele NUNCA nascer tocando o chão
                        Vector3 posicaoSpawn = pontoOrbita.position + new Vector3(1.5f, 1.5f, 0f); 
                        
                        GameObject novoDrone = Instantiate(dronePrefab, posicaoSpawn, Quaternion.identity);
                        DroneSuporte droneScript = novoDrone.GetComponent<DroneSuporte>();
                        
                        if (droneScript != null)
                        {
                            droneScript.Inicializar(pontoOrbita, playerCtrl.bulletPrefab);
                        }
                    }
                    break;

                case 5: // Radioterapia Direcionada (Tiro Penetrante)
                    // Para fazer o tiro atravessar os inimigos, vamos mudar uma variável na própria Bullet.
                    // Adicione 'public bool penetrante = true;' no seu script Bullet.cs se quiser expandir isso!
                    playerCtrl.moveSpeed *= 1.15f; // Como compensação temporária pura, dá velocidade
                    break;

                case 6: // Memória Imune
                    playerCtrl.moveSpeed += 2f; // Aumenta permanentemente a velocidade de movimento do Point & Click
                    Debug.Log("Upgrade aplicado: Velocidade de movimento aumentada!");
                    break;
            }
        }

        PlaySomUpgrade(); // Som de confirmação
        
        isDrafting = false;
        Time.timeScale = 1f; // Descongela a partida
        AudioListener.pause = false; // Devolve o áudio ao mundo
    }

    public void IniciarGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;          // Congela o tempo e para o jogo instantaneamente!
        AudioListener.pause = true;   // (Opcional) Pausa todos os sons do ambiente
    }
}