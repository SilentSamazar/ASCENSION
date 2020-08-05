using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


//               █████╗ ███████╗ ██████╗███████╗███╗   ██╗███████╗██╗ ██████╗ ███╗   ██╗
//              ██╔══██╗██╔════╝██╔════╝██╔════╝████╗  ██║██╔════╝██║██╔═══██╗████╗  ██║
//              ███████║███████╗██║     █████╗  ██╔██╗ ██║███████╗██║██║   ██║██╔██╗ ██║
//              ██╔══██║╚════██║██║     ██╔══╝  ██║╚██╗██║╚════██║██║██║   ██║██║╚██╗██║
//              ██║  ██║███████║╚██████╗███████╗██║ ╚████║███████║██║╚██████╔╝██║ ╚████║
//              ╚═╝  ╚═╝╚══════╝ ╚═════╝╚══════╝╚═╝  ╚═══╝╚══════╝╚═╝ ╚═════╝ ╚═╝  ╚═══╝
//  ╔═╗  ┌─┐┬ ┬┌┐ ┌─┐┬─┐┌─┐┬ ┬┌┐┌┬┌─  ┬─┐┌─┐┌─┐┬ ┬┌─┐  ┬  ┬┬┌─┌─┐  ┌┬┐┬─┐┌─┐┌─┐┌─┐   ┬ ┬┌─┐  ┌─┐┌─┐┌┬┐┌─┐
//  ╠═╣  │  └┬┘├┴┐├┤ ├┬┘├─┘│ ││││├┴┐  ├┬┘│ ││ ┬│ │├┤───│  │├┴┐├┤    ││├┬┘├┤ └─┐└─┐───│ │├─┘  │ ┬├─┤│││├┤ 
//  ╩ ╩  └─┘ ┴ └─┘└─┘┴└─┴  └─┘┘└┘┴ ┴  ┴└─└─┘└─┘└─┘└─┘  ┴─┘┴┴ ┴└─┘  ─┴┘┴└─└─┘└─┘└─┘   └─┘┴    └─┘┴ ┴┴ ┴└─┘

//TODO:
//
// Create a shitload of catalog items
// Create a random damage tester, generate death and incapacitation criteria, fill slots with relationship and vitality information
// Create opponent body and combat functions (maybe do tasks and events first)
// Create pools or some other identifier for what type of equipment is needed to install each component (do later)
// Add something that chops off the front of the Event Log
// Screen for organic parents to prevent augment-on-robotic installation
// Find out why everything under the cruses isnt being brought into the parts list

// SIMON input:
// Use ENUMs for the STATE variable on components
// Use csv to hold all the information about slots, like the flavor text or even names


[System.Serializable]
public class ASCENSION : MonoBehaviour
{
    #region Classes
    
    public class MakeModel //The catalog information for each augmentation (or organic)
    {
        public string Name;
        public string Description; //Flavor text
        public string Slot; //Type of slot this item is allowed in (to prevent installing an arm into ones jaw)
        public int ItemNumber; // Placeholder int used to select the correct row for the attribute serialization (Obsolete?)
        public int[] Attributes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; //Attribute bonuses, also includes reliability information
    };

    public class Component //One specific augment/organic 
    {
        public int State; // 0 is Broken, 1 is malfunctioning or 2 is working
        public int OperatingWeeks; // Time in weeks item has spent operating. This will initialize at 0 for new augments and be random for scavenged ones based on the makemodel shelf life (reliability curve)
        public int InstalledNew;
        public MakeModel MakeModel;
        public int CompID;
    };

    public class Slot // Places of the players body which can be augmented
    {
        public string Name;
        public string Desc;
        public Component ActiveComponent; //NONE as a component type ? Or can this be handled as NULL?
        public int Dependent;
        public string Owner;
        public int NeedsOrg;
    };

    public class Attribute
    {
        public string Name;
        public int Value;
    };
        
    public Dictionary<string, Slot> BodyDict = new Dictionary<string, Slot>()
    {
        { "BrainStem",  new Slot { Name = "Brain Stem", ActiveComponent = EMPTYC, Desc = "Affects wakefulness and metabolism." }},
        { "StemAug",  new Slot { Name = "Brain Stem Augmentation", ActiveComponent = EMPTYC, Desc = "Affects wakefulness and metabolism.", NeedsOrg = 1, Owner = "BrainStem"}},
        { "Cerebellum", new Slot { Name = "Cerebellum", ActiveComponent = EMPTYC, Desc = "Determines maximum agility." }},
        { "CerAug", new Slot { Name = "Cerebellum Augmentation", ActiveComponent = EMPTYC, Desc = "Determines maximum agility.", NeedsOrg = 1, Owner = "Cerebellum" }},
        { "Frontal", new Slot { Name = "Frontal Lobe", ActiveComponent = EMPTYC, Desc = "Affects diplomacy, emotional control, critical thinking, charm and threat." }},
        { "FroAug", new Slot { Name = "Frontal Lobe Augmentation", ActiveComponent = EMPTYC, Desc = "Affects diplomacy, emotional control, critical thinking, charm and threat.", NeedsOrg = 1, Owner = "Frontal" }},
        { "Parietal", new Slot { Name = "Parietal Lobe", ActiveComponent = EMPTYC, Desc = "Determines maximum spatial sense. Affects pain tolerance and diplomacy." }},
        { "ParAug", new Slot { Name = "Parietal Lobe Augmentation", ActiveComponent = EMPTYC, Desc = "Determines maximum spatial sense. Affects pain tolerance and diplomacy.", NeedsOrg = 1, Owner = "Parietal" }},
        { "Occipital", new Slot { Name = "Occipital Lobe", ActiveComponent = EMPTYC, Desc = "Determines maximum visual detail, and visible spectra." }},
        { "OccAug", new Slot { Name = "Occipital Lobe Augmentation", ActiveComponent = EMPTYC, Desc = "Determines maximum visual detail, and visible spectra.", NeedsOrg = 1, Owner = "Occipital" }},
        { "Temporal", new Slot { Name = "Temporal Lobe", ActiveComponent = EMPTYC,   Desc = "Affects critical thinking and diplomacy." }},
        { "TemAug", new Slot { Name = "Temporal Lobe Augmentation", ActiveComponent = EMPTYC,   Desc = "Affects critical thinking and diplomacy.", NeedsOrg = 1, Owner = "Temporal" }},
        { "Hypothalamus", new Slot { Name = "Hypothalamus", ActiveComponent = EMPTYC,   Desc = "Affects wakefulness and metabolism." }},
        { "HypAug", new Slot { Name = "Hypothalamus Augmentation", ActiveComponent = EMPTYC,   Desc = "Affects wakefulness and metabolism.", NeedsOrg = 1, Owner = "Hypothalamus" }},
        { "Pituitary", new Slot { Name = "Pituitary Gland", ActiveComponent = EMPTYC,   Desc = "Greatly affects stress response and tolerance." } },
        { "PitAug", new Slot { Name = "Pituitary Gland Augmentation", ActiveComponent = EMPTYC,   Desc = "Greatly affects stress response and tolerance.", NeedsOrg = 1, Owner = "Pituitary" }},
        { "Pineal", new Slot { Name = "Pineal Gland", ActiveComponent = EMPTYC,   Desc = "Greatly affects wakefulness." }},
        { "PinAug", new Slot { Name = "Pineal Gland Augmentation", ActiveComponent = EMPTYC,   Desc = "Greatly affects wakefulness.", NeedsOrg = 1, Owner = "Pineal" }},
        { "Thalamus", new Slot { Name = "Thalamus", ActiveComponent = EMPTYC,   Desc = "Affects alertness and pain tolerance." } },
        { "ThaAug", new Slot { Name = "Thalamus Augmentation", ActiveComponent = EMPTYC,   Desc = "Affects alertness and pain tolerance.", NeedsOrg = 1, Owner = "Thalamus" }},
        { "Basal", new Slot { Name = "Basal Ganglia", ActiveComponent = EMPTYC,   Desc = "Determines maximum deftness and fine motor function." }},
        { "BasAug", new Slot { Name = "Basal Ganglia Augmentation", ActiveComponent = EMPTYC,   Desc = "Determines maximum deftness and fine motor function.", NeedsOrg = 1, Owner = "Basal" }},
        { "Limbic", new Slot { Name = "Limbic System", ActiveComponent = EMPTYC,   Desc = "Affects emotional control and threat." } },
        { "LimAug", new Slot { Name = "Limbic System Augmentation", ActiveComponent = EMPTYC,   Desc = "Affects emotional control and threat.", NeedsOrg = 1, Owner = "Limbic" }},
        { "Heart", new Slot { Name = "Heart", ActiveComponent = EMPTYC, Desc = "Sets maximum organic load, and offers minor augmentation load boost." }},
        { "HeartAug", new Slot { Name = "Heart Augmentation", ActiveComponent = EMPTYC, Desc = "Sets maximum organic load, and offers minor augmentation load boost.", NeedsOrg = 1, Owner = "Heart" }},
        { "Stomach", new Slot { Name = "Stomach", ActiveComponent = EMPTYC,   Desc = "Affects metabolic efficiency." }},
        { "StomachAug", new Slot { Name = "Stomach Augmentation", ActiveComponent = EMPTYC, Desc = "Affects metabolic efficiency.", NeedsOrg = 1, Owner = "Stomach" }},
        { "Intestines", new Slot { Name = "Intestines", ActiveComponent = EMPTYC,   Desc = "Affects metabolic efficiency." }},
        { "IntestineAug", new Slot { Name = "Intestinal Augmentation", ActiveComponent = EMPTYC, Desc = "Affects metabolic efficiency.", NeedsOrg = 1, Owner = "Intestines" }},
        { "Adrenal", new Slot { Name = "Adrenal Gland", ActiveComponent = EMPTYC,   Desc = "Boosts all physical stats, reflex, stress response and pain tolerance." } },
        { "AdrenalAug", new Slot { Name = "Adrenal Gland Augmentation", ActiveComponent = EMPTYC, Desc = "Boosts all physical stats, reflex, stress response and pain tolerance.", NeedsOrg = 1, Owner = "Adrenal" }},
        { "Thyroid", new Slot { Name = "Thyroid", ActiveComponent = EMPTYC,   Desc = "Affects metabolic efficiency." }},
        { "ThyroidAug", new Slot { Name = "Thyroid Augmentation", ActiveComponent = EMPTYC, Desc = "Affects metabolic efficiency.", NeedsOrg = 1, Owner = "Thyroid" }},
        { "BArmRU", new Slot { Name = "Right Humerus", ActiveComponent = EMPTYC, Desc = "Determines maximum bench press rating.", NeedsOrg = 1, Owner = "ArmRU" }},
        { "BArmLU", new Slot { Name = "Left Humerus", ActiveComponent = EMPTYC, Desc = "Determines maximum bench press rating.", NeedsOrg = 1, Owner = "ArmLU" }},
        { "BArmRF", new Slot { Name = "Right Radius and Ulna", ActiveComponent = EMPTYC, Desc = "Determines maximum bench press rating.", NeedsOrg = 1, Owner = "ArmRF" } },
        { "BArmLF", new Slot { Name = "Left Radius and Ulna", ActiveComponent = EMPTYC, Desc = "Determines maximum bench press rating.", NeedsOrg = 1, Owner = "ArmLF" }},
        { "BLegRU", new Slot { Name = "Right Femur", ActiveComponent = EMPTYC, Desc = "Determines maximum squat rating.", NeedsOrg = 1, Owner = "LegRU" }},
        { "BLegLU", new Slot { Name = "Left Femur", ActiveComponent = EMPTYC, Desc = "Determines maximum squat rating.", NeedsOrg = 1, Owner = "LegLU" }},
        { "BLegRL", new Slot { Name = "Right Tibia and Fibula", ActiveComponent = EMPTYC, Desc = "Determines maximum squat rating.", NeedsOrg = 1, Owner = "LegRL" }},
        { "BLegLL", new Slot { Name = "Left Tibia and Fibula", ActiveComponent = EMPTYC, Desc = "Determines maximum squat rating.", NeedsOrg = 1, Owner = "LegLL" }},
        { "Ribs", new Slot { Name = "Rib Cage", ActiveComponent = EMPTYC,   Desc = "Determines maximum bench press rating. Protects the heart and lungs." }},
        { "Skull", new Slot { Name = "Skull", ActiveComponent = EMPTYC,   Desc = "Protects all brain components. Can affect charm and threat." }},
        { "Pelvis", new Slot { Name = "Pelvis", ActiveComponent = EMPTYC,   Desc = "Determines maximum squat rating." }},
        { "Spine", new Slot { Name = "Spine", ActiveComponent = EMPTYC,   Desc = "Determines maximum squat rating and pain tolerance. Affects agility and reflexes." }},
        { "SpineAug", new Slot { Name = "Spinal Augmentation", ActiveComponent = EMPTYC, Desc = "Determines maximum squat rating and pain tolerance. Affects agility and reflexes.", NeedsOrg = 1, Owner = "Spine" }},
        { "ArmRU", new Slot { Name = "Right Upper Arm", ActiveComponent = EMPTYC, Desc = "Affects bench press rating and agility." }},
        { "ArmLU", new Slot { Name = "Left Upper Arm", ActiveComponent = EMPTYC, Desc = "Affects bench press rating and agility." }},
        { "ArmRF", new Slot { Name = "Right Forearm", ActiveComponent = EMPTYC, Desc = "Affects bench press rating, grip strength and agility.", Owner = "ArmRU" }},
        { "ArmLF", new Slot { Name = "Left Forearm", ActiveComponent = EMPTYC, Desc = "Affects bench press rating, grip strength and agility.", Owner = "ArmLU" }},
        { "LegRU", new Slot { Name = "Right Thigh", ActiveComponent = EMPTYC, Desc = "Affects squat rating, speed and agility." }},
        { "LegLU", new Slot { Name = "Left Thigh", ActiveComponent = EMPTYC, Desc = "Affects squat rating, speed and agility." }},
        { "LegRL", new Slot { Name = "Right Crus", ActiveComponent = EMPTYC, Desc = "Affects squat rating, speed and agility.", Owner = "LegRU" }},
        { "LegLL", new Slot { Name = "Left Crus", ActiveComponent = EMPTYC, Desc = "Affects squat rating, speed and agility.", Owner = "LegLU" }},
        { "HandR", new Slot { Name = "Right Hand", ActiveComponent = EMPTYC, Desc = "Determines maximum grip strength. Affects deftness.", Owner = "ArmRF" }},
        { "HandL", new Slot { Name = "Left Hand", ActiveComponent = EMPTYC, Desc = "Determines maximum grip strength. Affects deftness.", Owner = "ArmLF" }},
        { "FootR", new Slot { Name = "Right Foot", ActiveComponent = EMPTYC, Desc = "Affects speed.", Owner = "LegRL" }},
        { "FootL", new Slot { Name = "Left Foot", ActiveComponent = EMPTYC, Desc = "Affects speed.", Owner = "LegLL" }},
        { "Lungs", new Slot { Name = "Lungs", ActiveComponent = EMPTYC, Desc = "Sets maximum organic load." }},
        { "LungAug", new Slot { Name = "Lung Augmentation", ActiveComponent = EMPTYC, Desc = "Sets maximum organic load.", NeedsOrg = 1, Owner = "Lungs" }},
        { "Vocal", new Slot { Name = "Vocal Cords", ActiveComponent = EMPTYC, Desc = "Makes diplomacy possible."}},
        { "VocalAug", new Slot { Name = "Vocal Cord Augmentation", ActiveComponent = EMPTYC, Desc = "Affects charm and threat.", NeedsOrg = 1, Owner = "Vocal" }},
        { "EyeL", new Slot { Name = "Left Eye", ActiveComponent = EMPTYC, Desc = "Affects spatial sense, spectra, distance and detail of vision." }},
        { "EyeR", new Slot { Name = "Right Eye", ActiveComponent = EMPTYC, Desc = "Affects spatial sense, spectra, distance and detail of vision." }},
        { "Ears", new Slot { Name = "Ears", ActiveComponent = EMPTYC, Desc = "Affects spatial sense." }},
        { "Jaw", new Slot { Name = "Jaw", ActiveComponent = EMPTYC, Desc = "Can protect the skull and affect charm or threat. Makes diplomacy possible." }},
        { "Face", new Slot { Name = "Face", ActiveComponent = EMPTYC, Desc = "Can affect vision, alertness, charm, threat and reflexes." }},
        { "Battery", new Slot { Name = "Battery", ActiveComponent = EMPTYC, Desc = "Requires a robotic or augmented spine. Determines maximum augmentation load." }} // This just needs a special check in the install function.
    };
    
    public Dictionary<string, Attribute> AttDict = new Dictionary<string, Attribute>
    {
        { "MOL", new Attribute {Name = "Maximum Organic Load", Value = 0 } },
        { "MAL", new Attribute {Name = "Maximum Augmentation Load", Value = 0 } },
        { "COL", new Attribute {Name = "Current Organic Load", Value = 0 } },
        { "CAL", new Attribute {Name = "Current Augmentation Load", Value = 0 } },
        { "Press", new Attribute {Name = "Maximum Bench", Value = 0 } },
        { "Squat", new Attribute {Name = "Maximum Squat", Value = 0 } },
        { "Grip", new Attribute {Name = "Grip Strength", Value = 0 } },
        { "Speed", new Attribute {Name = "Land Speed", Value = 0 } },
        { "Agi", new Attribute {Name = "Agility Rating", Value = 0 } },
        { "Deft", new Attribute {Name = "Deftness", Value = 0 } },
        { "Alert", new Attribute {Name = "Passive Alertness", Value = 0 } },
        { "Reflex", new Attribute {Name = "Reflex Response", Value = 0 } },
        { "Spatial", new Attribute {Name = "Spatial Awareness", Value = 0 } },
        { "Stress", new Attribute {Name = "Stress Response", Value = 0 } },
        { "PainTol", new Attribute {Name = "Pain Tolerance", Value = 0 } },
        { "EmoCon", new Attribute {Name = "Emotional Suppression", Value = 0 } },
        { "CritThink", new Attribute {Name = "Critical Thought Rating", Value = 0 } },
        { "Charm", new Attribute {Name = "Passive Charm", Value = 0 } },
        { "Threat", new Attribute {Name = "Passive Threat", Value = 0 } },
        { "Diplo", new Attribute {Name = "Diplomacy", Value = 0 } },
        { "Wake", new Attribute {Name = "Wakefulness", Value = 0 } },
        { "Metabol", new Attribute {Name = "Metabolic Efficiency", Value = 0 } },
        { "VisDist", new Attribute {Name = "Far Sight", Value = 0 } },
        { "VisDet", new Attribute {Name = "Visual Acuity", Value = 0 } },
        { "VIR", new Attribute {Name = "Infrared Vision", Value = 0 } },
        { "VUV", new Attribute {Name = "Ultraviolet Vision", Value = 0 } },
        { "VXR", new Attribute {Name = "X-Ray Vision", Value = 0 } },
        { "VisLit", new Attribute {Name = "Lowest Visible Light", Value = 0 } }
    };
    
    public class Being
    {
        public string Name;
        public Dictionary<string, Slot> Body = new Dictionary<string, Slot>();
        public Dictionary<string, Attribute> Atts = new Dictionary<string, Attribute>();
    }

    public class Fighter
    {
        public Being being;
        public Slot lhand;
        public Slot rhand;
        public Slot jaws;
        public string state;
    }
    
    #endregion

    #region Slots and Attributes

    public int CompIDNow = 1; // Incremented and assigned to components as they are recieved. To tell different items of the same Make/Model apart. May be unnecessary. Indexed at 1 because 0 is EMPTY
    System.Random rnd = new System.Random();

    public Being PLAYER = new Being();
    public Being OPPONENT = new Being();

    public Fighter PFighter = new Fighter();
    public Fighter OFighter = new Fighter();

    public static MakeModel EMPTYMM = new MakeModel { Name = "EMPTY", Description = "EMPTY", Slot = "EMPTY", ItemNumber = 9999 };
    public static Component EMPTYC = new Component { State = 0, OperatingWeeks = 0, InstalledNew = 1, MakeModel = EMPTYMM, CompID = 0 };

    public static Slot EMPTYS = new Slot {Name = "EMPTY", Desc = "EMPTY"};

    // LISTS
    public List<MakeModel> HumanMakeModels = new List<MakeModel>();
    public List<Component> HumanComponents = new List<Component>();
    public List<MakeModel> Catalog = new List<MakeModel>();
    public List<Component> Inventory = new List<Component>();
    public List<string> BodySlotKeys = new List<string>
    {
        "BrainStem", "StemAug", "Cerebellum", "CerAug", "Frontal", "FroAug", "Parietal", "ParAug",
        "Occipital",  "OccAug", "Temporal", "TemAug", "Hypothalamus", "HypAug", "Pituitary", "PitAug",
        "Pineal", "PinAug", "Thalamus", "ThaAug", "Basal", "BasAug", "Limbic", "LimAug", "Heart",
        "HeartAug", "Stomach", "StomachAug", "Intestines", "IntestineAug", "Adrenal", "AdrenalAug",
        "Thyroid", "ThyroidAug",  "BArmRU", "BArmLU", "BArmRF", "BArmLF", "BLegRU",  "BLegLU",
        "BLegRL", "BLegLL", "Ribs", "Skull", "Pelvis", "Spine", "SpineAug", "ArmRU", "ArmLU",
        "ArmRF", "ArmLF", "LegRU", "LegLU", "LegRL", "LegLL", "HandR", "HandL", "FootR", "FootL",
        "Lungs", "LungAug", "Vocal", "VocalAug", "EyeL", "EyeR", "Ears", "Jaw", "Face", "Battery"
    };

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

    // UI Outputs
    
    public List<Text> InvText = new List<Text>();
    public List<Text> UISlots = new List<Text> {};

    public Slot SelectedSlot = new Slot();
    public Component SelectedComponent = new Component();

    public Text bstxt;
    public Text bsatxt;
    public Text certxt;
    public Text ceratxt;
    public Text frotxt;
    public Text froatxt;
    public Text partxt;
    public Text paratxt;
    public Text occtxt;
    public Text occatxt;
    public Text temtxt;
    public Text tematxt;
    public Text hyptxt;
    public Text hypatxt;
    public Text pittxt;
    public Text pitatxt;
    public Text pintxt;
    public Text pinatxt;
    public Text thatxt;
    public Text thaatxt;
    public Text bastxt;
    public Text basatxt;
    public Text limtxt;
    public Text limatxt;
    public Text hrttxt;
    public Text hrtatxt;
    public Text stomtxt;
    public Text stomatxt;
    public Text intesttxt;
    public Text intestatxt;
    public Text adrtxt;
    public Text adratxt;
    public Text thytxt;
    public Text thyatxt;
    public Text spitxt;
    public Text spiatxt;
    public Text ribtxt;
    public Text skltxt;
    public Text rhumtxt;
    public Text lhumtxt;
    public Text rrutxt;
    public Text lrutxt;
    public Text rfemtxt;
    public Text lfemtxt;
    public Text rfibtxt;
    public Text lfibtxt;
    public Text peltxt;
    public Text ruatxt;
    public Text luatxt;
    public Text rfatxt;
    public Text lfatxt;
    public Text rhtxt;
    public Text lhtxt;
    public Text rthitxt;
    public Text lthitxt;
    public Text rcrutxt;
    public Text lcrutxt;
    public Text rftxt;
    public Text lfttxt;
    public Text lngtxt;
    public Text lngatxt;
    public Text voctxt;
    public Text vocatxt;
    public Text reyetxt;
    public Text leyetxt;
    public Text eartxt;
    public Text jawtxt;
    public Text factxt;
    public Text battxt;

    public Text Inv1;
    public Text Inv2;
    public Text Inv3;
    public Text Inv4;
    public Text Inv5;
    public Text Inv6;
    public Text Inv7;
    public Text Inv8;
    public Text Inv9;
    public Text Inv10;
    public Text Inv11;
    public Text Inv12;
    public Text Inv13;
    public Text Inv14;

    public Text moltxt;
    public Text maltxt;
    public Text coltxt;
    public Text caltxt;
    public Text presstxt;
    public Text squattxt;
    public Text griptxt;
    public Text speedtxt;
    public Text agitxt;
    public Text defttxt;
    public Text alerttxt;
    public Text reflextxt;
    public Text spatialtxt;
    public Text stresstxt;
    public Text paintoltxt;
    public Text emotxt;
    public Text critthinktxt;
    public Text charmtxt;
    public Text threattxt;
    public Text diplotxt;
    public Text waketxt;
    public Text metatxt;
    public Text visdisttxt;
    public Text visdettxt;
    public Text spectratxt;
    public Text vislittxt;

    public Text EventText;
    public Text ScavText;
    public Text HoursText;

    //public string input;

    #endregion

    #region Start and Update
    // Start is called before the first frame update
    void Start()
    {
        SelectedComponent = EMPTYC;
        
        MMGrab(HumanMakeModels, "ASSETS/Resources/HumanPartsDUMMY.csv");
        MMGrab(Catalog, "ASSETS/Resources/Catalog.csv");
        HumanCompsGrab();

        InitBeing(PLAYER);
        OnlyHuman(PLAYER);                
        PullAttributes(PLAYER);

        InitBeing(OPPONENT);

        Gimme();
        Gimme();
        Gimme();

        ClearEvents();
        ClearScav();
                
        UpdateUISlots();

        Debug.Log("START VOID COMPLETE");
        return;
    }

    // Update is called once per frame
    void Update()
    {
        //input = Input.inputString;
        //Debug.Log(input);
    }

    #endregion

    #region Functions
    
    #region Attribute Pulling

    int WAtt(Slot s, int i)
    {
        int watt;
        watt = (int)(s.ActiveComponent.MakeModel.Attributes[i] * 0.5 * s.ActiveComponent.State);
        return watt;
    }

    int WAttC(Component c, int i) // for component only checks (used by MOL / MAL check)
    {
        int watt;
        watt = (int)(c.MakeModel.Attributes[i] * 0.5 * c.State);
        return watt;
    }

    void PullMOL(Being t) // Lungs&Aug or heart&Aug, whichever is smaller. Lungs provide oxygen, heart must supply it to the body
    {
        int i = 0; // This number is the index of the MOL attribute in the attributes array

        t.Atts["MOL"].Value = System.Math.Min(WAtt(t.Body["Heart"], i) + WAtt(t.Body["HeartAug"], i), WAtt(t.Body["Lungs"], i) + WAtt(t.Body["LungAug"], i));
    }

    void PullMAL(Being t) //Spine&Aug or heart&Aug, whichever is smaller. Then add battery value.
    {
        int i = 1;

        if ((WAtt(t.Body["Spine"], i) + WAtt(t.Body["SpineAug"],i)) > (WAtt(t.Body["Heart"],i) + WAtt(t.Body["HeartAug"],i)))
        {
            t.Atts["MAL"].Value = WAtt(t.Body["Heart"], i) + WAtt(t.Body["HeartAug"], i) + WAtt(t.Body["Battery"], i);
        }
        else
        {
            t.Atts["MAL"].Value = WAtt(t.Body["Spine"], i) + WAtt(t.Body["SpineAug"], i) + WAtt(t.Body["Battery"],i);
        };
    }

    void PullCOL(Being t)
    {
        int i = 2;
        t.Atts["COL"].Value = 0;
        foreach (KeyValuePair<string, Slot> s in t.Body)
        {
            t.Atts["COL"].Value += WAtt(s.Value,i);
        }
    }

    void PullCAL(Being t)
    {
        int i = 3;
        t.Atts["CAL"].Value = 0;
        foreach (KeyValuePair<string, Slot> s in t.Body)
        {
            t.Atts["CAL"].Value += WAtt(s.Value, i);
        }
    }

    void PullPress(Being t)
    {
        int i = 4;
        int org = 31; // Is organic where 1 is YES
        int cap;
        int armcap;
        int leftcap;
        int rightcap;
        float lhnd = 0.5f;
        float rhnd = 0.5f;

        if (t.Body["HandL"].ActiveComponent != EMPTYC) // do you have a left hand
        {
            lhnd = 1.0f;
        }

        if (t.Body["HandR"].ActiveComponent != EMPTYC) // do you have a right hand
        {
            rhnd = 1.0f;
        }

        int presspot = (int)((WAtt(t.Body["ArmRU"], i) + WAtt(t.Body["ArmLU"], i) + WAtt(t.Body["ArmLF"], i) + WAtt(t.Body["ArmRF"], i) + WAtt(t.Body["Adrenal"], i) + WAtt(t.Body["AdrenalAug"], i)) * lhnd * rhnd); // The maximum output possible before capping

        if (t.Body["ArmLU"].ActiveComponent.MakeModel.Attributes[org] == 0) // If the upper arm is robotic it means both parts are robotic
        {
            leftcap = 0;
        }
        else
        {
            if (t.Body["ArmLF"].ActiveComponent.MakeModel.Attributes[org] == 0)
            {
                leftcap = WAtt(t.Body["BArmLU"], i); // If the upper arm is human but the forearm is robotic
            } else
            {
                leftcap = System.Math.Min(WAtt(t.Body["BArmLU"], i), WAtt(t.Body["BArmLF"], i)); // If both parts are human
            }
        }

        if (t.Body["ArmRU"].ActiveComponent.MakeModel.Attributes[org] == 0) // If the upper arm is robotic it means both parts are robotic
        {
            rightcap = 0;
        }
        else
        {
            if (t.Body["ArmRF"].ActiveComponent.MakeModel.Attributes[org] == 0)
            {
                rightcap = WAtt(t.Body["BArmRU"], i); // If the upper arm is human but the forearm is robotic
            }
            else
            {
                rightcap = System.Math.Min(WAtt(t.Body["BArmRU"], i), WAtt(t.Body["BArmRF"], i)); // If both parts are human, the cap is the least strong bone
            }
        }

        if(leftcap == 0) //Left arm is robotic
        {
            if(rightcap == 0) //Right arm is robotic
            {
                armcap = 0;
            } else // Left is robotic but right is not
            {
                armcap = rightcap;
            }
        } else
        {
            if(rightcap == 0) // Left arm is at least partially organic but right arm is robotic
            {
                armcap = leftcap;
            } else // Both arms are at least partially organic
            {
                armcap = (rightcap + leftcap) / 2; // Average of the arms minimums
            }
        }

        cap = System.Math.Min(WAtt(t.Body["Ribs"], i), armcap); // Weakest point between the ribs and arm bones

        t.Atts["Press"].Value = System.Math.Min(cap, presspot); // If your press potential is above the cap, choose the cap.


        // Debug.Log("This body's potential press rating is " + presspot);
    }

    void PullSquat(Being t)
    {
        int i = 5;
        int org = 31; // Is organic where 1 is YES
        int cap;
        int legcap;
        int lbcap;
        int leftcap;
        int rightcap;
        float lft = 0.5f;
        float rft = 0.5f;

        if (t.Body["FootL"].ActiveComponent != EMPTYC) // do you have a left foot
        {
            lft = 1.0f;
        }

        if (t.Body["FootR"].ActiveComponent != EMPTYC) // do you have a right foot
        {
            rft = 1.0f;
        }

        int squatpot = (int)((WAtt(t.Body["LegRU"], i) + WAtt(t.Body["LegLU"], i) + WAtt(t.Body["LegLL"], i) + WAtt(t.Body["LegRL"], i) + WAtt(t.Body["Adrenal"], i) + WAtt(t.Body["AdrenalAug"], i)) * lft * rft); // The maximum output possible before capping

        if (t.Body["LegLU"].ActiveComponent.MakeModel.Attributes[org] == 0) // If the thigh is robotic it means both parts are robotic
        {
            leftcap = 0;
        }
        else
        {
            if (t.Body["LegLL"].ActiveComponent.MakeModel.Attributes[org] == 0)
            {
                leftcap = WAtt(t.Body["BLegLU"], i); // If the thigh is human but the crus is robotic
            }
            else
            {
                leftcap = System.Math.Min(WAtt(t.Body["BLegLU"], i), WAtt(t.Body["BLegLL"], i)); // If both parts are human
            }
        }

        if (t.Body["LegRU"].ActiveComponent.MakeModel.Attributes[org] == 0) // If the thigh is robotic it means both parts are robotic
        {
            rightcap = 0;
        }
        else
        {
            if (t.Body["LegRL"].ActiveComponent.MakeModel.Attributes[org] == 0)
            {
                rightcap = WAtt(t.Body["BLegRU"], i); // If the thigh is human but the crus is robotic
            }
            else
            {
                rightcap = System.Math.Min(WAtt(t.Body["BLegRU"], i), WAtt(t.Body["BLegRL"], i)); // If both parts are human, the cap is the least strong bone
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

        lbcap = System.Math.Min(WAtt(t.Body["Pelvis"], i), legcap); // Weakest point between the pelvis and arm bones

        cap = System.Math.Min(lbcap, WAtt(t.Body["Spine"], i)); // Weakest point between the entire lower body and the spine

        t.Atts["Squat"].Value = System.Math.Min(cap, squatpot); // If your squat potential is above the cap, choose the cap.

        //Debug.Log("This body's potential squat rating is " + squatpot);
    }

    void PullGrip(Being t)
    {
        int i = 6;
        int org = 31;
        float lhnd = 0.5f;
        float rhnd = 0.5f;
        int hasahand = 0;
        int lcap;
        int rcap;
        int cap;

        if (t.Body["HandL"].ActiveComponent != EMPTYC) // do you have a left hand
        {
            lhnd = 1.0f;
        }

        if (t.Body["HandR"].ActiveComponent != EMPTYC) // do you have a right hand
        {
            rhnd = 1.0f;
        }

        if (t.Body["HandL"].ActiveComponent != EMPTYC | t.Body["HandR"].ActiveComponent != EMPTYC)
        {
            hasahand = 1;
        }
        
        int grippot = (int)((WAtt(t.Body["ArmLF"], i) + WAtt(t.Body["ArmRF"], i) + WAtt(t.Body["Adrenal"], i) + WAtt(t.Body["AdrenalAug"], i)) * lhnd * rhnd * hasahand);

        if (t.Body["ArmLF"].ActiveComponent.MakeModel.Attributes[org] == 0) // if the left forearm is robotic
        {
            lcap = WAtt(t.Body["HandL"], i);
        } else // If the left forearm is organic
        {
            lcap = System.Math.Min(WAtt(t.Body["HandL"], i), WAtt(t.Body["BArmLF"], i));
        }

        if (t.Body["ArmRF"].ActiveComponent.MakeModel.Attributes[org] == 0) // if the right forearm is robotic
        {
            rcap = WAtt(t.Body["HandR"], i);
        }
        else // If the right forearm is organic
        {
            rcap = System.Math.Min(WAtt(t.Body["HandR"], i), WAtt(t.Body["BArmRF"], i));
        }

        cap = (rcap + lcap) / 2; // If the muscles would break the hands or arm bones, set grip to the cap

        t.Atts["Grip"].Value = System.Math.Min(cap, grippot);
        //Debug.Log("This body's potential grip rating is " + grippot);
    }
        
    void PullSpeed(Being t) // Affected by legs (not bones) and adrenal
    {
        int i = 7;
        float lft = 0.5f;
        float rft = 0.5f;
        t.Atts["Speed"].Value = 0;

        if (t.Body["FootL"].ActiveComponent != EMPTYC)
        {
            lft = 1.0f;
        }

        if(t.Body["FootR"].ActiveComponent != EMPTYC)
        {
            rft = 1.0f;
        }

        foreach (KeyValuePair<string, Slot> s in t.Body)
        {
            t.Atts["Speed"].Value += WAtt(s.Value, i);
        }

        t.Atts["Speed"].Value = (int)(t.Atts["Speed"].Value * lft * rft);
    }

    void PullAgility(Being t)
    {
        int i = 8;
        t.Atts["Agi"].Value = 0;

        foreach (KeyValuePair<string,Slot> s in t.Body)
        {
            t.Atts["Agi"].Value += WAtt(s.Value, i);
        }

        t.Atts["Agi"].Value -= WAtt(t.Body["Cerebellum"], i) + WAtt(t.Body["CerAug"], i); // Remove the ratings from the cerebellum since it is a cap value

        t.Atts["Agi"].Value = System.Math.Min(t.Atts["Agi"].Value, WAtt(t.Body["Cerebellum"], i) + WAtt(t.Body["CerAug"], i)); // Cerebellum determines cap
    }

    void PullDeft(Being t)
    {
        int i = 9;

        t.Atts["Deft"].Value = System.Math.Min(((WAtt(t.Body["HandL"], i)+WAtt(t.Body["HandR"],i))/2) , WAtt(t.Body["Basal"], i) + WAtt(t.Body["BasAug"], i)); // Average of the hands or the maximum the basal ganglia can control. Will return zero if no hands.
    }

    void PullAlert(Being t)
    {
        int i = 10;

        float earstate = 0.5f; // Can still communicate with paper / charades
        float leyst = 0.5f; // Can still communicate without a jaw
        float reyst = 0.5f;

        if (t.Body["EyeR"].ActiveComponent != EMPTYC) // these check damage and existence of jaw, lungs, ears, voicebox
        {
            reyst = System.Math.Max(0.5f, t.Body["EyeR"].ActiveComponent.State / 2.0f);
        }
        if (t.Body["EyeL"].ActiveComponent != EMPTYC)
        {
            leyst = System.Math.Max(0.5f, t.Body["EyeL"].ActiveComponent.State / 2.0f); // Not counting vocal cord aug since it's simply a measure of whether you can communicate
        }
        if (t.Body["Ears"].ActiveComponent != EMPTYC)
        {
            earstate = System.Math.Max(0.5f, t.Body["Ears"].ActiveComponent.State / 2.0f);
        }
        
        t.Atts["Alert"].Value = (int)((WAtt(t.Body["Thalamus"], i) + WAtt(t.Body["ThaAug"],i))*leyst*reyst*earstate); //Thalamus determines cap alertness
    }

    void PullReflex(Being t) // Affected by spine and adrenal
    {
        int i = 11;
        t.Atts["Reflex"].Value = 0;

        foreach (KeyValuePair<string, Slot> s in t.Body) // Probably just the Spine with an adrenal boost
        {
            t.Atts["Reflex"].Value += WAtt(s.Value, i);
        }
    }

    void PullSpatial(Being t)
    {
        int i = 12;
        
        t.Atts["Spatial"].Value = System.Math.Min( WAtt(t.Body["EyeL"],i) + WAtt(t.Body["EyeR"], i) + WAtt(t.Body["Ears"], i) , WAtt(t.Body["Parietal"], i) + WAtt(t.Body["ParAug"], i)); // Spatial sense is capped by the parietal lobe
    }

    void PullStress(Being t)
    {
        int i = 13;
        t.Atts["Stress"].Value = WAtt(t.Body["Pituitary"], i) + WAtt(t.Body["PitAug"],i) + WAtt(t.Body["Adrenal"], i) + WAtt(t.Body["AdrenalAug"], i);
    }

    void PullPainTol(Being t)
    {
        int i = 14;
        int braintol = (WAtt(t.Body["Parietal"], i) + WAtt(t.Body["ParAug"], i) + WAtt(t.Body["Thalamus"], i) + WAtt(t.Body["ThaAug"], i)) / 2; //Average between these components
        t.Atts["PainTol"].Value = System.Math.Max(WAtt(t.Body["Spine"], i) + WAtt(t.Body["SpineAug"], i), braintol) + WAtt(t.Body["Adrenal"], i) + WAtt(t.Body["AdrenalAug"], i); // Either the spine or the brain handles pain, then adrenal gland supplies a boost
    }

    void PullEmo(Being t)
    {
        int i = 15;
        t.Atts["EmoCon"].Value = (WAtt(t.Body["Frontal"], i) + WAtt(t.Body["FroAug"],i) + WAtt(t.Body["Limbic"], i) + WAtt(t.Body["LimAug"],i)) / 2; // Average between frontal and limbic systems
    }

    void PullCritThink(Being t)
    {
        int i = 16;
        t.Atts["CritThink"].Value = (WAtt(t.Body["Frontal"], i) + WAtt(t.Body["FroAug"], i) + WAtt(t.Body["Temporal"], i) + WAtt(t.Body["TemAug"], i)) / 2; // Average between frontal and temporal
    }

    void PullCharm(Being t)
    {
        int i = 17;
        t.Atts["Charm"].Value = 0;
        foreach (KeyValuePair<string, Slot> s in t.Body) // No caps on charm, anything can add charm
        {
            t.Atts["Charm"].Value += WAtt(s.Value, i);
        }
    }

    void PullThreat(Being t)
    {
        int i = 18;
        t.Atts["Threat"].Value = 0;
        foreach (KeyValuePair<string, Slot> s in t.Body) // No caps on threat, anything can add threat
        {
            t.Atts["Threat"].Value += WAtt(s.Value, i);
        }
    }

    void PullDiplo(Being t) // Maybe ears and voice should also be boosters for this instead of just checks?
    {
        int i = 19;
        float earstate = 0.5f; // Can still communicate with paper / charades
        float jawstate = 0.5f; // Can still communicate without a jaw
        float voicestate = 0.5f;
        float lungstate = 0.0f; // Hopefully this shouldn't be necessary

        if(t.Body["Jaw"].ActiveComponent != EMPTYC) // these check damage and existence of jaw, lungs, ears, voicebox
        {
            jawstate = System.Math.Max(0.5f, t.Body["Jaw"].ActiveComponent.State / 2.0f);
        }
        if(t.Body["Vocal"].ActiveComponent != EMPTYC)
        {
            voicestate = System.Math.Max(0.5f, t.Body["Vocal"].ActiveComponent.State / 2.0f); // Not counting vocal cord aug since it's simply a measure of whether you can communicate
        }
        if(t.Body["Ears"].ActiveComponent != EMPTYC)
        {
            earstate = System.Math.Max(0.5f, t.Body["Ears"].ActiveComponent.State / 2.0f);
        }
        if(t.Body["Lungs"].ActiveComponent != EMPTYC)
        {
            lungstate = t.Body["Lungs"].ActiveComponent.State / 2.0f; // Not including lung aug since it's simply a check if you can move air
        }

        t.Atts["Diplo"].Value = (int)(System.Math.Min(WAtt(t.Body["Frontal"], i) + WAtt(t.Body["FroAug"], i), (WAtt(t.Body["Temporal"], i) + WAtt(t.Body["TemAug"], i) + WAtt(t.Body["Parietal"], i) + WAtt(t.Body["ParAug"], i)) / 2) * jawstate * voicestate * earstate * lungstate);
        // Average between parietal and temporal, capped by frontal and then modified by the states of jaw, ears, voice, lungs
    }

    void PullWake(Being t)
    {
        int i = 20;
        
        t.Atts["Wake"].Value = 0;
        foreach (KeyValuePair<string,Slot> s in t.Body) // Usually just the brainstem, hypothalamus and pineal gland
        {
            t.Atts["Wake"].Value += WAtt(s.Value, i);
        }
    }

    void PullMetabol(Being t)
    {
        int i = 21;
        t.Atts["Metabol"].Value = 0;

        float stomst = 0.5f;
        float gutst = 0.5f;
        float thyst = 0.5f;

        if (t.Body["Stomach"].ActiveComponent != EMPTYC) // these check damage and existence of jaw, lungs, ears, voicebox
        {
            stomst = System.Math.Max(0.5f , t.Body["Stomach"].ActiveComponent.State / 2.0f); // Returns 0.5 if the component is broken
        }
        if (t.Body["Intestines"].ActiveComponent != EMPTYC)
        {
            gutst = System.Math.Max(0.5f, t.Body["Intestines"].ActiveComponent.State / 2.0f);
        }
        if (t.Body["Ears"].ActiveComponent != EMPTYC)
        {
            thyst = System.Math.Max(0.5f, t.Body["Thyroid"].ActiveComponent.State / 2.0f);
        }
        
        foreach (KeyValuePair<string,Slot> s in t.Body)
        {
            t.Atts["Metabol"].Value += WAtt(s.Value, i);
        }

        t.Atts["Metabol"].Value = (int)(t.Atts["Metabol"].Value * stomst * gutst * thyst);
    }

    void PullVisDist(Being t)
    {
        int i = 22;
        t.Atts["VisDist"].Value = System.Math.Max(WAtt(t.Body["EyeR"], i), WAtt(t.Body["EyeL"], i)) + WAtt(t.Body["Face"], i); //Best eye plus face augment if applicable
    }

    void PullVisDet(Being t)
    {
        int i = 23;
        int visdetpot = System.Math.Max(WAtt(t.Body["EyeR"], i), WAtt(t.Body["EyeL"], i)); //Best eye plus face augment if applicable
        t.Atts["VisDet"].Value = System.Math.Min(WAtt(t.Body["Occipital"], i) + WAtt(t.Body["OccAug"], i), visdetpot) + WAtt(t.Body["Face"], i); // Detail potential capped by what the occipital lobe can process
    }

    void PullSpectra(Being t)
    {
        int virn = 24;
        int vuvn = 25;
        int vxrn = 26;

        t.Atts["VIR"].Value = (WAtt(t.Body["Occipital"], virn) | WAtt(t.Body["OccAug"], virn) & (WAtt(t.Body["EyeR"], virn) | WAtt(t.Body["EyeL"], virn))); // If the occipital can intake IR and at least one eye provides IR signal
        t.Atts["VUV"].Value = (WAtt(t.Body["Occipital"], vuvn) | WAtt(t.Body["OccAug"], vuvn) & (WAtt(t.Body["EyeR"], vuvn) | WAtt(t.Body["EyeL"], vuvn)));
        t.Atts["VXR"].Value = (WAtt(t.Body["Occipital"], vxrn) | WAtt(t.Body["OccAug"], vxrn) & (WAtt(t.Body["EyeR"], vxrn) | WAtt(t.Body["EyeL"], vxrn)));
    }

    void PullVLit(Being t)
    {
        int i = 27;
        t.Atts["VisLit"].Value = System.Math.Max(WAtt(t.Body["EyeR"], i), WAtt(t.Body["EyeL"], i)) + WAtt(t.Body["Face"],i) + WAtt(t.Body["Skull"],i); // Whichever is better as seeing in the dark, and bonuses from face/skull augs
    }

    void PrintAtts(Being t)
    {
        string vircap = "";
        string vuvcap = "";
        string vxrcap = "";

        if (t.Atts["VIR"].Value == 1)
        {
            vircap = "This body is capable of processing infrared light." + "\n";
        }
        else
        {
            vircap = "This body is not capable of processing infrared light." + "\n";
        }
        if (t.Atts["VUV"].Value == 1)
        {
            vuvcap = "This body is capable of processing ultraviolet light." + "\n";
        }
        else
        {
            vuvcap = "This body is not capable of processing ultraviolet light." + "\n";
        }
        if (t.Atts["VXR"].Value == 1)
        {
            vxrcap = "This body is capable of processing xray spectral light." + "\n";
        }
        else
        {
            vxrcap = "This body is not capable of processing xray spectral light." + "\n";
        }
        
        moltxt.text = "This body's maximum organic load is " + t.Atts["MOL"].Value;
        maltxt.text = "This body's maximum augmentation load is " + t.Atts["MAL"].Value;
        coltxt.text = "This body's current organic load is " + t.Atts["COL"].Value;
        caltxt.text = "This body's current augmentation load is " + t.Atts["CAL"].Value;
        presstxt.text = "This body's bench press rating is " + t.Atts["Press"].Value;
        squattxt.text = "This body's squat rating is " + t.Atts["Squat"].Value;
        griptxt.text = "This body's grip strength rating is " + t.Atts["Grip"].Value;
        speedtxt.text = "This body's sprint speed rating is " + t.Atts["Speed"].Value;
        agitxt.text = "This body's agility rating is " + t.Atts["Agi"].Value;
        defttxt.text = "This body's deftness rating is " + t.Atts["Deft"].Value;
        alerttxt.text = "This body's alertness rating is " + t.Atts["Alert"].Value;
        reflextxt.text = "This body's reflex response rating is " + t.Atts["Reflex"].Value;
        spatialtxt.text = "This body's spatial sense rating is " + t.Atts["Spatial"].Value;
        stresstxt.text = "This body's stress response rating is " + t.Atts["Stress"].Value;
        paintoltxt.text = "This body's pain tolerance rating is " + t.Atts["PainTol"].Value;
        emotxt.text = "This body's emotional control is " + t.Atts["EmoCon"].Value;
        critthinktxt.text = "This body's critical thought rating is " + t.Atts["CritThink"].Value;
        charmtxt.text = "This body's charm rating is " + t.Atts["Charm"].Value;
        threattxt.text = "This body's threat rating is " + t.Atts["Threat"].Value;
        diplotxt.text = "This body's diplomacy rating is " + t.Atts["Diplo"].Value;
        waketxt.text = "This body's wakefulness rating is " + t.Atts["Wake"].Value;
        metatxt.text = "This body's metabolic efficiency rating is " + t.Atts["Metabol"].Value;
        visdisttxt.text = "This body's maximum distance of vision is " + t.Atts["VisDist"].Value;
        visdettxt.text = "This body's visual detail threshold rating is " + t.Atts["VisDet"].Value;
        spectratxt.text = vircap + vuvcap + vxrcap;
        vislittxt.text = "This body's low-light vision rating is " + t.Atts["VisLit"].Value;
    }
    
    void PullAttributes(Being t)
    {
        PullMOL(t);
        PullMAL(t);
        PullCOL(t);
        PullCAL(t);
        PullPress(t);
        PullSquat(t);
        PullGrip(t);
        PullSpeed(t);
        PullAgility(t);
        PullDeft(t);
        PullAlert(t);
        PullReflex(t);
        PullSpatial(t);
        PullStress(t);
        PullPainTol(t);
        PullEmo(t);
        PullCritThink(t);
        PullCharm(t);
        PullThreat(t);
        PullDiplo(t);
        PullWake(t);
        PullMetabol(t);
        PullVisDist(t);
        PullVisDet(t);
        PullSpectra(t);
        PullVLit(t);

        if(t == PLAYER)
        {
            PrintAtts(t);
        }

        UpdateUI();
    }

    #endregion
    // Load a CSV file into an array of rows and columns.
    // Assume there may be blank lines but every line has the same number of fields.
    private string[,] LoadCsv(string filename) //Use @ASSETS/whatever
    {
        // Get the file's text.
        string whole_file = System.IO.File.ReadAllText(filename);

        // Split into lines.
        whole_file = whole_file.Replace("\n", "");
        string[] lines = whole_file.Split(new char[] { '\r' });

        // See how many rows and columns there are.
        int num_rows = lines.Length;
        int num_cols = lines[0].Split(',').Length;

        // Allocate the data array.
        string[,] values = new string[num_rows, num_cols];

        // Load the array.
        for (int r = 0; r < num_rows; r++)
        {
            string[] line_r = lines[r].Split(',');
            for (int c = 0; c < num_cols; c++)
            {
                values[r, c] = line_r[c];
            }
        }

        // Return the values.

        return values;
    }

    void InstallComponent(Component cmp) // needs to check if that slot needs an organic item, or if the part can be installed (limb parts cannot) (maybe no human parts can be installed?)
    {
        Slot target = null;
        KeyValuePair<string, Slot> tkvp;

        int targetID = 0;
        int cmpID = cmp.CompID;
        int augID = 0;

        foreach (string str in BanList)
        {
            if (str == cmp.MakeModel.Name)
            {
                PushEvent("You cannot transplant that.\n");
                return;
            };
        }

        foreach (KeyValuePair<string, Slot> s in PLAYER.Body) // Select the target slot using the make model info
        {
            if (cmp.MakeModel.Slot == s.Value.Name)
            {
                target = s.Value;
                tkvp = s;
                targetID = s.Value.ActiveComponent.CompID;
            }
        }
               
        #region Limb Gap Check

        if (cmp.MakeModel.Slot == "Right Upper Arm" && PLAYER.Body["ArmRF"].ActiveComponent.MakeModel.Attributes[31] == 1)
        {
            PushEvent("You cannot have a robotic gap in organic limbs.\n");
            return;
        }
        if (cmp.MakeModel.Slot == "Left Upper Arm" && PLAYER.Body["ArmLF"].ActiveComponent.MakeModel.Attributes[31] == 1)
        {
            PushEvent("You cannot have a robotic gap in organic limbs.\n");
            return;
        }
        if (cmp.MakeModel.Slot == "Right Forearm" && PLAYER.Body["HandR"].ActiveComponent.MakeModel.Attributes[31] == 1)
        {
            PushEvent("You cannot have a robotic gap in organic limbs.\n");
            return;
        }
        if (cmp.MakeModel.Slot == "Left Forearm" && PLAYER.Body["HandL"].ActiveComponent.MakeModel.Attributes[31] == 1)
        {
            PushEvent("You cannot have a robotic gap in organic limbs.\n");
            return;
        }

        if (cmp.MakeModel.Slot == "Right Thigh" && PLAYER.Body["LegRL"].ActiveComponent.MakeModel.Attributes[31] == 1)
        {
            PushEvent("You cannot have a robotic gap in organic limbs.\n");
            return;
        }
        if (cmp.MakeModel.Slot == "Left Thigh" && PLAYER.Body["LegLL"].ActiveComponent.MakeModel.Attributes[31] == 1)
        {
            PushEvent("You cannot have a robotic gap in organic limbs.\n");
            return;
        }
        if (cmp.MakeModel.Slot == "Right Crus" && PLAYER.Body["FootR"].ActiveComponent.MakeModel.Attributes[31] == 1)
        {
            PushEvent("You cannot have a robotic gap in organic limbs.\n");
            return;
        }
        if (cmp.MakeModel.Slot == "Left Crus" && PLAYER.Body["FootL"].ActiveComponent.MakeModel.Attributes[31] == 1)
        {
            PushEvent("You cannot have a robotic gap in organic limbs.\n");
            return;
        }

        #endregion

        if (target.NeedsOrg == 1 && PLAYER.Body[target.Owner].ActiveComponent.MakeModel.Attributes[31] != 1) //Check if the aug is being installed into a robotic part
        {
            PushEvent("This component is designed for organic components.\n");
            return;
        }
                       
        if (target.ActiveComponent.MakeModel.Attributes[31] == 1 && cmp.MakeModel.Attributes[31] != 1) // If you are trying to replace an organic with a non-organic
        {
            foreach (KeyValuePair<string, Slot> m in PLAYER.Body)
            {
                if (m.Value.Owner == tkvp.Key && m.Value.NeedsOrg == 1) // Find its augmentation
                {
                    augID = m.Value.ActiveComponent.CompID; // Note the augment's ID
                    RemoveComponent(m.Value); // And remove it
                }
            }
        }

        if (target.ActiveComponent != EMPTYC)
        {
            RemoveComponent(target); // Remove the targeted organ
        }

        target.ActiveComponent = cmp;

        PullAttributes(PLAYER); // UPDATE attributes

        if (PLAYER.Atts["MOL"].Value < PLAYER.Atts["COL"].Value)
        {
            PushEvent("This would exceed your maximum organic load by " + (PLAYER.Atts["COL"].Value - PLAYER.Atts["MOL"].Value).ToString() + "\nUpgrade your heart & lungs, or cut something off.\n");
            if(targetID != 0) //If you actually removed something
            {
                foreach (Component c in Inventory) // Look through the inventory
                {
                    if (c.CompID == targetID) // Check each one's ID for the part you removed
                    {
                        target.ActiveComponent = c; // And put that part back in
                    }
                }
            } else
            {
                target.ActiveComponent = EMPTYC; // if not take out what you just installed
            }
            if(augID != 0) //If the component had an augmentation
            {
                foreach(Component c in Inventory) // Look through the inventory
                {
                    if(c.CompID == augID) // Check each one's ID for the part you removed
                    {
                        foreach(KeyValuePair<string, Slot> kvp in PLAYER.Body)
                        {
                            if(c.MakeModel.Slot == kvp.Value.Name)
                            {
                                kvp.Value.ActiveComponent = c;
                            }
                        }
                    }
                }
            }

            Component TargetLoc = null;
            Component AugLoc = null;
            
            foreach(Component c in Inventory)
            {
                if(c.CompID == targetID)
                {
                    TargetLoc = c;
                }
                if(c.CompID == augID)
                {
                    AugLoc = c;
                }
            }

            if(TargetLoc != null)
            {
                Inventory.Remove(TargetLoc);
                Debug.Log("Removed the " + TargetLoc.MakeModel.Name + " ID no. " + TargetLoc.CompID);
            }

            if(AugLoc != null)
            {
                Inventory.Remove(AugLoc);
            }
            
            PullAttributes(PLAYER); // UPDATE attributes
            
            return;
        }

        if (PLAYER.Atts["MAL"].Value < PLAYER.Atts["CAL"].Value)
        {
            PushEvent("This would exceed your maximum augmentation load by " + (PLAYER.Atts["CAL"].Value - PLAYER.Atts["MAL"].Value).ToString() + "\nUpgrade your heart, spine, or battery.\n");
            if (targetID != 0) //If you actually removed something
            {
                foreach (Component c in Inventory) // Look through the inventory
                {
                    if (c.CompID == targetID) // Check each one's ID for the part you removed
                    {
                        target.ActiveComponent = c; // And put that part back in
                    }
                }
            }
            else
            {
                target.ActiveComponent = EMPTYC; // if not take out what you just installed
            }
            if (augID != 0) //If the component had an augmentation
            {
                foreach (Component c in Inventory) // Look through the inventory
                {
                    if (c.CompID == augID) // Check each one's ID for the part you removed
                    {
                        foreach (KeyValuePair<string, Slot> kvp in PLAYER.Body)
                        {
                            if (c.MakeModel.Slot == kvp.Value.Name)
                            {
                                kvp.Value.ActiveComponent = c;
                            }
                        }
                    }
                }
            }

            Component TargetLoc = null;
            Component AugLoc = null;

            foreach (Component c in Inventory)
            {
                if (c.CompID == targetID)
                {
                    TargetLoc = c;
                }
                if (c.CompID == augID)
                {
                    AugLoc = c;
                }
            }

            if (TargetLoc != null)
            {
                Inventory.Remove(TargetLoc);
                Debug.Log("Removed the " + TargetLoc.MakeModel.Name + " ID no. " + TargetLoc.CompID);
            }

            if (AugLoc != null)
            {
                Inventory.Remove(AugLoc);
            }

            PullAttributes(PLAYER); // UPDATE attributes

            return;
        }



        PullAttributes(PLAYER); // UPDATE attributes
        PushEvent("You have installed the " + cmp.MakeModel.Name + " into your body.\n");

        Inventory.Remove(cmp);

        UpdateUIInventory();
    }

    void RemoveComponent(Slot s) // needs lots of checks from medical supplies to tools to children removal and dependency
    {
        if (s.ActiveComponent != EMPTYC)
        {
            Inventory.Add(s.ActiveComponent); // Puts it into inventory
            s.ActiveComponent = EMPTYC; // Makes the slot empty
            PullAttributes(PLAYER);
        }
    }

    public void SelectSlot(string s)
    {
        SelectedSlot = PLAYER.Body[s];
    }

    public void SelectComponent(int i)
    {
        if (i < Inventory.Count)
        {
            SelectedComponent = Inventory[i];
        }
        else
        {
            SelectedComponent = EMPTYC;
        }
    }

    public void CompareSelected()
    {
        if(SelectedComponent == EMPTYC) // No selection check
        {
            PushEvent("You do not have a component selected.\n");
            return;
        }


        Slot target = null;
        KeyValuePair<string, Slot> tkvp;

        int targetID = 0;
        int cmpID = SelectedComponent.CompID;
        int augID = 0;
        int parentID = 0;
        int orgparentID = 0;

        string compstr = "";

        List<int> oldatts = new List<int>();

        foreach(KeyValuePair<string, Attribute> akvp in PLAYER.Atts) // grab all of the old attributes
        {
            oldatts.Add(akvp.Value.Value);
        }

        foreach (KeyValuePair<string, Slot> s in PLAYER.Body) // Select the target slot using the make model info
        {
            if (SelectedComponent.MakeModel.Slot == s.Value.Name)
            {
                target = s.Value;
                tkvp = s;
                targetID = s.Value.ActiveComponent.CompID;
            }
        }

        if(SelectedComponent.MakeModel.Name == target.ActiveComponent.MakeModel.Name && target.ActiveComponent.State == SelectedComponent.State)
        {
            PushEvent("These components are identical!\n");
            return;
        }

        // Swaps

        if (target.ActiveComponent.MakeModel.Attributes[31] == 1 && SelectedComponent.MakeModel.Attributes[31] != 1) // If you are trying to replace an organic with a non-organic
        {
            foreach (KeyValuePair<string, Slot> m in PLAYER.Body)
            {
                if (m.Value.Owner == tkvp.Key && m.Value.NeedsOrg == 1) // Find its augmentation
                {
                    augID = m.Value.ActiveComponent.CompID; // Note the augment's ID
                    RemoveComponent(m.Value); // And remove it
                }
            }
        }

        if(target.NeedsOrg == 1 && PLAYER.Body[target.Owner].ActiveComponent.MakeModel.Attributes[31] != 1) //If the target slot needs an organic parent and the current parent is not organic
        {
            Component orgpar = new Component(); //Create a new component
            orgpar.State = 2;
            orgpar.CompID = CompIDNow; //Give it a unique ID
            orgparentID = orgpar.CompID; //Note the ID for removal later
            CompIDNow++;

            parentID = PLAYER.Body[target.Owner].ActiveComponent.CompID; // Note the parent's ID
            RemoveComponent(PLAYER.Body[target.Owner]); // Take out the parent

            foreach(MakeModel mm in HumanMakeModels) // Look through the organic parts list
            {
                if(mm.Slot == target.Name) // Find the organic MM that goes there
                {
                    orgpar.MakeModel = mm; // Set the new component as that organ
                }
            }

            PLAYER.Body[target.Owner].ActiveComponent = orgpar; // Make the parent the organic version (leaving the old robotic version in the inventory)

            compstr += "(If installed into an organic component)\n";
        }

        if (target.ActiveComponent != EMPTYC)
        {
            RemoveComponent(target); // Remove the targeted organ
        }

        target.ActiveComponent = SelectedComponent; //Put the selected component into place


        PullAttributes(PLAYER); // UPDATE attributes

        compstr += "This component would:\n";
        int noeff = 0;

        for(int i = 0; i < PLAYER.Atts.Count - 5; i++) // Stopping at visual detail
        {
            string inordec;
            int difference = PLAYER.Atts.ElementAt(i).Value.Value - oldatts[i];

            if(difference > 0)
            {
                inordec = "INCREASE";
            }
            else
            {
                difference *= -1;
                inordec = "DECREASE";
            }

            if(difference != 0)
            {
                compstr += inordec + " your " + PLAYER.Atts.ElementAt(i).Value.Name + " by " + difference + "\n";
            } else
            {
                noeff += 1;
            }

        }

        for(int i = PLAYER.Atts.Count - 4; i < PLAYER.Atts.Count - 1; i++)
        {
            string inordec;
            int difference = PLAYER.Atts.ElementAt(i).Value.Value - oldatts[i];

            if (difference > 0)
            {
                inordec = "GIVE you ";
            }
            else
            {
                inordec = "REMOVE your ";
            }

            if (difference != 0)
            {
                compstr += inordec + PLAYER.Atts.ElementAt(i).Value.Name + "\n";
            } else
            {
                noeff += 1;
            }
        }

        if(PLAYER.Atts.ElementAt(PLAYER.Atts.Count - 1).Value.Value != oldatts[PLAYER.Atts.Count - 1])
        {
            int difference = PLAYER.Atts.ElementAt(PLAYER.Atts.Count - 1).Value.Value - oldatts[PLAYER.Atts.Count - 1];
            string inordec;

            if (difference > 0)
            {
                inordec = "INCREASE";
            }
            else
            {
                inordec = "DECREASE";
                difference *= -1;
            }

            compstr += inordec + " your " + PLAYER.Atts.ElementAt(PLAYER.Atts.Count - 1).Value.Name + " by " + difference + "\n";
        }
        else
        {
            noeff += 1;
        }

        if(noeff == 27)
        {
            compstr += "Have NO IMMEDIATE EFFECTS.\nIt may require improvement of other components.\n";
        }

        PushEvent(compstr);

        if (parentID != 0) // If you had to swap out the parent
        {
            foreach (Component c in Inventory)
            {
                if (c.CompID == parentID)
                {
                    PLAYER.Body[target.Owner].ActiveComponent = c; // Put the parent back (leaving a duplicate in inventory)
                }
            }
        }

        if (targetID != 0) //If you actually removed something
        {
            foreach (Component c in Inventory) // Look through the inventory
            {
                if (c.CompID == targetID) // Check each one's ID for the part you removed
                {
                    target.ActiveComponent = c; // And put that part back in (leaving a duplicate in the inventory)
                }
            }
        }
        else
        {
            target.ActiveComponent = EMPTYC; // if not take out what you just installed
        }

        if (augID != 0) //If the component had an augmentation
        {
            foreach (Component c in Inventory) // Look through the inventory
            {
                if (c.CompID == augID) // Check each one's ID for the part you removed
                {
                    foreach (KeyValuePair<string, Slot> kvp in PLAYER.Body) // Check the player's body
                    {
                        if (c.MakeModel.Slot == kvp.Value.Name) // When you find the slot the component belongs to
                        {
                            kvp.Value.ActiveComponent = c; //Put the augment in that slot (leaving a duplicate in the inventory)
                        }
                    }
                }
            }
        }        

        Component TargetLoc = null;
        Component AugLoc = null;
        Component ParLoc = null;

        foreach (Component c in Inventory)
        {
            if (c.CompID == targetID)
            {
                TargetLoc = c;
            }
            if (c.CompID == augID)
            {
                AugLoc = c;
            }
            if(c.CompID == parentID)
            {

            }
        }

        if (TargetLoc != null)
        {
            Inventory.Remove(TargetLoc); // Destroy the duplicate part
        }

        if (AugLoc != null)
        {
            Inventory.Remove(AugLoc); // Destroy the duplicate aug
        }

        if(ParLoc != null)
        {
            Inventory.Remove(ParLoc); // Destroy the duplicate parent (Organic was overwritten by putting the old parent back)
        }

        PullAttributes(PLAYER); // UPDATE attributes
        return;        
    }

    public void RemoveSelected()
    {
        if(SelectedSlot.ActiveComponent == EMPTYC)
        {
            PushEvent("There is nothing in this slot!\n");
            return;
        }

        foreach (string s in BanList)
        {
            if (SelectedSlot.ActiveComponent.MakeModel.Name == s)
            {
                PushEvent("You cannot remove this organ without replacing it.\nTry installing a replacement instead.\n");
                return;
            }
        }
        RemoveComponent(SelectedSlot);
        UpdateUISlots();
    }
       
    public void InstallSelected()
    {
        if (SelectedComponent != EMPTYC)
        {
            InstallComponent(SelectedComponent);
            UpdateUI();
        }
        else
        {
            PushEvent("You have no component selected.\n");
        }
        SelectedComponent = EMPTYC;
    }

    public void DestroySelected()
    {
        if (SelectedComponent != EMPTYC)
        {
            PushEvent("You have permanently discarded the " + SelectedComponent.MakeModel.Name + ".\n");
            Inventory.Remove(SelectedComponent);
            SelectedComponent = EMPTYC;
            UpdateUIInventory();
        }
        else
        {
            PushEvent("You do not have a component selected.\n");
        }
    }

    public void DestroyAll()
    {
        Inventory.Clear();
        UpdateUIInventory();
        PushEvent("Your inventory was cleared.\n");
    }

    #region UI Handling

    void UpdateUI()
    {
        UpdateUISlots();
        UpdateUIInventory();
    }

    string InvString(int i)
    {
        int c = Inventory.Count;

        // Debug.Log(Inventory.Count);
        
        if(i+1 <= c)
        {
            return Inventory[i].MakeModel.Name + "\n" + Inventory[i].MakeModel.Slot.ToUpper() + " [" + StateString(Inventory[i].State) + "]";
        } else
        {
            return "EMPTY";
        }
    }

    string StateString(int i)
    {
        string str = "";

        switch (i)
        {
            case 0:
                str = "DESTROYED";
                break;
            case 1:
                str = "DAMAGED";
                break;
            case 2:
                str = "FUNCTIONAL";
                break;
        }

        return str;
    }

    void SlotTextFill(Text output, Slot target)
    {
        string outstr = "";

        if (output == null)
        {
            return;
        }

        if (target.NeedsOrg == 1 && PLAYER.Body[target.Owner].ActiveComponent.MakeModel.Attributes[31] != 1)
        {
            outstr = target.Name.ToUpper() + ":\n(NONE INSTALLED)\n(" + PLAYER.Body[target.Owner].Name.ToUpper() + " is SYNTHETIC)";
            output.text = outstr;
            return;
        }

        if (target.ActiveComponent == EMPTYC | target.ActiveComponent == null)
        {
            outstr = target.Name.ToUpper() + ":\n(NONE INSTALLED)";
            output.text = outstr;
            return;
        }

        outstr = target.Name.ToUpper() + ":\n(" + StateString(target.ActiveComponent.State) + ") \n" + target.ActiveComponent.MakeModel.Name + "\n" + target.Desc;

        // Debug.Log("Filling " + output + " with information on the " + target.Name);
        output.text = outstr;
    }

    void UpdateUISlots()
    {
        SlotTextFill(bstxt, PLAYER.Body["BrainStem"]);
        SlotTextFill(bsatxt, PLAYER.Body["StemAug"]);
        SlotTextFill(certxt, PLAYER.Body["Cerebellum"]);
        SlotTextFill(ceratxt, PLAYER.Body["CerAug"]);
        SlotTextFill(frotxt, PLAYER.Body["Frontal"]);
        SlotTextFill(froatxt, PLAYER.Body["FroAug"]);
        SlotTextFill(partxt, PLAYER.Body["Parietal"]);
        SlotTextFill(paratxt, PLAYER.Body["ParAug"]);
        SlotTextFill(occtxt, PLAYER.Body["Occipital"]);
        SlotTextFill(occatxt, PLAYER.Body["OccAug"]);
        SlotTextFill(temtxt, PLAYER.Body["Temporal"]);
        SlotTextFill(tematxt, PLAYER.Body["TemAug"]);
        SlotTextFill(hyptxt, PLAYER.Body["Hypothalamus"]);
        SlotTextFill(hypatxt, PLAYER.Body["HypAug"]);
        SlotTextFill(pittxt, PLAYER.Body["Pituitary"]);
        SlotTextFill(pitatxt, PLAYER.Body["PitAug"]);
        SlotTextFill(pintxt, PLAYER.Body["Pineal"]);
        SlotTextFill(pinatxt, PLAYER.Body["PinAug"]);
        SlotTextFill(thatxt, PLAYER.Body["Thalamus"]);
        SlotTextFill(thaatxt, PLAYER.Body["ThaAug"]);
        SlotTextFill(bastxt, PLAYER.Body["Basal"]);
        SlotTextFill(basatxt, PLAYER.Body["BasAug"]);
        SlotTextFill(limtxt, PLAYER.Body["Limbic"]);
        SlotTextFill(limatxt, PLAYER.Body["LimAug"]);
        SlotTextFill(hrttxt, PLAYER.Body["Heart"]);
        SlotTextFill(hrtatxt, PLAYER.Body["HeartAug"]);
        SlotTextFill(stomtxt, PLAYER.Body["Stomach"]);
        SlotTextFill(stomatxt, PLAYER.Body["StomachAug"]);
        SlotTextFill(intesttxt, PLAYER.Body["Intestines"]);
        SlotTextFill(intestatxt, PLAYER.Body["IntestineAug"]);
        SlotTextFill(adrtxt, PLAYER.Body["Adrenal"]);
        SlotTextFill(adratxt, PLAYER.Body["AdrenalAug"]);
        SlotTextFill(thytxt, PLAYER.Body["Thyroid"]);
        SlotTextFill(thyatxt, PLAYER.Body["ThyroidAug"]);
        SlotTextFill(rhumtxt, PLAYER.Body["BArmRU"]);
        SlotTextFill(lhumtxt, PLAYER.Body["BArmLU"]);
        SlotTextFill(rrutxt, PLAYER.Body["BArmRF"]);
        SlotTextFill(lrutxt, PLAYER.Body["BArmLF"]);
        SlotTextFill(rfemtxt, PLAYER.Body["BLegRU"]);
        SlotTextFill(lfemtxt, PLAYER.Body["BLegLU"]);
        SlotTextFill(rfibtxt, PLAYER.Body["BLegRL"]);
        SlotTextFill(lfibtxt, PLAYER.Body["BLegLL"]);
        SlotTextFill(ribtxt, PLAYER.Body["Ribs"]);
        SlotTextFill(skltxt, PLAYER.Body["Skull"]);
        SlotTextFill(peltxt, PLAYER.Body["Pelvis"]);
        SlotTextFill(spitxt, PLAYER.Body["Spine"]);
        SlotTextFill(spiatxt, PLAYER.Body["SpineAug"]);
        SlotTextFill(ruatxt, PLAYER.Body["ArmRU"]);
        SlotTextFill(luatxt, PLAYER.Body["ArmLU"]);
        SlotTextFill(rfatxt, PLAYER.Body["ArmRF"]);
        SlotTextFill(lfatxt, PLAYER.Body["ArmLF"]);
        SlotTextFill(rthitxt, PLAYER.Body["LegRU"]);
        SlotTextFill(lthitxt, PLAYER.Body["LegLU"]);
        SlotTextFill(rcrutxt, PLAYER.Body["LegRL"]);
        SlotTextFill(lcrutxt, PLAYER.Body["LegLL"]);
        SlotTextFill(rhtxt, PLAYER.Body["HandR"]);
        SlotTextFill(lhtxt, PLAYER.Body["HandL"]);
        SlotTextFill(rftxt, PLAYER.Body["FootR"]);
        SlotTextFill(lfttxt, PLAYER.Body["FootL"]);
        SlotTextFill(lngtxt, PLAYER.Body["Lungs"]);
        SlotTextFill(lngatxt, PLAYER.Body["LungAug"]);
        SlotTextFill(voctxt, PLAYER.Body["Vocal"]);
        SlotTextFill(vocatxt, PLAYER.Body["VocalAug"]);
        SlotTextFill(leyetxt, PLAYER.Body["EyeL"]);
        SlotTextFill(reyetxt, PLAYER.Body["EyeR"]);
        SlotTextFill(eartxt, PLAYER.Body["Ears"]);
        SlotTextFill(jawtxt, PLAYER.Body["Jaw"]);
        SlotTextFill(factxt, PLAYER.Body["Face"]);
        SlotTextFill(battxt, PLAYER.Body["Battery"]);        
}

    void UpdateUIInventory()
    {

        Inv1.text = InvString(0);
        Inv2.text = InvString(1);
        Inv3.text = InvString(2);
        Inv4.text = InvString(3);
        Inv5.text = InvString(4);
        Inv6.text = InvString(5);
        Inv7.text = InvString(6);
        Inv8.text = InvString(7);
        Inv9.text = InvString(8);
        Inv10.text = InvString(9);
        Inv11.text = InvString(10);
        Inv12.text = InvString(11);
        Inv13.text = InvString(12);
        Inv14.text = InvString(13);
    }

    void UpdateUIOther()
    {
        // HoursText.text = (PLAYER.Atts["Wake"].Value * 22)/;
    }

    void PushEvent(string s)
    {
        EventText.text += "\n> [" + System.DateTime.Now.ToString("h:mm:ss") + "] " + s;
    }
    
    void PushScav(string s)
    {
        ScavText.text += "\n> [" + System.DateTime.Now.ToString("h:mm:ss") + "] " + s;
    }

    public void ClearEvents()
    {
        EventText.text = "> ASCENSION";
    }

    public void ClearScav()
    {
        ScavText.text = "> ASCENSION";
    }

    #endregion

    void Death(string reason) // This accepts a death reason and runs the death menu
    {

    }
    
    public void DeadCheck(Being t) //Check if the player is dead based on component status
    {
        int braindamage = 0;

        if (t.Body["Brainstem"].ActiveComponent.State == 0)
        {
            Death("Your brain stem was severed.");
        }

        if (t.Body["Cerebellum"].ActiveComponent.State == 0)
        {
            braindamage++;
        }
        if (t.Body["Frontal"].ActiveComponent.State == 0)
        {
            braindamage++;
        }
        if (t.Body["Parietal"].ActiveComponent.State == 0)
        {
            braindamage++;
        }
        if (t.Body["Occipital"].ActiveComponent.State == 0)
        {
            braindamage++;
        }
        if (t.Body["Temporal"].ActiveComponent.State == 0)
        {
            braindamage++;
        }
        if (t.Body["Hypothalamus"].ActiveComponent.State == 0)
        {
            braindamage++;
        }
        if (t.Body["Pituitary"].ActiveComponent.State == 0)
        {
            braindamage++;
        }
        if (t.Body["Pineal"].ActiveComponent.State == 0)
        {
            braindamage++;
        }
        if (t.Body["Thalamus"].ActiveComponent.State == 0)
        {
            braindamage++;
        }
        if (t.Body["Basal"].ActiveComponent.State == 0)
        {
            braindamage++;
        }

        if(t.Body["Heart"].ActiveComponent.State == 0)
        {
            Death("Your heart was stopped.");
        }
        if (t.Body["Lungs"].ActiveComponent.State == 0)
        {
            Death("You suffocated from collapsed lungs.");
        }

        if (braindamage >= 3)
        {
            Death("You suffered too much damage to your brain.");
        }
    }
    
    void TakeDamage(Being target)
    {
        int rn = 0;
        rn = rnd.Next(0, target.Body.Count);
        target.Body[BodySlotKeys[rn]].ActiveComponent.State = System.Math.Max(0, target.Body[BodySlotKeys[rn]].ActiveComponent.State - 1);
        PullAttributes(target);
        //Debug.Log("The " + target.Body[BodySlotKeys[rn]].Name + " now has a state of " + target.Body[BodySlotKeys[rn]].ActiveComponent.State);

        UpdateUI();
    }

    public void Gimme() // make this take arguments for component info
    {
        
        int draw = rnd.Next(0,Catalog.Count - 1);
        MakeModel RndModel = Catalog[draw];

        Component newitem = new Component {State = 2, OperatingWeeks = 0, InstalledNew = 1, CompID = CompIDNow, MakeModel = RndModel };
        CompIDNow++;

        Inventory.Add(newitem);
        UpdateUIInventory();

        PushEvent(newitem.MakeModel.Name + " has been added to your inventory.\n");
        PushScav(newitem.MakeModel.Name + " has been added to your inventory.\n");

        //Debug.Log("The item " + newitem.MakeModel.Name + " was just added to your inventory. It is a " + newitem.MakeModel.Slot + ".");
    }

    void MMGrab(List<MakeModel> partsList, string DTFile)
    {
        //Create empty makemodel parts to fill before writing
        string[,] dropTable = LoadCsv(DTFile);
        string pname;
        string pdesc;
        string pslot;
        int pnum;
        
        for (int i = 0;  i < dropTable.GetLength(0) - 2; i++)  // The -2 here is to keep the loop from running off of the list
        {
            int[] patts = new int[dropTable.GetLength(1)-4];

            pname = dropTable[i + 2,2]; //Right now this is indexing away from the headers with the +2
            pdesc = dropTable[i + 2, 3];
            pslot = dropTable[i + 2, 0];
            pnum = int.Parse(dropTable[i + 2, 1]);
            for (int c = 0; c < dropTable.GetLength(1) - 4; c++)
            {
                // Debug.Log(dropTable[i + 2, c+ 4]);
                patts[c] = int.Parse(dropTable[i + 2, c + 4]); //plus 4 pushes past the string data in the left columns
            }

            partsList.Add(new MakeModel { Name = pname, Description = pdesc, Slot = pslot, ItemNumber = pnum, Attributes = patts });
        }
    }

    void HumanCompsGrab()
    {
        for (int i = 0; i < HumanMakeModels.Count; i++)
        {
            HumanComponents.Add(new Component { State = 2, OperatingWeeks = 0, InstalledNew = 1, CompID = CompIDNow, MakeModel = HumanMakeModels[i] }); //Should human component operating weeks be randomized depending on player age??
            //Debug.Log("Just added a component which is a " + HumanComponents[i].MakeModel.Slot);
            //Debug.Log("I was trying to add a " + HumanMakeModels[i].Slot);
            CompIDNow++;
        }
    }

    void InitBeing(Being being) //Can be a body function?
    {
        foreach (KeyValuePair<string, Slot> kvp in BodyDict)
        {
            being.Body.Add(kvp.Key, kvp.Value);
        }
        foreach (KeyValuePair<string, Attribute> kvp in AttDict)
        {
            being.Atts.Add(kvp.Key, kvp.Value);
        }
    }

    void EmptyBody(Being t)
    {
        foreach(KeyValuePair<string, Slot> s in t.Body)
        {
            s.Value.ActiveComponent = EMPTYC;
        }
    }

    void OnlyHuman(Being t) //Fill body slots with human components
    {
        foreach (KeyValuePair<string,Slot> s in t.Body)
        {
            
            foreach (Component c in HumanComponents)
            {
                if (c.MakeModel.Slot == s.Value.Name) //Filling slots by matching makemodel slot type to the name of the slot
                {
                    s.Value.ActiveComponent = c;
                    s.Value.ActiveComponent.CompID = CompIDNow;
                    CompIDNow++;
                }
            }
        }

        UpdateUISlots();
    }

    #region OBSOLETE

    //void GenerateOpponent()
    //{
    //    EmptyBody(OPPONENT);
    //    OnlyHuman(OPPONENT);
    //    PullAttributes(OPPONENT);
    //    foreach (KeyValuePair<string, Attribute> kvp in OPPONENT.Atts)
    //    {
    //        Debug.Log("This opponent's " + kvp.Value.Name + " is " + kvp.Value.Value);
    //    }
    //}

    //#region Fighting


    //void FightEvent(string s)
    //{
    //    FightText.text += "\n> [" + System.DateTime.Now.ToString("h:mm:ss") + "] " + s;
    //}

    //public void ClearFight()
    //{
    //    FightText.text = "> ASCENSION";
    //}

    //void InitFighter(Fighter fighter)
    //{
    //    fighter.lhand = EMPTYS;
    //    fighter.rhand = EMPTYS;
    //    fighter.jaws = EMPTYS;
    //    fighter.state = "awake";
    //}

    //public void Fight() // Add condition to prevent swapping limbs etc while fighting
    //{
    //    GenerateOpponent();
    //    InitFighter(PFighter);
    //    InitFighter(OFighter);
    //    //int order = rnd.Next(0,1);
    //    int order = 0;

    //    ClearEvents();
    //    FightEvent("\n\nA challenger approaches! He is entirely human. Destroy him.\n");

    //    while (PFighter.state == "awake" && OFighter.state == "awake") // As long as both fighters are awake
    //    {
    //        FightCycle(order);
    //    }

    //    FightResults();
    //}

    //void FightCycle(int order) // Make this take an int for who goes first
    //{
    //    if(order == 0) //Player goes first
    //    {
    //        PlayerTurn();
    //        OpponentTurn();
    //    } else
    //    {
    //        OpponentTurn();
    //        PlayerTurn();
    //    }
    //}

    //void PlayerTurn()
    //{
    //    FightOrFlight();
    //    OFighter.state = "dead";
    //}

    //void OpponentTurn()
    //{
    //    OFighter.state = "dead";
    //}

    //void FightOrFlight()
    //{
    //    FightEvent("What would you like to do? (Press the corresponding key)\n\na. Attack\nb. Flee\nc. Observe opponent (Free action)\n");
    //    //while (input != "a" && input != "b" && input != "c")
    //    //{
    //    //    if (input == "a")
    //    //    {
    //    //        FightEvent("You chose to attack.");
    //    //        return;
    //    //    }
    //    //    if (input == "b")
    //    //    {
    //    //        FightEvent("You chose to flee.");
    //    //        return;
    //    //    }
    //    //    if (input == "c")
    //    //    {
    //    //        FightEvent("You chose to observe.");
    //    //        return;
    //    //    }
    //    //}

    //    //foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
    //    //{
    //    //    if (Input.GetKey(vKey))
    //    //    {
    //    //        //your code here

    //    //    }
    //    //}
    //}

    //void StrikeResponse(Fighter attacker, Fighter defender)
    //{
    //    //If the player is the defender, prompt for reaction before resolving
    //    //check reflex and prompt opponent, otherwise send to resolution
    //}

    //void StrikeRes() //Intakes response information (blocked, caught, etc)
    //{
    //    //divies damage
    //}

    //void FightResults()
    //{
    //    if(OFighter.state != "awake")
    //    {
    //        FightEvent("You have killed your opponent!");
    //    }
    ////}

    //#endregion

    #endregion

    #endregion
}