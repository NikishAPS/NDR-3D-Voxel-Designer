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


    public static Grid GetFaceGrid(Vector3 direction)
    {
        int[] indexes = new int[4]
        {
            Direction.Left, Direction.Right, Direction.Back, Direction.Forward
        };

        float minAngle = 0f;
        int indexMin = 0;
        for(int i = 0; i < indexes.Length; i++)
        {
            float angle = Vector3.Angle(-Direction.Directions[indexes[i]], direction);
            if (angle > minAngle)
            {
                minAngle = angle;
                indexMin = i;
            }
        }

        return Grids[indexes[indexMin]];
    }
}

