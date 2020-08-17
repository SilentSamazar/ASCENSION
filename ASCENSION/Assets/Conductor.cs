using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Used for the text handling (may be temporary)

public class Conductor : MonoBehaviour
{

    GameObject Player;
    GameObject EntireChalupa;
    GameObject HideoutInventory;
    GameObject UIController;
    public System.Random rnd = new System.Random();

    public Transform ComponentPrefab;
        
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("PLAYER");
        EntireChalupa = GameObject.Find("EntireChalupa");
        HideoutInventory = GameObject.Find("Hideout Inventory");
        UIController = GameObject.Find("UIController");
        
        EntireChalupa.GetComponent<CatalogDropTables>().CreateDictionary(); //The tables need to be created first before the body can fill.

        //Transform DummyComponent = Instantiate(ComponentPrefab);
        //DummyComponent.GetComponent<ComponentBehavior>().InitializeSpecific("Apex FA-6000 \"Vader\" (Left)");

        Transform DummyComponent = Instantiate(ComponentPrefab);
        DummyComponent.GetComponent<ComponentBehavior>().InitializeRandom("Master Catalog");

        Transform DummyAug = Instantiate(ComponentPrefab);
        DummyAug.GetComponent<ComponentBehavior>().InitializeRandom("Master Catalog");

        Transform DummyComp2 = Instantiate(ComponentPrefab);
        DummyComp2.GetComponent<ComponentBehavior>().InitializeRandom("Master Catalog");

        DummyComponent.parent = HideoutInventory.transform;
        DummyComp2.parent = HideoutInventory.transform;
        DummyAug.parent = HideoutInventory.transform;

        Player.GetComponent<BeingBehavior>().InitializeBeing();

        UIController.GetComponent<UIStuff>().UpdateInventory();
        
        //Debug.Log(Body.GetComponent<BodyBehavior>().InstallComponent(DummyComponent.transform));
        //Debug.Log(Body.GetComponent<BodyBehavior>().InstallComponent(DummyComp2.transform));
        //Debug.Log(Body.GetComponent<BodyBehavior>().InstallComponent(DummyAug.transform));
        //Debug.Log(Body.GetComponent<BodyBehavior>().InstallComponent(Forearm.transform));

        UIController.GetComponent<UIStuff>().LogStringWithReturn("You are back home.");
        UIController.GetComponent<UIStuff>().LogStringWithReturn("Select an action:\n1. Perform Surgery\n2. Examine Self\n");
        UIController.GetComponent<UIStuff>().DisplayLoggedText();
        UIController.GetComponent<UIStuff>().UpdateInventory();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
