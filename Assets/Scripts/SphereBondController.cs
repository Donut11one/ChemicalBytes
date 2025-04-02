using UnityEngine;

public class SphereBondController : MonoBehaviour
{

    public int bondCount = 0;      // Total bonds currently connected.
    public float fixedDistance = 2f;  // Distance from center to each node.
    
    public int maxBonds;       // Maximum bonds allowed.
    // Predefined local positions for the 5 nodes.
    public Vector3[] bondPositions;
    // Tracks which nodes are already occupied.
    public bool[] bondOccupied;
    void Awake()
    {
        // Initialize arrays for 5 nodes.
        bondPositions = new Vector3[5];
        bondOccupied = new bool[5];

        // Node 0: Head node (top).
        bondPositions[0] = Vector3.up * fixedDistance;
        // Node 1: Bottom node.
        bondPositions[1] = Vector3.down * fixedDistance;
        // Nodes 2-4: Leg nodes, evenly spaced in the XZ plane.
        bondPositions[2] = new Vector3(fixedDistance, 0, 0);
        float angleRad = 120 * Mathf.Deg2Rad;
        bondPositions[3] = new Vector3(fixedDistance * Mathf.Cos(angleRad), 0, fixedDistance * Mathf.Sin(angleRad));
        angleRad = 240 * Mathf.Deg2Rad;
        bondPositions[4] = new Vector3(fixedDistance * Mathf.Cos(angleRad), 0, fixedDistance * Mathf.Sin(angleRad));

        // Initially, no node is occupied.
        for (int i = 0; i < bondOccupied.Length; i++)
        {
            bondOccupied[i] = false;
        }
    }
    public void setProperties(int bonds)
    {
        maxBonds = bonds;
    }

    public bool HasFreeBond()
    {
        return bondCount < maxBonds;
    }

    public bool IsBondOccupied(int index)
    {
        if (index < 0 || index >= bondOccupied.Length) return true;
        return bondOccupied[index];
    }

    public bool OccupyBond(int index)
    {
        if (index < 0 || index >= bondOccupied.Length) return false;
        if (bondOccupied[index]) return false;
        bondOccupied[index] = true;
        bondCount++;
        return true;
    }
}
