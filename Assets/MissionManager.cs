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
            "Example Description",
            new ResourceDemand[] { new ResourceDemand(Resources.ResourceType.Mass, 50) }
        ),
    };
    }
    
    public Mission RandomMissionSelection()
    {
        int index = Random.Range(0, 16);
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