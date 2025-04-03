using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class MolecularBuilder : MonoBehaviour
{
    public List<GameObject> spheres = new List<GameObject>();
    public List<GameObject> bonds = new List<GameObject>();
    public TMP_Text moleculeNameText;

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
            smiles = DFSSmiles(startAtom, adjacencyList, visited, "", null, new Dictionary<AtomController, int>());
        }

        return smiles;
    }

    string DFSSmiles(AtomController currentAtom, Dictionary<AtomController, List<AtomController>> adjacencyList, HashSet<AtomController> visited, string currentSmiles, AtomController parentAtom, Dictionary<AtomController, int> ringClosures)
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
            int bondTypeIndex = currentAtom.connectedAtoms.IndexOf(neighbor);

            if (bondTypeIndex >= 0 && bondTypeIndex < currentAtom.bondTypes.Count)
            {
                BondType bondType = currentAtom.bondTypes[bondTypeIndex];
                string bondSymbol = "";

                switch (bondType)
                {
                    case BondType.Single:
                        bondSymbol = "";
                        break;
                    case BondType.Double:
                        bondSymbol = "=";
                        break;
                    case BondType.Triple:
                        bondSymbol = "#";
                        break;
                }

                // Generate neighbor smiles first
                string neighborSmiles = DFSSmiles(neighbor, adjacencyList, visited, "", currentAtom, ringClosures);

                // Add parenthesis only when the current atom has more than 1 neighbor.
                if (neighbors.Count > 1 && neighborSmiles != "")
                {
                    currentSmiles += "(" + bondSymbol + neighborSmiles + ")";
                }
                else if (neighborSmiles != "")
                {
                    currentSmiles += bondSymbol + neighborSmiles;
                }
            }
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

    AtomController findCentralAtom(Dictionary<AtomController, List<AtomController>> adjacencyList)
    {
        AtomController centralAtom = adjacencyList.Keys.First();
        int maxConnections = adjacencyList[centralAtom].Count;
        foreach (AtomController atom in adjacencyList.Keys)
        {
            if (adjacencyList[atom].Count > maxConnections)
            {
                centralAtom = atom;
                maxConnections = adjacencyList[atom].Count;
            }
        }
        return centralAtom;
    }

    public async void DisplaySMILES()
    {
        string smiles = GenerateSMILES();
        Debug.Log("SMILES: " + smiles);

        try
        {
            string jsonResponse = await PubChemAPI.Instance.GetMoleculeNameAsync(smiles);

            if (string.IsNullOrEmpty(jsonResponse))
            {
                moleculeNameText.text = "Molecule not found on PubChem.";
            }
            else
            {
                (string moleculeName, string smilesFormat) = PubChemAPI.ExtractMoleculeData(jsonResponse);

                if (!string.IsNullOrEmpty(moleculeName))
                {
                    Debug.Log(moleculeName);
                    moleculeNameText.text = "Molecule Name: " + moleculeName;
                }
                else
                {
                    moleculeNameText.text = "Molecule name not found in response.";
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during PubChem API call: " + ex.Message);
            moleculeNameText.text = "Molecule Does not Exist";
        }
    }

    public void RemoveSphere(GameObject sphere)
    {
        if (spheres.Contains(sphere))
        {
            spheres.Remove(sphere);
        }
    }
}

public class PubChemAPI
{
    private static PubChemAPI instance;
    public static PubChemAPI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PubChemAPI();
            }
            return instance;
        }
    }

    private readonly HttpClient client = new HttpClient();

    private PubChemAPI() { }

    public async Task<string> GetMoleculeNameAsync(string smiles)
    {
        string url = $"https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/smiles/{Uri.EscapeDataString(smiles)}/JSON";
        HttpResponseMessage response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();
        else
            throw new Exception("PubChem request failed.");
    }

    public static (string Name, string Smiles) ExtractMoleculeData(string jsonResponse)
    {
        string moleculeName = "";
        string smilesFormat = "";

        try
        {
            PubChemResponse response = JsonUtility.FromJson<PubChemResponse>(jsonResponse);

            if (response != null && response.PC_Compounds != null && response.PC_Compounds.Length > 0)
            {
                PC_Compounds compound = response.PC_Compounds[0];

                if (compound.props != null)
                {
                    foreach (Prop prop in compound.props)
                    {
                        if (prop.urn != null && prop.value != null)
                        {
                            if (prop.urn.label == "IUPAC Name" && prop.urn.name == "Systematic")
                            {
                                moleculeName = prop.value.sval;
                            }
                            else if (prop.urn.label == "SMILES" && prop.urn.name == "Canonical")
                            {
                                smilesFormat = prop.value.sval;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing JSON: {ex.Message}");
        }

        return (moleculeName, smilesFormat);
    }
}

[System.Serializable]
public class PC_Compounds
{
    public Prop[] props;
}

[System.Serializable]
public class Prop
{
    public Urn urn;
    public Value value;
}

[System.Serializable]
public class Urn
{
    public string label;
    public string name;
}

[System.Serializable]
public class Value
{
    public string sval;
}

[System.Serializable]
public class PubChemResponse
{
    public PC_Compounds[] PC_Compounds;
}

// Extension method for string repetition
public static class StringExtensions
{
    public static string Repeat(this string value, int count)
    {
        return new System.Text.StringBuilder(value.Length * count)
            .Insert(0, value, count)
            .ToString();
    }
}