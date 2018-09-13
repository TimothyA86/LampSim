using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RangeDropDownAttribute))]
public class RangeDropDownDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var range = attribute as RangeDropDownAttribute;
		var options = new string[range.max - range.min + 1];

		for (int i = 0; i < options.Length; ++i)
		{
			options[i] = range.prefix + (range.min + i);
		}

		property.intValue = EditorGUI.Popup(position, label.text, property.intValue, options);
		property.serializedObject.ApplyModifiedProperties();
	}
}
