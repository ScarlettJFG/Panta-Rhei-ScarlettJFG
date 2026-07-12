using System.Linq;
using System.Numerics;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;

namespace Content.Shared.Humanoid;

[DataDefinition]
[Serializable, NetSerializable]
public sealed partial class HumanoidCharacterAppearance : ICharacterAppearance, IEquatable<HumanoidCharacterAppearance>
{
    [DataField("hair")]
    public string HairStyleId { get; set; } = HairStyles.DefaultHairStyle;

    [DataField]
    public Color HairColor { get; set; } = Color.Black;
    [DataField]
    public bool HairGlowing { get; set; } = false; //Starlight

    [DataField("facialHair")]
    public string FacialHairStyleId { get; set; } = HairStyles.DefaultFacialHairStyle;

    [DataField]
    public Color FacialHairColor { get; set; } = Color.Black;
    [DataField]
    public bool FacialHairGlowing { get; set; } = false; //Starlight

    [DataField]
    public Color EyeColor { get; set; } = Color.Black;
    [DataField]
    public bool EyeGlowing { get; set; } = false; //Starlight

    [DataField]
    public Color SkinColor { get; set; } = Color.FromHsv(new Vector4(0.07f, 0.2f, 1f, 1f));

    [DataField]
    public List<Marking> Markings { get; set; } = new();

    public HumanoidCharacterAppearance(string hairStyleId,
        Color hairColor,
        bool hairGlowing, //Starlight
        string facialHairStyleId,
        Color facialHairColor,
        bool facialHairGlowing, //Starlight
        Color eyeColor,
        bool eyeGlowing, //Starlight
        Color skinColor,
        List<Marking> markings)
    {
        HairStyleId = hairStyleId;
        HairColor = ClampColor(hairColor);
        FacialHairStyleId = facialHairStyleId;
        FacialHairColor = ClampColor(facialHairColor);
        EyeColor = ClampColor(eyeColor);
        SkinColor = ClampColor(skinColor);
        Markings = markings;
        HairGlowing = hairGlowing; //Starlight
        FacialHairGlowing = facialHairGlowing; //Starlight
        EyeGlowing = eyeGlowing; //Starlight
    }

    // Starlight, function changed to support glowing
    public HumanoidCharacterAppearance(HumanoidCharacterAppearance other) :
        this(other.HairStyleId, other.HairColor, other.HairGlowing, other.FacialHairStyleId, other.FacialHairColor, other.FacialHairGlowing, other.EyeColor, other.EyeGlowing, other.SkinColor, new(other.Markings))
    {

    }

    // Starlight, Function changed to support glowing
    public HumanoidCharacterAppearance WithHairStyleName(string newName)
    {
        return new(newName, HairColor, HairGlowing, FacialHairStyleId, FacialHairColor, FacialHairGlowing, EyeColor, EyeGlowing, SkinColor, Markings);
    }

    // Starlight, Function changed to support glowing
    public HumanoidCharacterAppearance WithHairColor(Color newColor)
    {
        return new(HairStyleId, newColor, HairGlowing, FacialHairStyleId, FacialHairColor, FacialHairGlowing, EyeColor, EyeGlowing, SkinColor, Markings);
    }

    // Starlight Start
    public HumanoidCharacterAppearance WithHairGlowing(bool newGlowing)
    {
        return new(HairStyleId, HairColor, newGlowing, FacialHairStyleId, FacialHairColor, FacialHairGlowing, EyeColor, EyeGlowing, SkinColor, Markings);
    }
    // Starlight End

    // Starlight, Function changed to support glowing
    public HumanoidCharacterAppearance WithFacialHairStyleName(string newName)
    {
        return new(HairStyleId, HairColor, HairGlowing, newName, FacialHairColor, FacialHairGlowing, EyeColor, EyeGlowing, SkinColor, Markings);
    }

    //Starlight Start
    public HumanoidCharacterAppearance WithFacialHairGlowing(bool newGlowing)
    {
        return new(HairStyleId, HairColor, HairGlowing, FacialHairStyleId, FacialHairColor, newGlowing, EyeColor, EyeGlowing, SkinColor, Markings);
    }
    //Starlight End

    // Starlight, Function changed to support glowing
    public HumanoidCharacterAppearance WithFacialHairColor(Color newColor)
    {
        return new(HairStyleId, HairColor, HairGlowing, FacialHairStyleId, newColor, FacialHairGlowing, EyeColor, EyeGlowing, SkinColor, Markings);
    }

    // Starlight, Function changed to support glowing
    public HumanoidCharacterAppearance WithEyeColor(Color newColor)
    {
        return new(HairStyleId, HairColor, HairGlowing, FacialHairStyleId, FacialHairColor, FacialHairGlowing, newColor, EyeGlowing, SkinColor, Markings);
    }

    // Starlight Start
    public HumanoidCharacterAppearance WithEyeGlowing(bool newGlowing)
    {
        return new(HairStyleId, HairColor, HairGlowing, FacialHairStyleId, FacialHairColor, FacialHairGlowing, EyeColor, newGlowing, SkinColor, Markings);
    }
    // Starlight End

    // Starlight, Function changed to support glowing
    public HumanoidCharacterAppearance WithSkinColor(Color newColor)
    {
        return new(HairStyleId, HairColor, HairGlowing, FacialHairStyleId, FacialHairColor, FacialHairGlowing, EyeColor, EyeGlowing, newColor, Markings);
    }

    // Starlight, Function changed to support glowing
    public HumanoidCharacterAppearance WithMarkings(List<Marking> newMarkings)
    {
        return new(HairStyleId, HairColor, HairGlowing, FacialHairStyleId, FacialHairColor, FacialHairGlowing, EyeColor, EyeGlowing, SkinColor, newMarkings);
    }

    public static HumanoidCharacterAppearance DefaultWithSpecies(string species)
    {
        var protoMan = IoCManager.Resolve<IPrototypeManager>();
        var speciesPrototype = protoMan.Index<SpeciesPrototype>(species);
        var skinColoration = protoMan.Index(speciesPrototype.SkinColoration).Strategy;
        var skinColor = skinColoration.InputType switch
        {
            SkinColorationStrategyInput.Unary => skinColoration.FromUnary(speciesPrototype.DefaultHumanSkinTone),
            SkinColorationStrategyInput.Color => skinColoration.ClosestSkinColor(speciesPrototype.DefaultSkinTone),
            _ => skinColoration.ClosestSkinColor(speciesPrototype.DefaultSkinTone),
        };

        return new(
            HairStyles.DefaultHairStyle,
            Color.Black,
            false, //Starlight
            HairStyles.DefaultFacialHairStyle,
            Color.Black,
            false, //Starlight
            Color.Black,
            false, //Starlight
            skinColor,
            new()
        );
    }

    private static IReadOnlyList<Color> _realisticEyeColors = new List<Color>
    {
        Color.Brown,
        Color.Gray,
        Color.Azure,
        Color.SteelBlue,
        Color.Black
    };

    public static HumanoidCharacterAppearance Random(string species, Sex sex)
    {
        var random = IoCManager.Resolve<IRobustRandom>();
        var markingManager = IoCManager.Resolve<MarkingManager>();
        var hairStyles = markingManager.MarkingsByCategoryAndSpecies(MarkingCategories.Hair, species).Keys.ToList();
        var facialHairStyles = markingManager.MarkingsByCategoryAndSpecies(MarkingCategories.FacialHair, species).Keys.ToList();

        var newHairStyle = hairStyles.Count > 0
            ? random.Pick(hairStyles)
            : HairStyles.DefaultHairStyle.Id;

        var newFacialHairStyle = facialHairStyles.Count == 0 || sex == Sex.Female
            ? HairStyles.DefaultFacialHairStyle.Id
            : random.Pick(facialHairStyles);

        var newHairColor = random.Pick(HairStyles.RealisticHairColors);
        newHairColor = newHairColor
            .WithRed(RandomizeColor(newHairColor.R))
            .WithGreen(RandomizeColor(newHairColor.G))
            .WithBlue(RandomizeColor(newHairColor.B));

        // TODO: Add random markings

        var newEyeColor = random.Pick(_realisticEyeColors);

        var protoMan = IoCManager.Resolve<IPrototypeManager>();
        var skinType = protoMan.Index<SpeciesPrototype>(species).SkinColoration;
        var strategy = protoMan.Index(skinType).Strategy;

        var newSkinColor = strategy.InputType switch
        {
            SkinColorationStrategyInput.Unary => strategy.FromUnary(random.NextFloat(0f, 100f)),
            SkinColorationStrategyInput.Color => strategy.ClosestSkinColor(new Color(random.NextFloat(1), random.NextFloat(1), random.NextFloat(1), 1)),
            _ => strategy.ClosestSkinColor(new Color(random.NextFloat(1), random.NextFloat(1), random.NextFloat(1), 1)),
        };

        return new HumanoidCharacterAppearance(newHairStyle, newHairColor, false, newFacialHairStyle, newHairColor, false, newEyeColor, false, newSkinColor, new ()); //Starlight, glowing

        float RandomizeColor(float channel)
        {
            return MathHelper.Clamp01(channel + random.Next(-25, 25) / 100f);
        }
    }

    public static Color ClampColor(Color color)
    {
        return new(color.RByte, color.GByte, color.BByte);
    }

    public static HumanoidCharacterAppearance EnsureValid(HumanoidCharacterAppearance appearance, string species, Sex sex)
    {
        var hairStyleId = appearance.HairStyleId;
        var facialHairStyleId = appearance.FacialHairStyleId;

        var hairColor = ClampColor(appearance.HairColor);
        var facialHairColor = ClampColor(appearance.FacialHairColor);
        var eyeColor = ClampColor(appearance.EyeColor);

        var proto = IoCManager.Resolve<IPrototypeManager>();
        var markingManager = IoCManager.Resolve<MarkingManager>();

        if (!markingManager.MarkingsByCategory(MarkingCategories.Hair).ContainsKey(hairStyleId))
        {
            hairStyleId = HairStyles.DefaultHairStyle;
        }

        if (!markingManager.MarkingsByCategory(MarkingCategories.FacialHair).ContainsKey(facialHairStyleId))
        {
            facialHairStyleId = HairStyles.DefaultFacialHairStyle;
        }

        var markingSet = new MarkingSet();
        var skinColor = appearance.SkinColor;
        if (proto.TryIndex(species, out SpeciesPrototype? speciesProto))
        {
            markingSet = new MarkingSet(appearance.Markings, speciesProto.MarkingPoints, markingManager, proto);
            markingSet.EnsureValid(markingManager);

            var strategy = proto.Index(speciesProto.SkinColoration).Strategy;
            skinColor = strategy.EnsureVerified(skinColor);

            markingSet.EnsureSpecies(species, skinColor, markingManager);
            markingSet.EnsureSexes(sex, markingManager);
        }

        return new HumanoidCharacterAppearance(
            hairStyleId,
            hairColor,
            appearance.HairGlowing, //Starlight
            facialHairStyleId,
            facialHairColor,
            appearance.FacialHairGlowing, //Starlight
            eyeColor,
            appearance.EyeGlowing, //Starlight
            skinColor,
            markingSet.GetForwardEnumerator().ToList());
    }

    public bool MemberwiseEquals(ICharacterAppearance maybeOther)
    {
        if (maybeOther is not HumanoidCharacterAppearance other) return false;
        if (HairStyleId != other.HairStyleId) return false;
        if (!HairColor.Equals(other.HairColor)) return false;
        if (HairGlowing != other.HairGlowing) return false; //Starlight
        if (FacialHairStyleId != other.FacialHairStyleId) return false;
        if (!FacialHairColor.Equals(other.FacialHairColor)) return false;
        if (FacialHairGlowing != other.FacialHairGlowing) return false; //Starlight
        if (!EyeColor.Equals(other.EyeColor)) return false;
        if (EyeGlowing != other.EyeGlowing) return false; //Starlight
        if (!SkinColor.Equals(other.SkinColor)) return false;
        if (!Markings.SequenceEqual(other.Markings)) return false;
        return true;
    }

    public bool Equals(HumanoidCharacterAppearance? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return HairStyleId == other.HairStyleId &&
               HairColor.Equals(other.HairColor) &&
               HairGlowing == other.HairGlowing && //Starlight
               FacialHairStyleId == other.FacialHairStyleId &&
               FacialHairColor.Equals(other.FacialHairColor) &&
               FacialHairGlowing == other.FacialHairGlowing && //Starlight
               EyeColor.Equals(other.EyeColor) &&
               EyeGlowing == other.EyeGlowing && //Starlight
               SkinColor.Equals(other.SkinColor) &&
               Markings.SequenceEqual(other.Markings);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is HumanoidCharacterAppearance other && Equals(other);
    }

    public override int GetHashCode()
    {
        //starlight, reworked this function as it exceeded the original limit for the built in hashcode generics
        //now it uses the add syntax to add each field to the hashcode
        HashCode hash = new();
        hash.Add(HairStyleId);
        hash.Add(HairColor);
        hash.Add(HairGlowing);
        hash.Add(FacialHairStyleId);
        hash.Add(FacialHairColor);
        hash.Add(FacialHairGlowing);
        hash.Add(EyeColor);
        hash.Add(EyeGlowing);
        hash.Add(SkinColor);
        hash.Add(Markings);
        return hash.ToHashCode();
    }

    public HumanoidCharacterAppearance Clone()
    {
        return new(this);
    }
}
