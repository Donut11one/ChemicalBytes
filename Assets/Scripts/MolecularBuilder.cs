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

    public string GenerateSMILES()
    {
        Dictionary<AtomController, List<AtomController>> adjacencyList = GetAdjacencyList();
        HashSet<AtomController> visited = new HashSet<AtomController>();
        string smiles = "";

        if (adjacencyList.Count > 0)
        {
            AtomController startAtom = findCentralAtom(adjacencyList);
            smiles = DFSSmiles(startAtom, adjacencyList, visited, "", null);
        }

        return smiles;
    }

    string DFSSmiles(AtomController currentAtom, Dictionary<AtomController, List<AtomController>> adjacencyList, HashSet<AtomController> visited, string currentSmiles, AtomController parentAtom)
    {
        visited.Add(currentAtom);
        currentSmiles += currentAtom.element;

        // Filter out any null references, and those already visited or equal to the parent.
        List<AtomController> neighbors = adjacencyList[currentAtom]
            .Where(neighbor => neighbor != null && !visited.Contains(neighbor) && neighbor != parentAtom)
            .ToList();

        // Sort the neighbors by number of connections (largest first)
        neighbors.Sort((a, b) => adjacencyList[b].Count - adjacencyList[a].Count);

        for (int i = 0; i < neighbors.Count; i++)
        {
            AtomController neighbor = neighbors[i];
            int bondTypeIndex = currentAtom.connectedAtoms.IndexOf(neighbor); // Get the index of the neighbor.

            if (bondTypeIndex >= 0 && bondTypeIndex < currentAtom.bondTypes.Count)
            {
                BondType bondType = currentAtom.bondTypes[bondTypeIndex];

                if (bondType == BondType.Double)
                {
                    currentSmiles += "=";
                }
                else if (bondType == BondType.Triple)
                {
                    currentSmiles += "#";
                }
            }

            currentSmiles += "(";
            currentSmiles = DFSSmiles(neighbor, adjacencyList, visited, currentSmiles, currentAtom);
            currentSmiles += ")";
        }

        return currentSmiles;
    }


    public Dictionary<AtomController, List<AtomController>> GetAdjacencyList()
    {
        Dictionary<AtomController, List<AtomController>> adjacencyList = new Dictionary<AtomController, List<AtomController>>();

        foreach (GameObject sphere in spheres)
        {
            AtomController atom = sphere.GetComponent<AtomController>();
            if (atom != null)
            {
                adjacencyList[atom] = atom.connectedAtoms;
            }
        }

        return adjacencyList;
    }
    AtomController findCentralAtom(Dictionary<AtomController,List<AtomController>> adjacencyList)
    {
        AtomController centralAtom = adjacencyList.Keys.First();
        int maxConnections = adjacencyList[centralAtom].Count;
        foreach(AtomController atom in adjacencyList.Keys)
        {
            if (adjacencyList[atom].Count > maxConnections)
            {
                centralAtom = atom;
                maxConnections = adjacencyList[atom].Count;
            }
        }
        return centralAtom;
    }

    public void DisplaySMILES()
    {
        string smiles = GenerateSMILES();
        Debug.Log("SMILES: " + smiles);
    }

    public void RemoveSphere(GameObject sphere)
    {
        if (spheres.Contains(sphere))
        {
            spheres.Remove(sphere);
        }
    }

}