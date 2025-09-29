using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Class, Inherited = true)]
public class InfoHeaderClassAttribute : PropertyAttribute
{
    public readonly string message;

    public InfoHeaderClassAttribute(string message)
    {
        message = message.Replace("\\n", "\n"); // allow "\n" in strings
        this.message = message;
    }
}
