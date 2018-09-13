using UnityEngine;
using UnityEngine.Assertions;

public class Lamp : MonoBehaviour
{
	public const int NumberOfServos = 4;

	[SerializeField] protected Servo[] servos = new Servo[NumberOfServos];
	private float cameraAspect;

	public Vector2 ViewPortExtends => new Vector2(1f, 1f / cameraAspect);
	public Camera Camera { get; private set; }

	private void Awake()
	{
		FetchCamera();
		Camera.aspect = 16f / 9f;
		cameraAspect = Camera.aspect;
	}

	public void FetchCamera()
	{
		Camera = GetComponentInChildren<Camera>();
		Assert.IsNotNull(Camera, "ERROR [" + name + "] does not have a camera in its branch");
	}

	public Vector3 WorldToViewportPoint(Vector3 position)
	{
	// Project the position onto the cameras local xy plane (normalized with (-1, -1) being the bottom left and (1, 1) being the top right)
	// z value is the different between the cameras z and the positions z (not normalized)

		var point = Camera.WorldToViewportPoint(position);
		point.x = (point.x - 0.5f) * 2f;
		point.y = (point.y - 0.5f) * 2f / Camera.aspect;
		return point;
	}

	public Vector3 ViewportToWorldPoint(Vector3 position)
	{
	// Convert the position from the cameras viewport c0ordinates into a world position

		position.x = position.x / 2f + 0.5f;
		position.y = position.y * Camera.aspect / 2f + 0.5f;
		return Camera.ViewportToWorldPoint(position);
	}

	public Servo GetServo(int servo)
	{
		Assert.IsFalse(servo < 0 || servo >= servos.Length, "Servo index " + servo + " is out of bounds.");
		Assert.IsNotNull(servos[servo], "Servo at index " + servo + " is null");
		return servos[servo];
	}
}
