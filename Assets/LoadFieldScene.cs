using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LoadFieldScene : MonoBehaviour
{
    public void StartGame(InputAction.CallbackContext ctx)
    {
        Debug.Log("Start button pressed");
        if (!ctx.performed) return;          // only trigger once on press
        SceneManager.LoadScene("FieldScene"); // scene name must match exactly
    }
}
