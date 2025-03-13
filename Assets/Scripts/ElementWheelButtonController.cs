using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ElementWheelButtonController : MonoBehaviour
{
    public int Id;
    private Animator anim;
    public string          itemName;
    public TextMeshProUGUI itemNameText;
    public Image selectedItem;
    private bool isSelected = false;
    public Sprite icon;

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
            itemNameText.text   = itemName;
        }
    }

    public void Selected()
    {
        isSelected = true;
        ElementWheelController.elementID = Id;
    }

    public void Deselected()
    {
        isSelected = false;
        ElementWheelController.elementID = 0;

    }

    public void HoverEnter()
    {
        // set the bool we named in animator "Hover" to true. Important bool must match name in animator
        anim.SetBool("Hover", true);
        itemNameText.text = itemName;
    }

    public void HoverExit()
    {
        // set the bool we named in animator "Hover" to true. Important bool must match name in animator
        anim.SetBool("Hover", false);
        itemNameText.text = "";
    }


}
