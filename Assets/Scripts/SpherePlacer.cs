using UnityEngine;

public class SpherePlacer : MonoBehaviour
{ 
    public MolecularBuilder molecularBuilder;
    public BondType currentBondType = BondType.Single;
    public int bondTypeIndex = 0; // Index to cycle through bond types


    [Header("Prefabs")]
    public GameObject spherePrefab;      // Sphere prefab (should have SphereBondController)
    public GameObject bondPrefab;        // Cylinder prefab for visual bonds
    public GameObject previewPrefab;     // Semi‑transparent preview sphere prefab

    [Header("Placement Settings")]
    public Transform playerCamera;       // Reference to the player's camera
    public float fixedDistance = 2f;     // Distance used for bond offsets (must match SphereBondController)
    public float activationRange = 5f;   // Maximum distance from a candidate node to the camera ray
    public float maxSpawnDistance = 10f; // Maximum allowed world-space distance from the camera for spawning

    // Internal references
    private GameObject currentPreview;
    private GameObject selectedBaseSphere;
    // This will store the index of the selected free node on the parent.
    private int selectedNodeIndex = -1;
    private GameObject lastHighlightedSphere;

    void Start()
    {
        if (molecularBuilder == null)
        {
            Debug.LogError("MolecularBuilder is not assigned in the Inspector! Disabling SpherePlacer.");
            return;
        }
        // If no sphere exists, spawn the initial sphere.
        GameObject[] existingSpheres = GameObject.FindGameObjectsWithTag("Sphere");
        Debug.Log("Number of spheres found: " + existingSpheres.Length);

        if (existingSpheres.Length == 0)
        {
            Debug.Log("No spheres found. Spawning initial sphere at (0,0,0).");
            GameObject startingSphere = Instantiate(spherePrefab, Vector3.zero, Quaternion.identity);
            startingSphere.tag = "Sphere";

            string defaultElem = ElementWheelController.Instance.CurrentElement;
            int defaultCharge = ElementWheelController.Instance.CurrentCharge;


            AtomController atomController = startingSphere.GetComponent<AtomController>();
            if (atomController != null)
            {
                atomController.SetAtomProperties(defaultElem, defaultCharge);
            }
                
            Renderer rend = startingSphere.GetComponent<Renderer>();
            if (rend != null)
                rend.material.color = GetElementColor(defaultElem);

            // Add a SphereBondController and initialize bond count to 0.
            SphereBondController sbc = startingSphere.GetComponent<SphereBondController>();
            if (sbc == null)
            {
                sbc = startingSphere.AddComponent<SphereBondController>();
                sbc.fixedDistance = fixedDistance;
            }
            sbc.maxBonds = defaultCharge;
            sbc.bondCount = 0;
            molecularBuilder.AddSphere(startingSphere);
        }
    }

    void Update()
    {
        UpdatePreview();

        if (!PauseMenu.isPaused)
        {
            if (Input.GetKeyDown(KeyCode.E) && currentPreview != null)
            {
                PlaceSphere();
                molecularBuilder.DisplaySMILES();
            }
            // Mouse wheel input to change bond type
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                if (scroll > 0) // Scroll up
                {
                    bondTypeIndex++;
                }
                else // Scroll down
                {
                    bondTypeIndex--;
                }

                // Cycle through bond types
                bondTypeIndex = Mathf.Clamp(bondTypeIndex, 0, System.Enum.GetValues(typeof(BondType)).Length - 1);
                currentBondType = (BondType)bondTypeIndex;

                Debug.Log("Current Bond Type: " + currentBondType); // Optional debug log
            }
        }

    }

    // Returns the perpendicular distance from a point to a ray.
    float DistancePointToRay(Vector3 point, Ray ray)
    {
        return Vector3.Magnitude(Vector3.Cross(ray.direction, point - ray.origin));
    }

    /// <summary>
    /// For every sphere that can accept a new bond (i.e. has free nodes),
    /// iterate over its nodes (as defined in SphereBondController) and find the candidate node
    /// whose world position is closest to the camera ray (and within activationRange)
    /// and within maxSpawnDistance from the camera.
    /// </summary>
    void UpdatePreview()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        float bestDist = Mathf.Infinity;
        GameObject bestSphere = null;
        Vector3 bestCandidatePos = Vector3.zero;
        int bestNodeIndex = -1;

        GameObject[] spheres = GameObject.FindGameObjectsWithTag("Sphere");
        foreach (GameObject sphere in spheres)
        {
            SphereBondController sbc = sphere.GetComponent<SphereBondController>();
            if (sbc == null) continue;
            if (!sbc.HasFreeBond()) continue;

            // Iterate over all 5 nodes.
            for (int i = 0; i < sbc.bondPositions.Length; i++)
            {
                if (sbc.IsBondOccupied(i))
                    continue;
                // Compute candidate node's world position.
                Vector3 candidate = sphere.transform.TransformPoint(sbc.bondPositions[i]);
                // Check if candidate is within the max spawn distance from the camera.
                if (Vector3.Distance(playerCamera.position, candidate) > maxSpawnDistance)
                    continue;

                float dist = DistancePointToRay(candidate, ray);
                if (dist < bestDist && dist <= activationRange)
                {
                    bestDist = dist;
                    bestSphere = sphere;
                    bestCandidatePos = candidate;
                    bestNodeIndex = i;
                }
            }
        }

        if (bestSphere != null && bestNodeIndex != -1)
        {
            if (currentPreview == null)
            {
                currentPreview = Instantiate(previewPrefab, bestCandidatePos, Quaternion.identity);
                // Disable the preview's collider so it doesn't interfere with deletion raycasts.
                Collider col = currentPreview.GetComponent<Collider>();
                if (col != null)
                    col.enabled = false;
            }
            else
            {
                currentPreview.transform.position = bestCandidatePos;
            }
            selectedBaseSphere = bestSphere;
            selectedNodeIndex = bestNodeIndex;
            HighlightSphere(bestSphere);
        }
        else
        {
            if (currentPreview != null)
                Destroy(currentPreview);
            UnhighlightLastSphere();
            selectedBaseSphere = null;
            selectedNodeIndex = -1;
        }
    }

    /// <summary>
    /// Places a new sphere attached to the selected parent's free node.
    /// The parent's selected node (currentPreview position) is used as the attachment point.
    /// The new sphere is placed so that its head node (node 0) touches that point.
    /// The new sphere is rotated so its head faces the parent's node.
    /// Both spheres' bond counters are updated.
    /// </summary>
    void PlaceSphere()
    {
        // Retrieve the SphereBondController from the selected base sphere
        SphereBondController parentSBC = selectedBaseSphere.GetComponent<SphereBondController>();
        if (parentSBC == null || parentSBC.bondCount >= parentSBC.maxBonds)
        {
            Debug.LogWarning("Cannot place sphere: No available bonds on parent sphere.");
            return;
        }

        // Get the parent's node world position (the connection point)
        Vector3 parentCenter = selectedBaseSphere.transform.position;
        Vector3 parentNodePos = selectedBaseSphere.transform.TransformPoint(parentSBC.bondPositions[selectedNodeIndex]);

        // Attempt to mark the selected node as occupied
        if (!parentSBC.OccupyBond(selectedNodeIndex))
        {
            Debug.LogWarning("Failed to occupy parent's node.");
            return;
        }

        // Compute direction from parent's center to the selected node
        Vector3 D = (parentNodePos - parentCenter).normalized;

        // Compute the new sphere's center position
        Vector3 newSphereCenter = parentCenter + 2f * fixedDistance * D;

        // Compute the new sphere's rotation.
        // We want its head node (local position Vector3.up * fixedDistance) to connect to parent's node.
        // That is, newSphereCenter + newRot*(Vector3.up*fixedDistance) should equal parent's center + fixedDistance*D (i.e. parent's node).
        // Rearranging, newRot*(Vector3.up*fixedDistance) should equal parent's center + fixedDistance*D - newSphereCenter.
        // Given newSphereCenter = parent's center + 2*fixedDistance*D, the right‐side becomes:
        // parent's center + fixedDistance*D - (parent's center + 2*fixedDistance*D) = -fixedDistance*D.
        // So we want newRot*(Vector3.up*fixedDistance) = -fixedDistance*D,
        // which is achieved by:
        Quaternion newRot = Quaternion.FromToRotation(Vector3.up, -D);

        // Instantiate the new sphere at the computed position and rotation
        GameObject newSphere = Instantiate(spherePrefab, newSphereCenter, newRot);
        newSphere.tag = "Sphere";

        // Set its element properties
        string selectedElement = ElementWheelController.Instance.CurrentElement;
        int selectedCharge = ElementWheelController.Instance.CurrentCharge;
        AtomController atomController = newSphere.GetComponent<AtomController>();
        if (atomController != null)
            atomController.SetAtomProperties(selectedElement, selectedCharge);
        Renderer newRenderer = newSphere.GetComponent<Renderer>();
        if (newRenderer != null)
            newRenderer.material.color = GetElementColor(selectedElement);

        // Add a SphereBondController to the new sphere if needed
        SphereBondController newSBC = newSphere.GetComponent<SphereBondController>();
        if (newSBC == null)
        {
            newSBC = newSphere.AddComponent<SphereBondController>();
            newSBC.fixedDistance = fixedDistance;
        }
        newSBC.OccupyBond(0); // Occupy the head node (node 0)
        newSBC.bondCount = (int)currentBondType+1;
        newSBC.maxBonds = selectedCharge;

        // Create a visual bond between the parent and the new sphere
        GameObject newBond = CreateBond(selectedBaseSphere.transform.position, newSphere.transform.position);
        molecularBuilder.AddSphere(newSphere); // Add to MolecularBuilder
        molecularBuilder.AddBond(newBond); // Add bond to MolecularBuilder
        AtomController parentAtom = selectedBaseSphere.GetComponent<AtomController>();
        AtomController childAtom = newSphere.GetComponent<AtomController>();
        if (parentAtom != null && childAtom != null)
        {
            parentAtom.AddConnection(childAtom, currentBondType);
        }

        // Cleanup preview and unhighlight
        Destroy(currentPreview);
        currentPreview = null;
        UnhighlightLastSphere();
    }


    /// <summary>
    /// Creates a visual bond (cylinder) connecting two points.
    /// </summary>
    GameObject CreateBond(Vector3 startPos, Vector3 endPos)
    {
        int totalbonds = (int)currentBondType;
        Vector3 direction = endPos - startPos;
        float bondLength = direction.magnitude;
        Vector3 bondDirection = direction.normalized;

        GameObject parentBond = new GameObject("BondGroup");
        parentBond.transform.position = (startPos + endPos) / 2f;
        parentBond.transform.up = bondDirection;

        float offset = 0.3f;

        Vector3 offsetDirection = Vector3.Cross(bondDirection, Vector3.up).normalized; // Perpendicular vector.
        if (offsetDirection == Vector3.zero)
        {
            offsetDirection = Vector3.Cross(bondDirection, Vector3.forward).normalized; // Alternative perpendicular vector.
        }

        for (int i = 0; i < totalbonds+1; i++)
        {
            Vector3 bondPositionOffset = Vector3.zero;

            if (totalbonds == 1)
            {
                bondPositionOffset = offsetDirection * (i == 0 ? -offset / 2f : offset / 2f);
            }
            else if (totalbonds == 2)
            {
                bondPositionOffset = offsetDirection * (i - 1) * offset;
            }

            GameObject bond = Instantiate(bondPrefab, parentBond.transform.position, Quaternion.identity, parentBond.transform);
            bond.transform.localPosition = bondPositionOffset;
            bond.transform.up = bondDirection;

            Vector3 bondScale = bond.transform.localScale;
            bondScale.y = bondLength / 2f;
            bond.transform.localScale = bondScale;
        }

        return parentBond;
    }

    void HighlightSphere(GameObject sphere)
    {
        UnhighlightLastSphere();
        Renderer rend = sphere.GetComponent<Renderer>();
        if (rend != null)
            rend.material.SetColor("_EmissionColor", Color.yellow);
        lastHighlightedSphere = sphere;
    }

    void UnhighlightLastSphere()
    {
        if (lastHighlightedSphere != null)
        {
            Renderer rend = lastHighlightedSphere.GetComponent<Renderer>();
            if (rend != null)
                rend.material.SetColor("_EmissionColor", Color.black);
            lastHighlightedSphere = null;
        }
    }

    private Color GetElementColor(string element)
    {
        switch (element)
        {
            case "C": return new Color(0.5f, 0.5f, 0.5f);
            case "O": return new Color(1f, 0f, 0f);
            case "N": return new Color(0f, 0f, 1f);
            case "Na": return new Color(1f, 0.5f, 0f);
            case "Cl": return new Color(0f, 1f, 0f);
            case "S": return new Color(1f, 1f, 0f);
            case "P": return new Color(1f, 0f, 1f);
            case "F": return new Color(0f, 1f, 1f);
            default: return Color.white;
        }
    }
}
