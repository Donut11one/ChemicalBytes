using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MolecularBuilder : MonoBehaviour
{
    public List<GameObject> spheres = new List<GameObject>();
    public List<GameObject> bonds = new List<GameObject>();

    public void AddSphere(GameObject sphere)
    {
        spheres.Add(sphere);
    }

    public void AddBond(GameObject bond)
    {
        bonds.Add(bond);
    }

    public string GetMolecularFormula()
    {
        Dictionary<string, int> elementCounts = new Dictionary<string, int>();
        int totalHydrogens = 0; // Directly sum up hydrogens

        foreach (GameObject sphere in spheres)
        {
            AtomController atom = sphere.GetComponent<AtomController>();
            if (atom != null)
            {
                if (atom.element != "H") // Skip hydrogen counting.
                {
                    if (elementCounts.ContainsKey(atom.element))
                    {
                        elementCounts[atom.element]++;
                    }
                    else
                    {
                        elementCounts[atom.element] = 1;
                    }

                    // Calculate hydrogen based on formal charge.
                    int bonds = atom.connectedAtoms.Count;
                    int impliedHydrogens = Mathf.Max(0, atom.valency - bonds);
                    totalHydrogens += impliedHydrogens; // Sum up hydrogens directly
                }
            }
        }

        var sortedElements = elementCounts.Keys.OrderBy(element => element);

        string formula = "";

        foreach (string element in sortedElements)
        {
            formula += element;
            if (elementCounts[element] > 1)
            {
                formula += elementCounts[element];
            }
        }

        if (totalHydrogens > 0)
        {
            formula += "H";
            if (totalHydrogens > 1)
            {
                formula += totalHydrogens;
            }
        }

        return formula;
    }


    public void DisplayMolecularFormula()
    {
        string formula = GetMolecularFormula();
        Debug.Log("Molecular Formula: " + formula);
    }
}