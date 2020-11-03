using System;
using UnityEngine;

namespace Util
{
    public static class Tools
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
    
        public static string RotationCommandToText((RSide, RotationType) command)
        {
            var (rubikSide, rotationType) = command;
            string txt;
            switch (rubikSide)
            {
                case RSide.Front:
                    txt = "F";
                    break;
                case RSide.Left:
                    txt = "L";
                    break;
                case RSide.Right:
                    txt = "R";
                    break;
                case RSide.Down:
                    txt = "D";
                    break;
                case RSide.Up:
                    txt = "U";
                    break;
                case RSide.Back:
                    txt = "B";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (rotationType)
            {
                case RotationType.Clockwise:
                    txt += "";
                    break;
                case RotationType.CounterClockwise:
                    txt += "'";
                    break;
                case RotationType.Halfturn:
                    txt += "2";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return txt;
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
}
