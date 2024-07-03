using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public Image icon;
    public Text nameText;
    public void UpdateDisplay(Sprite sprite, string text)
    {
        icon.sprite = sprite;
        nameText.text = text;
    }
}