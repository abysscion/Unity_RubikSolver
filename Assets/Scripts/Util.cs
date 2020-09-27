using UnityEngine;

public class Util
{
    public static RColor GetRColorByTag(string faceTag)
    {
        switch (faceTag)
        {
            case "Red":
                return RColor.Red;
            case "Blue":
                return RColor.Blue;
            case "Green":
                return RColor.Green;
            case "White":
                return RColor.White;
            case "Yellow":
                return RColor.Yellow;
            case "Orange":
                return RColor.Orange;
            default:
                Debug.LogError(nameof(GetRColorByTag) + " got wrong color or no color at all! Argument: " + faceTag);
                return RColor.Error;
        }
    }
}
