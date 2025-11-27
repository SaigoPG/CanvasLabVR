using UnityEngine;

public class ConsoleUIManager : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el GameObject del Canvas (o el panel padre) que quieres controlar")]
    [SerializeField] private GameObject uiCanvasObject;

    [Header("Configuración")]
    [Tooltip("Si marcas esto, el UI se ocultará automáticamente al iniciar el juego")]
    [SerializeField] private bool hideOnStart = true;

    [Tooltip("Si es verdadero, el Canvas rotará para mirar a la cámara principal al abrirse")]
    [SerializeField] private bool faceUserOnOpen = true;

    private void Start()
    {
        // Validación de seguridad
        if (uiCanvasObject == null)
        {
            Debug.LogError($"Falta asignar el Canvas en el script ConsoleUIManager de {gameObject.name}");
            return;
        }

        // Estado inicial
        if (hideOnStart)
        {
            uiCanvasObject.SetActive(false);
        }
    }

    /// <summary>
    /// Esta es la función que llamará tu botón.
    /// Si está apagado lo prende, si está prendido lo apaga.
    /// </summary>
    public void ToggleVisibility()
    {
        if (uiCanvasObject != null)
        {
            // Invertimos el estado actual ( !true = false, !false = true )
            bool newState = !uiCanvasObject.activeSelf;
            uiCanvasObject.SetActive(newState);
            
            // Si acabamos de abrirlo y la opción está activa, miramos al usuario
            if (newState && faceUserOnOpen)
            {
                OrientToUser();
            }
        }
    }

    private void OrientToUser()
    {
        if (Camera.main != null)
        {
            // Hacemos que el canvas mire a la cámara principal
            uiCanvasObject.transform.LookAt(Camera.main.transform);
            
            // Los Canvas en World Space suelen mirar "hacia atrás", así que rotamos 180 grados en Y
            // para que el texto quede de frente al usuario.
            uiCanvasObject.transform.Rotate(0, 180, 0);
            
            // Opcional: Si quieres que solo rote en el eje Y (para que no se incline hacia arriba/abajo)
            // puedes resetear la rotación en X y Z aquí.
            Vector3 euler = uiCanvasObject.transform.eulerAngles;
            uiCanvasObject.transform.eulerAngles = new Vector3(0, euler.y, 0);
        }
    }
}