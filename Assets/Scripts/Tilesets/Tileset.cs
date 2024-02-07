using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tileset
{
    public string tilesetName { get; private set; } = "";

    private List<File> tiles = new List<File>();

    public int Count
    {
        get => tiles.Count;
    }


    public Tileset(string name) : this(name, new List<File>()) { }

    public Tileset(string name, List<File> tiles)
    {
        tilesetName = name;
        this.tiles = tiles;
    }


    public void AddTile(File tile)
    {
        if (!tiles.Contains(tile))
        {
            tiles.Add(tile);
        }
    }
    public bool RemoveTile(File tile)
    {
        return tiles.Remove(tile);
    }
}
