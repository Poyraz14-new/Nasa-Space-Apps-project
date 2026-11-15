using UnityEngine;

public class PartButton : ShopButton
{
    public enum PartType { Cabin, Body, Thruster }

    [SerializeField] private PartType partType;
    [SerializeField] [Range(1, 3)] private int tier = 1;

    public override float Price()
    {
        switch (partType)
        {
            case PartType.Cabin:
                return Cabin().money;
            case PartType.Body:
                return Body().money;
            default:
                return Thruster().money;
        }
    }

    public override string StatsText()
    {
        switch (partType)
        {
            case PartType.Cabin:
                SpaceCraftParts.Cabin cabin = Cabin();
                return $"Mass: +{cabin.mass} kg\nStorage: +{cabin.storage}\nTravel time: -{cabin.daysSaved} days";
            case PartType.Body:
                SpaceCraftParts.Body body = Body();
                return $"Mass: +{body.mass} kg\nStorage: +{body.storage}\nFuel: +{body.fuel}";
            default:
                SpaceCraftParts.Thruster thruster = Thruster();
                return $"Mass: +{thruster.mass} kg\nFuel: +{thruster.fuel}";
        }
    }

    public override bool Buy(TravelSystem travelSystem)
    {
        bool bought;
        switch (partType)
        {
            case PartType.Cabin:
                bought = travelSystem.BuyPart(Cabin());
                break;
            case PartType.Body:
                bought = travelSystem.BuyPart(Body());
                break;
            default:
                bought = travelSystem.BuyPart(Thruster());
                break;
        }

        if (bought)
        {
            RocketBuild.Equip(partType, Icon);
        }
        return bought;
    }

    private SpaceCraftParts.Cabin Cabin()
    {
        if (tier == 1)
        {
            return SpaceCraftParts.t1Cabin;
        }
        if (tier == 2)
        {
            return SpaceCraftParts.t2Cabin;
        }
        return SpaceCraftParts.t3Cabin;
    }

    private SpaceCraftParts.Body Body()
    {
        if (tier == 1)
        {
            return SpaceCraftParts.t1Body;
        }
        if (tier == 2)
        {
            return SpaceCraftParts.t2Body;
        }
        return SpaceCraftParts.t3Body;
    }

    private SpaceCraftParts.Thruster Thruster()
    {
        if (tier == 1)
        {
            return SpaceCraftParts.t1Thruster;
        }
        if (tier == 2)
        {
            return SpaceCraftParts.t2Thruster;
        }
        return SpaceCraftParts.t3Thruster;
    }
}
