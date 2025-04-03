using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ElementWheelButtonController : MonoBehaviour
{
    [Header("Element Data for This Button")]
    public string elementSymbol = "C";
    public int elementCharge = 0;
    public Sprite icon;

    [Header("UI References")]
    // References for the GUI
    public TextMeshProUGUI itemNameText;
    public Image selectedItem;

    private Animator anim;
    private bool isSelected = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isSelected)
        {
            selectedItem.sprite = icon;
            itemNameText.text   = elementSymbol;
        }
    }

    /// <summary>
    /// Set the element and charge when the button is selected, updating the UI and assigning appropriate animation in unity.
    /// </summary>
    public void Selected()
    {
        isSelected = true;

        // Instead of just setting a static elementID, 
        // call a method on the ElementWheelController to set the element
        ElementWheelController.Instance.SetElement(elementSymbol, elementCharge);
    }

    /// <summary>
    /// Deselect the button, clearing the UI and assign animation in unity.
    /// </summary>
    public void Deselected()
    {
        isSelected = false;
        // Reset the text and sprite
        itemNameText.text = "";
    }

    /// <summary>
    /// Hover enter event for the button, assign animation in unity.
    /// </summary>
    public void HoverEnter()
    {
        // set the bool we named in animator "Hover" to true. Important bool must match name in animator
        anim.SetBool("Hover", true);
        itemNameText.text = "";
    }

    /// <summary>
    /// Hover exit event for the button, assign animation in unity.
    /// </summary>
    public void HoverExit()
    {
        // set the bool we named in animator "Hover" to true. Important bool must match name in animator
        anim.SetBool("Hover", false);
        itemNameText.text = "";
    }
}
