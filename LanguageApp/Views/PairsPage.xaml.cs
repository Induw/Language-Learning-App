namespace LanguageApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

public partial class PairsPage : ContentPage
{
    private Random random = new();
    private Dictionary<string, string> WordPairs = new()
        {
            { "Hej", "Hello" }, { "Ja", "Yes" }, { "Nej", "No" }, { "Tack", "Thank you" },
            { "F�rl�t", "Sorry" }, { "God morgon", "Good morning" }, { "God natt", "Good night" },
            { "Hej d�", "Goodbye" }, { "V�n", "Friend" }, { "Familj", "Family" },
            { "K�rlek", "Love" }, { "Mat", "Food" }, { "Vatten", "Water" }, { "Bil", "Car" },
            { "Hus", "House" }
        };
    private List<string> CorrectMessages = new()
        {
            "Great job! Keep going!",
            "Well done!",
            "Awesome!",
            "You're doing amazing!",
            "Keep it up!",
            "Nice work!",
            "You're on fire!"
        };

    private Dictionary<Button, string> ButtonPairs = new();
    private Button SelectedLeftButton = null;
    private Button SelectedRightButton = null;
    private bool isGridFrozen = false;

    public PairsPage()
    {
        InitializeComponent();
        LoadNewPairs();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        var selectedLanguage = Preferences.Get("SelectedLanguage", "sv");
        Title = $"Word Match in - {GetLanguageFullName(selectedLanguage)}";
    }
    private void LoadNewPairs()
    {
        PairsGrid.Children.Clear();
        ButtonPairs.Clear();

        var randomPairs = WordPairs.OrderBy(x => Guid.NewGuid()).Take(5).ToList();
        var leftWords = randomPairs.Select(p => p.Key).OrderBy(x => Guid.NewGuid()).ToList();
        var rightWords = randomPairs.Select(p => p.Value).OrderBy(x => Guid.NewGuid()).ToList();

        for (int i = 0; i < leftWords.Count; i++)
        {
            var leftButton = CreateWordButton(leftWords[i], isLeftColumn: true);
            var rightButton = CreateWordButton(rightWords[i], isLeftColumn: false);

            PairsGrid.Add(leftButton, 0, i);
            PairsGrid.Add(rightButton, 1, i);
        }
    }

    private Button CreateWordButton(string text, bool isLeftColumn)
    {
        var button = new Button
        {
            Text = text,
            BackgroundColor = Colors.White,
            TextColor = Colors.Black,
            BorderColor = Colors.Gray,
            BorderWidth = 1,
            CornerRadius = 10,
            FontSize = 18,
            Padding = new Thickness(10),
            WidthRequest = 120

        };

        button.Clicked += (sender, args) => HandleSelection((Button)sender, isLeftColumn);
        ButtonPairs[button] = text;
        return button;
    }

    private void HandleSelection(Button button, bool isLeftColumn)
    {
        if (isGridFrozen) 
        {
            return;
        }
        if (isLeftColumn)
        {
            if (SelectedLeftButton != null)
            {
                ResetButtonStyle(SelectedLeftButton);
            }
            SelectedLeftButton = button;
        }
        else 
        {
            if (SelectedRightButton != null)
            {
                ResetButtonStyle(SelectedRightButton);
            }
            SelectedRightButton = button;
        }
        button.BorderColor = Colors.DarkOliveGreen;
        button.BorderWidth = 2;
        button.BackgroundColor = Color.FromArgb("#94D2BD") ;
        button.TextColor = Color.FromArgb("#004850");
        button.FontAttributes = FontAttributes.Bold;
        if (SelectedLeftButton != null && SelectedRightButton != null)
        {
            ValidatePair();
        }
    }

    private void ValidatePair()
    {
        isGridFrozen = true;
        string leftText = ButtonPairs[SelectedLeftButton];
        string rightText = ButtonPairs[SelectedRightButton];

        if (WordPairs.ContainsKey(leftText) && WordPairs[leftText] == rightText)
        {
            ErrorMessage.Text = CorrectMessages[random.Next(CorrectMessages.Count)]; ;
            ErrorMessage.TextColor = Colors.DarkGreen;

            this.Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(0.7), () =>
            {
                PairsGrid.Remove(SelectedLeftButton);
                PairsGrid.Remove(SelectedRightButton);
                SelectedLeftButton = null;
                SelectedRightButton = null;
                ErrorMessage.Text = " ";
                isGridFrozen = false;
                if (!PairsGrid.Children.Any())
                {
                    LoadNewPairs();
                }
            });
        }
        else
        {
            SelectedLeftButton.BorderColor = Colors.Red;
            SelectedLeftButton.BackgroundColor = Colors.MistyRose;
            SelectedLeftButton.BorderWidth = 2;
            SelectedRightButton.BorderColor = Colors.Red;
            SelectedRightButton.BackgroundColor = Colors.MistyRose;
            SelectedRightButton.BorderWidth = 2;
            SelectedLeftButton.WidthRequest = 120 ;
            SelectedRightButton.WidthRequest = 120;
            ErrorMessage.Text = "Try again! You'll get it next time!";
            ErrorMessage.TextColor = Colors.Maroon;
            ErrorMessage.IsVisible = true;

            this.Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(1), () =>
            {
                ResetButtonStyle(SelectedLeftButton);
                ResetButtonStyle(SelectedRightButton);
                SelectedLeftButton = null;
                SelectedRightButton = null;
                ErrorMessage.Text = " ";
                isGridFrozen = false;
            });

        }
    }
    private void ResetButtonStyle(Button button)
    {
        if (button != null)
        {
            button.BackgroundColor = Colors.White;
            button.BorderColor = Colors.Gray;
            button.TextColor = Colors.Black;
            button.BorderWidth = 1;
            button.FontAttributes = FontAttributes.None;

        }
    }
    private string GetLanguageFullName(string languageCode)
    {
        return languageCode switch
        {
            "sv" => "Swedish",
            "no" => "Norwegian",
            "fi" => "Finnish",
            "da" => "Danish",
            "is" => "Icelandic",
            _ => "Swedish"
        };
    }
}
