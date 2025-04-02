using UnityEngine;


public class Bond : MonoBehaviour
{
    public static GameObject spherePlacerObject;
    public int BondNumber;

    void Awake()
    {
        spherePlacerObject = GameObject.Find("SpherePlacer");
        BondNumber = (int)spherePlacerObject.GetComponent<SpherePlacer>().currentBondType;
    }
}

