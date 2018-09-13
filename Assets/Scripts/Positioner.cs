using UnityEngine;

public class Positioner : MonoBehaviour
{
// Used to positions lamp parts relative to joints
// Only relevatnt to the Unity Editor and not the final build

#if UNITY_EDITOR
	[SerializeField] private Transform graphic;
	[SerializeField] private LampJoint joint;

	[SerializeField] private float width;
	[SerializeField] private float length;
	[SerializeField] private float positionScale = 1f;

	private void OnValidate()
	{
		if (graphic)
		{
			var scale = new Vector3(width, length, width);
			var position = graphic.localPosition;
			position.y = length * positionScale;
			graphic.localScale = scale;
			graphic.localPosition = position;

			if (joint)
			{
				position.y += length * positionScale;
				joint.transform.localPosition = position;
			}
		}
	}
#endif
}
