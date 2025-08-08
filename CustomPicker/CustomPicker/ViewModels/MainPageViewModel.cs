using CommunityToolkit.Mvvm.ComponentModel;
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
        [ObservableProperty] public CultureInfo _selectedCulture;
        [ObservableProperty] public List<CultureInfo> _cultures;
        [ObservableProperty] public List<LanguageModel> _languages;
        [ObservableProperty] public LanguageModel _selectedLanguage;
        [ObservableProperty] public Func<string, bool> _textValidation;

        [ObservableProperty] public string _selectedText;
        [ObservableProperty] public string _selectedText1;
        [ObservableProperty] public string _selectedText2;
        public MainPageViewModel()
        {
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
        }

        public ICommand RightImageCommand => new Command<string>(OnRightImageClicked);

        private void OnRightImageClicked(string text)
        {
            // Use the text from EntryBox
            Console.WriteLine($"User typed: {text}");
        }
    }
}