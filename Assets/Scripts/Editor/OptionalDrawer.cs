using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomPropertyDrawer(typeof(OptionalAttribute))]
public class OptionalDrawer : PropertyDrawer
{
	private bool show = false;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return show ? base.GetPropertyHeight(property, label) : 0f;
	}
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var optional = attribute as OptionalAttribute;
		var fieldProp = property.serializedObject.FindProperty(optional.field);

		if (fieldProp == null)
		{
			var content = new GUIContent("[Optional(" + optional.field +")]",
				"Could not find field " + optional.field + ".");
			var style = new GUIStyle();
			style.normal.textColor = Color.red;

			EditorGUI.LabelField(position, content, style);
			show = false;
		}

		switch (fieldProp.propertyType)
		{
		case SerializedPropertyType.Boolean:
			show = fieldProp.boolValue == (bool)optional.value;
			break;
		case SerializedPropertyType.Enum:
			show = fieldProp.enumValueIndex == (int)optional.value;
			break;
		default:
			show = false;
			break;
		}

		if (show)
		{
			EditorGUI.PropertyField(position, property);
		}
		else
		{
			var targetObject = property.serializedObject.targetObject;

			targetObject.GetType()
			.GetField(property.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
			?.SetValue(targetObject, null);

			position.height = 0f;
		}
	}
}