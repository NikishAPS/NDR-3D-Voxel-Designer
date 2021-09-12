using UnityEngine;

public static class GridManager
{
    public static readonly Grid[] Grids;

    static GridManager()
    {
        Grids = Object.FindObjectsOfType<Grid>();   
        for (int i = 0; i < Grids.Length; i++)
        {
            for(int j = i; j < Grids.Length; j++)
            {
                if(Direction.Directions[i] == -Grids[j].transform.up)
                {
                    Grid grid = Grids[i];
                    Grids[i] = Grids[j];
                    Grids[j] = grid;
                    Grids[i].Active = false;
                }
            }
        }
    }

    public static bool IsGrid(Vector3Int position)
    {
        foreach(Grid grid in Grids)
        {
            if(grid.Active)
            {
                if (grid.IsGrid(position))
                    return true;
            }
        }

        return false;
    }

}

