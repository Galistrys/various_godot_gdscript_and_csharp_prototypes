using Godot;
using System;

public class ReaderGUIState : GUIState
{
    GUIManager _guiManager;
    Reader _reader;

    string _itemId;

    bool _instantMode = false;

    public ReaderGUIState(string itemId)
    {
        _guiManager = Singletons.MainManager.GameManager.GUIManager;

        if (_guiManager.PeekState() is CrosshairGUIState)
        {
            _instantMode = true;
            _guiManager.PushState(new BackgroundGUIState());
            _guiManager.Journal.Visible = true;
        }
        
        _itemId = itemId;
        _guiManager = Singletons.MainManager.GameManager.GUIManager;
        _reader = _guiManager.Journal.Reader;
    }

    public override void AtStart()
    {
        _reader.Clean();
        _reader.ShowContent(_itemId);
        _reader.Visible = true;
    }

    public override void OnResume()
    {
        
    }

    
    public override void OnPause()
    {
        
    }

    public override void AtEnd()
    {
        _reader.Visible = false;
        _reader.Clean();
        if (_instantMode)
        {
            _guiManager.Journal.Visible = false;
        }
    }

    public override void OnEscapePress()
    {
        _guiManager.PopState();
    }
}
