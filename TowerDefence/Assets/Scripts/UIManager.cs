using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text defenderCountText; // Reference to the UI text element for defender count

    private int currentDefenders = 0;

    public void UpdateDefenderCountText(int count)
    {
        currentDefenders = count;
        defenderCountText.text = "Defenders: " + currentDefenders;
    }
}






