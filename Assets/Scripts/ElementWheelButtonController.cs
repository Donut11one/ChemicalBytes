using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public void Selected()
    {
        isSelected = true;

        // Instead of just setting a static elementID, 
        // call a method on the ElementWheelController to set the element
        ElementWheelController.Instance.SetElement(elementSymbol, elementCharge);
    }

    public void Deselected()
    {
        isSelected = false;
        // Reset the text and sprite
        itemNameText.text = "";
        //selectedItem.sprite = Resources.Load<Sprite>("None");
    }

    public void HoverEnter()
    {
        // set the bool we named in animator "Hover" to true. Important bool must match name in animator
        anim.SetBool("Hover", true);
        itemNameText.text = "";
    }

    public void HoverExit()
    {
        // set the bool we named in animator "Hover" to true. Important bool must match name in animator
        anim.SetBool("Hover", false);
        itemNameText.text = "";
    }


}
