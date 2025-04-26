using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NationButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject overlay;
    [SerializeField] Image colorOverlay;
    private int nationIndex;

    public void SetNation()
    {
        PlayerManager.Instance.TryChangePlayer(nationIndex);
    }
    public void SetUpButton(Color color, int index)
    {
        colorOverlay.color = color;
        nationIndex = index;
        text.text = $"Player {index + 1}";
    }
    public void UnlockNation(bool unlock)
    {
        overlay.SetActive(!unlock);
    }
}