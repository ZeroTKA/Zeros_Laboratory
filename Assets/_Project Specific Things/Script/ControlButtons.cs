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
            default:
                Debug.Log("Unknown Control Button Pressed");
                break;
        }
    }
}
