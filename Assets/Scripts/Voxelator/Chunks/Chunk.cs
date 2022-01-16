using UnityEngine;
using UnityEngine.Rendering;

public abstract class Chunk<U> where U : Unit
{
    public bool Active { get => _gameObject.activeSelf; set => _gameObject.SetActive(value); }
    public readonly Vector3Int Position;
    public readonly Vector3Int Size;
    public readonly U[] Units;
    public int UnitsCount { get; private set; }

    protected readonly GameObject _gameObject;
    protected readonly Mesh _mesh;

    public Chunk(Vector3Int position, Vector3Int size, Material material, string name)
    {
        Position = position;
        Size = size;
        Units = new U[Size.x * Size.y * Size.z];

        _gameObject = new GameObject(name);
        _mesh = new Mesh();
        _mesh.indexFormat = IndexFormat.UInt32;

        _gameObject.AddComponent<MeshFilter>().mesh = _mesh;
        //MeshRenderer meshRenderer = _gameObject.AddComponent<MeshRenderer>();
        //meshRenderer.material = material;
        //meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _gameObject.AddMeshRenderer(material, false);
    }

    public void SetUnit(U unit)
    {
        int index = GetUnitIndex(unit.Position);
        if (index == -1) return;
        if (Units[index] == null) UnitsCount++;

        Units[index] = unit;
    }

    public void DeleteUnit(Vector3Int position)
    {
        int index = GetUnitIndex(position);
        if (index == -1 || Units[index] == null) return;

        OnBeforeDeleteUnit(position);

        Units[index].Release();

        UnitsCount--;
        Units[index] = null;
    }

    public U GetUnit(Vector3Int position)
    {
        int index = GetUnitIndex(position);
        return index != -1 ? Units[index] : null;
    }

    public void Release()
    {
        Object.Destroy(_gameObject);
        OnRelease();
    }

    public abstract void UpdateMesh();

    protected int GetUnitIndex(Vector3Int globalPosition) => VoxelatorArray.GetIndex(Size, GetLocalPosition(globalPosition));
    protected Vector3Int GetLocalPosition(Vector3Int globalPosition) => globalPosition - Position;
    //protected abstract void OnCreateUnit(int index, Vector3Int position);
    protected virtual void OnBeforeDeleteUnit(Vector3Int position) { }
    protected virtual void OnRelease() { }
}
