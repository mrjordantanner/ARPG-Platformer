using UnityEngine;
using System.Reflection;
using UnityEditor;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public static class Utilities
    {

        public static void ClearConsole()
        {
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }


    }
}
