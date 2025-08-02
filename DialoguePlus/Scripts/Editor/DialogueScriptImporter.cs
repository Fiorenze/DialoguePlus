using UnityEditor.AssetImporters;
using UnityEngine;

// Version = 1, Extension = "dlg"
[ScriptedImporter(1, "dlg")]
public class DialogueScriptImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        string text = System.IO.File.ReadAllText(ctx.assetPath);

        var textAsset = new TextAsset(text);

        ctx.AddObjectToAsset("Text", textAsset);
        ctx.SetMainObject(textAsset);
    }
}