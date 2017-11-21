using UnityEditor;

public class DisableAutoimportMaterials : AssetPostprocessor
{
    public void OnPreprocessModel()
    {
        var modelImporter = (ModelImporter)assetImporter;
        modelImporter.importMaterials = false;
    }
}
