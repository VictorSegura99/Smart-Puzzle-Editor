using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class V3Helper
{
    static public Vector3Int V3ToV3Int(Vector3 v)
    {
        return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
    }
}
