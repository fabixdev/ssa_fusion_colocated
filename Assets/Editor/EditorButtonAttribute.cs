using UnityEngine;
using UnityEditor;

[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
public class EditorButtonAttribute : PropertyAttribute
{
    public string Label { get; private set; }

    public EditorButtonAttribute(string label = null)
    {
        Label = label;
    }
}

[CustomEditor(typeof(MonoBehaviour), true)]
public class EditorButtonDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var targetObject = target as MonoBehaviour;
        var methods = targetObject.GetType().GetMethods(
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic);

        foreach (var method in methods)
        {
            var attributes = method.GetCustomAttributes(typeof(EditorButtonAttribute), true);
            if (attributes.Length > 0)
            {
                var attribute = (EditorButtonAttribute)attributes[0];
                string buttonLabel = string.IsNullOrEmpty(attribute.Label) ? method.Name : attribute.Label;

                if (GUILayout.Button(buttonLabel))
                {
                    method.Invoke(targetObject, null);
                }
            }
        }
    }
}
