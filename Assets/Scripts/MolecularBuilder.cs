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
            smiles = DFSSmiles(startAtom, adjacencyList, visited, "");
        }

        return smiles;
    }

    string DFSSmiles(AtomController currentAtom, Dictionary<AtomController, List<AtomController>> adjacencyList, HashSet<AtomController> visited, string currentSmiles, AtomController parentAtom = null)
    {
        visited.Add(currentAtom);
        currentSmiles += currentAtom.element;

        List<AtomController> neighbors = adjacencyList[currentAtom].Where(neighbor => !visited.Contains(neighbor) && neighbor != parentAtom).ToList();

        // Sort neighbors by the number of connections.
        neighbors.Sort((a, b) => adjacencyList[b].Count - adjacencyList[a].Count);

        foreach (AtomController neighbor in neighbors)
        {
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
}