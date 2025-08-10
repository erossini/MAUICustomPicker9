using CommunityToolkit.Maui.Extensions;
using CustomPicker.Components;
using CustomPicker.Interfaces;
using CustomPicker.ViewModels;
using System.Globalization;

namespace CustomPicker
{
    public partial class MainPage : ContentPage
    {
        MainPageViewModel vm;
        int count = 0;

        public MainPage(INavigationService navigation)
        {
            InitializeComponent();

            vm = new MainPageViewModel(navigation);
            BindingContext = vm;
        }

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            var popup = new PickerControlView<CultureInfo>
            {
                ItemsSource = new List<CultureInfo>()
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("fr-FR"),
                    new CultureInfo("de-DE"),
                },
                ItemTemplate = new DataTemplate(() =>
                {
                    Label label = new Label();
                    label.HeightRequest = 40;
                    label.TextColor = Colors.Black;
                    label.SetBinding(Label.TextProperty, "DisplayName");
                    return label;
                }),
            };

            var result = await this.ShowPopupAsync<CultureInfo>(popup);

            if (result.WasDismissedByTappingOutsideOfPopup)
            {
                // popup dismissed by tapping outside
                lblDropDown.Text = $"Popup dismissed by tapping outside";
            }
            else if (result.Result is CultureInfo c)
            {
                // selected culture received
                lblDropDown.Text = $"Selected culture: {c.DisplayName}";
            }
            else
            {
                // no culture detected
                lblDropDown.Text = $"No selected culture";
            }

            var t = vm.SelectedCulture;
        }
    }
}
