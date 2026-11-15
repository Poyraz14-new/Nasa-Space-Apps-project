public static class SpaceCraftParts
{
    public static Cabin t1Cabin = new Cabin(700f, 0.2f, 400f, 25f);
    public static Body t1Body = new Body(1000f, 30f, 550f, 75f);
    public static Thruster t1Thruster = new Thruster(850f, 60f, 700f);

    public static Cabin t2Cabin = new Cabin(600f, 0.4f, 900f, 40f);
    public static Body t2Body = new Body(900f, 50f, 1100f, 110f);
    public static Thruster t2Thruster = new Thruster(800f, 110f, 1300f);

    public static Cabin t3Cabin = new Cabin(500f, 0.6f, 1600f, 60f);
    public static Body t3Body = new Body(800f, 75f, 1900f, 150f);
    public static Thruster t3Thruster = new Thruster(750f, 170f, 2200f);

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
        public float mass;
        public float fuel;
        public float money;

        public Thruster(float mass, float fuel, float money)
        {
            this.mass = mass;
            this.fuel = fuel;
            this.money = money;
        }
    }
}