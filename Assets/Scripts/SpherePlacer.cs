using UnityEngine;

public class SpherePlacer : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject spherePrefab;    // Your sphere prefab (ensure it has a collider and is tagged "Sphere")
    public GameObject bondPrefab;      // A cylinder prefab for the bond
    public GameObject previewPrefab;   // A semi-transparent preview sphere prefab

    [Header("Placement Settings")]
    public Transform playerCamera;     // Reference to the player’s camera (or the object used for aiming)
    public float fixedDistance = 2f;   // Fixed distance between spheres (and bond length)
    public float activationRange = 5f; // How close the player must be to a candidate position for the preview to show

    // Internal references
    private GameObject currentPreview;
    private GameObject selectedBaseSphere;
    private GameObject lastHighlightedSphere;

    // Element and charge settings
    private string selectedElement = "C"; // Default element is Carbon
    private int selectedCharge = 0;       // Default charge is 0  

    void Start()
    {
        // Check if any sphere exists (tagged "Sphere")
        GameObject[] existingSpheres = GameObject.FindGameObjectsWithTag("Sphere");
        Debug.Log("Number of spheres found: " + existingSpheres.Length);

        if (existingSpheres.Length == 0)
        {
            // Set the starting atom type to Carbon with a charge of 0.
            SetAtomType("C", 0);

            Debug.Log("No spheres found. Spawning initial sphere at (0,0,0).");
            GameObject startingSphere = Instantiate(spherePrefab, Vector3.zero, Quaternion.identity);
            startingSphere.tag = "Sphere";

            // If the starting sphere has an AtomController, update its properties.
            AtomController atomController = startingSphere.GetComponent<AtomController>();
            if (atomController != null)
            {
                atomController.SetAtomProperties("C", 0);
            }

            // Set the starting sphere's color based on Carbon.
            Renderer rend = startingSphere.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = GetElementColor("C");
            }
        }
    }


    void Update()
    {
        UpdatePreview();

        // Change atom type when a number key is pressed
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetAtomType("C", 0); // Carbon
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetAtomType("O", 0); // Oxygen
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetAtomType("N", 0); // Nitrogen
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetAtomType("Na", 1); // Sodium +1
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetAtomType("Cl", -1); // Chlorine -1
        if (Input.GetKeyDown(KeyCode.Alpha6)) SetAtomType("S", 0); // Sulfur
        if (Input.GetKeyDown(KeyCode.Alpha7)) SetAtomType("P", 0); // Phosphorous
        if (Input.GetKeyDown(KeyCode.Alpha8)) SetAtomType("F", 0); // Fluorine

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
    /// Called when the user confirms placement. Instantiates the new sphere,
    /// sets its atom properties and color, and creates a bond.
    /// </summary>
    void PlaceSphere()
    {
        // Instantiate the new sphere at the preview's position.
        GameObject newSphere = Instantiate(spherePrefab, currentPreview.transform.position, Quaternion.identity);
        newSphere.tag = "Sphere"; // Ensure it's tagged for future searches

        // Set atom properties based on the current selection.
        // (Make sure your spherePrefab has an AtomController component if needed.)
        AtomController atomController = newSphere.GetComponent<AtomController>();
        if (atomController != null)
        {
            atomController.SetAtomProperties(selectedElement, selectedCharge);
        }

        // Change the sphere's color based on the selected element.
        Renderer newAtomRenderer = newSphere.GetComponent<Renderer>();
        if (newAtomRenderer != null)
        {
            newAtomRenderer.material.color = GetElementColor(selectedElement);
        }

        // Create the bond between the selected base sphere and the new sphere.
        if (selectedBaseSphere != null && bondPrefab != null)
        {
            CreateBond(selectedBaseSphere.transform.position, newSphere.transform.position);
        }

        // Clear the preview.
        Destroy(currentPreview);
        currentPreview = null;
        UnhighlightLastSphere();
    }

    /// <summary>
    /// Updates the current selected element and charge.
    /// </summary>
    void SetAtomType(string element, int charge)
    {
        selectedElement = element;
        selectedCharge = charge;
    }

    /// <summary>
    /// Instantiates a bond (cylinder) connecting two points.
    /// The cylinder is positioned at the midpoint, aligned along the connecting vector,
    /// and scaled so that its height equals the fixed distance.
    /// </summary>
    void CreateBond(Vector3 startPos, Vector3 endPos)
    {
        Vector3 midPoint = (startPos + endPos) / 2;
        GameObject bond = Instantiate(bondPrefab, midPoint, Quaternion.identity);

        Vector3 direction = endPos - startPos;
        bond.transform.up = direction.normalized;

        Vector3 bondScale = bond.transform.localScale;
        bondScale.y = fixedDistance / 2f; // Assuming default cylinder height is 2 units.
        bond.transform.localScale = bondScale;
    }

    /// <summary>
    /// Highlights the provided sphere to indicate that it is the active base for placement.
    /// This could be done, for example, by modifying its emission color.
    /// </summary>
    void HighlightSphere(GameObject sphere)
    {
        UnhighlightLastSphere();

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
        if (lastHighlightedSphere != null)
        {
            Renderer rend = lastHighlightedSphere.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.SetColor("_EmissionColor", Color.black);
            }
            lastHighlightedSphere = null;
        }
    }

    /// <summary>
    /// Returns the color corresponding to a given element.
    /// </summary>
    private Color GetElementColor(string element)
    {
        switch (element)
        {
            case "C": return new Color(0.5f, 0.5f, 0.5f); // Grey for Carbon
            case "O": return new Color(1f, 0f, 0f);       // Red for Oxygen
            case "N": return new Color(0f, 0f, 1f);       // Blue for Nitrogen
            case "Na": return new Color(1f, 0.5f, 0f);      // Orange for Sodium
            case "Cl": return new Color(0f, 1f, 0f);        // Green for Chlorine
            case "S": return new Color(1f, 1f, 0f);         // Yellow for Sulfur
            case "P": return new Color(1f, 0f, 1f);         // Magenta for Phosphorous
            case "F": return new Color(0f, 1f, 1f);         // Cyan for Fluorine
            default: return Color.white;                   // Default to white
        }
    }
}
