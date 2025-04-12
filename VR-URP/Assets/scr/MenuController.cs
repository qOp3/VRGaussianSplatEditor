using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    public GameObject menu;  
    public InputActionProperty thumbstickPress; // Assign in Inspector

    private bool isMenuVisible = false;

    void Update()
    {
        if(isMenuVisible)
        {
            ShowMenu();
        }
        else
        {
            HideMenu();
        }


        if (thumbstickPress.action.WasPressedThisFrame())
        {
            isMenuVisible = !isMenuVisible;
        }

        if (thumbstickPress.action.WasReleasedThisFrame())
        {
            Debug.Log("Thumbstick Released!");
        }
    }

    void ShowMenu()
    {
        menu.SetActive(true);
    }

    void HideMenu()
    {
        menu.SetActive(false);
    }
}
