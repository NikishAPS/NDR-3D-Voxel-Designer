namespace Source.Components.GUI
{
    using UnityEngine;
    using UnityEngine.UI;

    using Voxelator;

    [AddComponentMenu("Source/GUI/GUI Height Param"), DisallowMultipleComponent]
    public sealed class GUIHeightParam : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputField _inputField = default;
        [SerializeField] private VoxelGrid _voxelGrid = default;

        private void OnValueChanged(string value)
        {
            if (int.TryParse(value, out int result))
            {
                if (result <= 0)
                {
                    _inputField.text = "-";
                    _voxelGrid.height = -1;
                }
                _voxelGrid.height = result;
            }
            else
                _voxelGrid.height = -1;
        }

        private void Awake() => _inputField.onValueChanged.AddListener(OnValueChanged);

        private void Start()
        {
            if (_voxelGrid.height > 0)
                _inputField.text = _voxelGrid.height.ToString();
            else
                _inputField.text = "-";
        }
    }
}