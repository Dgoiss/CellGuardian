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
    private AudioSource musicaAudioSource;
    public AudioClip musicaFaseNormal;
    public AudioClip musicaChefao;
    public AudioClip somTiro;
    public AudioClip somMorteInimigo;
    public AudioClip somMortePlayer;
    public AudioClip somUpgrade;
    public AudioClip somLevelUp;
    public AudioClip somDanoPlayer;

    // Guarda os 3 números sorteados para a rodada atual de upgrades
    private int[] dadosSorteados = new int[3];

    [Header("ESTADO JOGO")]
    public bool isGameOver = false; // Controla se a partida acabou

    [Header("SISTEMA DO CHEFÃO")]
    public GameObject bossPrefab;
    public bool isBossPhase = false;    // Controla se já estamos na fase do chefe
    public bool isVictory = false;      // Controla se o jogador venceu o jogo

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        efeitosAudioSource = gameObject.AddComponent<AudioSource>();

        musicaAudioSource = gameObject.AddComponent<AudioSource>();
        musicaAudioSource.playOnAwake = false;
        musicaAudioSource.loop = true;
        musicaAudioSource.volume = 0.5f;
    }

    void Start()
    {
        if (musicaFaseNormal != null)
        {
            musicaAudioSource.clip = musicaFaseNormal;
            musicaAudioSource.Play();
        }
    }

    void Update()
    {
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

    public void PlaySomLevelUp(AudioClip clip)
    {
        if (clip != null)
        {
            efeitosAudioSource.ignoreListenerPause = true;
            efeitosAudioSource.PlayOneShot(clip);
        }
    }

    public void PlaySomDanoPlayer(AudioClip clip)
    {
        if (clip != null)
        {
            efeitosAudioSource.ignoreListenerPause = true;
            efeitosAudioSource.PlayOneShot(clip);
        }
    }

    public void PlaySomExplosaoArea(AudioClip clip)
    {
        if (clip != null)
        {
            efeitosAudioSource.ignoreListenerPause = true;
            efeitosAudioSource.PlayOneShot(clip);
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
        PlaySomLevelUp(somLevelUp);
        currentXP -= xpToNextLevel; 
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpMultiplier);
        TriggerDiceDraft();

        if (currentLevel >= 10 && !isBossPhase)
        {
            IniciarFaseChefe();
        }
    }

public void TriggerDiceDraft()
    {
        if (isDrafting || isGameOver) return;

        isDrafting = true;
        Time.timeScale = 0f; // Congela o jogo
        AudioListener.pause = true; // Pausa o som do mundo

        // ====== ALTERADO AQUI: Sorteia apenas 2 dados (de 1 a 6) ======
        for (int i = 0; i < dadosSorteados.Length; i++)
        {
            dadosSorteados[i] = Random.Range(1, 7);
        }

        // Exibe no console para teste (Útil para verificar se funcionou)
        Debug.Log($"[DICE DRAFT] Nível UP! Opções sorteadas: Dado A = {dadosSorteados[0]} | Dado B = {dadosSorteados[1]}");

    }

    // Retorna o Nome e a Descrição Científica do Upgrade baseado no dado rolado
    (string nome, string descricao) GetUpgradeDetails(int value)
    {
        switch (value)
        {
            case 1: 
                return ("Anticorpos Avançados", "Estimula a produção de proteínas de defesa.\nEfeito: +Velocidade de Ataque (Atira mais rápido)");
            case 2: 
                return ("Quimioterapia de Impacto", "Bloqueia o crescimento celular desordenado.\nEfeito: ++Grande Aumento de Dano Base");
            case 3:
                return ("Propulsão Bio-Cinética", "Efeito: Aumenta a velocidade de viagem de todos os projéteis (Seus e dos Drones) em +30%.");
            case 4: 
                return ("Células T Aliadas", "Recruta linfócitos auxiliares para o combate.\nEfeito: Spawn de Drones de suporte");
            case 5: 
                return ("Radioterapia Direcionada", "Destrói o DNA mutado com feixes de energia.\nEfeito: Tiro Penetrante Acumulável");
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
        estiloTexto.fontSize = Mathf.RoundToInt(Screen.height * 0.02f); 
        estiloTexto.alignment = TextAnchor.MiddleLeft;

        GUIStyle estiloTitulo = new GUIStyle(GUI.skin.box);
        estiloTitulo.fontSize = Mathf.RoundToInt(Screen.height * 0.025f);
        estiloTitulo.fontStyle = FontStyle.Bold;
        estiloTitulo.alignment = TextAnchor.UpperCenter;

        GUIStyle estiloBotao = new GUIStyle(GUI.skin.button);
        estiloBotao.fontSize = Mathf.RoundToInt(Screen.height * 0.018f);
        estiloBotao.alignment = TextAnchor.MiddleCenter;

        GUIStyle estiloVidaGrande = new GUIStyle(GUI.skin.label);
        estiloVidaGrande.fontSize = Mathf.RoundToInt(Screen.height * 0.035f); 
        estiloVidaGrande.fontStyle = FontStyle.Bold;
        estiloVidaGrande.alignment = TextAnchor.MiddleRight; 
        estiloVidaGrande.normal.textColor = new Color(0.3f, 1f, 0.4f); 

        // =============================================================
        // 1. PRIORIDADE MÁXIMA: TELA DE GAME OVER (BLOQUEIA TUDO POR BAIXO)
        // =============================================================
        if (isGameOver)
        {
            // Criar uma textura preta pura para cobrir a tela inteira
            Texture2D texturaPreta = new Texture2D(1, 1);
            texturaPreta.SetPixel(0, 0, Color.black);
            texturaPreta.Apply();

            GUIStyle estiloFundoPreto = new GUIStyle();
            estiloFundoPreto.normal.background = texturaPreta;

            // Desenha o fundo preto ocupando 100% da largura e altura da tela
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", estiloFundoPreto);

            // Define as dimensões da janela central de derrota
            float goWidth = Screen.width * 0.5f;
            float goHeight = Screen.height * 0.4f;
            if (goWidth < 400f) goWidth = 400f;
            if (goHeight < 250f) goHeight = 250f;

            float posX = (Screen.width - goWidth) / 2f;
            float posY = (Screen.height - goHeight) / 2f;

            // Estilo personalizado para o título em vermelho vivo destacado no preto
            GUIStyle estiloGameOver = new GUIStyle(GUI.skin.box);
            estiloGameOver.fontSize = Mathf.RoundToInt(Screen.height * 0.04f); // Ligeiramente maior
            estiloGameOver.fontStyle = FontStyle.Bold;
            estiloGameOver.alignment = TextAnchor.UpperCenter;
            estiloGameOver.normal.textColor = Color.red;

            // Desenha a caixa de texto da derrota centralizada
            GUI.Box(new Rect(posX, posY, goWidth, goHeight), "--- GAME OVER ---", estiloGameOver);

            // Mensagem explicativa
            GUIStyle estiloTextoGO = new GUIStyle(GUI.skin.label);
            estiloTextoGO.fontSize = Mathf.RoundToInt(Screen.height * 0.022f);
            estiloTextoGO.alignment = TextAnchor.MiddleCenter;
            estiloTextoGO.normal.textColor = Color.white; // Garante que o texto fique branco no fundo preto
            
            GUI.Label(new Rect(posX + 20f, posY + 70f, goWidth - 40f, 40f), 
                "O tumor se espalhou e as defesas falharam.", estiloTextoGO);

            // Botão para Reiniciar a Partida
            float btnWidth = goWidth * 0.6f;
            float btnHeight = 45f;
            float btnX = posX + (goWidth - btnWidth) / 2f;
            float btnY = posY + goHeight - 70f;

            GUIStyle estiloBtnGO = new GUIStyle(GUI.skin.button);
            estiloBtnGO.fontSize = Mathf.RoundToInt(Screen.height * 0.02f);

            if (GUI.Button(new Rect(btnX, btnY, btnWidth, btnHeight), "Tentar Novamente (Reiniciar)", estiloBtnGO))
            {
                Time.timeScale = 1f; 
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            }

            // O return impede que o Unity execute qualquer código abaixo se o jogador perdeu!
            return; 
        }

        // =============================================================
        // 2. PRIORIDADE SECUNDÁRIA: SELEÇÃO DE TRATAMENTO (DRAFT)
        // =============================================================
        if (isDrafting)
        {
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
            float hudWidth = Screen.width * 0.2f; 
            if (hudWidth < 180f) hudWidth = 180f; 
            
            float hudHeight = 55f;
            float hudX = 15f; 
            float hudY = 15f;

            GUI.Box(new Rect(hudX, hudY, hudWidth, hudHeight), "Célula de Defesa", estiloTitulo);
            GUI.Label(new Rect(hudX + 10f, hudY + 30f, hudWidth - 20f, 20f), $"Nível: {currentLevel}  |  XP: {currentXP} / {xpToNextLevel}", estiloTexto);

            PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
            string textoVida = "Vidas: 0";
            
            if (playerHealth != null)
            {
                textoVida = $"Vidas: {playerHealth.totalLives}";
            }

            float vidaWidth = 350f;
            float vidaHeight = 50f;
            float vidaX = Screen.width - vidaWidth - 20f;
            float vidaY = 15f; 

            GUI.Label(new Rect(vidaX, vidaY, vidaWidth, vidaHeight), textoVida, estiloVidaGrande);
        }
    }

    [Header("PREFABS DE UPGRADE")]
    public GameObject dronePrefab; // Arraste seu prefab de drone aqui no GameManager

    void ApplyUpgrade(int upgradeEscolhido)
    {
        Debug.Log($"Aplicando efeitos reais do Dado número {upgradeEscolhido}...");

        // Localiza os componentes do Player necessários para aplicar as mudanças
        PlayerController playerCtrl = FindAnyObjectByType<PlayerController>();
        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();

        if (playerCtrl != null)
        {
            switch (upgradeEscolhido)
            {
                case 1: // Anticorpos Avançados (+Velocidade de Ataque)
                    // Reduz o tempo de espera entre os tiros em 15% 
                    playerCtrl.fireCooldown *= 0.85f; 
                    
                    if (playerCtrl.fireCooldown < 0.05f) 
                    {
                        playerCtrl.fireCooldown = 0.05f; 
                    }
                    
                    Debug.Log($"Upgrade aplicado: Tiro Acelerado! Novo Cooldown: {playerCtrl.fireCooldown:F3}s");
                    break;

                case 2: // Quimioterapia de Impacto (Super Dano Puro)
                    playerCtrl.bulletDamage += 2;
                    Debug.Log($"Upgrade aplicado: Quimioterapia! Novo Dano Base: {playerCtrl.bulletDamage}");
                    break;

                case 3: // Propulsão Bio-Cinética (NOVO: Velocidade do Projétil)
                    // Multiplica a velocidade de viagem da bala por 1.3 (+30%)
                    playerCtrl.bulletForce *= 1.3f;
                    
                    Debug.Log($"Upgrade aplicado: Velocidade do projétil aumentada para {playerCtrl.bulletForce:F1}");
                    break;

                case 4: // Células T Aliadas (Spawn Drone de Suporte)
                    if (dronePrefab != null && playerCtrl != null && playerCtrl.droneOrbitPoint != null)
                    {
                        Transform orbita = playerCtrl.droneOrbitPoint;
                        Vector3 posicaoSpawn = orbita.position + orbita.forward * 2f;
                        
                        GameObject novoDrone = Instantiate(dronePrefab, posicaoSpawn, Quaternion.identity);
                        DroneSuporte droneScript = novoDrone.GetComponent<DroneSuporte>();
                        
                        if (droneScript != null)
                        {
                            // Garante que o drone use o mesmo projétil do jogador
                            droneScript.bulletPrefab = playerCtrl.bulletPrefab;
                            
                            // O drone herda a velocidade de tiro atualizada do jogador!
                            droneScript.bulletForce = playerCtrl.bulletForce;
                            
                            // Inicializa passando o ponto de órbita correto do player
                            droneScript.Inicializar(orbita, playerCtrl.bulletPrefab);
                        }
                    }
                    break;

                case 5: // Radioterapia Direcionada (Tiro Penetrante Acumulável)
                    // Aumenta em +1 o número de inimigos que a bala consegue perfurar antes de sumir
                    playerCtrl.maxPenetrationCount += 1; 
                    playerCtrl.moveSpeed *= 1.05f; // Bônus leve de velocidade de movimento (+5%)
                    
                    Debug.Log($"Upgrade aplicado: Radioterapia Nível {playerCtrl.maxPenetrationCount}! Tiros agora atravessam {playerCtrl.maxPenetrationCount} alvos.");
                    break;

                case 6: // Memória Imune (+Velocidade de Movimento)
                    playerCtrl.moveSpeed += 1.5f; 
                    Debug.Log($"Upgrade aplicado: Velocidade de movimento aumentada para {playerCtrl.moveSpeed}");
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
        AudioListener.pause = true;
    }

    void IniciarFaseChefe()
    {
        isBossPhase = true;
        Debug.Log("ALERTA: Nível 10 atingido! Parando spawns ordinários e invocando o Tumor Primário (Chefão)!");

        // Para a música atual e inicia a música do Chefão
        if (musicaAudioSource != null)
        {
            musicaAudioSource.Stop();
            if (musicaChefao != null)
            {
                musicaAudioSource.clip = musicaChefao;
                musicaAudioSource.Play();
            }
        }

        // Limpa os inimigos comuns remanescentes da arena
        GameObject[] inimigosComuns = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject inimigo in inimigosComuns)
        {
            Destroy(inimigo);
        }

        // Posição padrão caso o script não encontre o jogador na cena
        Vector3 posicaoSpawnChefe = new Vector3(0, 1, 0); 

        PlayerController jogadorAtivo = FindAnyObjectByType<PlayerController>();

        if (jogadorAtivo != null)
        {
            // Sorteia uma direção aleatória em 2D (X e Z) ao redor do jogador
            Vector3 direcaoAleatoria = Random.onUnitCircle.normalized; 
            
            // Calcula o deslocamento afastando o chefe por 12 metros de distância segura
            Vector3 deslocamento = new Vector3(direcaoAleatoria.x, 0f, direcaoAleatoria.y) * 12f; 

            // Define a posição final do spawn (Posição do Player + Distância Segura)
            posicaoSpawnChefe = jogadorAtivo.transform.position + deslocamento;
            posicaoSpawnChefe.y = 1f; // Mantém o chefe alinhado na altura correta do chão
        }
        else
        {
            Debug.LogWarning("GameManager não conseguiu encontrar o objeto do Player para calcular o spawn seguro do Chefe!");
        }

        // Instancia o Chefão na posição segura calculada
        if (bossPrefab != null)
        {
            Instantiate(bossPrefab, posicaoSpawnChefe, Quaternion.identity);
        }
    }

    public void MarcarVitoria()
    {
        isVictory = true;
        Time.timeScale = 0f; // Congela o jogo na vitória
        Debug.Log("VITÓRIA! O Tumor Primário foi erradicado e o organismo está salvo!");
    }
}