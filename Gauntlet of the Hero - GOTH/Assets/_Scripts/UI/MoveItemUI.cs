using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoveItemUI : MonoBehaviour 
{
    public TextMeshProUGUI moveInfoText;
    public Toggle moveToggle;

    // Čuvamo ID poteza da bi znali šta je igrač kliknuo
    [HideInInspector] public string moveId; 
}