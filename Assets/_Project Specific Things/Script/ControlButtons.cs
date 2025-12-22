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
                SpawnManagerTest.instance.UpdateMaxEnemies(1);
                break;
            case 2:
                SpawnManagerTest.instance.UpdateMaxEnemies(-1);
                break;
            case 3:
                SpawnManagerTest.instance.UpdateMaxEnemies(10);
                break;
            case 4:
                SpawnManagerTest.instance.UpdateMaxEnemies(-10);
                break;
            case 5:
                SpawnManagerTest.instance.UpdateMaxEnemies(100);
                break;
            case 6:
                SpawnManagerTest.instance.UpdateMaxEnemies(-100);
                break;
            case 7:
                SpawnManagerTest.instance.UpdateSpawnsPerSecond(1);
                Debug.Log("Control Button 7 Pressed");
                break;
            case 8:
                SpawnManagerTest.instance.UpdateSpawnsPerSecond(-1);
                Debug.Log("Control Button 8 Pressed");
                break;
            case 9:
                SpawnManagerTest.instance.UpdateSpawnsPerSecond(10);
                Debug.Log("Control Button 9 Pressed");
                break;
            case 10:
                SpawnManagerTest.instance.UpdateSpawnsPerSecond(-10);
                Debug.Log("Control Button 10 Pressed");
                break;
            case 11:
                SpawnManagerTest.instance.UpdateSpawnsPerSecond(100);
                Debug.Log("Control Button 11 Pressed");
                break;
            case 12:
                SpawnManagerTest.instance.UpdateSpawnsPerSecond(-100);
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
            case 99:
                Debug.Log("Control Button 99 Pressed");
                SpawnManagerTest.instance.ToggleSpawning();
                break;
            default:
                Debug.Log("Unknown Control Button Pressed");
                break;
        }
    }
}
