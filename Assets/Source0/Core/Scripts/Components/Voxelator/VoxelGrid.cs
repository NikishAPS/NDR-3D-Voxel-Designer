using System.Runtime.CompilerServices;

namespace Source.Components.Voxelator
{
    using UnityEngine;

    [AddComponentMenu("Source/Voxelator/Voxel Grid"), RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Collider)), DisallowMultipleComponent]
    public sealed class VoxelGrid : MonoBehaviour
    {
        private static readonly int SHADER_GRID_SIZE_ID = Shader.PropertyToID("_Size");

        [SerializeField, HideInInspector] private Transform _transform = default;
        [SerializeField, HideInInspector] private MeshFilter _meshFilter = default;
        [SerializeField, HideInInspector] private MeshRenderer _meshRenderer = default;
        [SerializeField, HideInInspector] private Collider _collider = default;

        [Header("Grid Settings")]
        [SerializeField] private int _width = default;
        [SerializeField] private int _depth = default;
        [SerializeField] private int _height = default;

        [Space(8.0f)]

        [SerializeField] private float _voxelGridUnit = default;
        [SerializeField] private int _voxelsPerUnit = default;

        public int width
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _width;
            set
            {
                _width = value;
                if (_width <= 0)
                    _width = 1;
                UpdateLocalScale();
            }
        }

        public int height
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _height;
            set
            {
                _height = value;
                if (_height < 0)
                    _height = 0;
            }
        }

        public int depth
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _depth;
            set
            {
                _depth = value;
                if (_depth <= 0)
                    _depth = 1;
                UpdateLocalScale();
            }
        }

        public float voxelGridUnit
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (float) _voxelGridUnit * _voxelsPerUnit;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _voxelGridUnit = value;
        }

        public int voxelsPerUnit
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _voxelsPerUnit;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _voxelsPerUnit = value;
        }

        private void UpdateLocalScale()
        {
            transform.localScale = new Vector3
            {
                x = _width,
                y = _depth,
                z = 1.0f,
            };
        }

#if UNITY_EDITOR
        private void Reset()
        {
            _transform = transform;
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _collider = GetComponent<Collider>();
        }

        private void OnValidate()
        {
            if (_transform == null || _meshFilter == null || _meshRenderer == null || _collider == null)
                Reset();

            width = _width;
            height = _height;
            depth = _depth;

            UpdateLocalScale();
        }
#endif
    }
}