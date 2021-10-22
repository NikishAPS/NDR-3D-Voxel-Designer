using UnityEngine;

public class GridManager : MonoBehaviour
{
    private static GridManager _this;
    public static Grid[] Grids { get; private set; }

    public static Vector3Int Size
    {
        set
        {
            _this.transform.localScale = value.ToVector3();
            foreach (Grid grid in Grids)
            {
                grid.SetOffset(0);
                grid.UpdateTiling();
            }
        }
    }

    public string wall;
    public static bool IsGrid(Vector3Int position)
    {
        _this.wall = "";
        foreach (Grid grid in Grids)
        {
            if(grid.Active)
            {
                if(grid.IsGrid(position))
                {
                    _this.wall = grid.transform.name;
                    return true;
                }
            }
        }

        return false;
    }

    public static Grid GetWallGridByDirection(Vector3 direction)
    {
        int[] indexes = new int[4]
        {
            Direction.Left, Direction.Right, Direction.Back, Direction.Forward
        };

        float minAngle = 0f;
        int indexMin = 0;
        for(int i = 0; i < indexes.Length; i++)
        {
            float angle = Vector3.Angle(Direction.Directions[indexes[i]], direction);
            if (angle > minAngle)
            {
                minAngle = angle;
                indexMin = i;
            }
        }

        return Grids[indexes[indexMin]];
    }

    private void Start()
    {
        _this = FindObjectOfType<GridManager>();

        Grids = FindObjectsOfType<Grid>();

        for (int i = 0; i < Grids.Length; i++)
        {
            for (int j = i + 1; j < Grids.Length; j++)
            {
                //if (Direction.Directions[i] == -Grids[j].transform.forward)
                if (Direction.Directions[i] == -Grids[j].Normal)
                {
                    Grid grid = Grids[i];
                    Grids[i] = Grids[j];
                    Grids[j] = grid;
                    //_grids[i].Active = false;
                }
            }
            Grids[i].Active = false;
        }
    }


}

