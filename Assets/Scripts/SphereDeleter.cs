/**
 * 
 */

using UnityEngine;

public class SphereDeleter : MonoBehaviour
{
    public Transform playerCamera; // Assign the player’s camera in the Inspector
    public LayerMask sphereLayer; // Assign the "Spheres" layer in the Inspector
    public float deleteRange = 10f; // Max distance for deleting spheres

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Press Q to delete
        {
            DeleteSphere();
        }
    }

    void DeleteSphere()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        // Check if the ray hits a sphere
        if (Physics.Raycast(ray, out hit, deleteRange, sphereLayer))
        {
            Destroy(hit.collider.gameObject); // Destroy the sphere
        }
    }
}
