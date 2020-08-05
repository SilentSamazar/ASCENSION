using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyBehavior : MonoBehaviour
{

    public Transform prefab;
    
    // Start is called before the first frame update
    void Start()
    {
        FillBody();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FillBody()
    {
        Transform HeartSlot = Instantiate(prefab);
        HeartSlot.transform.parent = this.transform;
        HeartSlot.name = "Heart Slot";
    }
}
