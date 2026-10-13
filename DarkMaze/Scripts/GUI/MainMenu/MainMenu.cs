using Godot;
using System;
using System.Collections.Generic;

public class MainMenu : Control
{
	Stack<MenuGUIState> _mainGuiStateStack;

    public HBoxContainer MainSection { get; private set; }
    public NewGameSection NewGameSection { get; private set; }
    public LoadGameSection LoadGameSection { get; private set; }
    public CenterContainer SettingsSection { get; private set; }
    public CenterContainer AboutSection { get; private set; }
	public ConfirmationDialog ExitConfirmationDialog { get; private set; }

    public override void _Ready()
    {
		Singletons.ResetSingletons();

		MenuGUIState.MainMenu = this;
        _mainGuiStateStack = new Stack<MenuGUIState>();

        MainSection = GetNode<HBoxContainer>("MainSection");
        NewGameSection = GetNode<NewGameSection>("NewGameSection");
        LoadGameSection = GetNode<LoadGameSection>("LoadGameSection");
        SettingsSection = GetNode<CenterContainer>("SettingsSection");
        AboutSection = GetNode<CenterContainer>("AboutSection");
		ExitConfirmationDialog = GetNode<ConfirmationDialog>("ExitConfirmationDialog");

		PushState(new MainMenuGUIState());
    }

	public void PushState(MenuGUIState state)
	{
		var peekState = PeekState();
		if (peekState != null)
		{
			Logger.Log("Pause MenuGUIState: " + peekState.GetType().ToString());
			peekState.OnPause();
		}

		Logger.Log("Push MenuGUIState: " + state.GetType().ToString());

		state.AtStart();
		_mainGuiStateStack.Push(state);
	}

	public MenuGUIState PeekState()
	{
		if (_mainGuiStateStack.Count > 0)
			return _mainGuiStateStack.Peek();
		else
			return null;
	}

	public void PopState()
	{
		if (_mainGuiStateStack.Count > 0)
		{
			Logger.Log("Pop MenuGUIState: " + PeekState().GetType().ToString());
			_mainGuiStateStack.Pop().AtEnd();
			PeekState().OnResume();
		}
	}

	public override void _Process(float delta)
	{
		var currentState = PeekState();
		if (currentState != null)
		{
			if (Input.IsActionJustPressed("ui_cancel"))
			{
				Logger.Log("Send OnEscapePress to: " + currentState.GetType().ToString());
				currentState.OnEscapePress();
			}
		}
	}

	public void OnNewGameButtonPressed()
	{
		PushState(new NewGameMenuGUIState());
	}

	public void OnLoadGameButtonPressed()
	{
		PushState(new LoadGameMenuGUIState());
	}

	public void OnSettingsButtonPressed()
	{
		Logger.Log("OnSettingsButtonPressed");
	}

	public void OnAboutButtonPressed()
	{
		Logger.Log("OnAboutButtonPressed");
	}

	public void OnExitGameButtonPressed()
	{
		Logger.Log("OnExitGameButtonPressed");
		PushState(new ExitMenuGUIState());
	}

	public void OnExitConfirmationDialogConfirmed()
	{
		Logger.Log("OnExitConfirmationDialogConfirmed");
		GetTree().Quit(0);
	}

	public void OnExitConfirmationDialogPopupHide()
	{
		Logger.Log("OnExitConfirmationDialogPopupHide");
		PopState();
	}
}
