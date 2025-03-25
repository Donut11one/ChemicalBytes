using UnityEngine;
using System.Collections.Generic;

public class AtomController : MonoBehaviour
{
    public string element;
    public int formalCharge = 0;
    public bool isSelected = false;
    public List<AtomController> connectedAtoms = new List<AtomController>(); // List of connected atoms
    public List<int> bondTypes = new List<int>(); // List of bond types for each connection

    private Renderer atomRenderer;

    void Start()
    {
        atomRenderer = GetComponent<Renderer>();
    }

    public void SelectAtom(bool select)
    {
        isSelected = select;
        if (isSelected)
        {
            atomRenderer.material.color = Color.yellow;
        }
        else
        {
            atomRenderer.material.color = Color.white;
        }
    }

    public void SetAtomProperties(string elem, int charge)
    {
        element = elem;
        formalCharge = charge;
    }

    public void AddConnection(AtomController otherAtom, int bondType)
    {
        connectedAtoms.Add(otherAtom);
        bondTypes.Add(bondType);
    }
}
