using UnityEngine;

public class ControlButtons : MonoBehaviour
{
public static void ButtonPushedDoTheThing(int buttonID)
    {
        switch (buttonID)
        {
            case 0:
                Debug.Log("ButtonID is 0. Did you forget to set the correct ButtonID?");
                break;
            case 1:
                Debug.Log("Control Button 1 Pressed");
                break;
            case 2:
                Debug.Log("Control Button 2 Pressed");
                break;
            case 3:
                Debug.Log("Control Button 3 Pressed");
                break;
            case 4:
                Debug.Log("Control Button 4 Pressed");
                break;
            case 5:
                Debug.Log("Control Button 5 Pressed");
                break;
            case 6:
                Debug.Log("Control Button 6 Pressed");
                break;
            case 7:
                Debug.Log("Control Button 7 Pressed");
                break;
            case 8:
                Debug.Log("Control Button 8 Pressed");
                break;
            case 9:
                Debug.Log("Control Button 9 Pressed");
                break;
            case 10:
                Debug.Log("Control Button 10 Pressed");
                break;
            case 11:
                Debug.Log("Control Button 11 Pressed");
                break;
            case 12:
                Debug.Log("Control Button 12 Pressed");
                break;
            case 13:
                Debug.Log("Control Button 13 Pressed");
                break;
            case 14:
                Debug.Log("Control Button 14 Pressed");
                break;
            case 15:
                Debug.Log("Control Button 15 Pressed");
                break;
            case 16:
                Debug.Log("Control Button 16 Pressed");
                break;
            default:
                Debug.Log("Unknown Control Button Pressed");
                break;
        }
    }
}
