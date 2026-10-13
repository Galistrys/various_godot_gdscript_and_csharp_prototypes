using Godot;
using System;

public class HUD : VBoxContainer
{
    Label _interactableNameLabel;
    Label _actionLabel;
    TextureRect _crosshairTexture;
    const string CROSSHAIR_PATH = "res://Assets/Textures/Pointers/";
    const string CROSSHAIR_EXTENSION = ".png";

    public static HUD Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;

        _interactableNameLabel = GetNode<Label>("InteractableName");
        _actionLabel = GetNode<Label>("Action");
        _crosshairTexture = GetNode<TextureRect>("CrossHairBox/CrossHair");

        _interactableNameLabel.Text = "";
        _actionLabel.Text = "";
    }

    public void ShowInteractableInfo(IInteractable interactable)
    {
        _interactableNameLabel.Text = interactable.InteractableName;
        
        if (Singletons.MainManager.GameManager.Player.IsHoldingItem)
        {
            _actionLabel.Text = GenerateUseText();
        }
        else
        {
            ChangeCrosshair(interactable.Action);
            _actionLabel.Text = GenerateActionText(interactable);
        }
    }

    public void HideInteractableInfo(bool resetCross)
    {
        _interactableNameLabel.Text = "";
        _actionLabel.Text = "";
        if (resetCross && !Singletons.MainManager.GameManager.Player.IsHoldingItem)
            ChangeCrosshair(EntityAction.INVALID);
    }

    private string GenerateActionText(IInteractable interactable)
    {
        return "Press [LMB] to " + interactable.Action;
    }

    private string GenerateUseText()
    {
        return "Press [LMB] to use";
    }

    private void ChangeCrosshair(EntityAction action)
    {
        _crosshairTexture.Texture = ResourceLoader.Load<Texture>(CROSSHAIR_PATH + action.ToString() + CROSSHAIR_EXTENSION);
    }

    public void ChangeCrosshair(string itemId)
    {
        _crosshairTexture.Texture = ResourceLoader.Load<Texture>(Item.ITEM_DATA_PATH + itemId + "/" + Item.ITEM_ICON_NAME + Item.ITEM_ICON_EXT);
    }
}
