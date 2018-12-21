using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorUtils
{

    [MenuItem("Sprites/Set Pivot(s)")]
    static void SetPivots()
    {

        Object[] textures = GetSelectedTextures();

        Selection.objects = new Object[0];
        foreach (Texture2D texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.isReadable = false;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            ti.isReadable = true;
            List<SpriteMetaData> newData = new List<SpriteMetaData>();
            Vector2 pivot = new Vector2(128, 64);
            var spritesheet = ti.spritesheet;
            for (int i = 0; i < spritesheet.Length; i++)
            {
                SpriteMetaData d = spritesheet[i];
                if (Mathf.Approximately(d.rect.width, 256f))
                {
                    Debug.Log(d.name);
                    d.alignment = 9;
                    d.pivot = new Vector2(0.5f, pivot.y/d.rect.height);
                }
                newData.Add(d);
            }
            ti.spritesheet = newData.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }

    static Object[] GetSelectedTextures()
    {
        return Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
    }

}
