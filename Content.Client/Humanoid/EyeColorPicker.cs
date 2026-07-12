using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Humanoid;

public sealed class EyeColorPicker : Control
{
    public event Action<Color>? OnEyeColorPicked;
    public Action<bool>? OnGlowingChanged; //Starlight

    private readonly ColorSelectorSliders _colorSelectors;
    //Starlight Start
    private readonly CheckBox _glowCheckBox = new CheckBox()
    {
        Text = Loc.GetString("marking-glowing")
    };
    //Starlight End

    public void SetData(Color color, bool isGlowing) //Starlight edited function signature
    {
        _glowCheckBox.Pressed = isGlowing; //Starlight
        _colorSelectors.Color = color;
    }

    public EyeColorPicker()
    {
        var vBox = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Vertical
        };
        AddChild(vBox);

        vBox.AddChild(_colorSelectors = new ColorSelectorSliders());
        _colorSelectors.SelectorType = ColorSelectorSliders.ColorSelectorType.Hsv; // defaults color selector to HSV

        _colorSelectors.OnColorChanged += ColorValueChanged;

        //Starlight Start
        vBox.AddChild(_glowCheckBox);

        _glowCheckBox.OnToggled += args =>
        {
            OnGlowingChanged?.Invoke(args.Pressed);
        };
        //Starlight End
    }

    private void ColorValueChanged(Color newColor)
    {
        OnEyeColorPicked?.Invoke(newColor);
    }
}
