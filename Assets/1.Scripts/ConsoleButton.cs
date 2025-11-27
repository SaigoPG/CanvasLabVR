using UnityEngine;
// Importante para usar los eventos de Meta si los necesitas directos, 
// pero usaremos UnityEvents estándares para simplicidad.

public class ConsoleButton : MonoBehaviour
{
    [Tooltip("El nombre exacto del prefab que este botón va a crear")]
    [SerializeField] private string contentIDToSpawn = "Content_Cube";

    // Esta función la llamaremos desde el evento del botón de Meta
    public void OnButtonPressed()
    {
        PresentationManager.Instance.SpawnContent(contentIDToSpawn);
        
        // Aquí podrías agregar sonido de feedback
        // AudioSource.PlayClipAtPoint(clickSound, transform.position);
    }
}