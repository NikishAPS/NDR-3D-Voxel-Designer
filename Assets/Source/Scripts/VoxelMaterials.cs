using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxelator
{
    public class VoxelMaterials : MonoBehaviour
    {
        [SerializeField]
        public Material standard, select, selectMain, copy;

        private MeshRenderer meshRenderer;

        public void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        void Update()
        {

        }

        public void Standard()
        {
            return;
            if (!meshRenderer)
                Start();
            meshRenderer.material = standard;
        }

        public void SetStandard(Material material)
        {
            return;

            standard = material;
            Standard();
        }

        public void Select()
        {
            return;

            meshRenderer.material = select;
        }

        public void SelectMain()
        {
            return;

            meshRenderer.material = selectMain;
        }

        public void Copy()
        {
            return;

            meshRenderer.material = copy;
        }
    }
}
