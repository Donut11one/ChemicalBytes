using UnityEngine;
using System.Collections.Generic;

public enum BondType { Single = 0, Double = 1, Triple=2}

public class AtomController : MonoBehaviour
{
    public string element;
    public int valency;
    public bool isSelected = false;

    public List<AtomController> connectedAtoms = new List<AtomController>();
    public List<BondType> bondTypes = new List<BondType>(); // Use BondType enum

    private Renderer atomRenderer;

    void Start()
    {
        atomRenderer = GetComponent<Renderer>();
    }

    public void SelectAtom(bool select)
    {
        isSelected = select;
        atomRenderer.material.color = isSelected ? Color.yellow : Color.white;
    }

    public void SetAtomProperties(string elem, int charge)
    {        
        element = elem;
        valency = charge;
    }

    public void AddConnection(AtomController otherAtom, BondType bondType) // Accept BondType
    {
        if (!connectedAtoms.Contains(otherAtom))
        {
            connectedAtoms.Add(otherAtom);
            bondTypes.Add(bondType);
            otherAtom.connectedAtoms.Add(this);
            otherAtom.bondTypes.Add(bondType);
            valency -= 1;
        }
    }
}