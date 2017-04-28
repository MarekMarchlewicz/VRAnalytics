using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnalyticsData
{
    public string name = "DATA";

    public List<ViewData> views = new List<ViewData>();
}

[System.Serializable]
public class ViewData
{
    public List<ViewPosition> positions = new List<ViewPosition>();
}

[System.Serializable]
public struct ViewPosition
{
    public float x, y, z;
    public float t;

    public Vector3 GetPosition()
    {
        return new Vector3(x, y, z);
    }
}
