using System.Collections.Generic;
using UnityEngine;

public class Resources : MonoBehaviour
{
    public static List<string> tools;
    public static float storage;
    public static float money;
    public static float time;
    public static float water;
    public static float food;
    public static float oxygen;
    public static float fuel;
    public static float mass;
    public static float research;
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