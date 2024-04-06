using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> jewels = new List<GameObject>();

    [SerializeField]
    private int maxJewelCount;

    [SerializeField]
    private int jewelCount;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            GameObject jewelObject = child.gameObject;
            jewels.Add(jewelObject);
     
        }

        maxJewelCount = jewels.Count;
        jewelCount = jewels.Count;
    }

    public void jewelCollected(GameObject Collected)
    {
        if (Collected != null && jewels.Contains(Collected))
        {
            jewels.Remove(Collected);
            Destroy(Collected); // Remove the GameObject from the scene
            jewelCount = jewels.Count;
        }

        if (jewelCount == 0)
        {
            allJewelsCollected();
        }
    }

    public void allJewelsCollected()
    {
        Debug.Log("All Jewels Collected");
    }
}
