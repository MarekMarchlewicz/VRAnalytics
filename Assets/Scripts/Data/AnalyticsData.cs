using System.Collections.Generic;
using UnityEngine;

public class AnalyticsData
{
    public List<ViewData> views = new List<ViewData>();
}

public class ViewData
{
    public List<Vector4> positions = new List<Vector4>();
}
