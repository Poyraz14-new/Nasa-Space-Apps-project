using System.Data.SqlTypes;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SpaceCraftParts : MonoBehaviour
{
    public struct Cabin
    {
        private float mass;
        private float time;
        private float money;
        private float storage;
        public Cabin(float mass,float time,float money, float storage)
        {
            this.mass=mass;
            this.time=time;
            this.money=money;
            this.storage=storage;
        }
    }
    public struct Body
    {
        private float mass;
        private float fuel;
        private float money;
        private float storage;
        public Body(float mass,float fuel,float money,float storage)
        {
            this.mass=mass;
            this.fuel=fuel;
            this.money=money;
            this.storage=storage;
        }
    }
    public struct Thruster
    {
        private float fuel;
        private float mass;
        private float money;
        public Thruster(float fuel,float mass, float money)
        {
            this.fuel=fuel;
            this.mass=mass;
            this.money=money;
        }
    }



}

    

