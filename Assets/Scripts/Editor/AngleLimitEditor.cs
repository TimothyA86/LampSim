using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AngleLimit))]
public class AngleLimitEditor : Editor
{
	private const float Radius = 10;
	private AngleLimit limit;

	public void OnSceneGUI()
	{
		limit = target as AngleLimit;

		float angle = limit.Angle;

		var transform = limit.transform;
		var position = transform.position;
		var forward = transform.forward;
		var right = transform.right;
		var up = transform.up;

		Handles.color = Color.magenta;
		Handles.DrawWireArc(position, right, Quaternion.AngleAxis(-angle, right) * forward, angle * 2, Radius);
		Handles.DrawLine(position, position + Quaternion.AngleAxis(-angle, right) * forward * Radius);
		Handles.DrawLine(position, position + Quaternion.AngleAxis(angle, right) * forward * Radius);

		Handles.color = Color.cyan;
		Handles.DrawWireArc(position, up, Quaternion.AngleAxis(-angle, up) * forward, angle * 2, Radius);
		Handles.DrawLine(position, position + Quaternion.AngleAxis(-angle, up) * forward * Radius);
		Handles.DrawLine(position, position + Quaternion.AngleAxis(angle, up) * forward * Radius);
	}
}
