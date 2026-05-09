using UnityEngine;

public interface IUICommand
{
    void Execute();
}

public class BuildTowerCommand : IUICommand
{
    private readonly UI_BuildButton buildButton;

    public BuildTowerCommand(UI_BuildButton buildButton)
    {
        this.buildButton = buildButton;
    }

    public void Execute()
    {
        buildButton.ConfirmTowerBuild();
    }
}

public class SelectBuildButtonCommand : IUICommand
{
    private readonly UI_BuildButtonsHolder buildButtonsHolder;
    private readonly int buttonIndex;

    public SelectBuildButtonCommand(UI_BuildButtonsHolder buildButtonsHolder, int buttonIndex)
    {
        this.buildButtonsHolder = buildButtonsHolder;
        this.buttonIndex = buttonIndex;
    }

    public void Execute()
    {
        buildButtonsHolder.SelectNewButton(buttonIndex);
    }
}

public class RotatePreviewCommand : IUICommand
{
    private readonly Transform target;
    private readonly float angle;

    public RotatePreviewCommand(Transform target, float angle)
    {
        this.target = target;
        this.angle = angle;
    }

    public void Execute()
    {
        if (target == null)
            return;

        target.Rotate(0, angle, 0);
        target.GetComponent<ForwardAttackDisplay>()?.UpdateLines();
    }
}