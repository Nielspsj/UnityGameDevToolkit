using UnityEngine;

public class InfoTextAttribute : PropertyAttribute
{
    public string text;

    public InfoTextAttribute(string text)
    {
        this.text = text;
    }
}
