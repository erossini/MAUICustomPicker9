using CommunityToolkit.Mvvm.ComponentModel;
using CustomPicker.Components;
using CustomPicker.Interfaces;
using CustomPicker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CustomPicker.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly INavigationService _navigation;

        [ObservableProperty] public CultureInfo _selectedCulture;
        [ObservableProperty] public List<CultureInfo> _cultures;
        [ObservableProperty] public List<LanguageModel> _languages;
        [ObservableProperty] public LanguageModel _selectedLanguage;
        [ObservableProperty] public Func<string, bool> _textValidation;

        [ObservableProperty] public string _selectedText;
        [ObservableProperty] public string _selectedText1;
        [ObservableProperty] public string _selectedText2;

        [ObservableProperty] public List<MenuOption> _menuOptions;

        public MainPageViewModel(INavigationService navigation)
        {
            _navigation = navigation;

            Cultures = new List<CultureInfo>()
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("fr-FR"),
                    new CultureInfo("de-DE"),
                };

            Languages = new List<LanguageModel>() {
                new LanguageModel() { Name = "English", Code = "en" },
                new LanguageModel() { Name = "French", Code = "fr" },
                new LanguageModel() { Name = "German", Code = "de" },
            };

            TextValidation = (text) =>
            {
                // Example validation: check if the text is not empty
                return !string.IsNullOrWhiteSpace(text) && text.Length >= 2;
            };

            // text for a menu
            MenuOptions = new List<MenuOption>
            {
                new MenuOption {
                    Title = "Preferences",
                    Actions = new List<MenuAction> {
                        new MenuAction { Icon = "arrow_right.png", Command = new Command(() => NavigateTo("Preferences")) }
                    }
                },
                new MenuOption
                {
                    Title = "Dictionary",
                    Icon = "uk.png",
                    Actions = new List<MenuAction>
                    {
                        new MenuAction { Text = "Edit", Icon = "add.png", Command = new Command(() => NavigateTo("Edit")) },
                        new MenuAction { Text = "Remove", Icon = "clear_icon2.png", Command = new Command(() => NavigateTo("Remove")) }
                    }
                },
                new MenuOption {
                    Title = "Profile",
                    Actions = new List<MenuAction> {
                        new MenuAction { Icon = "arrow_right.png", Command = new Command(() => NavigateTo("Profile")) }
                    }
                },
                new MenuOption
                {
                    Title = "Notifications",
                    Icon = "bell.png",
                    ShowImageBorder = false,
                    Actions = new List<MenuAction>
                    {
                        new MenuAction { Icon = "arrow_right.png", Command = new Command(ConfigureNotifications) }
                    }
                }
            };
        }

        private void ConfigureNotifications(object obj)
        {
            _navigation.NavigateToAsync("NavPage");
        }

        void NavigateTo(string pageName)
        {
            // Navigation logic here
        }

        public ICommand RightImageCommand => new Command<string>(OnRightImageClicked);

        private void OnRightImageClicked(string text)
        {
            // Use the text from EntryBox
            Console.WriteLine($"User typed: {text}");
        }
    }
}