/*
 * Скрипт для создания и копирования вокселей
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxelator
{
    public class CreateInstanceVoxel : MonoBehaviour
    {
        public UnityEngine.UI.Image colorControl;
        public VoxelMeshGenerator voxelMeshGenerator;
        public GameObject voxelCopy;
        public List<GameObject> voxelsCopy;

        private VoxelsControl voxelsControl;
        private MaterialsControl materialsControl;

        void Start()
        {
            voxelMeshGenerator.CreateShape();
            voxelMeshGenerator.gameObject.SetActive(false);


            voxelsControl = Camera.main.GetComponent<VoxelsControl>();
            materialsControl = voxelsControl.GetComponent<MaterialsControl>();
        }

        void Update()
        {

        }

        public VoxelMeshGenerator Create(Vector3 pos)
        {
            VoxelMeshGenerator voxelMeshGenerator = Instantiate(this.voxelMeshGenerator, pos, Quaternion.identity);
            voxelMeshGenerator.gameObject.SetActive(true);
            voxelMeshGenerator.GetComponent<MeshRenderer>().material.color = colorControl.color;

            voxelsControl.voxels.Add(voxelMeshGenerator);

            return voxelMeshGenerator;
        }

        public VoxelMeshGenerator Create(Vector3 pos, int index)
        {
            VoxelMeshGenerator voxelMeshGenerator = Instantiate(this.voxelMeshGenerator, pos, Quaternion.identity);
            voxelMeshGenerator.gameObject.SetActive(true);
            voxelMeshGenerator.GetComponent<VoxelMaterials>().standard = materialsControl.GetMaterial(index);
            voxelMeshGenerator.GetComponent<MeshRenderer>().material.color = colorControl.color;

            voxelsControl.voxels.Add(voxelMeshGenerator);

            return voxelMeshGenerator;
        }

        public void PastBuild(Vector3 pos)
        {
            voxelsControl.voxels.Add(Instantiate(voxelCopy, pos, Quaternion.identity).GetComponent<VoxelMeshGenerator>());
        }

        public List<GameObject> Past()
        {
            List<GameObject> co = new List<GameObject>();

            for (int i = 0; i < voxelsCopy.Count; i++)
            {
                GameObject go = Instantiate(voxelsCopy[i], voxelsCopy[i].transform.position, Quaternion.identity);

                voxelsControl.voxels.Add(go.GetComponent<VoxelMeshGenerator>());

                co.Add(go);
                co[i].GetComponent<VoxelMaterials>().Start();
            }

            return co;
        }

        public void SetVoxelsCopy(List<GameObject> voxels)
        {
            voxelsCopy = voxels;
            voxelCopy = voxels[voxels.Count - 1];
        }


        public GameObject Copy(GameObject go, Vector3 pos)
        {
            GameObject go_I = Instantiate(go, pos, Quaternion.identity);
            voxelsControl.voxels.Add(go_I.GetComponent<VoxelMeshGenerator>());
            //go.GetComponent<VoxelMaterials>().Standard();
            return go_I;
        }

        public VoxelMeshGenerator Copy(VoxelMeshGenerator go, Vector3 pos)
        {
            VoxelMeshGenerator go_I = Instantiate(go, pos, Quaternion.identity).GetComponent<VoxelMeshGenerator>();
            voxelsControl.voxels.Add(go_I);
            //go.GetComponent<VoxelMaterials>().Standard();
            return go_I;
        }

        public VoxelMeshGenerator CopyOnly(VoxelMeshGenerator go, Vector3 pos)
        {
            return Instantiate(go, pos, Quaternion.identity).GetComponent<VoxelMeshGenerator>();
        }
    }
}
