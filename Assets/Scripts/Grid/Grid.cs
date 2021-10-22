using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool Active
    {
        get => gameObject.activeSelf;
        set => gameObject.SetActive(value);
    }

    public Vector3Int Position
    {
        get => transform.localPosition.ToVector3Int();
        set => transform.localPosition = value;
    }

    public Vector3Int Size { get; private set; }

    public Vector3Int Area { get; private set; }

    public Vector3 Normal { get; private set; }

    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private DirectionType _normalType;
    private float _yOffset = -0.51f;
    private Vector3 _offset;

    public bool IsGrid(Vector3Int position)
    {
        return VoxelatorManager.WithinTheArray(Size, position - _offset);

        return
            position.x >= Position.x && position.x < Position.x + Size.x &&
            position.y >= Position.y && position.y < Position.y + Size.y &&
            position.z >= Position.z && position.z < Position.z + Size.z;
    }

    public void SetOffset(int offset)
    {
        transform.localPosition = (Normal * offset).Div(transform.lossyScale);

        _offset = (Normal * offset) + _meshRenderer.transform.localPosition.Mul(_meshRenderer.transform.lossyScale) +
            (Normal - Vector3.one.Mul(Normal.Abs()));
    }

    public void UpdateTiling()
    {
        _meshRenderer.material.SetTextureScale("_MainTex", new Vector3(_meshRenderer.transform.lossyScale.x,
            _meshRenderer.transform.lossyScale.y));

        //_meshRenderer.transform.localPosition =
        //    new Vector3((size.x * 0.5f - 0.5f) / size.x, (size.y * 0.5f - 0.5f) / size.y, -0.5f / size.z);

        Size = (transform.lossyScale.Mul(Vector3.one - Normal.Abs()) + Normal.Abs()).RoundToInt();
    }

    private void Awake()
    {
        if (_meshRenderer == null)
            _meshRenderer = GetComponent<MeshRenderer>();

        Normal = Direction.Directions[(int)_normalType];
    }

    private void Start()
    {
        return;

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CustomMesh planeMesh = MeshGenerator.GenerateHorizontalPlane(Vector3.one * 0.5f);
        mesh.SetCustomMesh(planeMesh);

        mesh.RecalculateNormals();
        mesh.Optimize();

    }

}


