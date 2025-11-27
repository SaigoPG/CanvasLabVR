using UnityEngine;
using System.Collections.Generic;

public class PresentationManager : MonoBehaviour
{
    public static PresentationManager Instance { get; private set; }

    [Header("Content Database")]
    // Aquí arrastraremos nuestros prefabs (Cubo, Texto, Video, etc.)
    // La clave (string) será el ID, ejemplo: "Cube", "VideoScreen"
    [SerializeField] private List<GameObject> contentPrefabs; 
    
    // Diccionario para búsqueda rápida
    private Dictionary<string, GameObject> prefabLookup = new Dictionary<string, GameObject>();

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint; // Un punto vacío frente al usuario o al domo

    private void Awake()
    {
        // Configuración Singleton robusta
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Llenar el diccionario para acceso rápido
        foreach (var prefab in contentPrefabs)
        {
            if(prefab != null)
            {
                // Usaremos el nombre del prefab como ID por ahora
                if (!prefabLookup.ContainsKey(prefab.name))
                {
                    prefabLookup.Add(prefab.name, prefab);
                }
            }
        }
    }

    /// <summary>
    /// Método público para instanciar elementos en el domo
    /// </summary>
    /// <param name="contentID">El nombre exacto del prefab en la lista</param>
    public void SpawnContent(string contentID)
    {
        if (prefabLookup.TryGetValue(contentID, out GameObject prefabToSpawn))
        {
            // Instanciar
            GameObject newObj = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
            
            // Opcional: Asignarle un padre para mantener la jerarquía limpia
            newObj.transform.SetParent(this.transform); 

            Debug.Log($"[PresentationManager] Spawned: {contentID}");
        }
        else
        {
            Debug.LogError($"[PresentationManager] No se encontró prefab con ID: {contentID}");
        }
    }
}