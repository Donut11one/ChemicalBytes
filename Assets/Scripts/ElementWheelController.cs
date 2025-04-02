using UnityEngine;
using UnityEngine.UI;

public class ElementWheelController : MonoBehaviour
{

    // Simple singleton for easy access in SpherePlacer
    public static ElementWheelController Instance;

    // holds our PlayerCam script
    public PlayerCam playerCam;

    // These will be used by the spawner script to determine which element to spawn
    public string CurrentElement { get; private set; } = "C"; // Default element is Carbon
    public int CurrentCharge { get; private set; } = 4; // Default charge is 4

    public Animator anim;
    public Image selectedItem;
    public Sprite noImage;
    // The current selection ID
    public static int elementID;

    private bool elementWheelSelected = false;

    private void Awake()
    {
        // a simple singleton reference so Spawner can easily grab our instance
        Instance = this;
    }

    /// <summary>
    /// Called by numeric keys *or* button scripts to set the element/charge.
    /// </summary>
    public void SetElement(string elementSymbol, int charge)
    {
        CurrentElement = elementSymbol;
        CurrentCharge = charge;
        Debug.Log("SetElement: " + CurrentElement + " (valency " + CurrentCharge + ")");

        // This is purely for visual feedback for the keyboard input
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
            case "C": return Resources.Load<Sprite>("PreviewIcons/carbon");
            case "O": return Resources.Load<Sprite>("PreviewIcons/oxygen");
            case "N": return Resources.Load<Sprite>("PreviewIcons/nitrogen");
            case "Na": return Resources.Load<Sprite>("PreviewIcons/sodium");
            case "Cl": return Resources.Load<Sprite>("PreviewIcons/chlorine");
            case "S": return Resources.Load<Sprite>("PreviewIcons/sulfur");
            case "P": return Resources.Load<Sprite>("PreviewIcons/phosphorous");
            case "F": return Resources.Load<Sprite>("PreviewIcons/fluorine");
            default: return noImage;
        }
    }


    // Update is called once per frame
    void Update()
    {
        // Only allow element selection if the game is not paused
        if (!PauseMenu.isPaused)
        {

            // Numerical key equip your element
            if (Input.GetKeyDown(KeyCode.Alpha1)) SetElement("C", 4);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SetElement("O", 2);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SetElement("N", 3);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SetElement("Na", 1);
            if (Input.GetKeyDown(KeyCode.Alpha5)) SetElement("Cl", 1);
            if (Input.GetKeyDown(KeyCode.Alpha6)) SetElement("S", 2);
            if (Input.GetKeyDown(KeyCode.Alpha7)) SetElement("P", 3);
            if (Input.GetKeyDown(KeyCode.Alpha8)) SetElement("F", 1);
        }

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
