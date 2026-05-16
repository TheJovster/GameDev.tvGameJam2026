using UnityEngine;

public enum PlugColor
{
    Red,
    Blue,
    Yellow,
    Green,
    White
}

public static class PlugColorHelper
{
    public static Color ToColor(PlugColor plugColor)
    {
        switch (plugColor)
        {
            case PlugColor.Red: return new Color(0.9f, 0.15f, 0.15f);
            case PlugColor.Blue: return new Color(0.15f, 0.35f, 0.9f);
            case PlugColor.Yellow: return new Color(0.95f, 0.85f, 0.1f);
            case PlugColor.Green: return new Color(0.15f, 0.8f, 0.25f);
            case PlugColor.White: return Color.white;
            default: return Color.magenta;
        }
    }

    public static void ApplyColor(PlugColor plugColor, Renderer renderer)
    {
        if (renderer == null) return;

        if (Application.isPlaying)
            renderer.material.color = ToColor(plugColor);
        else
            renderer.sharedMaterial.color = ToColor(plugColor);
    }
}