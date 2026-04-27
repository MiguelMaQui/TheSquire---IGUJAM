using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    
    [Header("Visuales")]
    public SpriteRenderer visualRenderer;

    [Header("Salud y UI")]
    public int health = 3;
    public Image[] heartImages;
    public Sprite fullHeart, emptyHeart;
    private Vector3[] originalHeartPositions;

    [Header("Tirachinas")]
    public bool hasSlingshot = false;
    public GameObject projectilePrefab;
    [SerializeField] GameObject Menu;

    [Header("Sonidos")]
    public AudioSource audioSource; 
    public AudioClip shootSound, damageSound;

    void Start()
    {
        Time.timeScale = 1f;
        rb = GetComponent<Rigidbody2D>();
        if (visualRenderer == null) visualRenderer = GetComponentInChildren<SpriteRenderer>();
        
        originalHeartPositions = new Vector3[heartImages.Length];
        for (int i = 0; i < heartImages.Length; i++)
            originalHeartPositions[i] = heartImages[i].transform.localPosition;

        UpdateHealthUI(); 
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        if (moveInput.sqrMagnitude > 1) moveInput.Normalize();

        // El player siempre hace flip lateral
        if (moveInput.x != 0) visualRenderer.flipX = (moveInput.x < 0);

        if (hasSlingshot && Input.GetMouseButtonDown(0)) Shoot();
        if (Input.GetKeyDown(KeyCode.Escape)) ToggleMenu();
    }

    void FixedUpdate() { rb.linearVelocity = moveInput * moveSpeed; }

    public void TakeDamage(int damage)
    {
        health -= damage;
        UpdateHealthUI();
        if (audioSource != null && damageSound != null) audioSource.PlayOneShot(damageSound);
        StartCoroutine(FlashRed());
        StartCoroutine(ShakeHearts());
        if (health <= 0) StartCoroutine(Die());
    }

    void UpdateHealthUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
            heartImages[i].sprite = (i < health) ? fullHeart : emptyHeart;
    }

    void Shoot()
    {
        if (audioSource != null && shootSound != null) audioSource.PlayOneShot(shootSound);
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 direction = (mousePos - transform.position).normalized;
        GameObject bullet = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * 12f;
    }

    void ToggleMenu()
    {
        Time.timeScale = (Time.timeScale == 1) ? 0 : 1;
        if(Menu != null) Menu.SetActive(Time.timeScale == 0);
    }

    IEnumerator ShakeHearts()
    {
        float elapsed = 0f;
        while (elapsed < 0.2f)
        {
            for (int i = 0; i < heartImages.Length; i++)
            {
                float x = Random.Range(-1f, 1f) * 5f;
                float y = Random.Range(-1f, 1f) * 5f;
                heartImages[i].transform.localPosition = originalHeartPositions[i] + new Vector3(x, y, 0);
            }
            yield return null;
            elapsed += Time.deltaTime;
        }
        for (int i = 0; i < heartImages.Length; i++)
            heartImages[i].transform.localPosition = originalHeartPositions[i];
    }

    IEnumerator FlashRed()
    {
        visualRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        visualRenderer.color = Color.white;
    }

    IEnumerator Die()
    {
        this.enabled = false;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}