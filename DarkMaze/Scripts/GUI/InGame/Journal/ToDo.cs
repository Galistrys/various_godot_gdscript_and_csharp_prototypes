using Godot;
using Godot.Collections;
using System;

public class ToDo : PanelContainer
{
    RichTextLabel _contentRichTextLabel;

    public override void _Ready()
    {
        _contentRichTextLabel = GetNode<RichTextLabel>("HBoxContainer/VBoxContainer/Content");
    }

    public void UpdateContent()
    {
        Logger.Log("Updating ToDo list.");

        string result = "";
        Dictionary<string, int> mainQuestsInProgress = Singletons.MainManager.GameManager.GetMainQuestsInProgress();

        foreach (var mainQuestPair in mainQuestsInProgress)
        {
            Logger.Log($"Adding {mainQuestPair.Key} quest to ToDo list.");

            MainQuest mainQuest = Singletons.Database.GetMainQuest(mainQuestPair.Key);

            result += $"{Translate(mainQuest.Name).ToUpper()}\n";

            for (int i = 0; i <= mainQuestPair.Value; i++)
            {
                if (i < mainQuestPair.Value)
                    result += $"[indent][color=gray]{Translate(mainQuest.SubQuests[i].Name)}[/color][/indent]\n";
                else
                    result += $"[indent]-> {Translate(mainQuest.SubQuests[i].Name)}[/indent]\n";
            }

            result += "\n";
        }

        _contentRichTextLabel.BbcodeText = result;

        GD.Print("@@@@@@@@@ CONTENT:    @@@@@@@@@");
        GD.Print(_contentRichTextLabel.BbcodeText.ToString());
        GD.Print("@@@@@@@@@ END CONTENT @@@@@@@@@");
    }

    public void CleanContent()
    {
        _contentRichTextLabel.BbcodeText = "";
    }

    string Translate(string text)
    {
        return TranslationServer.Translate(text);
    }
}
