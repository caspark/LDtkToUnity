﻿using LDtkUnity.Runtime.Data;
using UnityEditor;
using UnityEngine;

namespace LDtkUnity.Editor.AssetManagement.Drawers
{
    public abstract class LDtkReferenceDrawer<T> where T : ILDtkIdentifier
    {
        protected float LabelWidth(float controlRectWidth)
        {
            const float divisor = 2.24f;
            const float offset = -33;
            float totalWidth = controlRectWidth + EditorGUIUtility.singleLineHeight;
            return Mathf.Max(totalWidth / divisor + offset, EditorGUIUtility.labelWidth);
        }

        public void Draw(T definition)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            DrawInternal(controlRect, definition);
        }

        protected abstract void DrawInternal(Rect controlRect, T data);
        
        protected void DrawLabel(Rect controlRect, T definition)
        {
            controlRect.xMin += controlRect.height;
            EditorGUI.LabelField(controlRect, definition.identifier);
        }

        protected Rect DrawLeftIcon(Rect controlRect, Texture2D icon)
        {
            Rect iconRect = GetLeftIconRect(controlRect);
            GUI.DrawTexture(iconRect, icon);
            return iconRect;
        }
        
        protected Rect GetLeftIconRect(Rect controlRect)
        {
            Rect textureRect = new Rect(controlRect)
            {
                width = controlRect.height
            };
            return textureRect;
        }


        protected virtual void DrawSelfSimple(Rect controlRect, Texture2D iconTex, T item)
        {
            DrawLeftIcon(controlRect, iconTex);
            DrawLabel(controlRect, item);
        }
        
        protected bool DrawRightFieldIconButton(Rect controlRect, string iconContent)
        {
            float labelWidth = LabelWidth(controlRect.width);
            
            Rect buttonRect = new Rect(controlRect)
            {
                x = controlRect.x + labelWidth - controlRect.height,
                
                width = controlRect.height,
                height = controlRect.height,
            };
            bool isPressed = GUI.Button(buttonRect, GUIContent.none);

            Texture refreshImage = EditorGUIUtility.IconContent(iconContent).image;
            Rect imageContent = new Rect(buttonRect)
            {
                width = refreshImage.width,
                height = refreshImage.height,
                center = buttonRect.center
            };
            GUI.DrawTexture(imageContent, refreshImage);

            return isPressed;
        }
    }
}