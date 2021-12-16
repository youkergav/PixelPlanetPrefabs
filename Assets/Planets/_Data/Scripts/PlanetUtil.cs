using UnityEngine;

static public class PlanetUtil
{
    public static Texture2D GenTexture(Gradient gradient)
    {
        Texture2D texture = new Texture2D(128, 1);
        for (int h = 0; h < texture.height; h++)
        {
            for (int w = 0; w < texture.width; w++)
            {
                texture.SetPixel(w, h, gradient.Evaluate((float)w / texture.width));
            }
        }

        texture.Apply();
        texture.wrapMode = TextureWrapMode.Clamp;

        return texture;
    }
}
