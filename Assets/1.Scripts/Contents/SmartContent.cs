using UnityEngine;
using TMPro;
using UnityEngine.Video;

public class SmartContent : MonoBehaviour
{
    [Header("Configuración del Tipo")]
    [Tooltip("Elige qué tipo de contenido es este prefab")]
    [SerializeField] private ContentType type;

    [Header("Referencias (Asigna según el tipo)")]
    [SerializeField] private TextMeshPro textComponent; // Para textos 3D
    [SerializeField] private VideoPlayer videoPlayer;   // Para videos
    [SerializeField] private Renderer screenRenderer;   // La pantalla del video

    public enum ContentType { Text, Video, Image }

    /// <summary>
    /// Esta función es llamada por el Manager justo después de crear el objeto
    /// </summary>
    /// <param name="data">El texto a mostrar o el nombre del archivo de video</param>
    public void Initialize(string data)
    {
        switch (type)
        {
            case ContentType.Text:
                if (textComponent != null) 
                    textComponent.text = data;
                break;

            case ContentType.Video:
                if (videoPlayer != null)
                {
                    // Opción A: Cargar desde Resources (Más fácil para el concurso)
                    // Debes poner tus videos en una carpeta llamada "Resources/Videos"
                    VideoClip clip = Resources.Load<VideoClip>("Videos/" + data);
                    
                    if (clip != null)
                    {
                        videoPlayer.clip = clip;
                        videoPlayer.Play();
                    }
                    else
                    {
                        Debug.LogError($"No se encontró el video: Videos/{data}");
                    }
                }
                break;
        }
    }
}