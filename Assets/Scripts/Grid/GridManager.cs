using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridBlock[] Grids { get; private set; }

    private static GridManager _this;
    private static VoxelBoxCollider gridCollider;

    public static Vector3Int Size
    {
        set
        {
            gridCollider.Bounds = new BoundsInt(new Vector3Int(0, -1, 0), new Vector3Int(value.x, -1, value.x));
            gridCollider.Active = true;

            foreach (GridBlock grid in Grids)
            {
                grid.Size = value;
            }
        }
    }

    public void SetWall(Vector3Int firstPoint, Vector3Int secondPoint)
    {

    }

    private void Start()
    {
        _this = FindObjectOfType<GridManager>();

        gridCollider = new VoxelBoxCollider();

        Grids = FindObjectsOfType<GridBlock>();
        
    }


}

