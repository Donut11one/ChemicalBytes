using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject spherePrefab;
    public Transform playerCamera; // Assign the player’s camera in the Inspector
    public float spawnDistance = 2f; // Distance in front of the camera

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Calculate the spawn position relative to the player's view direction
            Vector3 spawnPosition = playerCamera.position + playerCamera.transform.forward * spawnDistance;

            // Instantiate the sphere at the calculated position with the default rotation
            Instantiate(spherePrefab, spawnPosition, Quaternion.identity);
        }
    }
}
