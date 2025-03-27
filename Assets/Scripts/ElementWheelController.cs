using UnityEngine;
using UnityEngine.UI;

public class ElementWheelController : MonoBehaviour
{

    // Simple singleton for easy access in SpherePlacer
    public static ElementWheelController Instance;

    // drag your PlayerCam script here in the Inspector
    public PlayerCam playerCam;
    // drag your PlayerMovement script here as well
    //public PlayerMovement playerMovement;

    // These will be used by the spawner script to determine which element to spawn
    public string CurrentElement { get; private set; } = "C"; // Default element is Carbon
    public int CurrentCharge { get; private set; } = 0; // Default charge is 0

    public Animator anim;
    public Image selectedItem;
    public Sprite noImage;
    // The current selection ID
    public static int elementID;

    private bool elementWheelSelected = false;

    private void Awake()
    {
        // Set up a simple singleton reference so Spawner can easily grab our instance
        Instance = this;
    }

    /// <summary>
    /// Called by numeric keys *or* button scripts to set the element/charge.
    /// </summary>
    public void SetElement(string elementSymbol, int charge)
    {
        CurrentElement = elementSymbol;
        CurrentCharge = charge;
        Debug.Log("SetElement: " + CurrentElement + " (charge " + charge + ")");

        // Optionally update the UI image to reflect the newly selected element
        // This is purely for visual feedback in the wheel's "selectedItem" image
        selectedItem.sprite = GetElementSprite(elementSymbol);
    }

    /// <summary>
    /// Simple helper to pick the correct sprite for each element.
    /// You can also do a dictionary or multiple if/else if you prefer.
    /// </summary>
    private Sprite GetElementSprite(string elementSymbol)
    {
        switch (elementSymbol)
        {
            case "C": return Resources.Load<Sprite>("carbon");
            case "O": return Resources.Load<Sprite>("oxygen");
            case "N": return Resources.Load<Sprite>("nitrogen");
            case "Na": return Resources.Load<Sprite>("sodium");
            case "Cl": return Resources.Load<Sprite>("chlorine");
            case "S": return Resources.Load<Sprite>("sulfur");
            case "P": return Resources.Load<Sprite>("phosphorous");
            case "F": return Resources.Load<Sprite>("fluorine");
            default: return noImage;
        }
    }


    // Update is called once per frame
    void Update()
    {

        // ---------------------------------------------------------
        // 1) Numeric Key Input to Directly Set the Element
        // ---------------------------------------------------------
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetElement("C", 0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetElement("O", 0);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetElement("N", 0);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetElement("Na", 1);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetElement("Cl", -1);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SetElement("S", 0);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SetElement("P", 0);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SetElement("F", 0);

        // Open menu on KeyDown
        if (Input.GetKeyDown(KeyCode.Tab) && !elementWheelSelected)
        {
            elementWheelSelected = true;
            anim.SetBool("OpenElementWheel", true);

            // Show and unlock the cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Disable camera rotation
            if (playerCam != null) playerCam.enabled = false;
        }
        // Close menu on KeyUp
        else if (Input.GetKeyUp(KeyCode.Tab) && elementWheelSelected)
        {
            elementWheelSelected = false;
            anim.SetBool("OpenElementWheel", false);

            // Hide and lock the cursor
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // Re-enable camera rotation
            if (playerCam != null) playerCam.enabled = true;
        }
    }
}
