using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ComponentBehavior : MonoBehaviour
{
    public string ModelName;
    public int CatalogIndex;
    public string FlavorText;
    public int[] Attributes;
    public int State;
    public string Slot;
    public int InstalledNew;
    public bool IsOrganic = false;

    GameObject EntireChalupa;

    public void InitializeHuman(string SlotName)
    {
        EntireChalupa = GameObject.Find("EntireChalupa");

        foreach(string[] partInfo in EntireChalupa.GetComponent<CatalogDropTables>().Tables["HumanPartsDUMMY"])
        {
            if (partInfo[0] == SlotName)
            {
                ModelName = partInfo[2];
                CatalogIndex = int.Parse(partInfo[1]);
                FlavorText = partInfo[3];

                int[] Atts = new int[partInfo.Length - 4];
                for (int h = 4; h < partInfo.Length; h++)
                {
                    //Debug.Log(HumanPartsTable[c, h]);
                    Atts[h - 4] = int.Parse(partInfo[h]);
                }
                Attributes = Atts;

                Slot = SlotName; // This might be improper
                InstalledNew = 1; // This might need to be handled a different way
                State = 2;
                IsOrganic = true;
            }
        }

    }

    public void InitializeRandom(string DropTable) // This will need fixing as the catalog is refined / columns changed.
    {
        EntireChalupa = GameObject.Find("EntireChalupa");
        
        int roll = EntireChalupa.GetComponent<Conductor>().rnd.Next(1, EntireChalupa.GetComponent<CatalogDropTables>().Tables[DropTable].Count);

        //Debug.Log(roll);

        string[] partInfo = EntireChalupa.GetComponent<CatalogDropTables>().Tables[DropTable].ElementAt(roll);

        Slot = partInfo[1];
        ModelName = partInfo[3];
        this.name = ModelName;
        CatalogIndex = int.Parse(partInfo[2]);
        FlavorText = partInfo[4];

        int[] Atts = new int[partInfo.Length - 5];
        for (int h = 5; h < partInfo.Length; h++)
        {
            //Debug.Log(HumanPartsTable[c, h]);
            Atts[h - 5] = int.Parse(partInfo[h]);
        }
        Attributes = Atts;

        InstalledNew = 1; // This might need to be handled a different way
        State = 2;
        if (int.Parse(partInfo[36]) == 1)
        {
            IsOrganic = true;
        }

        //Debug.Log("Initialized a " + this.name);
    }

    public void InitializeSpecific(string Model)
    {
        EntireChalupa = GameObject.Find("EntireChalupa");
        
        foreach (string[] partInfo in EntireChalupa.GetComponent<CatalogDropTables>().Tables["Master Catalog"])
        {
            if (partInfo[3] == Model)
            {
                Slot = partInfo[1];
                ModelName = partInfo[3];
                this.name = ModelName;
                CatalogIndex = int.Parse(partInfo[2]);
                FlavorText = partInfo[4];

                int[] Atts = new int[partInfo.Length - 5];
                for (int h = 5; h < partInfo.Length; h++)
                {
                    //Debug.Log(HumanPartsTable[c, h]);
                    Atts[h - 5] = int.Parse(partInfo[h]);
                }
                Attributes = Atts;

                InstalledNew = 1; // This might need to be handled a different way
                State = 2;
                if (int.Parse(partInfo[36]) == 1)
                {
                    IsOrganic = true;
                }
            }
        }

        Debug.Log("Initialized a " + this.name);
    }

}
