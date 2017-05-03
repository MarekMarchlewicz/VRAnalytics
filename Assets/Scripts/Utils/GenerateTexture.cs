using UnityEngine;

public class GenerateTexture : MonoBehaviour
{
	private void Start ()
    {
        Texture2D heatTexture = new Texture2D(256, 32);
        for(int x = 0; x < 256; x++)
        {
            for(int y = 0; y < 32; y++)
            {
                Color col;
                float xp = x / (255f);
                float lerp = -((xp - 1) * (xp - 1)) + 1;
                
                float h = 1f - Mathf.Lerp(1 / 3f, 1f, lerp);

                HSBColor hsb = new HSBColor(h, 1f, 1f);

                col= hsb.ToColor();

                col.a = lerp;

                heatTexture.SetPixel(x, y, col);
            }
        }

        System.IO.File.WriteAllBytes(Application.dataPath + "/HSBTex.png", heatTexture.EncodeToPNG());
    }
}
