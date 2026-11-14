using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;

public class SpaceCraftParts : MonoBehaviour
{

    public static Cabin t1Cabin;
    public static Body t1Body;
    public static Thruster t1Thruster;
//========================================
    public static Cabin t2Cabin;
    public static Body t2Body;
    public static Thruster t2Thruster;
//========================================
    public static Cabin t3Cabin;
    public static Body t3Body;
    public static Thruster t3Thruster;

    void Start()
    {
        t1Cabin=new Cabin(700f,0.2f,400f,25f);
        t1Body=new Body(1000f,30f,550f,75f);
        t1Thruster=new Thruster(850f,60f,700f);
//========================================
        t2Cabin=new Cabin();
        t2Body=new Body();
        t2Thruster=new Thruster();
//========================================
        t3Cabin=new Cabin();
        t3Body=new Body();
        t3Thruster=new Thruster();
    }
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
        public Thruster(float mass, float fuel, float money)
        {
            this.fuel = fuel;
            this.mass = mass;
            this.money = money;
        }
    }
}