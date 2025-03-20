using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject spherePrefab; // The sphere prefab
    public Transform playerCamera; // Assign the player’s camera in the Inspector
    public float spawnDistance = 2f; // Distance in front of the camera

    private Color selectedColor = Color.white; // Default color is white
    private GameObject previewSphere; // The preview sphere

    void Start()
    {
        // Create a preview sphere at the start
        previewSphere = Instantiate(spherePrefab);
        previewSphere.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.8f); // Transparent white
        previewSphere.GetComponent<Collider>().enabled = false; // Disable collisions
        Rigidbody rb = previewSphere.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true; // Make sure it doesn't move
    }

    void Update()
    {
        // Update preview sphere position
        Vector3 spawnPosition = playerCamera.position + playerCamera.transform.forward * spawnDistance;
        previewSphere.transform.position = spawnPosition;

        // Change preview sphere color when a number key is pressed
        if (Input.GetKeyDown(KeyCode.Alpha1)) UpdatePreviewColor(Color.red);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UpdatePreviewColor(Color.blue);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UpdatePreviewColor(Color.green);
        if (Input.GetKeyDown(KeyCode.Alpha4)) UpdatePreviewColor(Color.yellow);
        if (Input.GetKeyDown(KeyCode.Alpha5)) UpdatePreviewColor(Color.magenta);
        if (Input.GetKeyDown(KeyCode.Alpha6)) UpdatePreviewColor(Color.cyan);
        if (Input.GetKeyDown(KeyCode.Alpha7)) UpdatePreviewColor(new Color(1f, 0.5f, 0f)); // Orange
        if (Input.GetKeyDown(KeyCode.Alpha8)) UpdatePreviewColor(new Color(0.5f, 0.25f, 0f)); // Brown
        if (Input.GetKeyDown(KeyCode.Alpha9)) UpdatePreviewColor(Color.grey);

        // Spawn sphere when E is pressed
        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnSphere(selectedColor);
        }
    }

    void SpawnSphere(Color color)
    {
        // Instantiate the sphere at the preview position
        GameObject newSphere = Instantiate(spherePrefab, previewSphere.transform.position, Quaternion.identity);

        // Change sphere color dynamically
        Renderer sphereRenderer = newSphere.GetComponent<Renderer>();
        if (sphereRenderer != null)
        {
            sphereRenderer.material.color = color;
        }

        // Assign the sphere to a specific layer to allow deletion
        newSphere.layer = LayerMask.NameToLayer("Spheres");
    }

    void UpdatePreviewColor(Color color)
    {
        selectedColor = color;
        if (previewSphere != null)
        {
            Renderer previewRenderer = previewSphere.GetComponent<Renderer>();
            if (previewRenderer != null)
            {
                previewRenderer.material.color = new Color(color.r, color.g, color.b, 0.8f); // Set transparent color
            }
        }
    }
}
