// BatchRename.cs
// Unity Editor extension that allows batch renaming for GameObjects in Hierarchy
// Via Alan Thorn (TW: @thorn_alan)

using UnityEngine;
using UnityEditor;
using System.Collections;

public class BatchRename : ScriptableWizard
{

    /// <summary>
    /// Base name
    /// </summary>
    public string BaseName = "MyObject_";

    /// <summary>
    /// Start count
    /// </summary>
    public int StartNumber = 0;

    /// <summary>
    /// Increment
    /// </summary>
    public int Increment = 1;

    [MenuItem("Custom/Batch Rename GameObjects")]

    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Batch Rename", typeof(BatchRename), "Rename");
    }

    /// <summary>
    /// Called when the window first appears
    /// </summary>
    void OnEnable()
    {
        UpdateSelectionHelper();
    }

    /// <summary>
    /// Function called when selection changes in scene
    /// </summary>
    void OnSelectionChange()
    {
        UpdateSelectionHelper();
    }

    /// <summary>
    /// Update selection counter
    /// </summary>
    void UpdateSelectionHelper()
    {

        helpString = "";

        if (Selection.objects != null)
            helpString = "Number of objects selected: " + Selection.objects.Length;
    }


    /// <summary>
    /// Rename
    /// </summary>
    void OnWizardCreate()
    {

        // If selection is empty, then exit
        if (Selection.objects == null)
            return;

        // Current Increment
        int PostFix = StartNumber;

        // Cycle and rename
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            Selection.objects[i].name = BaseName + PostFix;
            PostFix += Increment;

            GameObject go = (GameObject)Selection.objects[i];

            go.transform.SetSiblingIndex(i);
        }
    }
}