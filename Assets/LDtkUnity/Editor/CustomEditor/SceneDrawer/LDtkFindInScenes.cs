﻿using System.Collections.Generic;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LDtkUnity.Editor
{
    public static class LDtkFindInScenes
    {
        public static List<T> FindInAllScenes<T>()
        {
            List<T> interfaces = new List<T>();

            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                TryAddToList(prefabStage.prefabContentsRoot, interfaces);
            }
            else
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    if (!scene.isLoaded)
                    {
                        continue;
                    }
                    
                    List<T> inScene = FindInScene<T>(scene);
                    foreach (T obj in inScene)
                    {
                        interfaces.Add(obj);
                    }
                }
            }

            return interfaces;
        }

        private static List<T> FindInScene<T>(Scene scene)
        {
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            List<T> interfaces = new List<T>();
            foreach(GameObject rootGameObject in rootGameObjects)
            {
                TryAddToList(rootGameObject, interfaces);
            }
            return interfaces;
        }

        private static void TryAddToList<T>(GameObject rootGameObject, List<T> interfaces)
        {
            T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
            foreach (T childInterface in childrenInterfaces)
            {
                interfaces.Add(childInterface);
            }
        }
    }
}