using UnityEngine;

[ExecuteInEditMode]
public class LampJoint : MonoBehaviour
{
	public event System.Action<LampJoint, int> Rotated;

	[SerializeField] private JointAxis axis;
	[SerializeField, HideInInspector] private int zeroAngle;
	[SerializeField, HideInInspector] private Vector3 axisVector = Vector3.zero;

	public JointAxis Axis => axis;
	public int ZeroAngle => zeroAngle;
	public Vector3 AxisVector => axisVector;
	public int Rotation => (int)Vector3.Dot(transform.localEulerAngles, axisVector) - zeroAngle;

	private void SetAxisVector()
	{
		switch (axis)
		{
		case JointAxis.X:
			axisVector = Vector3.right;
			break;
		case JointAxis.Y:
			axisVector = Vector3.up;
			break;
		case JointAxis.Z:
			axisVector = Vector3.forward;
			break;
		}
	}

	public void Rotate(int deltaAngle)
	{
		if (deltaAngle == 0)
		{
			return;
		}

		transform.Rotate(axisVector, deltaAngle);

		// Round to try and correct percision errors so that they don't add up to be something significant
		var eulers = transform.localEulerAngles;
		transform.localEulerAngles = new Vector3(Mathf.Round(eulers.x), Mathf.Round(eulers.y), Mathf.Round(eulers.z));

		Rotated?.Invoke(this, deltaAngle);
	}

	public void SetZeroAngle(int angle)
	{
		Rotate(angle - zeroAngle);
		zeroAngle = angle;
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		SetAxisVector();
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(transform.position, 0.2f);
	}
#endif
}

public enum JointAxis
{
	X,
	Y,
	Z
}
