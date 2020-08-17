using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BodyBehavior : MonoBehaviour
{
    // NOTES
    // In the final build, it would be best for lopping off limbs to remove all subcomponents
    // Since a nerve or electrical interface component would be needed (for limbs at least)
    // A separate ban list for lopping off limbs might be nice, to allows players to gimp themselves
    // The install component feature should only be used for body generation. Surgeries will need separate functions and be more specified.

    public Transform slotPrefab;
    GameObject EntireChalupa;
    GameObject HideoutInventory;

    public Dictionary<string, int[]> ComponentAttributes = new Dictionary<string, int[]>();
    public Dictionary<string, int> ComponentStates = new Dictionary<string, int>();

    public List<string> BanList = new List<string>
    {
        "Human Brainstem", "Human Cerebellum", "Human Frontal Lobe", "Human Parietal Lobe", "Human Occipital Lobe",
        "Human Temporal Lobe", "Human Hypothalmus", "Human Pituitary Gland", "Human Pineal Gland", "Human Thalamus",
        "Human Basal Ganglia", "Human Limbic System", "Human Right Humerus", "Human Left Humerus",
        "Human Right Radius and Ulna", "Human Left Radius and Ulna", "Human Right Femur", "Human Left Femur",
        "Human Right Tibia and Fibula", "Human Left Tibia and Fibula", "Human Rib Cage", "Human Skull",
        "Human Pelvis", "Human Spine", "Human Right Upper Arm", "Human Left Upper Arm", "Human Right Forearm",
        "Human Left Forearm", "Human Right Hand", "Human Left Hand", "Human Right Thigh", "Human Left Thigh",
        "Human Right Crus", "Human Left Crus", "Human Right Foot", "Human Left Foot"
    };

    #region Attribute Initialization
    public int MOL = 0;
    public int MAL = 0;
    public int COL = 0;
    public int CAL = 0;
    public int LeftPress = 0;
    public int LeftPressCap = 0;
    public int RightPress = 0;
    public int RightPressCap = 0;
    public int TotalPress = 0;
    public int TotalPressCap = 0;
    public int LeftSquat = 0;
    public int RightSquat = 0;
    public int TotalSquat = 0;
    public int LeftGrip = 0;
    public int RightGrip = 0;
    public int TotalGrip = 0;
    public int Speed = 0;
    public int Agility = 0;
    public int LeftDeftness = 0;
    public int RightDeftness = 0;
    public int TotalDeftness = 0;
    public int Alertness = 0;
    public int Reflexes = 0;
    public int SpatialAwareness = 0;
    public int StressResponse = 0;
    public int PainTolerance = 0;
    public int EmotionalControl = 0;
    public int CriticalThinking = 0;
    public int CharmRating = 0;
    public int ThreatRating = 0;
    public int Diplomacy = 0;
    public int Wakefulness = 0;
    public int Metabolism = 0;
    public int VisualDistance = 0;
    public int VisualDetail = 0;
    public int VisionInfrared = 0;
    public int VisionUltraviolet = 0;
    public int VisionXRay = 0;
    public int VisionLowLight = 0;
    #endregion

    #region Initializing and Whole Body Functions

    public void HumanizeBody()
    {
        foreach (Transform child in this.transform)
        {
            GameObject.Destroy(child.gameObject); // Kill everything
        }
        InitializeBody();
    }

    public void InitializeBody() //Bodies will always initialize as completely human.
    {
        EntireChalupa = GameObject.Find("EntireChalupa"); // Grab chalupa
        HideoutInventory = GameObject.Find("Hideout Inventory"); // Should this be here?

        List<string[]> SlotTemplateTable = EntireChalupa.GetComponent<CatalogDropTables>().Tables["Slot_Templates"]; //Grab the drop dictionary, find Slots List
                
        for (int a = 1; a < SlotTemplateTable.Count(); a++) //Start at one to skip the header.
        {
            InitializeSlot(SlotTemplateTable[a]); //Fill the body with each slot
        }

        UpdateBodyAttributes();

    }

    void InitializeSlot(string[] SlotInfo) // Initialize the body with slots (iterate this to create all slots)
    {
        Transform NewSlot = Instantiate(slotPrefab); // Create a new slot transform
        NewSlot.transform.parent = this.transform; // Make it a child of this transform (the body)
        NewSlot.name = SlotInfo[1]; // Name it
        NewSlot.GetComponent<SlotBehavior>().FullName = SlotInfo[1];
        NewSlot.GetComponent<SlotBehavior>().FlavorText = SlotInfo[2];
        NewSlot.GetComponent<SlotBehavior>().IsAugSlot = int.Parse(SlotInfo[3]);
        NewSlot.GetComponent<SlotBehavior>().MajorGroup = SlotInfo[4];
        NewSlot.GetComponent<SlotBehavior>().SubGroup = SlotInfo[5];
        NewSlot.GetComponent<SlotBehavior>().ProtectingSlot = SlotInfo[6];
        NewSlot.GetComponent<SlotBehavior>().SurgicalParent = SlotInfo[7];
        NewSlot.GetComponent<SlotBehavior>().ParentNeed = SlotInfo[8];
        NewSlot.GetComponent<SlotBehavior>().HumanComponent();
    }

    #endregion

    #region Attribute Pulling
    
    int[] GetSlotAttributes(Transform slot)
    {
        Transform CurrentComponent;

        if (slot.transform.childCount != 0)
        {
            CurrentComponent = slot.transform.GetChild(0); //Grabs the first child (which should be the only child)
            return CurrentComponent.GetComponent<ComponentBehavior>().Attributes;
        } else
        {
            return null;
        }
    }

    int GetSlotState(Transform slot)
    {
        Transform CurrentComponent;

        if (slot.transform.childCount != 0)
        {
            CurrentComponent = slot.transform.GetChild(0); //Grabs the first child (which should be the only child)
            return CurrentComponent.GetComponent<ComponentBehavior>().State;
        }
        else
        {
            return 0;
        }
    }

    public void UpdateBodyAttributes()
    {
        List<int[]> AttributeTable = new List<int[]>();
        ComponentAttributes.Clear(); //Clear out the dictionaries
        ComponentStates.Clear();

        foreach (Transform child in this.transform) 
        {
            ComponentAttributes.Add(child.name, GetSlotAttributes(child));
            ComponentStates.Add(child.name, GetSlotState(child));
        }

        PullMOL();
        PullMAL();
        PullCOL();
        PullCAL();
        PullPress();
        PullSquat();
        PullGrip();
        PullSpeed();
        PullAgility();
        PullDeft();
        PullAlert();
        PullReflex();
        PullSpatial();
        PullStress();
        PullPainTol();
        PullEmo();
        PullCritThink();
        PullCharm();
        PullThreat();
        PullDiplo();
        PullWake();
        PullMetabol();
        PullVisDist();
        PullVisDet();
        PullSpectra();
        PullVLit();

    }

    int WAtt(string s, int i)
    {
        int watt;
        int nullCheck;
        if (ComponentAttributes[s] == null)
        {
            nullCheck = 0;
        }
        else
        {
            nullCheck = ComponentAttributes[s][i];
        }
        watt = (int)(nullCheck * 0.5 * ComponentStates[s]);
        
        return watt;
    }
    
    void PullMOL() // Lungs&Aug or heart&Aug, whichever is smaller. Lungs provide oxygen, heart must supply it to the body
    {
        int i = 0; // This number is the index of the MOL attribute in the attributes array

        MOL = System.Math.Min(WAtt("Heart", i) + WAtt("Heart Augmentation", i), WAtt("Lungs", i) + WAtt("Lung Augmentation", i));
    }

    void PullMAL() //Spine&Aug or heart&Aug, whichever is smaller. Then add battery value.
    {
        int i = 1;

        if ((WAtt("Spine", i) + WAtt("Spinal Augmentation", i)) > (WAtt("Heart", i) + WAtt("Heart Augmentation", i)))
        {
            MAL = WAtt("Heart", i) + WAtt("Heart Augmentation", i) + WAtt("Battery", i);
        }
        else
        {
            MAL = WAtt("Spine", i) + WAtt("Spinal Augmentation", i) + WAtt("Battery", i);
        };
    }

    void PullCOL() // Sum of organic load requirement
    {
        int i = 2;
        COL = 0;
        foreach (KeyValuePair<string, int[]> s in ComponentAttributes)
        {
            COL += WAtt(s.Key, i);
        }
    }

    void PullCAL() // Sum of augmentation load requirements
    {
        int i = 3;
        CAL = 0;
        foreach (KeyValuePair<string, int[]> s in ComponentAttributes)
        {
            CAL += WAtt(s.Key, i);
        }
    }

    void PullPress()
    {
        int i = 4;
        int org = 31; // Is organic where 1 is YES
        int cap;
        int armcap;
        int leftcap;
        int rightcap;
        
        int lpresspot = (int)(WAtt("Left Upper Arm", i) + WAtt("Left Forearm", i) + WAtt("Adrenal Gland", i) + WAtt("Adrenal Gland Augmentation", i)); // The maximum output possible before capping
        int rpresspot = (int)(WAtt("Right Upper Arm", i) + WAtt("Right Forearm", i) + WAtt("Adrenal Gland", i) + WAtt("Adrenal Gland Augmentation", i)); // The maximum output possible before capping
        int presspot = lpresspot + rpresspot;

        if (this.transform.Find("Left Upper Arm").GetComponentInChildren<ComponentBehavior>().Attributes[org] == 0) // If the upper arm is robotic it means both parts are robotic
        {
            leftcap = 0;
            LeftPress = lpresspot;
        }
        else
        {
            if (this.transform.Find("Left Forearm").GetComponentInChildren<ComponentBehavior>().Attributes[org] == 0)
            {
                leftcap = WAtt("Left Humerus", i); // If the upper arm is human but the forearm is robotic
                LeftPress = System.Math.Min(lpresspot, leftcap);
            }
            else
            {
                leftcap = System.Math.Min(WAtt("Left Humerus", i), WAtt("Left Radius and Ulna", i)); // If both parts are human
                LeftPress = System.Math.Min(lpresspot, leftcap);
            }
        }


        if (this.transform.Find("Right Upper Arm").GetComponentInChildren<ComponentBehavior>().Attributes[org] == 0) // If the upper arm is robotic it means both parts are robotic
        {
            rightcap = 0;
            RightPress = rpresspot;
        }
        else
        {
            if (this.transform.Find("Right Forearm").GetComponentInChildren<ComponentBehavior>().Attributes[org] == 0)
            {
                rightcap = WAtt("Right Humerus", i); // If the upper arm is human but the forearm is robotic
                RightPress = System.Math.Min(rpresspot, rightcap);
            }
            else
            {
                rightcap = System.Math.Min(WAtt("Right Humerus", i), WAtt("Right Radius and Ulna", i)); // If both parts are human
                RightPress = System.Math.Min(rpresspot, rightcap);
            }
        }


        if (leftcap == 0) //Left arm is robotic
        {
            if (rightcap == 0) //Right arm is robotic
            {
                armcap = 0;
            }
            else // Left is robotic but right is not
            {
                armcap = rightcap;
            }
        }
        else
        {
            if (rightcap == 0) // Left arm is at least partially organic but right arm is robotic
            {
                armcap = leftcap;
            }
            else // Both arms are at least partially organic
            {
                armcap = (rightcap + leftcap) / 2; // Average of the arms minimums
            }
        }

        cap = System.Math.Min(WAtt("Rib Cage", i), armcap); // Weakest point between the ribs and arm bones

        TotalPress = System.Math.Min(cap, presspot); // If your press potential is above the cap, choose the cap.        
    }

    void PullSquat()
    {
        int i = 5;
        int org = 31; // Is organic where 1 is YES
        int cap;
        int legcap;
        int lbcap;
        int leftcap;
        int rightcap;
        
        int rsquatpot = (int)(WAtt("Right Thigh", i) + WAtt("Right Crus", i) + WAtt("Adrenal Gland", i) + WAtt("Adrenal Gland Augmentation", i));
        int lsquatpot = (int)(WAtt("Left Thigh", i) + WAtt("Left Crus", i) + WAtt("Adrenal Gland", i) + WAtt("Adrenal Gland Augmentation", i));
        int squatpot = rsquatpot + lsquatpot;

        if (this.transform.Find("Left Thigh").GetComponentInChildren<ComponentBehavior>().Attributes[org] == 0) // If the thigh is robotic it means both parts are robotic
        {
            leftcap = 0;
            LeftSquat = lsquatpot;
        }
        else
        {
            if (this.transform.Find("Left Crus").GetComponentInChildren<ComponentBehavior>().Attributes[org] == 0 )
            {
                leftcap = WAtt("Left Femur", i); // If the thigh is human but the crus is robotic
                LeftSquat = System.Math.Min(lsquatpot, leftcap);
            }
            else
            {
                leftcap = System.Math.Min(WAtt("Left Femur", i), WAtt("Left Tibia and Fibula", i)); // If both parts are human
                LeftSquat = System.Math.Min(lsquatpot, leftcap);
            }
        }


        if (this.transform.Find("Right Thigh").GetComponentInChildren<ComponentBehavior>().Attributes[org] == 0) // If the thigh is robotic it means both parts are robotic
        {
            rightcap = 0;
            RightSquat = rsquatpot;
        }
        else
        {
            if (this.transform.Find("Right Crus").GetComponentInChildren<ComponentBehavior>().Attributes[org] == 0)
            {
                rightcap = WAtt("Right Femur", i); // If the thigh is human but the crus is robotic
                RightSquat = System.Math.Min(rsquatpot, rightcap);
            }
            else
            {
                rightcap = System.Math.Min(WAtt("Right Femur", i), WAtt("Right Tibia and Fibula", i)); // If both parts are human
                RightSquat = System.Math.Min(rsquatpot, rightcap);
            }
        }


        if (leftcap == 0) //Left leg is robotic
        {
            if (rightcap == 0) //Right leg is robotic
            {
                legcap = 0;
            }
            else // Left is robotic but right is not
            {
                legcap = rightcap;
            }
        }
        else
        {
            if (rightcap == 0) // Left leg is at least partially organic but right leg is robotic
            {
                legcap = leftcap;
            }
            else // Both are at least partially organic
            {
                legcap = (rightcap + leftcap) / 2; // Average of the arms minimums
            }
        }

        lbcap = System.Math.Min(WAtt("Pelvis", i), legcap); // Weakest point between the pelvis and leg bones

        cap = System.Math.Min(lbcap, WAtt("Spine", i)); // Weakest point between the entire lower body and the spine

        TotalSquat = System.Math.Min(cap, squatpot); // If your squat potential is above the cap, choose the cap.

    }

    void PullGrip()
    {
        int i = 6;
        int org = 31;
        int lcap;
        int rcap;
                
        int lgrip = (int)(WAtt("Left Forearm", i) + WAtt("Adrenal Gland", i) + WAtt("Adrenal Gland Augmentation", i));
        int rgrip = (int)(WAtt("Right Forearm", i) + WAtt("Adrenal Gland", i) + WAtt("Adrenal Gland Augmentation", i));
        int grippot = lgrip + rgrip;


        if (this.transform.Find("Left Forearm").GetComponentInChildren<ComponentBehavior>().Attributes[org] == 0) // if the left forearm is robotic
        {
            lcap = WAtt("Left Hand", i);
            LeftGrip = System.Math.Min(lcap, lgrip);
        }
        else // If the left forearm is organic
        {
            lcap = System.Math.Min(WAtt("Left Hand", i), WAtt("Left Radius and Ulna", i)); // Does this even make any sense?
            LeftGrip = System.Math.Min(lcap, lgrip);
        }

        if (this.transform.Find("Right Forearm").GetComponentInChildren<ComponentBehavior>().Attributes[org] == 0) // if the right forearm is robotic
        {
            rcap = WAtt("Right Hand", i);
            RightGrip = System.Math.Min(rcap, rgrip);
        }
        else // If the right forearm is organic
        {
            rcap = System.Math.Min(WAtt("Right Hand", i), WAtt("Right Radius and Ulna", i)); // Does this even make any sense?
            RightGrip = System.Math.Min(rcap, rgrip);
        }
        
        TotalGrip = RightGrip + LeftGrip;
    }

    void PullSpeed() // Affected by legs (not bones) and adrenal
    {
        int i = 7;
        float lft = 0.5f;
        float rft = 0.5f;
        Speed = 0;

        if (this.transform.Find("Left Foot").childCount == 1)
        {
            lft = 1.0f;
        }

        if (this.transform.Find("Right Foot").childCount == 1)
        {
            rft = 1.0f;
        }

        foreach (KeyValuePair<string, int[]> s in ComponentAttributes)
        {
            Speed += WAtt(s.Key, i);
        }

        Speed = (int)(Speed * lft * rft);
    }

    void PullAgility()
    {
        int i = 8;
        Agility = 0;

        foreach (KeyValuePair<string, int[]> s in ComponentAttributes)
        {
            Agility += WAtt(s.Key, i);
        }

        Agility -= WAtt("Cerebellum", i) + WAtt("Cerebellum Augmentation", i); // Remove the ratings from the cerebellum since it is a cap value

        Agility = System.Math.Min(Agility, WAtt("Cerebellum", i) + WAtt("Cerebellum Augmentation", i)); // Cerebellum determines cap
    }

    void PullDeft()
    {
        int i = 9;

        LeftDeftness = System.Math.Min(WAtt("Left Hand", i), WAtt("Basal Ganglia", i) + WAtt("Basal Ganglia Augmentation", i));
        RightDeftness = System.Math.Min(WAtt("Right Hand", i), WAtt("Basal Ganglia", i) + WAtt("Basal Ganglia Augmentation", i));
        TotalDeftness = (LeftDeftness + RightDeftness) / 2; // Average of the hands or the maximum the basal ganglia can control. Will return zero if no hands.
    }

    void PullAlert() // This could be improved to be more lenient. Currently halves awareness at ANY sensory damage
    {
        int i = 10;

        float earstate = System.Math.Max(0.5f, ComponentStates["Ears"] / 2.0f);
        float leyst = System.Math.Max(0.5f, ComponentStates["Left Eye"] / 2.0f);
        float reyst = System.Math.Max(0.5f, ComponentStates["Right Eye"] / 2.0f);
        
        Alertness = (int)((WAtt("Thalamus", i) + WAtt("Thalamus Augmentation", i)) * leyst * reyst * earstate); //Thalamus determines cap alertness
    }

    void PullReflex() // Affected by spine and adrenal
    {
        int i = 11;
        Reflexes = 0;

        foreach (KeyValuePair<string, int[]> s in ComponentAttributes) // Probably just the Spine with an adrenal boost
        {
            Reflexes += WAtt(s.Key, i);
        }
    }

    void PullSpatial() // Why is alertness dependant on sensory states but not this? Shouldn't the lobe set the value and the senses act as booleans?
    {
        int i = 12;

        SpatialAwareness = System.Math.Min(WAtt("Left Eye", i) + WAtt("Right Eye", i) + WAtt("Ears", i), WAtt("Parietal Lobe", i) + WAtt("Parietal Lobe Augmentation", i)); // Spatial sense is capped by the parietal lobe
    }

    void PullStress()
    {
        int i = 13;
        StressResponse = WAtt("Pituitary Gland", i) + WAtt("Pituitary Gland Augmentation", i) + WAtt("Adrenal Gland", i) + WAtt("Adrenal Gland Augmentation", i);
    }

    void PullPainTol()
    {
        int i = 14;
        int braintol = (WAtt("Parietal Lobe", i) + WAtt("Parietal Lobe Augmentation", i) + WAtt("Thalamus", i) + WAtt("Thalamus Augmentation", i)) / 2; //Average between these components
        PainTolerance = System.Math.Max(WAtt("Spine", i) + WAtt("Spinal Augmentation", i), braintol) + WAtt("Adrenal Gland", i) + WAtt("Adrenal Gland Augmentation", i); // Either the spine or the brain handles pain, then adrenal gland supplies a boost
    }

    void PullEmo()
    {
        int i = 15;
        EmotionalControl = (WAtt("Frontal Lobe", i) + WAtt("Frontal Lobe Augmentation", i) + WAtt("Limbic System", i) + WAtt("Limbic System Augmentation", i)) / 2; // Average between frontal and limbic systems
    }

    void PullCritThink()
    {
        int i = 16;
        CriticalThinking = (WAtt("Frontal Lobe", i) + WAtt("Frontal Lobe Augmentation", i) + WAtt("Temporal Lobe", i) + WAtt("Temporal Lobe Augmentation", i)) / 2; // Average between frontal and temporal
    }

    void PullCharm() // Should this remove charm for mutilations and defects?
    {
        int i = 17;
        CharmRating = 0;
        foreach (KeyValuePair<string, int[]> s in ComponentAttributes) // No caps on charm, anything can add charm
        {
            CharmRating += WAtt(s.Key, i);
        }
    }

    void PullThreat()
    {
        int i = 18;
        ThreatRating = 0;
        foreach (KeyValuePair<string, int[]> s in ComponentAttributes) // No caps on threat, anything can add threat
        {
            ThreatRating += WAtt(s.Key, i);
        }
    }

    void PullDiplo() // Maybe ears and voice should also be boosters for this instead of just checks?
    {
        int i = 19;
        float earstate = System.Math.Max(0.5f, ComponentStates["Ears"] / 2.0f); // Can still communicate with paper / charades
        float jawstate = System.Math.Max(0.5f, ComponentStates["Jaw"] / 2.0f); // Can still communicate without a jaw
        float voicestate = System.Math.Max(0.5f, ComponentStates["Vocal Cords"] / 2.0f);
            // Not counting vocal cord aug since it's simply a measure of whether you can communicate
        float lungstate = ComponentStates["Lungs"] / 2.0f; // Hopefully this shouldn't be necessary
            // Not including lung aug since it's simply a check if you can move air
        
        Diplomacy = (int)(System.Math.Min(WAtt("Frontal Lobe", i) + WAtt("Frontal Lobe Augmentation", i), 
            (WAtt("Temporal Lobe", i) + WAtt("Temporal Lobe Augmentation", i) + WAtt("Parietal Lobe", i) + WAtt("Parietal Lobe Augmentation", i)) / 2) 
            * jawstate * voicestate * earstate * lungstate);
        // Average between parietal and temporal, capped by frontal and then modified by the states of jaw, ears, voice, lungs
    }

    void PullWake()
    {
        int i = 20;

        Wakefulness = 0;
        foreach (KeyValuePair<string, int[]> s in ComponentAttributes) // Usually just the brainstem, hypothalamus and pineal gland
        {
            Wakefulness += WAtt(s.Key, i);
        }
    }

    void PullMetabol()
    {
        int i = 21;
        Metabolism = 0;

        float stomst = System.Math.Max(0.5f, ComponentStates["Stomach"] / 2.0f);
        float gutst = System.Math.Max(0.5f, ComponentStates["Intestines"] / 2.0f);
        float thyst = System.Math.Max(0.5f, ComponentStates["Thyroid"] / 2.0f);
                
        foreach (KeyValuePair<string, int[]> s in ComponentAttributes)
        {
            Metabolism += WAtt(s.Key, i);
        }

        Metabolism = (int)(Metabolism * stomst * gutst * thyst);
    }

    void PullVisDist()
    {
        int i = 22;
        VisualDistance = System.Math.Max(WAtt("Right Eye", i), WAtt("Left Eye", i)) + WAtt("Face", i); //Best eye plus face augment if applicable
    }

    void PullVisDet()
    {
        int i = 23;
        int visdetpot = System.Math.Max(WAtt("Right Eye", i), WAtt("Left Eye", i)); //Best eye plus face augment if applicable
        VisualDetail = System.Math.Min(WAtt("Occipital Lobe", i) + WAtt("Occipital Lobe Augmentation", i), visdetpot) + WAtt("Face", i); // Detail potential capped by what the occipital lobe can process
    }

    void PullSpectra()
    {
        int virn = 24;
        int vuvn = 25;
        int vxrn = 26;

        VisionInfrared = (WAtt("Occipital Lobe", virn) | WAtt("Occipital Lobe Augmentation", virn) & WAtt("Right Eye", virn) | WAtt("Left Eye", virn)); // If the occipital can intake IR and at least one eye provides IR signal
        VisionUltraviolet = (WAtt("Occipital Lobe", vuvn) | WAtt("Occipital Lobe Augmentation", vuvn) & WAtt("Right Eye", vuvn) | WAtt("Left Eye", vuvn));
        VisionXRay = (WAtt("Occipital Lobe", vxrn) | WAtt("Occipital Lobe Augmentation", vxrn) & WAtt("Right Eye", vxrn) | WAtt("Left Eye", vxrn));
    }

    void PullVLit()
    {
        int i = 27;
        VisionLowLight = System.Math.Max(WAtt("Right Eye", i), WAtt("Left Eye", i)) + WAtt("Face", i) + WAtt("Skull", i); // Whichever is better as seeing in the dark, and bonuses from face/skull augs
    }

    #endregion

    #region Surgery and Component Swapping

    public void SwapComponentsNoCheck(Transform component)
    {
        Transform TargetSlot = this.transform.Find(component.GetComponent<ComponentBehavior>().Slot); // Note where it installs to

        Debug.Log("The swapper was asked to put a " + component.name + " into the " + TargetSlot.name + " slot.");

        if(TargetSlot.childCount != 0) // If it's installing in place of something else
        {
            foreach (Transform child in TargetSlot)
            {
                child.parent = HideoutInventory.transform; // Remove the item it is replacing
            }
        }
        component.parent = TargetSlot; // Put it in there
        UpdateBodyAttributes();
    }

    public string InstallComponent(Transform replacement) // Consider returning a short string, which another void can read the full text for via dictionary
    {
        Transform TargetSlot = this.transform.Find(replacement.GetComponent<ComponentBehavior>().Slot);
        Transform OldComponent;
        bool HasDependent = false;
        Transform SurgicalDependentSlot = null;
        Transform OldSurgicalDependentComponent = null;
        
        Debug.Log("Installing the " + replacement.name);

        // Note the surgical dependent for the swap and value check
        foreach (Transform slot in this.transform) // In the body
        {
            if (slot.GetComponent<SlotBehavior>().SurgicalParent == replacement.GetComponent<ComponentBehavior>().Slot) // If it's surgical parent is this part
            {
                SurgicalDependentSlot = slot; // Note the dependent
                Debug.Log("This part's surgical child slot is " + slot.name);
                if (SurgicalDependentSlot.childCount != 0)
                {
                    OldSurgicalDependentComponent = SurgicalDependentSlot.GetChild(0);
                    Debug.Log("The designated surgical child is the " + OldSurgicalDependentComponent.name);
                    HasDependent = true;
                    if (this.transform.Find(OldSurgicalDependentComponent.GetComponent<ComponentBehavior>().Slot).GetComponent<SlotBehavior>().MajorGroup == "SKELETAL")
                    {
                        break; // Prioritize 
                    }                    
                }
            }
        }

        #region Parent Eligibility Check
        Debug.Log("Running parent check...");
        if(TargetSlot.GetComponent<SlotBehavior>().SurgicalParent != "None") // If this component relies on another existing
        {
            if(this.transform.Find(TargetSlot.GetComponent<SlotBehavior>().SurgicalParent).childCount != 1) // If that slot is empty
            {
                return "NO PARENT";
            }
            if (TargetSlot.GetComponent<SlotBehavior>().ParentNeed == "Augmented") // If it's a battery
            {
                if(!this.transform.Find("Spine").GetChild(0).GetComponent<ComponentBehavior>().IsOrganic || 
                    this.transform.Find("Spinal Augmentation").childCount != 0) // If the spine is robotic, or you have no spinal augmentation
                {
                    // Continue
                } else
                {
                    return "BATTERY ON ORGANIC"; // Stop the installation here.
                }
            }
            if (TargetSlot.GetComponent<SlotBehavior>().ParentNeed == "Organic") // If it goes on an organic part and
            {
                if(!this.transform.Find(TargetSlot.GetComponent<SlotBehavior>().SurgicalParent).GetChild(0).GetComponent<ComponentBehavior>().IsOrganic) // That part is not organic
                {
                    return "AUGMENT ON ROBOTIC";
                }
            }
        }
        Debug.Log("Parent check passed.");
        #endregion

        #region Ban List Check
        Debug.Log("Running ban check...");
        if (BanCheck(replacement) != "OK")
        {
            return BanCheck(replacement);
        }
        Debug.Log("Ban check passed.");
        #endregion

        #region Install (Replacing Existing)
        if (TargetSlot.childCount != 0) // If the target slot has something in it
        {
            Debug.Log("Target slot has an item in it currently. It is a " + TargetSlot.GetChild(0).name);

            OldComponent = TargetSlot.GetChild(0); //Note the old child
            if (HasDependent && OldComponent.GetComponent<ComponentBehavior>().IsOrganic != replacement.GetComponent<ComponentBehavior>().IsOrganic) // If switching robotic/non
            {
                OldSurgicalDependentComponent.parent = HideoutInventory.transform; //  Take off the dependant
                UpdateBodyAttributes();
                Debug.Log("The " + OldSurgicalDependentComponent.name + " was also removed as a related component.");
            }
            SwapComponentsNoCheck(replacement); // Swap the main components
            
            Debug.Log("The parts have been switched. Running Post-Swap Error Check");
            string ErrorMessageChild = ErrorCheckPostSwap();

            if (ErrorMessageChild != "OK") // If there was an error
            {
                Debug.Log("Error check returned an error: " + ErrorMessageChild + ". Switching parts back.");
                SwapComponentsNoCheck(OldComponent); // Swap them back
                if (HasDependent)
                {
                    OldSurgicalDependentComponent.parent = SurgicalDependentSlot; // Put the dependent back on
                    UpdateBodyAttributes();
                    Debug.Log(OldSurgicalDependentComponent.name + " was re-installed to the " + SurgicalDependentSlot.name + " slot as well.");
                }
                return ErrorMessageChild; // Push the error code
            } else
            {
                Debug.Log("Error check found no issue. New component is installed.");
                return "OK";
            }
        }
        #endregion

        #region Install (Into Empty)
        // If there is no component already in the target slot (therefore no dependent)
        SwapComponentsNoCheck(replacement); 
        string errormessage = ErrorCheckPostSwap();
        if (errormessage != "OK") // If there was an error
        {
            Debug.Log("Error check returned an error: " + errormessage + ". Switching parts back.");
            replacement.parent = HideoutInventory.transform;
            UpdateBodyAttributes();
            return errormessage;
        }
        else
        {
            Debug.Log("Error check found no issue. New component is installed.");
            return "OK";
        }
        #endregion
    }

    string BanCheck(Transform component)
    {
        foreach (string str in BanList)
        {
            if (component.name == str)
            {
                return "BANLIST";
            };
        }
        return "OK";
    }

    string ErrorCheckPostSwap() // Place surgical checks here
    {

        #region Limb Gap Check

        foreach(Transform child in this.transform)
        {
            if(child.GetComponent<SlotBehavior>().SubGroup == "RIGHT ARM" ||  // If it's a limb
               child.GetComponent<SlotBehavior>().SubGroup == "LEFT ARM" ||  // This technically needs a skeletal check, but an inorganic above a skeletal part shouldn't happen.
               child.GetComponent<SlotBehavior>().SubGroup == "RIGHT LEG" ||
               child.GetComponent<SlotBehavior>().SubGroup == "LEFT LEG" ||
               child.GetComponent<SlotBehavior>().SubGroup == "RIGHT HAND" ||
               child.GetComponent<SlotBehavior>().SubGroup == "LEFT HAND")
            {
                //Debug.Log("Found a limb slot! It's a " + child.name);
                //if(child.GetComponent<SlotBehavior>().MajorGroup != "SKELETAL") // and it's not a bone
                //{ KEEP THESE TWO LINES COMMENTED OUT FOR NOW - OR IT WILL MISS THAT BONES ARE RELATED TO LIMBS
                    //Debug.Log("It's not a skeletal slot!");
                    if (child.childCount != 0) // and it's not missing
                    {
                        //Debug.Log("Wow! There's something in the slot!");
                        //Debug.Log("It's a " + child.GetChild(0).name + "!!!");
                        if (child.GetChild(0).GetComponent<ComponentBehavior>().IsOrganic) // and it's organic
                        {
                            //Debug.Log("This is an organic part! Wowzers!");
                            if (child.GetComponent<SlotBehavior>().SurgicalParent != "None") // and it has a parent
                            {
                                //Debug.Log("This slot has a surgical parent, which is the " + child.GetComponent<SlotBehavior>().SurgicalParent + "!");
                                if (!this.transform.Find(child.GetComponent<SlotBehavior>().SurgicalParent).GetChild(0).GetComponent<ComponentBehavior>().IsOrganic) // and the parent aint organic
                                {
                                    //Debug.Log("Wait a minute... that part is robotic! That's no good!");
                                    return "LIMB GAP";
                                }
                            }                            
                        }
                    }
                //}                 
            }
        }

        #endregion

        #region Augment Onto Robotic Check
        foreach(Transform child in this.transform)
        {
            if(child.childCount != 0)
            {
                if (child.name.Contains("Augmentation")) //If it is an augmentation
                {
                    Debug.Log("Checking " + child.name + " slot for an organic parent...");
                    if (!this.transform.Find(child.GetComponent<SlotBehavior>().SurgicalParent).GetChild(0).GetComponent<ComponentBehavior>().IsOrganic)
                    {
                        return "AUG ON ROBOTIC";
                    }
                }
            }
        }
        #endregion

        #region Organic and Augmentation Load Checks
        if (MOL < COL)
        {
            string errormessage = "ORGANIC OVERLOAD " + (COL - MOL);
            return errormessage;
        }

        if (MAL < CAL)
        {
            string errormessage = "AUGMENTATION OVERLOAD " + (CAL - MAL);
            return errormessage;
        }
        #endregion

        // Passed all checks at this point.
        return "OK";
    }

    public string RemoveIntoInventory(Transform component)
    {
        string errormessage = BanCheck(component);
        if (errormessage != "OK")
        {
            return errormessage;
        }
        else
        {
            component.parent = HideoutInventory.transform;
            return "OK";
        }
    }

    #endregion

    #region Body Checks

    int Pain()
    {
        int pain = 0;

        foreach (Transform child in this.transform)
        {
            if (child.childCount != 0)
            {
                if (child.GetChild(0).GetComponent<ComponentBehavior>().IsOrganic) //Currently installed organic components
                {
                    pain += (2 - child.GetChild(0).GetComponent<ComponentBehavior>().State); // 1x for damaged, 2x for broken
                    // This means 5 broken items KO's a normal human, and perfect pain tolerance can handle nearly the entire organic body
                }
            }
        }

        return pain;
    }

    int BrainDamage()
    {
        int BrainDamage = 0;

        foreach (Transform child in this.transform)
        {
            if (child.GetComponent<SlotBehavior>().MajorGroup == "BRAIN" && !child.GetComponent<SlotBehavior>().FullName.Contains("Augmentation")) //Brain parts.
            {
                BrainDamage += (2 - child.GetChild(0).GetComponent<ComponentBehavior>().State); // 1x for damaged, 2x for broken
                // Up to 24 total points of brain damage.
            }
        }

        return BrainDamage;
    }

    string IsConscious()
    {
        

        if (PainTolerance < Pain())
        {
            return "PAIN";
        }

        if (MOL < COL) // If your lungs can't keep up. This could later become an oxygen deficit check.
        {
            return "OXYGEN";
        }        

        if(BrainDamage() > 8) // Knocked unconscious (major damage to brain)
        {
            return "BRAIN";
        }

        return "TRUE";
    }

    string IsAlive()
    {
        if(this.transform.Find("Heart").GetChild(0).GetComponent<ComponentBehavior>().State == 0) // Die of a broken heart
        {
            return "HEART";
        }

        if(BrainDamage() > 16) // Brained
        {
            return "BRAIN";
        }

        if(Pain() > PainTolerance * 2)
        {
            return "DAMAGE";
        }

        return "TRUE";
    }
    #endregion

}
