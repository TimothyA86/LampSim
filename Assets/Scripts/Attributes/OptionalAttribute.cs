using UnityEngine;

public class OptionalAttribute : PropertyAttribute
{
	public string field;
	public object value;

	public OptionalAttribute(string field, object value)
	{
		this.field = field;
		this.value = value;
	}
}