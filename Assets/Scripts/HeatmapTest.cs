using System.Collections.Generic;
using UnityEngine;

public class HeatmapTest : MonoBehaviour
{
    [SerializeField]
    private Transform[] targets;

    [SerializeField]
    private Heatmap heatmap;

    private List<Vector4> positions = new List<Vector4>();
    
	private void Update ()
    {
        positions.Clear();

        for(int i = 0; i < targets.Length; i++)
        {
            positions.Add(targets[i].position);
        }

        heatmap.UpdatePosition(positions);
	}
}
