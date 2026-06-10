using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    
    [Tooltip("Selecione a Layer correspondente ao Chão aqui no Inspector")]
    public LayerMask groundLayer; 

    private Vector3 targetPosition;
    private bool isMoving = false;

    [Header("UPGRADES ATIVOS")]
    public float fireCooldown = 0.5f; // Tempo mínimo entre um tiro e outro
    private float nextFireTime = 0f;

    [Tooltip("Arraste um objeto vazio posicionado acima do player para os drones orbitarem")]
    public Transform droneOrbitPoint;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        // POINT AND CLICK: Botão direito define o destino
        if (Input.GetMouseButtonDown(1)) 
        {
            SetTargetPosition();
        }

        // SHOOTING: Botão esquerdo atira
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireCooldown; // Define quando poderá atirar de novo
        }

        Move();
        LookAtMouse();
    }

    void SetTargetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        // Linha visual vermelha no editor para ver onde o raio está batendo
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            isMoving = true;
            Debug.Log("Chão detectado! Alvo definido para: " + targetPosition);
        }
        else
        {
            Debug.LogWarning("O clique falhou! O raio não colidiu com a Layer selecionada.");
        }
    }

    void Move()
    {
        if (isMoving)
        {
            float distance = Vector3.Distance(transform.position, targetPosition);
            
            if (distance > 0.2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                isMoving = false;
            }
        }
    }

    void LookAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 targetLook = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            
            // Evita erro caso o mouse esteja exatamente em cima do player
            if (Vector3.Distance(transform.position, targetLook) > 0.1f)
            {
                transform.LookAt(targetLook);
            }
        }
    }

    void Shoot()
    {
        if (firePoint == null || bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
        }

        // ADICIONE ESTA LINHA AQUI:
        if (GameManager.instance != null) GameManager.instance.PlaySomTiro();
    }
}