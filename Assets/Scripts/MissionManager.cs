using System.Collections.Generic;
using UnityEngine;

public static class MissionManager
{
    public const string GeologicalHammer = "Geological Hammer";
    public const string LunarSamplingScoop = "Lunar Sampling Scoop";
    public const string SamplePouch = "Sample Pouch";
    public const string PortableSpectrometer = "Portable Spectrometer";
    public const string EvaRepairKit = "EVA Repair Kit";

    private static readonly Color OrangeSoilTint = new Color(0.95f, 0.55f, 0.25f);
    private static readonly Color IceTint = new Color(0.65f, 0.85f, 1f);
    private static readonly Color ShadowTint = new Color(0.55f, 0.65f, 0.85f);

    public static readonly Mission[] Missions =
    {
        new Mission(
            "First Footprints",
            "Apollo 11",
            "Set up your oxygen zone, crack open a rock with the hammer, analyse it with the spectrometer and seal it in a sample pouch.",
            new[] { GeologicalHammer, PortableSpectrometer, SamplePouch },
            "Big_rocks_1",
            Color.white
        ),

        new Mission(
            "Dust Beneath Our Feet",
            "Apollo 11 / Surveyor soil investigations",
            "Collect regolith from three spots inside your oxygen zone, analyse each sample and report how they differ.",
            new[] { LunarSamplingScoop, PortableSpectrometer, SamplePouch },
            "Small_Crater_hit_1",
            Color.white
        ),

        new Mission(
            "Find the Rim",
            "Apollo 14",
            "Reach the rim of the crater Mission Control marked, scoop one sample from the rim and one from normal ground, and compare them.",
            new[] { LunarSamplingScoop, SamplePouch },
            "Big_Crater_hit_1",
            Color.white
        ),

        new Mission(
            "Lunar Geology",
            "Apollo 15",
            "Scan the boulder field with the spectrometer, find the rock that is about 4 billion years old, chip off a piece and bag it.",
            new[] { GeologicalHammer, PortableSpectrometer, SamplePouch },
            "Spike_rock_1",
            Color.white
        ),

        new Mission(
            "Wrong Hypothesis",
            "Apollo 16",
            "Scientists think this region is volcanic. Analyse its rocks, show that impacts formed them instead and bag the evidence.",
            new[] { GeologicalHammer, PortableSpectrometer, SamplePouch },
            "Big_asteroid_impact_1",
            Color.white
        ),

        new Mission(
            "Orange Soil",
            "Apollo 17",
            "Find the strange orange soil, scoop a sample, analyse part of it to learn what it is made of and bag the rest.",
            new[] { LunarSamplingScoop, PortableSpectrometer, SamplePouch, EvaRepairKit },
            "Small_Crater_hit_2",
            OrangeSoilTint
        ),

        new Mission(
            "The Hidden Ice",
            "Lunar polar exploration",
            "Enter a permanently shadowed crater, scoop regolith where ice may be hiding and bring it back unanalysed for labs on Earth.",
            new[] { LunarSamplingScoop, SamplePouch },
            "Big_Crater_hit_1",
            IceTint
        ),

        new Mission(
            "South Pole Expedition",
            "Artemis lunar exploration",
            "Explore a permanently shadowed region while watching temperature and radiation, then compare regolith that never sees sunlight with sunlit regolith.",
            new[] { LunarSamplingScoop, PortableSpectrometer, SamplePouch, EvaRepairKit },
            "Small_Crater_hit_1",
            ShadowTint
        ),

        new Mission(
            "The Ancient Impact",
            "Lunar impact research",
            "Examine the exposed rock layers of an ancient impact crater, analyse them and bag material from the impact site.",
            new[] { GeologicalHammer, PortableSpectrometer, SamplePouch },
            "Big_asteroid_impact_2",
            Color.white
        ),
    };

    public static Mission CurrentMission { get; private set; }

    public static Mission PickRandomMission()
    {
        CurrentMission = Missions[Random.Range(0, Missions.Length)];
        return CurrentMission;
    }

    public static void ClearCurrentMission()
    {
        CurrentMission = null;
    }
}

public class Mission
{
    public string Title { get; }
    public string Inspiration { get; }
    public string Objective { get; }
    public string[] RequiredTools { get; }
    public string SitePropName { get; }
    public Color SitePropTint { get; }

    private readonly ResourceDemand[] requirements;

    public Mission(string title, string inspiration, string objective, string[] requiredTools, string sitePropName, Color sitePropTint, ResourceDemand[] requirements = null)
    {
        Title = title;
        Inspiration = inspiration;
        Objective = objective;
        RequiredTools = requiredTools;
        SitePropName = sitePropName;
        SitePropTint = sitePropTint;
        this.requirements = requirements ?? new ResourceDemand[0];
    }

    public List<string> MissingTools()
    {
        List<string> missingTools = new List<string>();
        foreach (string tool in RequiredTools)
        {
            if (!Resources.tools.Contains(tool))
            {
                missingTools.Add(tool);
            }
        }
        return missingTools;
    }

    public bool IsMet()
    {
        if (MissingTools().Count > 0)
        {
            return false;
        }

        foreach (ResourceDemand demand in requirements)
        {
            if (Resources.Get(demand.resource) < demand.amount)
            {
                return false;
            }
        }
        return true;
    }
}

public struct ResourceDemand
{
    public Resources.ResourceType resource;
    public float amount;

    public ResourceDemand(Resources.ResourceType resource, float amount)
    {
        this.resource = resource;
        this.amount = amount;
    }
}
