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

