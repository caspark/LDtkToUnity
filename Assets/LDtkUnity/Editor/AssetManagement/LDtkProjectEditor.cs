﻿using System.IO;
using LDtkUnity.Editor.AssetManagement.AssetFactories.EnumHandler;
using LDtkUnity.Editor.AssetManagement.Drawers;
using LDtkUnity.Runtime.Data;
using LDtkUnity.Runtime.Data.Definition;
using LDtkUnity.Runtime.Data.Level;
using LDtkUnity.Runtime.Tools;
using LDtkUnity.Runtime.UnityAssets.Entity;
using LDtkUnity.Runtime.UnityAssets.IntGridValue;
using LDtkUnity.Runtime.UnityAssets.Settings;
using LDtkUnity.Runtime.UnityAssets.Tileset;
using UnityEditor;
using UnityEngine;

namespace LDtkUnity.Editor.AssetManagement
{
    [CustomEditor(typeof(LDtkProject))]
    public class LDtkProjectEditor : UnityEditor.Editor
    {
        private LDtkDataProject? _data;

        private Vector2 _currentScroll;
        private bool _dropdown;

        private string ProjectPath => Path.GetDirectoryName(AssetDatabase.GetAssetPath(((LDtkProject)target).ProjectJson));
        
        public override void OnInspectorGUI()
        {
            SerializedObject serializedObj = new SerializedObject(target);
            ShowGUI(serializedObj);
            
            _dropdown = EditorGUILayout.Foldout(_dropdown, "Internal Data");
            if (_dropdown)
            {
                EditorGUI.indentLevel++;
                GUI.enabled = false;
                base.OnInspectorGUI();
                GUI.enabled = true;
                EditorGUI.indentLevel--;
            }
        }
        

        private void ShowGUI(SerializedObject serializedObj)
        {
            SerializedProperty textProp = serializedObj.FindProperty(LDtkProject.PROP_JSON);
            
            DrawWelcomeMessage(textProp);
            if (!AssignJsonField(textProp) || _data == null)
            {
                return;
            }
            EditorGUILayout.Space();

            LDtkDataProject projectData = _data.Value;
            
            DrawLevels(projectData.levels);
            
            EditorGUILayout.Space();
            
            SerializedProperty intGridProp = serializedObj.FindProperty(LDtkProject.PROP_INTGRID);
            intGridProp.arraySize = projectData.defs.layers.Length;
            DrawLayers(projectData.defs.layers, intGridProp);
            
            EditorGUILayout.Space();
            
            SerializedProperty entitiesProp = serializedObj.FindProperty(LDtkProject.PROP_ENTITIES);
            entitiesProp.arraySize = projectData.defs.entities.Length;
            DrawEntities(projectData.defs.entities, entitiesProp);
            
            EditorGUILayout.Space();
            
            GenerateEnumsButton(projectData, serializedObj);
            DrawEnums(projectData.defs.enums);
            
            EditorGUILayout.Space();
            
            SerializedProperty tilesetsProp = serializedObj.FindProperty(LDtkProject.PROP_TILESETS);
            tilesetsProp.arraySize = projectData.defs.tilesets.Length;
            DrawTilesets(projectData.defs.tilesets, tilesetsProp);
        }

        private void GenerateEnumsButton(LDtkDataProject projectData, SerializedObject serializedObj)
        {
            if (GUILayout.Button("Generate Enums"))
            {
                string assetPath = AssetDatabase.GetAssetPath(target);
                assetPath = Path.GetDirectoryName(assetPath);
                
                
                LDtkEnumGenerator.GenerateEnumScripts(projectData.defs.enums, assetPath, serializedObj.targetObject.name);
            }
        }

        private bool AssignJsonField(SerializedProperty textProp)
        {
            Object prevObj = textProp.objectReferenceValue;
            EditorGUILayout.ObjectField(textProp);
            Object newObj = textProp.objectReferenceValue;
            
            if (newObj == null)
            {
                return false;
            }
            
            TextAsset textAsset = (TextAsset)textProp.objectReferenceValue;
            
            if (!ReferenceEquals(prevObj, newObj))
            {
                _data = null;

                if (!LDtkToolProjectLoader.IsValidJson(textAsset.text))
                {
                    Debug.LogError("LDtk: Invalid LDtk format");
                    textProp.objectReferenceValue = null;
                    return false;
                }
            }
            
            if (_data == null)
            {
                _data = LDtkToolProjectLoader.DeserializeProject(textAsset.text);
            }

            return true;
        }


        
        private void DrawWelcomeMessage(SerializedProperty textProp)
        {
            Rect rect = EditorGUILayout.GetControlRect();
            string welcomeMessage = GetWelcomeMessage(textProp);
            EditorGUI.LabelField(rect, welcomeMessage);
        }
        private string GetWelcomeMessage(SerializedProperty textProp)
        {
            if (textProp.objectReferenceValue == null)
            {
                return "Assign a LDtk json text asset";
            }

            string details = "";

            if (_data != null)
            {
                LDtkDataProject data = _data.Value;
                details = $" v{data.jsonVersion}";
            }
            
            return $"LDtk Project{details}";

        }

        #region drawing
        private void DrawLevels(LDtkDataLevel[] lvls)
        {
            foreach (LDtkDataLevel level in lvls)
            {
                new LDtkReferenceDrawerLevelIdentifier().Draw(level);
            }
        }

        private void DrawEnums(LDtkDefinitionEnum[] definitions)
        {
            foreach (LDtkDefinitionEnum enumDefinition in definitions)
            {
                new LDtkReferenceDrawerEnum().Draw(enumDefinition);
            }
        }

        private void DrawTilesets(LDtkDefinitionTileset[] definitions, SerializedProperty tilesetArrayProp)
        {
            for (int i = 0; i < definitions.Length; i++)
            {
                LDtkDefinitionTileset tilesetData = definitions[i];
                SerializedProperty tilesetProp = tilesetArrayProp.GetArrayElementAtIndex(i);

                new LDtkReferenceDrawerTileset(tilesetProp, ProjectPath).Draw(tilesetData);
            }
        }
        
        private void DrawEntities(LDtkDefinitionEntity[] entities, SerializedProperty entityArrayProp)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                LDtkDefinitionEntity entityData = entities[i];
                SerializedProperty entityProp = entityArrayProp.GetArrayElementAtIndex(i);

                new LDtkReferenceDrawerEntity(entityProp).Draw(entityData);
            }
        }

        private void DrawLayers(LDtkDefinitionLayer[] layers, SerializedProperty intGridArrayProp)
        {
            foreach (LDtkDefinitionLayer layer in layers)
            {
                if (!layer.IsIntGridLayer) continue;

                new LDtkReferenceDrawerIntGridLayer().Draw(layer);
                for (int i = 0; i < layer.intGridValues.Length; i++)
                {
                    LDtkDefinitionIntGridValue valueData = layer.intGridValues[i];
                    SerializedProperty valueProp = intGridArrayProp.GetArrayElementAtIndex(i);

                    new LDtkReferenceDrawerIntGridValue(valueProp).Draw(valueData);
                }
            }
        }
        #endregion

        
        
        /*private Rect DrawEntry(Texture2D icon, string entryName)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            
            int indent = 15;
            controlRect.xMin += indent;

            //controlRect = EditorGUI.IndentedRect(controlRect);

            Rect textureRect = new Rect(controlRect)
            {
                width = controlRect.height
            };
            GUI.DrawTexture(textureRect, icon);

            controlRect.xMin += textureRect.width;
            
            //EditorGUI.DrawRect(controlRect, Color.red);
            

            Rect labelRect = new Rect(controlRect)
            {
                width = Mathf.Max(controlRect.width/2, EditorGUIUtility.labelWidth) - EditorGUIUtility.fieldWidth
            };
            EditorGUI.LabelField(labelRect, entryName);
            
            Rect fieldRect = new Rect(controlRect)
            {
                x = labelRect.xMax,
                width = Mathf.Max(controlRect.width - labelRect.width, EditorGUIUtility.fieldWidth)
            };
            return fieldRect;
        }*/

        /*private void DrawAssignableAssetEntry<T>(Rect fieldRect) where T : Object, ILDtkAsset
        {
            //present green if okay //TODO
            //present yellow if referenced item is null
            //present red if asset does not exist
        }*/
    }
}