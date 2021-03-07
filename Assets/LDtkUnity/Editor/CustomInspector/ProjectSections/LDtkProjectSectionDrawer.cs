﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LDtkUnity.Editor
{
    /// <summary>
    /// Reminder: Responsibility is just for drawing the Header content and and other unique functionality. All of the numerous content is handled in the Reference Drawers
    /// </summary>
    public abstract class LDtkProjectSectionDrawer<T> : IDisposable where T : ILDtkIdentifier
    {
        protected abstract string PropertyName { get; }
        protected abstract string GuiText { get; }
        protected abstract string GuiTooltip { get; }
        protected abstract Texture2D GuiImage { get; }
        
        protected readonly SerializedObject SerializedObject;
        protected SerializedProperty ArrayProp;
        private bool _dropdown;

        protected LDtkContentDrawer<T>[] Drawers;
        
        protected LDtkProject Project => (LDtkProject) SerializedObject.targetObject;
        public bool HasProblem => Drawers.Any(p => p.HasProblem());
        

        protected LDtkProjectSectionDrawer(SerializedObject serializedObject)
        {
            SerializedObject = serializedObject;
        }

        public void Init()
        {
            ArrayProp = SerializedObject.FindProperty(PropertyName);
            _dropdown = EditorPrefs.GetBool(PropertyName, true);
        }
        public void Dispose()
        {
            EditorPrefs.SetBool(PropertyName, _dropdown);
        }

        public void Draw(T[] datas)
        {
            int arraySize = GetSizeOfArray(datas);
            
            if (arraySize <= 0)
            {
                return;
            }

            if (ArrayProp != null)
            {
                ArrayProp.arraySize = arraySize;
            }

            LDtkDrawerUtil.DrawDivider();
            Rect area = EditorGUILayout.GetControlRect();
            DrawFoldoutArea(area);

            List<LDtkContentDrawer<T>> drawers = new List<LDtkContentDrawer<T>>();
            GetDrawers(datas, drawers);
            Drawers = drawers.ToArray();
            
            if (_dropdown)
            {
                DrawDropdownContent(datas);
            }
            else if (HasProblem)
            {
                Rect errorArea = new Rect(area)
                {
                    x = area.x + EditorGUIUtility.labelWidth,
                    width = area.height

                };
                
                GUI.DrawTexture(errorArea, EditorGUIUtility.IconContent("console.warnicon.sml").image);
            }
        }

        protected abstract void GetDrawers(T[] defs, List<LDtkContentDrawer<T>> drawers);


        protected virtual int GetSizeOfArray(T[] datas)
        {
            return datas.Length;
        }
        
        private void DrawFoldoutArea(Rect controlRect)
        {
            GUIContent content = new GUIContent()
            {
                text = GuiText,
                tooltip = GuiTooltip,
                image = GuiImage
            };
            
            _dropdown = EditorGUI.Foldout(controlRect, _dropdown, content);
        }

        protected virtual void DrawDropdownContent(T[] datas)
        {
            foreach (LDtkContentDrawer<T> drawer in Drawers)
            {
                drawer.Draw();
            }
        }
    }
}