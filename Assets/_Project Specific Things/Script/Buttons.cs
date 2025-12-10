using UnityEngine;

public class Buttons : MonoBehaviour
{
    [SerializeField] int buttonID;

    public void ButtonPressed()
    {
        ControlButtons.ButtonPushedDoTheThing(buttonID);
    }
}
