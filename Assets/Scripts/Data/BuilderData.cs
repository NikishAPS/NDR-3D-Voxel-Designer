using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuilderData
{
    public int FaceCount;
    public List<int> BuildedIndices;

    public BuilderData(int faceCount, List<int> buildedIndices)
    {
        FaceCount = faceCount;
        BuildedIndices = buildedIndices;
    }
}
