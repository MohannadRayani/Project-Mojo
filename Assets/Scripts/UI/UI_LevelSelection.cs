using UnityEngine;

public class UI_LevelSelection : MonoBehaviour
{
    private void MakeButtonsClickable(bool canClick)
    {
        LevelButton_Tile[] levelButtons = FindObjectsOfType<LevelButton_Tile>();

        foreach (var btn in levelButtons)
        {

            btn.CheckIfLevelUnlocked();
            btn.EnableClickOnButton(canClick);
        }
    }

    private void OnEnable()
    {
        MakeButtonsClickable(true);
    }

    private void OnDisable()
    {
        MakeButtonsClickable(false);
    }
}
