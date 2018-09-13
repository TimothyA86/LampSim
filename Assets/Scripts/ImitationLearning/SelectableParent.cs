using UnityEngine;
using UnityEngine.UI;

public class SelectableParent : MonoBehaviour
{
	private Selectable[] children;

	protected virtual void Awake()
	{
		children = GetComponentsInChildren<Selectable>();
	}

	public void SetChildrenInteractable(bool value)
	{
		foreach (var child in children)
		{
			child.interactable = value;
		}
	}
}
