using UnityEditor.AssetImporters;
using UnityEngine;

// This tells Unity:
// Version = 1, Extension = "dlg"
[ScriptedImporter(1, "dlg")]
public class DialogueScriptImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        // Read the file’s raw text
        string text = System.IO.File.ReadAllText(ctx.assetPath);

        // Create a new TextAsset with that text
        var textAsset = new TextAsset(text);

        // Register it as the main object
        ctx.AddObjectToAsset("Text", textAsset);
        ctx.SetMainObject(textAsset);
    }
}