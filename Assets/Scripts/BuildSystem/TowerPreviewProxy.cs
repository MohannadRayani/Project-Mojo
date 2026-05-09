using UnityEngine;

public class TowerPreviewProxy : MonoBehaviour
{
    private GameObject towerToBuild;
    private TowerPreview realPreview;

    public void SetupTowerPreview(GameObject newTowerToBuild)
    {
        towerToBuild = newTowerToBuild;
        gameObject.SetActive(false);
    }

    public void ShowPreview(bool showPreview, Vector3 previewPosition)
    {
        EnsureRealPreviewExists();
        realPreview.ShowPreview(showPreview, previewPosition);
    }

    private void EnsureRealPreviewExists()
    {
        if (realPreview != null)
            return;

        realPreview = gameObject.AddComponent<TowerPreview>();
        realPreview.SetupTowerPreview(towerToBuild);
    }
}