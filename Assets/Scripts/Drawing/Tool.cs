using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool
{
    private string _name;
    public string name { get => _name; private set => _name = value.ToLower(); }
    public bool useMovementInterpolation { get; private set; }
    public bool showBrushBorder { get; private set; }

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
        showBrushBorder = true
    };

    public static Tool Line = new Tool
    {
        name = "line",
        useMovementInterpolation = false,
        showBrushBorder = true
    };

    public static Tool Gradient = new Tool
    {
        name = "gradient",
        useMovementInterpolation = false,
        showBrushBorder = true
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

    public static Tool ToolNameToTool(string toolName)
    {
        Tool[] tools = new Tool[] { None, Pencil, Brush, Rubber, EyeDropper, GlobalEyeDropper, Fill, Shape, Line, Gradient, Move, Selection };
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
