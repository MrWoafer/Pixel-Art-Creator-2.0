using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileOutlineManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject tileOutlinePrefab;

    private Dictionary<Tile, SpriteRenderer> tileOutlines = new Dictionary<Tile, SpriteRenderer>();

    private DrawingArea drawingArea;

    private void Awake()
    {
        drawingArea = Finder.drawingArea;
    }

    private void ClearTileOutlines()
    {
        foreach (SpriteRenderer tileOutline in tileOutlines.Values)
        {
            Destroy(tileOutline.gameObject);
        }
        tileOutlines = new Dictionary<Tile, SpriteRenderer>();
    }

    public void RefreshTileOutlines()
    {
        ClearTileOutlines();

        foreach (Tile tile in drawingArea.file.tiles)
        {
            GameObject tileOutline = Instantiate(tileOutlinePrefab, transform);
            tileOutline.transform.localScale = new Vector3(tile.file.width / drawingArea.pixelsPerUnit, tile.file.height / drawingArea.pixelsPerUnit, 1f);

            tileOutlines[tile] = tileOutline.GetComponent<SpriteRenderer>();
            HideTileOutline(tile);
        }
    }

    public void HideShowTileOutline(Tile tile, bool show)
    {
        if (!tileOutlines.ContainsKey(tile))
        {
            throw new System.Exception("There is no tile outline associated with this tile.");
        }

        tileOutlines[tile].enabled = show;
        if (show)
        {
            tileOutlines[tile].transform.localPosition = (tile.centre - new Vector2(drawingArea.file.width, drawingArea.file.height) / 2f) / drawingArea.pixelsPerUnit;
        }
    }
    public void HideTileOutline(Tile tile)
    {
        HideShowTileOutline(tile, false);
    }
    public void ShowTileOutline(Tile tile)
    {
        HideShowTileOutline(tile, true);
    }
}
