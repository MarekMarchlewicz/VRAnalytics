using UnityEngine;
using System.Collections.Generic;

public class Heatmap : MonoBehaviour
{
	[SerializeField]
    private bool feedDataFromOutside;

	[SerializeField]
    private Transform[] targets;

	[SerializeField]
    private float radius = 5f;

    private Vector4[] positions;

    private Material material;

    public int count = 50;

    private void Start ()
    {
        positions = new Vector4[count];

		material = GetComponent<MeshRenderer> ().material;
    }

    private void Update()
    {
		if(!feedDataFromOutside)
			UpdatePositions ();


        if (positions.Length > 0)
        {
            var materialProperty = new MaterialPropertyBlock();

            materialProperty.SetFloat("_Radius", Mathf.Deg2Rad * radius);
            materialProperty.SetVectorArray("_Points", positions);
            gameObject.GetComponent<Renderer>().SetPropertyBlock(materialProperty);
        }
    }

	private void UpdatePositions()
	{
		material.SetInt("_PointsLength", targets.Length);

		if (positions == null || positions.Length < targets.Length)
			positions = new Vector4[targets.Length];

		for (int i = 0; i < targets.Length; i++)
		{
			positions [i] = targets [i].position;
		}
	}

	public void UpdatePosition(List<Vector4> gazePositions)
	{
		material.SetInt("_PointsLength", gazePositions.Count);

        positions = gazePositions.ToArray();
	}
}