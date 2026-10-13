using Godot;
using System;

public class ToDoGUIState : GUIState
{
    GUIManager _guiManager;
    ToDo _todoReader;

    public ToDoGUIState()
    {
        _guiManager = Singletons.MainManager.GameManager.GUIManager;
        _todoReader = _guiManager.Journal.ToDo;
    }

    public override void AtStart()
    {
        _todoReader.UpdateContent();
        _todoReader.Visible = true;
    }

    public override void OnResume()
    {
        
    }

    
    public override void OnPause()
    {
        
    }

    public override void AtEnd()
    {
        _todoReader.Visible = false;
        _todoReader.CleanContent();
    }

    public override void OnEscapePress()
    {
        _guiManager.PopState();
    }
}
