using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Lamp))]
public class LampEditor : Editor
{
	private SerializedProperty servosProp;

	private void OnEnable()
	{
		servosProp = serializedObject.FindProperty("servos");
	}

	public override void OnInspectorGUI()
	{
		var lamp = target as Lamp;

		serializedObject.Update();

		var divider = new GUIStyle(EditorStyles.toolbarButton);
		divider.border.top = divider.border.bottom = 1;
		divider.margin.top = divider.margin.bottom = 3;
		divider.fixedHeight = 2f;

		SerializedProperty elemProp;
		Servo servo;

		for (int i = 0; i < Lamp.NumberOfServos; ++i)
		{
			EditorGUILayout.Space();
			elemProp = servosProp.GetArrayElementAtIndex(i);
			servo = lamp.GetServo(i);
			EditorGUILayout.LabelField("Servo " + i + "  [" + servo.JointName + "]");
			EditorGUILayout.PropertyField(elemProp);
			servo.SetJointZeroAngle(EditorGUILayout.IntSlider("Joint Zero Angle", servo.JointZeroAngle, -180, 180));
			servo.SetOffset(EditorGUILayout.IntSlider("Current Offset", servo.Offset, -servo.MaxOffset, servo.MaxOffset));
			GUILayout.Box("", divider, GUILayout.ExpandWidth(true));
		}

		serializedObject.ApplyModifiedProperties();
	}
}