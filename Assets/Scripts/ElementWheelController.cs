using UnityEngine;
using UnityEngine.UI;

public class ElementWheelController : MonoBehaviour
{
    public Animator anim;
    private bool elementWheelSelected = false;
    public Image selectedItem;
    public Sprite noImage;
    public static int elementID;
 
    public PlayerCam playerCam;

    // Update is called once per frame
    void Update()
    {
        // Open menu on KeyDown
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            elementWheelSelected = true;
            anim.SetBool("OpenElementWheel", true);

            // Show and unlock the cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Disable camera rotation
            if (playerCam != null)
            {
                playerCam.enabled = false;
            }
        }

        // Close menu on KeyUp
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            elementWheelSelected = false;
            anim.SetBool("OpenElementWheel", false);https://discord.com/channels/168551375280537600/1162027197465374763

            // Hide and lock the cursor
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // Re-enable camera rotation
            if (playerCam != null)
            {
                playerCam.enabled = true;
            }
        }

        switch (elementID)
        {
            case 0: // nothing is selected
                selectedItem.sprite = noImage;
                break;
            case 1: // test element
                Debug.Log("Flask");
                selectedItem.sprite = Resources.Load<Sprite>("Fizzling-Flask");
                break;
        }
    }
}
