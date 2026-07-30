using UnityEngine;
using TMPro;

public class UIStateReport : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI UIState;
    public void ChangeTextForUIStatus()
    {
        UIState.text = $"UI State is: {UIManager.Instance.CurrentState.ToString()}";
    }
}
