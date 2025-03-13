using UnityEngine;
using UnityEngine.UI;

public class ElementWheelController : MonoBehaviour
{
    public Animator anim;
    private bool elementWheelSelected = false;
    public Image selectedItem;
    public Sprite noImage;
    public static int elementID;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            elementWheelSelected = !elementWheelSelected;

        }

        if (elementWheelSelected)
        {
            anim.SetBool("OpenElementWheel", true);
        }
        else
        {
            anim.SetBool("OpenElementWheel", false);
        }

        switch(elementID)
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
