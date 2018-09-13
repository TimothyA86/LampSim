using UnityEngine;

public class AngleLimit : MonoBehaviour
{
	[SerializeField, Range(0, 180f)] private float angle;

	public float Angle => angle;

	public bool IsInLimits(Vector3 point)
	{
		var transform = this.transform;
		var positionVector = (point - transform.position).normalized;
		var xzProjection = positionVector - Vector3.up * positionVector.y;
		var yzProjection = positionVector - Vector3.right * positionVector.x;
		float xzAngle = Vector3.SignedAngle(transform.forward, xzProjection, transform.up);
		float yzAngle = Vector3.SignedAngle(transform.forward, yzProjection, transform.right);

		// BUGGED: Need to measure angle differently
		float xzMedian = Mathf.Max(Mathf.Min(-angle, angle), xzAngle);
		float yzMedian = Mathf.Max(Mathf.Min(-angle, angle), yzAngle);

		print(xzAngle + " " + xzProjection);
		print(yzAngle + " " + yzProjection);
		print("---");

		return (xzAngle == xzMedian && yzAngle == yzMedian);
	}
}
