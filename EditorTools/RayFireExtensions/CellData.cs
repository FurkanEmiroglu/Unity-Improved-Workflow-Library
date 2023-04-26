﻿#if RAYFIRE
using UnityEngine;

namespace ImprovedWorkflow.EditorTools.RayfireExtensions
{
    [System.Serializable]
    public struct CellData
    {
        public Vector3[] positions;
        public Vector3[] rotations;
        public Vector3[] scales;

        public CellData(Vector3[] p, Vector3[] r, Vector3[] s)
        {
            positions = p;
            rotations = r;
            scales = s;
        }
    }
}
#endif