using UnityEngine;

public class SpherePlacer : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject spherePrefab;    // Your sphere prefab (make sure it has a collider and is tagged "Sphere")
    public GameObject bondPrefab;      // A cylinder prefab for the bond; create a simple cylinder in Unity
    public GameObject previewPrefab;   // A semi-transparent preview sphere prefab (can be a duplicate of your spherePrefab with a special material)

    [Header("Placement Settings")]
    public Transform playerCamera;     // Reference to the player’s camera (or the object used for aiming)
    public float fixedDistance = 2f;   // Fixed distance between spheres (and bond length)
    public float activationRange = 5f; // How close the player must be to a candidate position for the preview to show

    // Internal references
    private GameObject currentPreview;       
    private GameObject selectedBaseSphere;   
    private GameObject lastHighlightedSphere; 



    void Start()
    {
        // Check if any sphere exists (tagged "Sphere")
        GameObject[] existingSpheres = GameObject.FindGameObjectsWithTag("Sphere");
        Debug.Log("Number of spheres found: " + existingSpheres.Length);

        if (existingSpheres.Length == 0)
        {
            Debug.Log("No spheres found. Spawning initial sphere at (0,0,0).");
            GameObject startingSphere = Instantiate(spherePrefab, Vector3.zero, Quaternion.identity);
            startingSphere.tag = "Sphere";  
        }
    }


    void Update()
    {
        UpdatePreview();

        // When the user presses E, place a new sphere if a valid preview exists.
        if (Input.GetKeyDown(KeyCode.E) && currentPreview != null)
        {
            PlaceSphere();
        }
    }

    /// <summary>
    /// Checks for nearby spheres and calculates a candidate placement position.
    /// If a valid candidate is found (closest candidate within activation range),
    /// updates the preview sphere and highlights the base sphere.
    /// </summary>
    void UpdatePreview()
    {
        // Variables to track the best candidate hit.
        float bestT = Mathf.Infinity;
        GameObject bestSphere = null;
        Vector3 bestCandidatePos = Vector3.zero;

        Vector3 rayOrigin = playerCamera.position;
        Vector3 rayDir = playerCamera.forward;

        // Find all base spheres (make sure they are tagged "Sphere").
        GameObject[] spheres = GameObject.FindGameObjectsWithTag("Sphere");
        foreach (GameObject sphere in spheres)
        {
            Vector3 center = sphere.transform.position;
            float R = fixedDistance; // radius of the invisible sphere
            Vector3 L = center - rayOrigin;
            float tca = Vector3.Dot(L, rayDir);

            // If the sphere is behind the camera, skip it.
            if (tca < 0)
                continue;

            // Compute the squared distance from the sphere center to the ray.
            float d2 = Vector3.Dot(L, L) - tca * tca;
            if (d2 > R * R)
                continue; // Ray misses the invisible sphere.

            // Compute the distance from the ray to the intersection point.
            float thc = Mathf.Sqrt(R * R - d2);
            float t0 = tca - thc;
            float t1 = tca + thc;
            float t = t0 >= 0 ? t0 : t1; // choose the smallest positive intersection

            if (t < bestT)
            {
                bestT = t;
                bestSphere = sphere;
                bestCandidatePos = rayOrigin + rayDir * t;
            }
        }

        if (bestSphere != null)
        {
            // Update or create the preview sphere at the candidate position.
            if (currentPreview == null)
            {
                currentPreview = Instantiate(previewPrefab, bestCandidatePos, Quaternion.identity);
            }
            else
            {
                currentPreview.transform.position = bestCandidatePos;
            }
            selectedBaseSphere = bestSphere;
            HighlightSphere(bestSphere);
        }
        else
        {
            // If no candidate is found, remove the preview and clear the highlight.
            if (currentPreview != null)
            {
                Destroy(currentPreview);
            }
            UnhighlightLastSphere();
            selectedBaseSphere = null;
        }
    }


    /// <summary>
    /// Called when the user confirms placement. Instantiates the new sphere and bond.
    /// </summary>
    void PlaceSphere()
    {
        // Instantiate the new sphere at the preview's position.
        GameObject newSphere = Instantiate(spherePrefab, currentPreview.transform.position, Quaternion.identity);
        newSphere.tag = "Sphere"; // Ensure it's tagged for future searches

        // Create the bond between the selected base sphere and the new sphere.
        if (selectedBaseSphere != null && bondPrefab != null)
        {
            CreateBond(selectedBaseSphere.transform.position, newSphere.transform.position);
        }

        // Clear the preview.
        Destroy(currentPreview);
        currentPreview = null;

        // Optionally, remove highlight from the base sphere.
        UnhighlightLastSphere();
    }

    /// <summary>
    /// Instantiates a bond (cylinder) connecting two points.
    /// The cylinder is positioned at the midpoint, aligned along the connecting vector,
    /// and scaled so that its height equals the fixed distance.
    /// </summary>
    /// <param name="startPos">Position of the base sphere.</param>
    /// <param name="endPos">Position of the new sphere.</param>
    void CreateBond(Vector3 startPos, Vector3 endPos)
    {
        // Calculate the midpoint for positioning the bond.
        Vector3 midPoint = (startPos + endPos) / 2;
        GameObject bond = Instantiate(bondPrefab, midPoint, Quaternion.identity);

        // Determine the direction from start to end.
        Vector3 direction = endPos - startPos;
        // Align the bond so its "up" axis points along the direction.
        bond.transform.up = direction.normalized;

        // Assuming your bond prefab is a default cylinder (height 2 units),
        // scale its Y-axis so that its length equals the fixedDistance.
        Vector3 bondScale = bond.transform.localScale;
        bondScale.y = fixedDistance / 2f; // because 2 * (fixedDistance/2) = fixedDistance
        bond.transform.localScale = bondScale;
    }

    /// <summary>
    /// Highlights the provided sphere to indicate that it is the active base for placement.
    /// This could be done, for example, by enabling an outline component or changing its material.
    /// </summary>
    /// <param name="sphere">The sphere to highlight.</param>
    void HighlightSphere(GameObject sphere)
    {
        // Temporarily remove the Outline code until you add an Outline component to your project.
        // var outline = sphere.GetComponent<Outline>();
        // if (outline != null)
        // {
        //     outline.enabled = true;
        // }

        // Alternatively, change the material's emission color to highlight.
        Renderer rend = sphere.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.SetColor("_EmissionColor", Color.yellow);
        }

        lastHighlightedSphere = sphere;
    }


    /// <summary>
    /// Removes the highlight from the last highlighted sphere.
    /// </summary>
    void UnhighlightLastSphere()
    {
        
                Renderer rend = lastHighlightedSphere.GetComponent<Renderer>();
                if (rend != null)
                {
                    // Reset the emission color to black or its default value.
                    rend.material.SetColor("_EmissionColor", Color.black);
                }
           
    }
}
