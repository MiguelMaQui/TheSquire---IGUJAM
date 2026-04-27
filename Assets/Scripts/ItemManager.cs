using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    [Header("Configuración UI")]
    public TextMeshProUGUI counterText;
    public Image[] slotIcons;
    public Sprite emptySlotSprite;

    [Header("Ajustes de Inventario")]
    public int maxSlots = 3; 
    public float speedReductionPerItem = 0.5f;
    public float dropDistance = 1.2f; 
    public List<GameObject> allItemPrefabs; 

    [Header("Detección de Paredes")]
    public LayerMask wallLayer; 

    [Header("Sonidos")]
    public AudioSource audioSource;      
    public AudioClip pickupItemSound;    
    public AudioClip pickupWeaponSound;  
    public AudioClip dropItemSound;      

    private List<GameObject> inventory = new List<GameObject>();
    private PlayerControl PlayerControl; 
    private float baseSpeed;
    private Vector2 lastFacingDirection = Vector2.down; 

    void Start()
    {
        PlayerControl = GetComponent<PlayerControl>();
        baseSpeed = PlayerControl.moveSpeed;
        UpdateUI();
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.sqrMagnitude > 0) lastFacingDirection = input.normalized;

        if (Input.GetKeyDown(KeyCode.R)) DropItem();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Caso Tirachinas (Weapon)
        if (other.CompareTag("Weapon"))
        {
            if (audioSource != null && pickupWeaponSound != null)
                audioSource.PlayOneShot(pickupWeaponSound);

            PlayerControl.hasSlingshot = true; 
            Destroy(other.gameObject);
            return; 
        }

        // Caso Item normal
        if (other.CompareTag("Item"))
        {
            if (inventory.Count >= maxSlots)
            {
                Debug.Log("¡Inventario lleno!");
                return; 
            }

            // LIMPIEZA AVANZADA DE NOMBRE
            string nameInScene = other.gameObject.name;
            
            // 1. Quitar (Clone)
            if (nameInScene.Contains("(Clone)")) nameInScene = nameInScene.Replace("(Clone)", "");
            
            // 2. Quitar espacios y números de copia como " (1)", " (2)"
            if (nameInScene.Contains(" (")) {
                nameInScene = nameInScene.Split(" (")[0];
            }
            
            string cleanName = nameInScene.Trim();

            // BUSQUEDA INSENSIBLE A MAYÚSCULAS
            GameObject foundPrefab = allItemPrefabs.Find(p => p.name.Equals(cleanName, System.StringComparison.OrdinalIgnoreCase));

            if (foundPrefab != null)
            {
                if (audioSource != null && pickupItemSound != null)
                    audioSource.PlayOneShot(pickupItemSound);

                inventory.Insert(0, foundPrefab); 
                Destroy(other.gameObject);
                ApplySpeedChange();
                UpdateUI();
            }
            else 
            {
                // Este mensaje te dirá exactamente qué nombre está buscando y no encuentra
                Debug.LogWarning("No se encontró el prefab para: '" + cleanName + "'. Asegúrate de que el nombre coincide con el Prefab en la lista All Item Prefabs.");
            }
        }
    }

    void DropItem()
    {
        if (inventory.Count > 0)
        {
            if (audioSource != null && dropItemSound != null)
                audioSource.PlayOneShot(dropItemSound);

            GameObject prefabToDrop = inventory[0];
            inventory.RemoveAt(0);

            // Calculamos posición de soltado
            Vector3 finalPos = transform.position + (Vector3)(lastFacingDirection * dropDistance);
            
            // Raycast simple para no soltar objetos dentro de paredes
            RaycastHit2D hit = Physics2D.Raycast(transform.position, lastFacingDirection, dropDistance, wallLayer);
            if (hit.collider != null) finalPos = (Vector3)hit.point - (Vector3)(lastFacingDirection * 0.2f);

            Instantiate(prefabToDrop, finalPos, Quaternion.identity);

            ApplySpeedChange();
            UpdateUI();
        }
    }

    public bool TryUseItem(string itemName)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].name.Equals(itemName, System.StringComparison.OrdinalIgnoreCase))
            {
                inventory.RemoveAt(i); 
                ApplySpeedChange();
                UpdateUI();
                return true; 
            }
        }
        return false; 
    }

    void ApplySpeedChange()
    {
        float newSpeed = baseSpeed - (inventory.Count * speedReductionPerItem);
        PlayerControl.moveSpeed = Mathf.Max(newSpeed, 1.5f);
    }

    void UpdateUI()
    {
        if (counterText != null) counterText.text = "Items: " + inventory.Count + "/" + maxSlots;

        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (i < inventory.Count)
            {
                SpriteRenderer childRenderer = inventory[i].GetComponentInChildren<SpriteRenderer>();
                if (childRenderer != null)
                {
                    slotIcons[i].sprite = childRenderer.sprite;
                    slotIcons[i].color = Color.white;
                }
            }
            else
            {
                if (emptySlotSprite != null)
                {
                    slotIcons[i].sprite = emptySlotSprite;
                    slotIcons[i].color = Color.white;
                }
                else
                {
                    slotIcons[i].color = new Color(1, 1, 1, 0); 
                }
            }
        }
    }
}