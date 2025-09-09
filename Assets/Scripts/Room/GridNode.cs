using UnityEngine;

public class GridNode
{
    public GameGrid grid;
    public Vector2Int position;
    public bool walkable = true;
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;
    public GridNode parent;

    public GridNode(GameGrid grid, Vector2Int pos)
    {
        this.grid = grid;
        this.position = pos;
    }
}
