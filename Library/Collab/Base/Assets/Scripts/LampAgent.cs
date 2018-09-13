using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class LampAgent : Agent
{
// An agent takes actions that are provided by it's brain (Brain) and rewards those actions
// With our implementation, we have the agent control a Lamp and set the Target's position within the Lamp's camera view

	[SerializeField] protected Lamp lamp;
	[SerializeField] protected Transform target;
	[SerializeField] protected EActionType actionType;
	[SerializeField] protected EServoReset servoReset;
	[SerializeField] protected ETargetReset targetReset;
	[SerializeField, Range(0f, 1f)] float targetRandomRange = 1f;
	[SerializeField, Range(1f, 20f)] protected float targetZ = 1f;
	[SerializeField, Optional("agentParameters.onDemandDecision", true)] bool requestOnStart = false;

	public event System.Action ActionWasTaken;

	protected LampTrainingAcademy academy;
	protected Vector3 initialTargetViewportPosition = Vector3.zero;
	protected Vector3 currentTargetViewportPosition = Vector3.zero;
    protected int[] initialServoPositions;
	protected bool isInitialReset;
	protected int simulatedMaxSteps;

	public Lamp ControlledLamp => lamp;
	public Vector3 TargetPosition => target.position;

	public int SimulatedMaxSteps => simulatedMaxSteps;
	public int ActionsTaken { get; private set; } = 0;
	public int SuccessFulActions { get; private set; } = 0;
	public Vector3 InitialTargetViewportPosition => initialTargetViewportPosition;
	public Vector3 CurrentTargetViewportPosition => currentTargetViewportPosition;

	protected virtual void Start()
	{
		Assert.IsNotNull(lamp, "ERROR [" + name + "] Must provide a lamp for the agent to control");
		Assert.IsNotNull(target, "ERROR [" + name + "] Must provide a lamp target");
		Assert.IsTrue(Lamp.NumberOfServos == brain.brainParameters.vectorActionSize, "ERROR [" + name + "]: There should be an equal number of actions as there are servos");

		academy = FindObjectOfType<LampTrainingAcademy>();

		Assert.IsNotNull(academy, "ERROR [" + name + "] There must be a LampTrainingAcademy");

		initialServoPositions = new int[Lamp.NumberOfServos];

		if (agentParameters.onDemandDecision && requestOnStart)
		{
			RequestDecision();
		}
	}

	#region Agent
	public override void InitializeAgent()
	{
		base.InitializeAgent();
		isInitialReset = true;
		simulatedMaxSteps = agentParameters.maxStep;
	}

	public override void AgentReset()
	{
		if (!isInitialReset)
		{
			// AgentReset() is also invoked when an agent is initialized
			// We use this check to prevent the reset of the servos when the agent is initialized

			switch (servoReset)
			{
			case EServoReset.None:
				break;
			case EServoReset.Initial:
				SetServoPositions(initialServoPositions);
				break;
			case EServoReset.Random:
				SetRandomServoPositions();
				break;
			}
		}

		switch (targetReset)
		{
		case ETargetReset.None:
			break;
		case ETargetReset.Random:
			SetTargetRandomly(targetRandomRange);
			break;
		}

		SetInitialServoPositionsToCurrent();
		isInitialReset = false;
	}

	public override void CollectObservations()
	{
		// Vector observations are our input vector
		// So first so the inptut vector is in the form <servo 0 position, servo 1 position, servo 2 position, servo 3 position, target x offset, target y offset>
		// All components of the input vector are normalized between -1 and 1

		float[] obs = new float[6];
		int i = 0;

		for (; i < Lamp.NumberOfServos; ++i)
		{
			obs[i] = lamp.GetServo(i).NormalizedOffset;
			AddVectorObs(lamp.GetServo(i).NormalizedOffset);
		}

		obs[i++] = currentTargetViewportPosition.x;
		obs[i] = currentTargetViewportPosition.y;
		AddVectorObs(currentTargetViewportPosition.x);
		AddVectorObs(currentTargetViewportPosition.y);

		print("Observations: " + obs.AsString());
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
	// The vectorAction parameter is our "output vector"
	// Here the Agent decides how to take an action based on its actionType
	// Instant: Snap the Lamp's servos into the final positions (primarily used during reinforcement learning training sessions)
	// Gradual: Starts a coroutine that move the Lamp's servos into the final positions over a duration (primarily for viewing trained models and during imitation learning sessions)
	// Player: Moves the Lamp's servos based on key presses (used to test lamp movement in the editor)

		ActionResultFlags resultFlags;

		switch (actionType)
		{
		case EActionType.Instant:
			TakeInstantAIAction(vectorAction);
			AddReward(EvaluateAction(vectorAction, out resultFlags));

			if ((resultFlags & ActionResultFlags.Terminal) != 0)
			{
				Done();
			}
			else
			{
				ResetAgentIfNotDone();
			}

			ActionWasTaken?.Invoke();

			break;

		case EActionType.Gradual:
			print("Action: " + vectorAction.AsString());
			TakeGradualAIAction(vectorAction);
			break;

		case EActionType.Player:
			TakePlayerAction(vectorAction);
			EvaluateAction(vectorAction, out resultFlags);

			if ((resultFlags & ActionResultFlags.Success) != 0)
			{
				ResetAgentIfNotDone();
			}

			ActionWasTaken?.Invoke();

			break;
		}
	}
	#endregion

	#region Reward Functions
	/// <summary>
	/// Is invoked after the LampAgent takes an action
	/// </summary>
	/// <param name="vectorAction">action that was taken</param>
	/// <param name="resultFlags">results of the action as a bitmask</param>
	/// <returns>reward for the action</returns>
	protected virtual float EvaluateAction(float[] vectorAction, out ActionResultFlags resultFlags)
	{
	// Assumes the actions is already taken
	// vectorActions parameter is only provided just in case the implementation needs to know which action was taken
	// resultFlags is a bitmask that indicates if the action was successful and/or terminal

		resultFlags = ActionResultFlags.None;

		// Calculate the target's x an y offset from the center of the camera
		// And determine if the target is infront of the camera (on the cameras local z axis)
		// Here we only consider the distance that the target is from the center of the camera (on the x and y axis)

		currentTargetViewportPosition = lamp.WorldToViewportPoint(target.position);
		bool targetIsInFrontOfCamera = currentTargetViewportPosition.z > 0;
		float initialSqrOffset = ((Vector2)initialTargetViewportPosition).sqrMagnitude;
		float currentSqrOffset = ((Vector2)currentTargetViewportPosition).sqrMagnitude;

		if (Mathf.Abs(currentTargetViewportPosition.x) <= 1f && Mathf.Abs(currentTargetViewportPosition.y) <= 1f)
		{
			if (targetIsInFrontOfCamera)
			{
				if (currentSqrOffset <= initialSqrOffset)
				{
					float reward = Mathf.Pow(0.1f, currentSqrOffset / initialSqrOffset);

					if (currentSqrOffset <= academy.SuccessThreshold)
					{
						resultFlags |= ActionResultFlags.Success;
						reward = 1f;
					}

					return reward;
				}
			}
		}

		return -1f;
	}
	#endregion

	#region Actions
	protected void TakePlayerAction(float[] vectorAction)
	{
	// Interprets the actions as -1, 0 or +1 and offsets the servo's current position by that much

		Servo servo;

		for (int i = 0; i < Lamp.NumberOfServos; ++i)
		{
			servo = lamp.GetServo(i);
			servo.SetOffset(servo.Offset + (int)vectorAction[i]);
		}
	}

	protected void TakeInstantAIAction(float[] vectorAction)
	{
	// Interprets the actions as absolute positions

		for (int i = 0; i < Lamp.NumberOfServos; ++i)
		{
			lamp.GetServo(i).SetOffsetNormalized(vectorAction[i]);
		}
	}

	protected void TakeGradualAIAction(float[] vectorAction)
	{
		StartCoroutine(TakeGradualAIActionRoutine(vectorAction));
	}

	private IEnumerator TakeGradualAIActionRoutine(float[] vectorAction)
	{
	// Interprets the actions as absolute positions
	// Moves the Lamp's servo positions from their initial positions to their final positions over a duration of 2 seconds

		int count = Lamp.NumberOfServos;
		int[] endServoPositions = new int[count];

		for (int i = 0; i < count; ++i)
		{
			endServoPositions[i] = lamp.GetServo(i).OffsetFromNormalized(Mathf.Clamp(vectorAction[i], -1f, 1f));
		}

		float duration = 2f;
		float time = 0f;
		int offset;

		// Interpolate servo positions from their initial positions to their final (end) positions
		while (time < duration)
		{
			time = Mathf.Min(time + Time.deltaTime, duration);

			for (int i = 0; i < count; ++i)
			{
				offset = (int)Mathf.SmoothStep(initialServoPositions[i], endServoPositions[i], time / duration);
				lamp.GetServo(i).SetOffset(offset);
			}

			yield return null;
		}

		// Servos are now at their final positions, evaluate the action
		ActionResultFlags resultFlags;
		float reward = EvaluateAction(vectorAction, out resultFlags);
		bool success = (resultFlags & ActionResultFlags.Success) != 0;
		print(success ? "Success" : "Fail");
		print("Reward: " + reward);
		print("-----------\n");

		AddReward(reward);

		if (success)
		{
			++SuccessFulActions;
		}

		++ActionsTaken;
		ResetAgentIfNotDone(); // agent is automatically reset if its done... so we only reset if not done to avoid double reset

		ActionWasTaken?.Invoke();

		if (requestOnStart)
		{
			// requestOnStart should probably be renamed, but basically we just assume movement automation
			RequestDecision();
		}
	}
	#endregion

	#region Other
	protected bool ResetAgentIfNotDone()
	{
		if (!IsDone())
		{
			AgentReset();
			return true;
		}
		print("DONE");
		return false;
	}
	public void SetTargetRandomly(float range)
	{
		Assert.IsTrue(range >= 0f && range <= 1f, "ERROR [" + name + ".SetTargetRandomly(range)]: \"range\" must be in the range (0, 1)");

		// Avoid setting the target to the center
		var extends = lamp.ViewPortExtends * range;

		do
		{
			initialTargetViewportPosition.x = Random.Range(-extends.x, extends.x);
			initialTargetViewportPosition.y = Random.Range(-extends.y, extends.y);

		} while (initialTargetViewportPosition.x == 0f && initialTargetViewportPosition.y == 0f && range != 0f);

		// Place the target into the world space throught the camera viewport
		initialTargetViewportPosition.z = targetZ;
		target.position = lamp.ViewportToWorldPoint(initialTargetViewportPosition);
		currentTargetViewportPosition = lamp.WorldToViewportPoint(target.position);
	}

	public void SetTarget(float x, float y)
	{
		x = Mathf.Clamp(x, -1f, 1f) * lamp.ViewPortExtends.x;
		y = Mathf.Clamp(y, -1f, 1f) * lamp.ViewPortExtends.y;

		initialTargetViewportPosition.x = x;
		initialTargetViewportPosition.y = y;
		initialTargetViewportPosition.z = targetZ;

		target.position = lamp.ViewportToWorldPoint(initialTargetViewportPosition);
		currentTargetViewportPosition = lamp.WorldToViewportPoint(target.position);
	}

	public void SetTarget(Vector2 position)
	{
		SetTarget(position.x, position.y);
	}

	public void SetServoPositionsAndUpdateState(int[] positions)
	{
		SetServoPositions(positions);
		SetInitialServoPositionsToCurrent();
		currentTargetViewportPosition = lamp.WorldToViewportPoint(target.position);
	}

	public void SetServoPositionsNormalizedAndUpdateState(float[] positions)
	{
		SetServoPositionsNormalized(positions);
		SetInitialServoPositionsToCurrent();
		currentTargetViewportPosition = lamp.WorldToViewportPoint(target.position);
	}

	protected void SetInitialServoPositionsToCurrent()
	{
		for (int i = 0; i < initialServoPositions.Length; ++i)
		{
			initialServoPositions[i] = lamp.GetServo(i).Offset;
		}
	}

	protected void SetServoPositions(int[] positions)
	{
		for (int i = 0; i < Lamp.NumberOfServos; ++i)
		{
			lamp.GetServo(i).SetOffset(positions[i]);
		}
	}

	protected void SetServoPositionsNormalized(float[] positions)
	{
		for (int i = 0; i < Lamp.NumberOfServos; ++i)
		{
			lamp.GetServo(i).SetOffsetNormalized(positions[i]);
		}
	}

	protected void SetRandomServoPositions()
	{
		Servo servo;

		for (int i = 0; i < Lamp.NumberOfServos; ++i)
		{
			servo = lamp.GetServo(i);
			servo.SetOffset(Random.Range(-servo.MaxOffset, servo.MaxOffset));
		}
	}

	public int[] GetServoPositions()
	{
		int[] positions = new int[Lamp.NumberOfServos];

		for (int i = 0; i < positions.Length; ++i)
		{
			positions[i] = lamp.GetServo(i).Offset;
		}

		return positions;
	}

	public void GetServoPositionsNonAlloc(int[] positions)
	{
		Assert.AreEqual(positions.Length, Lamp.NumberOfServos, "ERROR[" + name + ".GetServoPositionsNonAlloc(positions)]: \"positions\" must be of length " + Lamp.NumberOfServos + ".");

		for (int i = 0; i < Lamp.NumberOfServos; ++i)
		{
			positions[i] = lamp.GetServo(i).Offset;
		}
	}
	
	public void GetNormalizedServoPositionsNonAlloc(float[] positions)
	{
		Assert.AreEqual(positions.Length, Lamp.NumberOfServos, "ERROR[" + name + ".GetNormalizedServoPositionsNonAlloc(positions)]: \"positions\" must be of length " + Lamp.NumberOfServos + ".");

		for (int i = 0; i < Lamp.NumberOfServos; ++i)
		{
			positions[i] = lamp.GetServo(i).NormalizedOffset;
		}
	}

	protected int[] GetServosDeltaPosition()
	{
		var delta = new int[Lamp.NumberOfServos];

		for (int i = 0; i < delta.Length; ++i)
		{
			delta[i] = lamp.GetServo(i).Offset - initialServoPositions[i];
		}

		return delta;
	}
	#endregion
}

public enum EServoReset
{
	None,
	Initial,
	Random
}

public enum ETargetReset
{
	None,
	Random
}

public enum EActionType
{
	Instant,
	Gradual,
	Player
}

[System.Flags]
public enum ActionResultFlags
{
	None = 0x0,
	Success = 0x1,
	Terminal = 0x2
}