using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the tools available to use and their properties.
/// </summary>
public class Tool
{
    private string _name;
    public string name { get => _name; private set => _name = value.ToLower(); }
    /// <summary>When the mouse position jumps between frames: true - the tool should be applied to each coord the mouse moved through; false - just applied to the ending coord.</summary>
    public bool useMovementInterpolation { get; private set; }
    /// <summary>Whether the outline of the brush shape should be shown.</summary>
    public bool showBrushBorder { get; private set; }
    /// <summary>What action causes a use of the tool to be ended.</summary>
    public MouseTargetDeselectMode finishMode { get; private set; } = MouseTargetDeselectMode.Unclick;
    public bool canBeCancelled = false;

    private Tool() { }

    public static Tool None = new Tool
    {
        name = "none",
        useMovementInterpolation = false,
        showBrushBorder = true
    };

    public static Tool Pencil = new Tool
    {
        name = "pencil",
        useMovementInterpolation = true,
        showBrushBorder = true
    };

    public static Tool Brush = new Tool
    {
        name = "brush",
        useMovementInterpolation = true,
        showBrushBorder = true
    };

    public static Tool Rubber = new Tool
    {
        name = "rubber",
        useMovementInterpolation = true,
        showBrushBorder = true
    };

    public static Tool EyeDropper = new Tool
    {
        name = "eye dropper",
        useMovementInterpolation = false,
        showBrushBorder = true
    };

    public static Tool GlobalEyeDropper = new Tool
    {
        name = "global eye dropper",
        useMovementInterpolation = false,
        showBrushBorder = false
    };

    public static Tool Fill = new Tool
    {
        name = "fill",
        useMovementInterpolation = false,
        showBrushBorder = true
    };

    public static Tool Shape = new Tool
    {
        name = "shape",
        useMovementInterpolation = false,
        showBrushBorder = true,
        canBeCancelled = true
    };

    public static Tool Line = new Tool
    {
        name = "line",
        useMovementInterpolation = false,
        showBrushBorder = true,
        canBeCancelled = true
    };

    public static Tool IsoBox = new Tool
    {
        name = "iso box",
        useMovementInterpolation = false,
        showBrushBorder = true,
        finishMode = MouseTargetDeselectMode.Manual,
        canBeCancelled = true
    };

    public static Tool Gradient = new Tool
    {
        name = "gradient",
        useMovementInterpolation = false,
        showBrushBorder = true,
        canBeCancelled = true
    };

    public static Tool Move = new Tool
    {
        name = "move",
        useMovementInterpolation = false,
        showBrushBorder = false
    };

    public static Tool Selection = new Tool
    {
        name = "selection",
        useMovementInterpolation = false,
        showBrushBorder = true
    };

    /// <summary>All implemented tools.</summary>
    public static readonly Tool[] tools = new Tool[] { None, Pencil, Brush, Rubber, EyeDropper, GlobalEyeDropper, Fill, Shape, Line, IsoBox, Gradient, Move, Selection };

    /// <summary>
    /// Gets the tool with that name.
    /// </summary>
    public static Tool StringToTool(string toolName)
    {
        foreach (Tool tool in tools)
        {
            if (tool.name == toolName)
            {
                return tool;
            }
        }
        throw new System.Exception("Unknown / unimplemented tool: " + toolName);
    }
}
