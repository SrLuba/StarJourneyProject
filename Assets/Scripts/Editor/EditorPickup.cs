#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PickupSO))]
public class EditorPickup : Editor
{
    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        PickupSO so = target as PickupSO;

        if (so == null || so.icon == null)
        {
            // Si el ScriptableObject o el Sprite son nulos, devuelve la miniatura predeterminada
            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        // Obtén la textura asociada al Sprite
        Texture2D originalTexture = so.icon.texture;

        // Obtiene la rectángulo de textura del Sprite
        Rect textureRect = so.icon.textureRect;

        // Asegúrate de que el rectángulo de recorte esté dentro de los límites de la textura
        float x = Mathf.Clamp(textureRect.x, 0, originalTexture.width);
        float y = Mathf.Clamp(textureRect.y, 0, originalTexture.height);
        float rectWidth = Mathf.Clamp(textureRect.width, 0, originalTexture.width - x);
        float rectHeight = Mathf.Clamp(textureRect.height, 0, originalTexture.height - y);

        // Recorta la textura original según el rectángulo de textura del Sprite
        int startX = Mathf.RoundToInt(x);
        int startY = Mathf.RoundToInt(y);
        int rectTexWidth = Mathf.RoundToInt(rectWidth);
        int rectTexHeight = Mathf.RoundToInt(rectHeight);
        Color[] pixels = originalTexture.GetPixels(startX, startY, rectTexWidth, rectTexHeight);

        // Crea una nueva textura con los píxeles recortados
        Texture2D newTexture = new Texture2D(rectTexWidth, rectTexHeight);
        newTexture.SetPixels(pixels);
        newTexture.Apply();

        return newTexture;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
#endif