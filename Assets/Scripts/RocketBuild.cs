using UnityEngine;

public static class RocketBuild
{
    public static Sprite CabinSprite { get; private set; }
    public static Sprite BodySprite { get; private set; }
    public static Sprite ThrusterSprite { get; private set; }

    public static bool HasAnyPart => CabinSprite != null || BodySprite != null || ThrusterSprite != null;

    public static void Equip(PartButton.PartType partType, Sprite sprite)
    {
        switch (partType)
        {
            case PartButton.PartType.Cabin:
                CabinSprite = sprite;
                break;
            case PartButton.PartType.Body:
                BodySprite = sprite;
                break;
            default:
                ThrusterSprite = sprite;
                break;
        }
    }

    public static void Clear()
    {
        CabinSprite = null;
        BodySprite = null;
        ThrusterSprite = null;
    }
}
