using UnityEngine;

public class SpaceCraftParts : MonoBehaviour
{
    public struct Cabin
    {
        public float mass;
        public float daysSaved;
        public float money;
        public float storage;
        public Cabin(float mass, float daysSaved, float money, float storage)
        {
            this.mass = mass;
            this.daysSaved = daysSaved;
            this.money = money;
            this.storage = storage;
        }
    }
    public struct Body
    {
        public float mass;
        public float fuel;
        public float money;
        public float storage;
        public Body(float mass, float fuel, float money, float storage)
        {
            this.mass = mass;
            this.fuel = fuel;
            this.money = money;
            this.storage = storage;
        }
    }
    public struct Thruster
    {
        public float fuel;
        public float mass;
        public float money;
        public Thruster(float fuel, float mass, float money)
        {
            this.fuel = fuel;
            this.mass = mass;
            this.money = money;
        }
    }
}