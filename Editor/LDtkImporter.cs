﻿using System.IO;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LDtkUnity.Editor
{
    [ScriptedImporter(1, "ldtk")]
    public class LDtkImporter : ScriptedImporter 
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string path = ctx.assetPath;
            string content = File.ReadAllText(path);
            TextAsset textAsset = new TextAsset(content);
            
            Texture2D tex = GetTexture2D();

            ctx.AddObjectToAsset ("ldtk", textAsset, tex);
            ctx.SetMainObject(textAsset);
            AssetDatabase.Refresh();
            
            Debug.Log("Detected updated LDtk project");
        }

        private static Texture2D GetTexture2D()
        {
            Texture2D tex = AssetPreview.GetMiniTypeThumbnail(typeof(TilemapCollider2D));
            if (tex == null)
            {
                Debug.LogError("tex null");
            }

            return tex;
        }
    }
}
