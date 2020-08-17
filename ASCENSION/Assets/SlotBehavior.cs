using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotBehavior : MonoBehaviour
{
    public string FullName;
    public string FlavorText;
    public int IsAugSlot;
    public string MajorGroup;
    public string SubGroup;
    public string ProtectingSlot;
    public string SurgicalParent;
    public string ParentNeed;

    public Transform componentPrefab;
    
    public void HumanComponent()
    {
        if (this.IsAugSlot == 1) //Remove all augments
        {
            foreach(Transform child in this.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        } else
        {
            InitializeComponent();
        }
    }

    void InitializeComponent()
    {
        Transform NewComponent = Instantiate(componentPrefab); // Create a new slot transform
        NewComponent.transform.parent = this.transform; // Make it a child of this transform (the body)
        NewComponent.name = "Human " + this.transform.name; // Name it
        NewComponent.GetComponent<ComponentBehavior>().InitializeHuman(this.name);
    }
    
}
