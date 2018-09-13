using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Servo), true)]
public class ServoDrawer : PropertyDrawer
{
	private const float MaxOffsetRectWidth = 200f;
	private const float FieldHeight = 16f;
	private readonly float PropertyHeight = Mathf.Max(FieldHeight, GUI.skin.label.CalcSize(new GUIContent("LABEL")).y);

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => PropertyHeight;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var jointProp = property.FindPropertyRelative("joint");
		var maxOffsetProp = property.FindPropertyRelative("maxOffset");

		var spacing = 10;
		var newlabel = new GUIContent("Joint");
		var labelSize = GUI.skin.label.CalcSize(newlabel);
		var labelRect = new Rect(position.x, position.y, labelSize.x, labelSize.y);
		var jointRect = new Rect(labelRect.x + labelRect.width + spacing, labelRect.y, position.width - labelRect.width - spacing, FieldHeight);

		if (jointProp.objectReferenceValue)
		{
			var oldLableWidth = labelRect.width;
			newlabel = new GUIContent("Max Offset");
			labelRect.width = GUI.skin.label.CalcSize(newlabel).x;

			var maxOffsetRect = new Rect(labelRect.x + labelRect.width + spacing, labelRect.y, MaxOffsetRectWidth, FieldHeight);
			EditorGUI.IntSlider(maxOffsetRect, maxOffsetProp, 0, 360, GUIContent.none);

			var jointRectXChange = maxOffsetRect.width + spacing + labelRect.width - oldLableWidth;
			jointRect.x += jointRectXChange;
			jointRect.width -= jointRectXChange;
		}

		EditorGUI.LabelField(labelRect, newlabel);
		EditorGUI.PropertyField(jointRect, jointProp, GUIContent.none);
	}
}