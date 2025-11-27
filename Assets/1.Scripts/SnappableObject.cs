using UnityEngine;

public class SnappableObject : MonoBehaviour
{
    [SerializeField] private bool isSnapEnabled = true;
    [SerializeField] private float snapDistance = 0.20f;
    
    // Cacheamos el Grid para no buscarlo siempre
    private DomeGridRenderer grid;
    private Transform myTransform;
    
    // Para optimizar: Solo calculamos si nos hemos movido lo suficiente
    private Vector3 lastPosition;
    private bool isBeingManipulated = false; // Activaremos esto cuando el usuario lo agarre

    public void SetManipulationState(bool state)
    {
        isBeingManipulated = state;
    }

    void Start()
    {
        myTransform = transform;
        lastPosition = myTransform.position;
        
        // Optimización: Buscar el grid solo una vez al inicio
        grid = FindFirstObjectByType<DomeGridRenderer>();
        
        if (grid == null) {
            Debug.LogError("No se encontró DomeGridRenderer en la escena!");
        }
    }

    void Update()
    {
        // Si no está habilitado, no hay grid, o NO se está moviendo/editando, no gastamos recursos
        if(!isSnapEnabled || grid == null) return;

        // Pequeña optimización: Si el objeto no se ha movido significativamente, no recalculamos snap
        if (Vector3.SqrMagnitude(myTransform.position - lastPosition) < 0.001f && !isBeingManipulated) return;

        SnapToGrid();
        lastPosition = myTransform.position;
    }

    private void SnapToGrid()
    {
        Vector3 closest = Vector3.zero;
        float closestDistSqr = Mathf.Infinity;
        float snapDistSqr = snapDistance * snapDistance; // Comparar cuadrados es más rápido

        // Iteramos los puntos. 
        // NOTA: Si SnapPoints tiene miles de puntos, esto seguirá siendo pesado. 
        // Idealmente en el futuro usaremos un KDTree, pero por ahora esto sirve.
        foreach(var point in grid.SnapPoints)
        {
            // Usamos sqrMagnitude para evitar raíces cuadradas costosas
            float distSqr = (myTransform.position - point).sqrMagnitude;

            if(distSqr < closestDistSqr)
            {
                closest = point;
                closestDistSqr = distSqr;
            }
        }

        // Si estamos dentro del rango, hacemos snap
        if(closestDistSqr < snapDistSqr)
        {
            myTransform.position = closest;
            // Orientamos el objeto hacia el centro (o desde el centro hacia afuera)
            myTransform.forward = (myTransform.position - grid.GetCenterPos()).normalized;
        }
    }
}