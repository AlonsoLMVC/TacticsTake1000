using System;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string Name;
    public int HP, MP, PA, MA, Speed, Defense, MagicDefense, MoveRange, CT;
    public Vector2 Position;
    public List<AbilitySet> AbilitySets;

    public Animator mainAnimator;
    public Animator weaponAnimator;

    private Compass compass;

    public Vector2 directionFacing;

    public DirectionIndicator directionIndicator;




    private void Start()
    {

        mainAnimator = transform.Find("Sprite").GetComponent<Animator>(); // Ensure the Animator is on a child GameObject
        weaponAnimator = transform.Find("Weapon Sprite").GetComponent<Animator>();

        compass = GameObject.FindAnyObjectByType<Compass>();

        // Set default animation parameters
        mainAnimator.SetFloat("directionX", 0);
        mainAnimator.SetFloat("directionZ", 1);

        directionFacing = new Vector2(0, 1);

    }

    public void updateSpriteRotation()
    {
        Debug.Log("Trying to update sprite rotation");
        Vector2 blendTreeValues = compass.convertDirectionToBlendTreeDirection(directionFacing);

        mainAnimator.SetFloat("directionX", blendTreeValues.x);
        mainAnimator.SetFloat("directionZ", blendTreeValues.y);
    }

    public void updateSpriteWithBlendTreeVector()
    {
        Debug.Log("calling a method we commented out");
        /*
        Vector2 rawValues = GetComponentInChildren<DirectionIndicator>().getEnlargedSpherePosition();

        Vector2 blendTreeValues = compass.convertDirectionToBlendTreeDirection(rawValues);


        mainAnimator.SetFloat("directionX", blendTreeValues.x);
        mainAnimator.SetFloat("directionZ", blendTreeValues.y);

        directionFacing = rawValues;
        */
        

    }

    public Unit(string name, int hp, int mp, int pa, int ma, int speed, int defense, int magicDefense, int moveRange)
    {
        Name = name;
        HP = hp;
        MP = mp;
        PA = pa;
        MA = ma;
        Speed = speed;
        Defense = defense;
        MagicDefense = magicDefense;
        MoveRange = moveRange;
        CT = 0;
        Position = new Vector2(0, 0);
        AbilitySets = new List<AbilitySet>();
    }

    public void AddAbilitySet(AbilitySet abilitySet)
    {
        AbilitySets.Add(abilitySet);
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = Math.Max(0, damage - Defense);
        HP -= finalDamage;
        if (HP <= 0) HP = 0;
    }

    public void TakeMagicDamage(int damage)
    {
        int finalDamage = Math.Max(0, damage - MagicDefense);
        HP -= finalDamage;
        if (HP <= 0) HP = 0;
    }

    public void IncreaseCT()
    {
        CT += Speed;
    }

    public bool CanAct()
    {
        return CT >= 100;
    }

    public void ResetCT(int reduction)
    {
        CT = Math.Max(0, CT - reduction);
    }

    public void Move(Vector2 newPosition)
    {
        Position = newPosition;
    }

    public bool IsInRange(Unit target, ActionAbility ability)
    {
        int distance = (int)Vector2.Distance(this.Position, target.Position);
        return distance <= ability.Range;
    }

    public void PerformAction(ActionAbility ability, Unit target)
    {
        if (!IsInRange(target, ability))
        {
            Console.WriteLine($"{Name} cannot reach {target.Name}!");
            return;
        }

        if (ability.IsMagic)
        {
            if (MP < ability.MPCost)
            {
                Console.WriteLine($"{Name} does not have enough MP!");
                return;
            }

            target.TakeMagicDamage(ability.Power);
            MP -= ability.MPCost;
        }
        else
        {
            target.TakeDamage(ability.Power);
        }

        ResetCT(100);
    }





}