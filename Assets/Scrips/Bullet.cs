using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f; 
    public float speed = 20f;   
    
    [HideInInspector] public int damage = 1;       
    [HideInInspector] public int penetrationRemaining = 0; 
    [HideInInspector] public float raioDaExplosao = 0f; // Definido pelo PlayerController

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            Vector3 proximaPosicao = transform.position + transform.forward * speed * Time.fixedDeltaTime;
            rb.MovePosition(proximaPosicao);
        }
    }

    void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Enemy"))
        {
            // Se o jogador já liberou o upgrade (raio maior que zero), causa a explosão em área!
            if (raioDaExplosao > 0f)
            {
                ExecutarDanoEmArea(other.transform.position);
            }
            else
            {
                // Se não for em área, tenta dar dano no inimigo comum
                EnemyHealth enemy = other.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
                else
                {
                    // NOVO: Se não for um inimigo comum, verifica se é o Chefão!
                    BossHealth boss = other.GetComponent<BossHealth>();
                    if (boss != null) boss.TakeDamage(damage);
                }
            }

            // Lógica de penetração acumulável (Dado 5)
            if (penetrationRemaining > 0)
            {
                penetrationRemaining--;
            }
            else
            {
                Destroy(gameObject); 
            }
        }
    }

    public AudioClip somExplosaoArea;

    void ExecutarDanoEmArea(Vector3 pontoImpacto)
    {
        AudioSource.PlayClipAtPoint(somExplosaoArea, pontoImpacto);

        Collider[] objetosAtingidos = Physics.OverlapSphere(pontoImpacto, raioDaExplosao);

        foreach (Collider col in objetosAtingidos)
        {
            if (col.CompareTag("Enemy"))
            {
                EnemyHealth inimigoNoRaio = col.GetComponent<EnemyHealth>();
                if (inimigoNoRaio != null)
                {
                    inimigoNoRaio.TakeDamage(damage);
                    Debug.Log($"Dano em área atingiu: {col.name} causando {damage} de dano.");
                }
                else
                {
                    
                    BossHealth bossNoRaio = col.GetComponent<BossHealth>();
                    if (bossNoRaio != null)
                    {
                        bossNoRaio.TakeDamage(damage);
                        Debug.Log($"Dano em área atingiu o Chefão causando {damage} de dano!");
                    }
                }
            }
        }
    }

    // Desenha o círculo do dano em área no modo de edição da Unity para testes visuais
    void OnDrawGizmosSelected()
    {
        if (raioDaExplosao > 0f)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, raioDaExplosao);
        }
    }
}