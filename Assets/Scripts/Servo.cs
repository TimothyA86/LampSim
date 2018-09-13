using UnityEngine;

[System.Serializable]
public class Servo
{
	[SerializeField] private LampJoint joint;
	[SerializeField] private int maxOffset;

	public string JointName => joint?.name ?? "Joint Not Set";
	public Vector3 JointAxisVector => joint?.AxisVector ?? Vector3.zero;
	public int JointRotation => joint?.Rotation ?? 0;
	public int JointZeroAngle => joint?.ZeroAngle ?? 0;
	public int MaxOffset => maxOffset;
	public int Offset { get; private set; }
	public float NormalizedOffset => (float)Offset / maxOffset;

	public void SetOffset(int offset)
	{
		offset = Mathf.Clamp(offset, -maxOffset, maxOffset);
		joint?.Rotate(offset - Offset);
		Offset = offset;
	}

	public void SetOffsetNormalized(float normalizedOffset)
	{
		SetOffset(OffsetFromNormalized(normalizedOffset));
	}

	public int OffsetFromNormalized(float normalizedOffset)
	{
		return Mathf.RoundToInt(maxOffset * normalizedOffset);
	}

	public float NormalizedFromOffset(int offset)
	{
		return (float)offset / maxOffset;
	}

	public void SetJointZeroAngle(int angle)
	{
		joint?.SetZeroAngle(angle);
	}
}