using UnityEngine;

public class Bond : MonoBehaviour
{
    public static GameObject spherePlacerObject;
    public int BondNumber;

    public GameObject sphereA;   // One sphere connected by this bond.
    public GameObject sphereB;   // The other sphere.
    public int nodeIndexA;       // The node index on sphereA.
    public int nodeIndexB;       // The node index on sphereB.

    void Awake()
    {
        spherePlacerObject = GameObject.Find("SpherePlacer");
        BondNumber = (int)spherePlacerObject.GetComponent<SpherePlacer>().currentBondType;
    }
}
