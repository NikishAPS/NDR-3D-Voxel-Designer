using UnityEngine;

public class GridManager : MonoBehaviour
{
    private static GridManager _this;
    public static GridBlock[] Grids { get; private set; }
    private static VoxelBoxCollider gridCollider;

    public static Vector3Int Size
    {
        set
        {
            gridCollider.Bounds = new BoundsInt(Vector3Int.zero, new Vector3Int(value.x, 0, value.x));
            gridCollider.Active = true;

            foreach (GridBlock grid in Grids)
            {
                grid.Size = value;
            }
        }
    }

    public string wall;
    public static bool IsGrid(Vector3Int position)
    {
        _this.wall = "";
        foreach (GridBlock grid in Grids)
        {
            if (grid.Active)
            {
                if (grid.Inside(position))
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
        //int[] indexes = new int[4]
        //{
        //    Direction.Left, Direction.Right, Direction.Back, Direction.Forward
        //};

        //float minAngle = 0f;
        //int indexMin = 0;
        //for(int i = 0; i < indexes.Length; i++)
        //{
        //    float angle = Vector3.Angle(Direction.Directions[indexes[i]], direction);
        //    if (angle > minAngle)
        //    {
        //        minAngle = angle;
        //        indexMin = i;
        //    }
        //}

        //return Grids[indexes[indexMin]];

        return null;
    }

    public void SetWall(Vector3Int firstPoint, Vector3Int secondPoint)
    {

    }

    private void Start()
    {
        _this = FindObjectOfType<GridManager>();

        gridCollider = new VoxelBoxCollider();

        Grids = FindObjectsOfType<GridBlock>();

        //for (int i = 0; i < Grids.Length; i++)
        //{
        //    for (int j = i + 1; j < Grids.Length; j++)
        //    {
        //        //if (Direction.Directions[i] == -Grids[j].transform.forward)
        //        if (Direction.Directions[i] == -Grids[j].Normal)
        //        {
        //            Grid grid = Grids[i];
        //            Grids[i] = Grids[j];
        //            Grids[j] = grid;
        //            //_grids[i].Active = false;
        //        }
        //    }
        //    Grids[i].Active = false;
        //}


    }


}

