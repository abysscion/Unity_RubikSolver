using UnityEngine;
using RSide = RubickSide.RSide;

public static class Util
{
    public static RubickColor GetRColorByTag(string faceTag)
    {
        switch (faceTag)
        {
            case "Red":
                return RubickColor.Red;
            case "Blue":
                return RubickColor.Blue;
            case "Green":
                return RubickColor.Green;
            case "White":
                return RubickColor.White;
            case "Yellow":
                return RubickColor.Yellow;
            case "Orange":
                return RubickColor.Orange;
            default:
                Debug.LogError(nameof(GetRColorByTag) + " got wrong color or no color at all! Argument: " + faceTag);
                return RubickColor.Error;
        }
    }
    
    public static (RotationType, RSide) TextToRotationCommand(string text)
    {
        var type = RotationType.Halfturn;
        var side = RSide.Front;

        switch (text)
        {
            case "F": case "B": case "R": case "L": case "U": case "D":
                type = RotationType.Clockwise;
                break;
            case "F'": case "B'": case "R'": case "L'": case "U'": case "D'":
                type = RotationType.CounterClockwise;
                break;
            case "F2": case "B2": case "R2": case "L2": case "U2": case "D2":
                type = RotationType.Halfturn;
                break;
            default:
                Debug.LogError("Can not resolve rotation command from text: [" + text + "]");
                return (type, side);
        }
        
        switch (text)
        {
            case "F": case "F2": case "F'":
                side = RSide.Front;
                break;
            case "B": case "B2": case "B'":
                side = RSide.Back;
                break;
            case "L": case "L2": case "L'":
                side = RSide.Left;
                break;
            case "R": case "R2": case "R'":
                side = RSide.Right;
                break;
            case "U": case "U2": case "U'":
                side = RSide.Up;
                break;
            case "D": case "D2": case "D'":
                side = RSide.Down;
                break;
            default:
                Debug.LogError("Can not resolve rotation command from text: [" + text + "]");
                return (type, side);
        }
        return (type, side);
    }
}
