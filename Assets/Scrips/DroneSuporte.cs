using UnityEngine;

public class DroneSuporte : MonoBehaviour
{
    public float orbitSpeed = 120f;     
    public float orbitRadius = 2.0f;    
    public float fireRate = 1.0f;       
    public GameObject bulletPrefab;
    public float bulletForce = 20f;

    private Transform orbitTarget;
    private float nextFireTime;

    public void Inicializar(Transform target, GameObject bullet)
    {
        orbitTarget = target;
        bulletPrefab = bullet;
        
        // 1. GARANTIA ANTI-BUG: Desativa a física bruta para ele não travar no chão
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;  // Avisa a Unity que ele se move por código
            rb.useGravity = false;  // Desativa totalmente a gravidade
        }

        // 2. GARANTIA ANTI-COLISÃO: Faz o colisor virar Trigger (fantasma)
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;   // Atravessa o player e o chão sem colidir
        }

        // 3. Faz o drone virar filho do ponto de órbita do Player
        transform.SetParent(target);
        
        // 4. Sorteia uma direção inicial aleatória baseada no raio de órbita
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 posicaoRelativa = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * orbitRadius;
        
        // 5. Zera a posição local: O Y em 0f força ele a ficar EXATAMENTE na altura do droneOrbitPoint
        transform.localPosition = new Vector3(posicaoRelativa.x, 0f, posicaoRelativa.z);
        
        // Zera rotações herdadas esquisitas
        transform.localRotation = Quaternion.identity; 
    }

    void Update()
    {
        // Se o jogo estiver pausado ou não tiver pai, não faz nada
        if ((GameManager.instance != null && GameManager.instance.isDrafting) || orbitTarget == null) return;

        // ====== MOVIMENTO DE ÓRBITA VIA HIERARQUIA ======
        // Como ele já se move junto com o player por ser filho, só precisamos girar ao redor dele
        transform.RotateAround(orbitTarget.position, Vector3.up, orbitSpeed * Time.deltaTime);

        // ====== SISTEMA DE TIRO AUTOMÁTICO ======
        if (Time.time >= nextFireTime)
        {
            AtirarNoInimigoProximo();
        }
    }

    void AtirarNoInimigoProximo()
    {
        GameObject[] inimigos = GameObject.FindGameObjectsWithTag("Enemy");
        if (inimigos.Length == 0) return;

        GameObject maisProximo = null;
        float menorDistancia = Mathf.Infinity;

        foreach (GameObject inimigo in inimigos)
        {
            float distancia = Vector3.Distance(transform.position, inimigo.transform.position);
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                maisProximo = inimigo;
            }
        }

        if (maisProximo != null && menorDistancia < 15f)
        {
            transform.LookAt(maisProximo.transform);

            Vector3 posicaoTiro = transform.position + transform.forward * 0.6f;
            GameObject bullet = Instantiate(bulletPrefab, posicaoTiro, transform.rotation);
            
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(transform.forward * bulletForce, ForceMode.Impulse);
            }
            
            nextFireTime = Time.time + fireRate;
        }
    }
}