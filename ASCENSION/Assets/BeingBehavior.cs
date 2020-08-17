using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeingBehavior : MonoBehaviour
{
    public Transform bodyPrefab;
    public GameObject LeftGrasp;
    public GameObject RightGrasp;
    public GameObject JawGrasp;

    public GameObject Pockets;
    public Transform Body;

    
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeBeing()
    {
        Body = Instantiate(bodyPrefab); // Create a new slot transform
        Body.transform.parent = this.transform; // Make it a child of this transform (the body)
        Body.name = "Body";
        Body.GetComponent<BodyBehavior>().InitializeBody();

        Pockets = new GameObject();
        Pockets.name = "Pockets";
        Pockets.transform.parent = this.transform;
        
    }
}
