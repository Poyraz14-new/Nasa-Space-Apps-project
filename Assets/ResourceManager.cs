using UnityEngine;

public class Resources : MonoBehaviour
{
    private static float storage;
    private static float money;
    private static float time;
    private static float water;
    private static float food;
    private static float oxygen;
    private static float fuel;
    private static float mass;
    private static float research;
    public enum ResourceType { Money, Time, Water, Food, Oxygen, Fuel, Mass, Research, Storage }

    public static float Get(ResourceType resource) => resource switch {
    ResourceType.Money => money,
    ResourceType.Time => time,
    ResourceType.Water => water,
    ResourceType.Food => food,
    ResourceType.Oxygen => oxygen,
    ResourceType.Fuel => fuel,
    ResourceType.Mass => mass,
    ResourceType.Storage => storage,
    ResourceType.Research => research,
    _ => 0f
};
}
