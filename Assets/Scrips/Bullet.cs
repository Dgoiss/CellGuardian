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

    private void OnTriggerEnter(Collider other)
    {
        // Se bater em uma parede ou limite do cenário, destrói a bala imediatamente
        if (other.CompareTag("Parede") || other.CompareTag("Obstaculo"))
        {
            Destroy(gameObject);
            return;
        }

        // Se bater em um inimigo (comum ou chefe)
        if (other.CompareTag("Enemy"))
        {
            // 1. Aplica o dano direto
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            else
            {
                BossHealth boss = other.GetComponent<BossHealth>();
                if (boss != null) boss.TakeDamage(damage);
            }

            if (penetrationRemaining <= 0)
            {
                Destroy(gameObject); 
            }
            else
            {
                // Se tiver o upgrade, consome 1 carga e continua atravessando
                penetrationRemaining--;
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