using UnityEngine;

public class SphereDeleter : MonoBehaviour
{
    public Transform playerCamera; // Assign the player's camera in the Inspector
    public LayerMask sphereLayer;  // Assign the "Spheres" layer in the Inspector
    public float deleteRange = 10f;  // Max distance for deleting spheres
    public AudioSource deleteSound;  // Assign the delete sound in the Inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Press Q to delete
        {
            DeleteSphere();
            Debug.DrawRay(playerCamera.position, playerCamera.forward * deleteRange, Color.red);
        }
    }

    void DeleteSphere()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, deleteRange, sphereLayer))
        {
            // Get the root object of the hit collider (in case collider is on a child)
            GameObject target = hit.collider.transform.root.gameObject;
            if (target.CompareTag("Sphere"))
            {
                Debug.Log("Deleting sphere: " + target.name);

                // Find all Bond objects in the scene using the new API
                Bond[] bonds = Object.FindObjectsByType<Bond>(FindObjectsSortMode.None);
                foreach (Bond b in bonds)
                {
                    // If the bond connects to the target sphere on sphereA:
                    if (b.sphereA == target)
                    {
                        SphereBondController otherSBC = b.sphereB.GetComponent<SphereBondController>();
                        if (otherSBC != null)
                        {
                            otherSBC.bondOccupied[b.nodeIndexB] = false;
                            otherSBC.bondCount = Mathf.Max(0, otherSBC.bondCount - 1);
                        }
                        Destroy(b.gameObject);
                    }
                    // Or if the bond connects to the target sphere on sphereB:
                    else if (b.sphereB == target)
                    {
                        SphereBondController otherSBC = b.sphereA.GetComponent<SphereBondController>();
                        if (otherSBC != null)
                        {
                            otherSBC.bondOccupied[b.nodeIndexA] = false;
                            otherSBC.bondCount = Mathf.Max(0, otherSBC.bondCount - 1);
                        }
                        Destroy(b.gameObject);
                    }
                }

                // Remove the target from MolecularBuilder if needed:
                MolecularBuilder mb = Object.FindAnyObjectByType<MolecularBuilder>();
                if (mb != null)
                {
                    mb.RemoveSphere(target); // Implement this method if you want to clean up the list.
                }

                if (deleteSound != null)
                {
                    deleteSound.Play();
                }
                // Finally, delete the sphere.
                Destroy(target);
            }
            else
            {
                Debug.Log("Hit object is not tagged 'Sphere'.");
            }
        }
        else
        {
            Debug.Log("No object hit by raycast.");
        }
    }
}
