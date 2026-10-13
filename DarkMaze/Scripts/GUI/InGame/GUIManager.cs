using Godot;
using System;
using System.Collections.Generic;

public class GUIManager : Control
{
    public HUD HUD { get; private set; }
    public Control UIControl { get; private set; }

    Inventory _inventory = null;
    public Inventory Inventory
    {
        get
        {
            if (_inventory == null)
            {
                _inventory = GetNodeOrNull<Inventory>("Center/UI/Inventory");
            }
            return _inventory;
        }
    }

    Journal _journal = null;
    public Journal Journal
    {
        get
        {
            if (_journal == null)
            {
                _journal = GetNodeOrNull<Journal>("Center/UI/Journal");
            }
            return _journal;
        }
    }

    PauseMenu _pauseMenu = null;
    public PauseMenu PauseMenu
    {
        get
        {
            if (_pauseMenu == null)
            {
                _pauseMenu = GetNodeOrNull<PauseMenu>("Center/UI/PauseMenu");
            }
            return _pauseMenu;
        }
    }

    Stack<GUIState> _guiStateStack;

    public override void _Ready()
    {
        _guiStateStack = new Stack<GUIState>();

        HUD = GetNode<HUD>("Center/HUD");
        UIControl = GetNode<Control>("Center/UI");
        _guiStateStack.Push(new CrosshairGUIState());
    }

    public override void _ExitTree()
    {
        _guiStateStack.Clear();
    }

    public void LateReady()
    {
        Journal.Notes.AddMissingNotes();
        Journal.Diary.AddMissingEntries();
    }

    public void PushState(GUIState state)
    {
        var peekState = PeekState();
		if (peekState != null)
		{
			Logger.Log("Pause GUIState: " + peekState.GetType().ToString());
			peekState.OnPause();
		}

		Logger.Log("Push GUIState: " + state.GetType().ToString());

		state.AtStart();
		_guiStateStack.Push(state);
    }

    public GUIState PeekState()
    {
        if (_guiStateStack.Count > 0)
            return _guiStateStack.Peek();
        else
            return null;
    }

    public void PopState()
    {
        if (_guiStateStack.Count > 0)
        {
            Logger.Log("Pop GUIState: " + PeekState().GetType().ToString());
            _guiStateStack.Pop().AtEnd();
            PeekState().OnResume();
        }
    }

    public bool HasState<T>() where T : GUIState
    {
        foreach (GUIState state in _guiStateStack.ToArray())
        {
            if (state is T)
            {
                return true;
            }
        }

        return false;
    }

    public override void _Process(float delta)
    {
        var currentState = PeekState();
        currentState?._Process(delta);
        if (currentState != null)
        {
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                Logger.Log("Send OnEscapePress to: " + currentState.GetType().ToString());
                currentState.OnEscapePress();
            }
            else if (Input.IsActionJustPressed("interaction"))
                currentState.OnInteractionEvent(true);
            else if (Input.IsActionJustReleased("interaction"))
                currentState.OnInteractionEvent(false);
            else if (Input.IsActionJustPressed("push"))
                currentState.OnPushEvent(true);
            else if (Input.IsActionJustReleased("push"))
                currentState.OnPushEvent(false);
        }
    }
}
