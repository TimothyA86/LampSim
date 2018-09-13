using UnityEngine;
using UnityEngine.Assertions;

[ExecuteInEditMode]
[RequireComponent(typeof(LampJoint))]
public class InverseJoint : MonoBehaviour
{
	// Rotates in the opposite direction of specified joint

	[SerializeField] private LampJoint otherJoint;
	private LampJoint joint;

	private void Start()
	{
		joint = GetComponent<LampJoint>();
		otherJoint.Rotated += OnOtherJointRotated;

		Assert.AreNotEqual(joint, otherJoint, "Failed assertions (" + name + "): Other Joint cannot be attached to this game object.");
	}

	private void OnOtherJointRotated(LampJoint otherJoint, int deltaAngle)
	{
		// Rotate the joint in the opposite direction of "otherJoint"
		joint.Rotate(-deltaAngle);
	}
}
