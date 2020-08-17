using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UIStuff : MonoBehaviour
{
    #region Legacy Stuff Not Currently Used
    public GameObject SkeletalMenu;
    public GameObject OuterMenu;
    public GameObject BrainMenu;
    public GameObject GutsMenu;
    public GameObject StatsMenu;

    public GameObject PlayerMenu;
    public GameObject CityMenu;

    public GameObject InvButtonPrefab;
    public GameObject InventoryContent;
    public Transform HideoutInventory;
    public float ButtonSpacing = 10.0f;



    public void OpenSkeletal()
    {
        SkeletalMenu.SetActive(true);
        OuterMenu.SetActive(false);
        BrainMenu.SetActive(false);
        GutsMenu.SetActive(false);
        StatsMenu.SetActive(false);
    }

    public void OpenOuter()
    {
        SkeletalMenu.SetActive(false);
        OuterMenu.SetActive(true);
        BrainMenu.SetActive(false);
        GutsMenu.SetActive(false);
        StatsMenu.SetActive(false);
    }

    public void OpenBrain()
    {
        SkeletalMenu.SetActive(false);
        OuterMenu.SetActive(false);
        BrainMenu.SetActive(true);
        GutsMenu.SetActive(false);
        StatsMenu.SetActive(false);
    }

    public void OpenGuts()
    {
        SkeletalMenu.SetActive(false);
        OuterMenu.SetActive(false);
        BrainMenu.SetActive(false);
        GutsMenu.SetActive(true);
        StatsMenu.SetActive(false);
    }

    public void OpenStats()
    {
        SkeletalMenu.SetActive(false);
        OuterMenu.SetActive(false);
        BrainMenu.SetActive(false);
        GutsMenu.SetActive(false);
        StatsMenu.SetActive(true);
    }

    public void OpenPlayer()
    {
        PlayerMenu.SetActive(true);
        CityMenu.SetActive(false);
    }

    public void OpenCity()
    {
        PlayerMenu.SetActive(false);
        CityMenu.SetActive(true);
    }

    #endregion

    List<string> InventoryList = new List<string>(); // from the text input tutorial
    public TMPro.TMP_Text inventoryText;

    List<string> actionLog = new List<string>(); // from the text input tutorial
    public TMPro.TMP_Text displayText;

    public void LogStringWithReturn(string stringToAdd)
    {
        actionLog.Add(stringToAdd + "\n");
    }

    public void DisplayLoggedText()
    {
        string logAsText = string.Join("", actionLog.ToArray());
        displayText.text = logAsText;
    }

    public void UpdateInventory()
    {
        HideoutInventory = GameObject.Find("Hideout Inventory").transform;
        string ItemsSplitByLine = "CURRENT INVENTORY: \n\n";

        for (int i = 0; i < HideoutInventory.childCount; i++)
        {
            ItemsSplitByLine += (i + 1) + ". " + HideoutInventory.GetChild(i).GetComponent<ComponentBehavior>().Slot.ToUpper() + ": \"" + HideoutInventory.GetChild(i).name + "\".\n";
        }
        inventoryText.text = ItemsSplitByLine;
    }

}
