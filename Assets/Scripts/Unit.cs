using System;
using System.Collections.Generic;
using UnityEngine;



public class Unit : MonoBehaviour
{
    public string Name;
    public string Job;
    public int Level, maxHP, currentHP, MP, WAtk, WDefense, MPow, MRes, Speed, Jump, MoveRange, Evade, SRes, CT;

    public int exp, maxExp;
    public Vector2 Position;
    public List<AbilitySet> AbilitySets;
    public bool IsAllied;
    public Vector2 directionFacing;

    public Animator mainAnimator;
    public Animator weaponAnimator;

    public DirectionIndicator directionIndicator;

    public Sprite displaySprite;

    public bool hasActed, hasMoved;

    public Node currentNode;

    public GameObject selectionArrow;
    
    



    private static Dictionary<string, double[]> JobGrowthRates = new Dictionary<string, double[]>()
    {
        { "Archer",       new double[] {1.3, 1.1, 1.2, 1.1, 1.0, 1.1, 1.15, 1.2} },
        { "Black Mage",   new double[] {1.2, 1.3, 1.0, 1.0, 1.4, 1.2, 1.05, 1.1} },
        { "Blue Mage",    new double[] {1.25, 1.2, 1.1, 1.1, 1.3, 1.15, 1.1, 1.2} },
        { "Fighter",      new double[] {1.4, 1.0, 1.4, 1.3, 1.0, 1.2, 1.1, 1.0} },
        { "Hunter",       new double[] {1.35, 1.1, 1.3, 1.2, 1.1, 1.1, 1.2, 1.3} },
        { "Illusionist",  new double[] {1.2, 1.3, 1.0, 1.0, 1.5, 1.3, 1.0, 1.2} },
        { "Ninja",        new double[] {1.2, 1.0, 1.3, 1.1, 1.1, 1.2, 1.25, 1.4} },
        { "Paladin",      new double[] {1.4, 1.0, 1.3, 1.4, 1.0, 1.3, 1.0, 1.2} },
        { "Soldier",      new double[] {1.35, 1.0, 1.3, 1.3, 1.0, 1.2, 1.0, 1.0} },
        { "Thief",        new double[] {1.25, 1.0, 1.2, 1.0, 1.0, 1.1, 1.3, 1.4} },
        { "White Mage",   new double[] {1.2, 1.4, 1.0, 1.0, 1.3, 1.3, 1.0, 1.0} }
    };





    private void Start()
    {
        
 
        AbilitySets = new List<AbilitySet>();
       
    }
    
    

    public void setJobandAllegianceAndInitialize(string job, bool isAllied)
    {
        IsAllied = isAllied;
        Job = job;
        Level = 1;


        string allegiance = IsAllied ? "Allied" : "Enemy";

        Name =  GetRandomName();
        gameObject.name = Name;


        int[] baseStats = UnitFactory.JobBaseStats[job];
        maxHP = baseStats[0];
        currentHP = baseStats[0];
        MP = baseStats[1];
        WAtk = baseStats[2];
        WDefense = baseStats[3];
        MPow = baseStats[4];
        MRes = baseStats[5];
        Speed = baseStats[6];
        Evade = baseStats[7];
        SRes = baseStats[8];

        LoadDisplaySprite();
        LoadAnimationController();

        mainAnimator = transform.Find("Sprite").GetComponent<Animator>();
        weaponAnimator = transform.Find("Weapon Sprite").GetComponent<Animator>();

        
        directionFacing = new Vector2(0, 1);

        

        CT = 0;

        //Debug.Log($"Unit Start() - Name: {Name}, Job: {Job}, Level: {Level}, HP: {HP}");
    }




    void LoadDisplaySprite()
    {
        // Construct the spritesheet name based on allegiance and job
        string allegiance = IsAllied ? "Allied" : "Enemy";
        string sanitizedJob = Job.Replace(" ", "_"); // Replace spaces with underscores
        string portraitPath = $"Portraits/{allegiance}_{sanitizedJob}_Portrait";

        Debug.Log(portraitPath);
       

        // Load all sprites from the specified spritesheet
        displaySprite = Resources.Load<Sprite>(portraitPath);

        

        if (displaySprite == null)
        {
            Debug.LogError("Display sprite not found!");
            return;
        }

        
    }


    string GetRandomName()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("names"); // Load file from Resources
        if (textAsset == null)
        {
            Debug.LogError("Could not find names.txt in Resources!");
            return "Unknown";
        }

        string[] names = textAsset.text.Split('\n');
        List<string> nameList = new List<string>();

        foreach (string name in names)
        {
            string trimmedName = name.Trim();
            if (!string.IsNullOrEmpty(trimmedName))
                nameList.Add(trimmedName);
        }

        return nameList.Count > 0 ? nameList[UnityEngine.Random.Range(0, nameList.Count)] : "Unknown";
    }

    void LoadAnimationController()
    {
        // Find the child named "Sprite" that has the Animator
        Transform spriteChild = transform.Find("Sprite");

        if (spriteChild == null)
        {
            Debug.LogError("Child object named 'Sprite' not found!");
            return;
        }

        Animator animator = spriteChild.GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("No Animator component found on 'Sprite' child!");
            return;
        }

        // Construct the animation controller path
        string allegiance = IsAllied ? "Allied" : "Enemy";
        string sanitizedJob = Job.Replace(" ", "_"); // Replace spaces with underscores
        string animControllerPath = $"Animations/{allegiance}_{sanitizedJob}/{allegiance}_{sanitizedJob}_OverrideController";

        // Load the Animator Override Controller from Resources
        RuntimeAnimatorController animController = Resources.Load<RuntimeAnimatorController>(animControllerPath);

        if (animController == null)
        {
            Debug.LogError($"Animation Controller not found at {animControllerPath}");
            return;
        }

        // Assign the animation controller to the Animator
        animator.runtimeAnimatorController = animController;
    }


    void setBaseStats()
    {

    }

 

    public void LevelUp()
    {
        Level++;
        double[] growth = JobGrowthRates[Job];

        maxHP = (int)(maxHP * growth[0]);
        MP = (int)(MP * growth[1]);
        WAtk = (int)(WAtk * growth[2]);
        WDefense = (int)(WAtk * growth[3]);
        MPow = (int)(MPow * growth[4]);
        MRes = (int)(MRes * growth[5]);
        Speed = (int)(Speed * growth[6]);
        Evade = (int)(Evade * growth[7]);

        Debug.Log($" {Name} the {Job} leveled up to {Level}!");
    }

    public void updateSpriteRotation()
    {
        Debug.Log("Trying to update sprite rotation");
        Vector2 blendTreeValues = PlayerController.Compass.convertDirectionToBlendTreeDirection(directionFacing);

        mainAnimator.SetFloat("directionX", blendTreeValues.x);
        mainAnimator.SetFloat("directionZ", blendTreeValues.y);
    }

    /*
    public int CalculateEvasion(Facing attackDirection)
    {
        int finalEvade = Evade;
        if (attackDirection == Facing.Side)
            finalEvade /= 2;
        else if (attackDirection == Facing.Rear)
            finalEvade /= 4;
        return Mathf.Clamp(finalEvade, 5, 95);
    }
    */

    public void IncreaseCT()
    {
        CT += Speed;
    }




    public bool CanAct()
    {
        bool canAct = CT >= 1000;
        //Debug.Log($"{Name} CanAct(): {canAct} (CT: {CT})");
        return canAct;
    }




    public void AddAbilitySet(AbilitySet abilitySet)
    {
        AbilitySets.Add(abilitySet);
    }

    public void SetSelectionArrowVisibility(bool isVisible)
    {
        if (selectionArrow != null)
            selectionArrow.SetActive(isVisible);
    }

    public UIManager uiManager;
    
    public void setDirectionFacing(Vector2 newDirection)
    {
        directionFacing = newDirection;
        updateSpriteRotation();
    }

    public void setIndicatorDirectionFacing(Vector2 newDirection)
    {
        directionIndicator.SetEnlargedSphere(newDirection);
    }   


}
