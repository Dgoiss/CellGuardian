using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("CONFIGURAÇÃO DA CENA")]
    [Tooltip("Digite o nome EXATO da cena do seu jogo principal")]
    public string nomeDaCenaJogo = "SampleScene"; 

    public void IniciarJogo()
    {
        // Certifica-se de que o tempo do jogo está normalizado
        Time.timeScale = 1f; 
        
        // Carrega a cena do gameplay
        SceneManager.LoadScene(nomeDaCenaJogo);
    }
}