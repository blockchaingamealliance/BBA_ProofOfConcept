using System.Collections.Generic;
using UnityEngine;
using B83.Win32;
using UnityEngine.UI;

/// <summary>
/// Used to enable drag and drop of a BBA on the window to load it easily.
/// </summary>
public class DragAndDrop : MonoBehaviour
{
    public Text txtFileName;

    // important to keep the instance alive while the hook is active.
    UnityDragAndDropHook hook;
    void OnEnable()
    {
        // must be created on the main thread to get the right thread id.
        hook = new UnityDragAndDropHook();
        hook.InstallHook();
        hook.OnDroppedFiles += OnFiles;
    }

    void OnDisable()
    {
        hook.UninstallHook();
    }

    void OnFiles(List<string> aFiles, POINT aPos)
    {
        // do something with the dropped file names. aPos will contain the 
        // mouse position within the window where the files has been dropped.

        string fileNameWithPath = aFiles[0];// string.Join("\n", aFiles.ToArray());
        txtFileName.text = "[" + fileNameWithPath + "]";
        //Debug.Log("Dropped " + aFiles.Count + " files at: " + aPos + "\n" + fileNameWithPath);

        // In this project aPos will not be used. It is however used in the Sandbox repo to control where the model is dropped.
        LoadZip.LoadZipFunc(fileNameWithPath, aPos.x, aPos.y);
    }
}