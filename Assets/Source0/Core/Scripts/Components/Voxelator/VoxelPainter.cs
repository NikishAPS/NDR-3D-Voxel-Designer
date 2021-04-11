namespace Source.Components.Voxelator
{
    using UnityEngine;

    using GUI;

    [AddComponentMenu("Source/Voxelator/Voxel Painter"), DisallowMultipleComponent]
    public sealed class VoxelPainter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _camera = default;

        [Space(8.0f)]

        [SerializeField] private Transform _selection = default;

        [Space(8.0f)]

        [SerializeField] private VoxelGrid _voxelGrid = default;
        [SerializeField] private Voxel _voxel = default;

        private Voxel _selectedVoxel;
        private float _voxelSizeUnit;

        private void Start()
        {
            _voxelSizeUnit = 1.0f / _voxelGrid.voxelGridUnit;

            var scale = Vector3.one * _voxelSizeUnit;
            _voxel.transform.localScale = scale;
            _selection.localScale = scale;

            _voxelSizeUnit *= 0.5f;
        }

        private void Update()
        {
            if (GUIBlock.isBlocked)
                return;

            var mousePosition = Input.mousePosition;

            var ray = _camera.ScreenPointToRay(mousePosition);
            var raycast = Physics.Raycast(ray, out RaycastHit hit) && !(hit.point.y < Mathf.Epsilon && hit.normal.y < 0.0f);

            var gridGoord = Vector3.zero;
            if (raycast)
            {
                if (hit.transform == _voxelGrid.transform)
                {
                    gridGoord = Math.GetGridCoord3D(hit.point, _voxelGrid.voxelGridUnit);
                    gridGoord.x += _voxelSizeUnit;
                    gridGoord.z += _voxelSizeUnit;
                    gridGoord.y = 0.0f;

                    if (_selectedVoxel == null)
                        _selection.position = gridGoord;
                    else
                        _selectedVoxel.transform.position = gridGoord + hit.normal * _voxelSizeUnit;
                }
                else
                {
                    gridGoord = hit.transform.position + hit.normal * _voxelSizeUnit;

                    if (_selectedVoxel == null)
                        _selection.position = gridGoord;
                    else
                        _selectedVoxel.transform.position = gridGoord + hit.normal * _voxelSizeUnit;
                }
                _selection.forward = -hit.normal;
            }

            var selectionGameObject = _selection.gameObject;
            var selectionVisiblity = raycast && !_selectedVoxel;
            if (selectionGameObject.activeSelf != selectionVisiblity)
                selectionGameObject.SetActive(selectionVisiblity);

            if (raycast)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var position = gridGoord + hit.normal * _voxelSizeUnit;
                    if (position.y < (float) _voxelGrid.height / _voxelGrid.voxelsPerUnit || _voxelGrid.height <= 0)
                    {
                        if (selectionVisiblity)
                            Instantiate(_voxel, position, Quaternion.identity);
                        else
                        {
                            if (_selectedVoxel != null)
                            {
                                _selectedVoxel.isTrigger = true;
                                _selectedVoxel.isSelected = false;
                                _selectedVoxel = null;
                            }
                        }
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (_selectedVoxel == null)
                    {
                        var voxel = hit.transform.GetComponent<Voxel>();
                        if (voxel != null)
                        {
                            _selectedVoxel = voxel;
                            _selectedVoxel.isTrigger = false;
                            _selectedVoxel.isSelected = true;
                        }
                    }
                }
            }
        }
    }
}