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
            case 3:
                Debug.Log("Go Time Baby");
                WaveManagerTest.instance.Wave1();
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
