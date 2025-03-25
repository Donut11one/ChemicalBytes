using UnityEngine;
using UnityEngine.UI;

public class SpawnerScript : MonoBehaviour
{
    public GameObject atomPrefab; // The atom prefab (with AtomController attached)
    public Transform playerCamera;
    public float spawnDistance = 2f;
    public Image crosshairImage;

    private string selectedElement = "C"; // Default element is Carbon
    private int selectedCharge = 0; // Default charge is 0  

    void Update()
    {
        // Change atom type when a number key is pressed
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetAtomType("C", 0); // Carbon
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetAtomType("O", 0); // Oxygen
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetAtomType("N", 0); // Nitrogen
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetAtomType("Na", 1); // Sodium +1
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetAtomType("Cl", -1); // Chlorine -1
        if (Input.GetKeyDown(KeyCode.Alpha6)) SetAtomType("S", 0); // Sulfur
        if (Input.GetKeyDown(KeyCode.Alpha7)) SetAtomType("P", 0); // Phosphorous
        if (Input.GetKeyDown(KeyCode.Alpha8)) SetAtomType("F", 0); // Fluorine

        // Spawn atom when E is pressed
        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnAtom();
        }
    }

    void SpawnAtom()
    {
        // Instantiate the atom at the player's camera position
        GameObject newAtom = Instantiate(atomPrefab, playerCamera.position + playerCamera.transform.forward * spawnDistance, Quaternion.identity);

        // Set the atom properties
        AtomController atomController = newAtom.GetComponent<AtomController>();
        if (atomController != null)
        {
            atomController.SetAtomProperties(selectedElement, selectedCharge);

            // Set the atom's color based on the selected element
            Renderer newAtomRenderer = newAtom.GetComponent<Renderer>();
            if (newAtomRenderer != null)
            {
                newAtomRenderer.material.color = GetElementColor(selectedElement);
            }
        }

        // Assign the atom to a specific layer to allow deletion
        newAtom.layer = LayerMask.NameToLayer("Spheres");
    }

    void SetAtomType(string element, int charge)
    {
        selectedElement = element;
        selectedCharge = charge;
    }

    private Color GetElementColor(string element)
    {
        switch (element)
        {
            case "C": return new Color(0.5f, 0.5f, 0.5f); // Grey for Carbon
            case "O": return new Color(1f, 0f, 0f); // Red for Oxygen
            case "N": return new Color(0f, 0f, 1f); // Blue for Nitrogen
            case "Na": return new Color(1f, 0.5f, 0f); // Orange for Sodium
            case "Cl": return new Color(0f, 1f, 0f); // Green for Chlorine
            case "S": return new Color(1f, 1f, 0f); // Yellow for Sulfur
            case "P": return new Color(1f, 0f, 1f); // Magenta for Phosphorous
            case "F": return new Color(0f, 1f, 1f); // Cyan for Fluorine
            default: return Color.white; // Default to white
        }
    }
}