using UnityEngine;
using System.Collections.Generic;

public class Heatmap : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    private float radius = 5f;

    private List<Vector4> positions;

    private Material material;
    
    private MaterialPropertyBlock materialPropertyBlock;

    private void Awake ()
    {
        positions = new List<Vector4>();

		material = meshRenderer.material;

        materialPropertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (positions.Count > 0)
        {
            materialPropertyBlock.SetFloat("_Radius", Mathf.Deg2Rad * radius);
            materialPropertyBlock.SetVectorArray("_Points", positions);
            gameObject.GetComponent<Renderer>().SetPropertyBlock(materialPropertyBlock);
        }
    }

	public void UpdatePosition(List<Vector4> gazePositions)
	{
		material.SetInt("_PointsLength", gazePositions.Count);

        positions = gazePositions;
	}
}
