using System.Resources;
using UnityEditor.Rendering;
using UnityEngine;
using System.Resources;
using System;
using Random = UnityEngine.Random;
using System.Linq.Expressions;

public class MissionManager : MonoBehaviour
{
    public Mission[] missions;
    void Start()
    {
        missions = new Mission[]
    {
        new Mission(
            "Example Mass",
            new ResourceDemand[] { new ResourceDemand(Resources.ResourceType.Mass, 50) }
        ),
    
        new Mission(
            "Example Stroage",
            new ResourceDemand[] { new ResourceDemand(Resources.ResourceType.Storage, 30) }
        ),

        new Mission(
            "Example Money",
            new ResourceDemand[] { new ResourceDemand(Resources.ResourceType.Money, 60) }
        ),

        new Mission(
            "Example time",
            new ResourceDemand[] { new ResourceDemand(Resources.ResourceType.Time, 180) }
        ),

        new Mission(
            "Example water",
            new ResourceDemand[] { new ResourceDemand(Resources.ResourceType.Water, 180) }
        ),
        
        new Mission(
            "Example Oxygen i guess",
            new ResourceDemand[] { new ResourceDemand(Resources.ResourceType.Oxygen, 100) }
        ),

        new Mission(
            "Example Research",
            new ResourceDemand[] { new ResourceDemand(Resources.ResourceType.Research, 30) }
        ),

        new Mission(
            "Example Fuel",
            new ResourceDemand[] { new ResourceDemand(Resources.ResourceType.Fuel, 40) }
        ),

        new Mission(
            "Example Food",
            new ResourceDemand[] { new ResourceDemand(Resources.ResourceType.Food, 20) }
        ),
    };
    
    }
    
    public Mission RandomMissionSelection()
    {
        int index = Random.Range(0, 10); //n+1 adet olackkk parantez içi
        return missions[index];
    }
}
public class Mission{
    private string missionDescription;
    
    private ResourceDemand[] requirements;
    public Mission(string missionDescription, ResourceDemand[] requirements){
        this.missionDescription=missionDescription;
        this.requirements=requirements;
    }

    public bool IsMet(){
        for(int i = 0; i < requirements.Length; i++)
        {
            if (requirements[i].amount < Resources.Get(requirements[i].resource))
            {
                continue;
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}
public struct ResourceDemand{
    public Resources.ResourceType resource;
    public float amount;
    public ResourceDemand(Resources.ResourceType resource, float amount) {
        this.resource = resource;
        this.amount = amount;
    }
}