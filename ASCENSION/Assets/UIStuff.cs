using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UIStuff : MonoBehaviour
{
    public GameObject SkeletalMenu;
    public GameObject OuterMenu;
    public GameObject BrainMenu;
    public GameObject GutsMenu;
    public GameObject StatsMenu;

    public GameObject PlayerMenu;
    public GameObject CityMenu;
    
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

}
