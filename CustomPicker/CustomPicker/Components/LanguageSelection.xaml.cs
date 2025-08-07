using CommunityToolkit.Maui.Extensions;
using System.Collections;
using System.Globalization;

namespace CustomPicker.Components;

public partial class LanguageSelection : ContentView
{
    #region Bindable Properties

    public static readonly BindableProperty ItemTemplateProperty =
    BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(LanguageSelection));

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(LanguageSelection), defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty ItemSourceProperty = 
        BindableProperty.Create(
            propertyName: nameof(ItemSource),
            returnType: typeof(IEnumerable),
            declaringType: typeof(LanguageSelection),
            defaultBindingMode: BindingMode.OneWay);

    public IEnumerable ItemSource
    {
        get => (IEnumerable)GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);
    }

    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public object SelectedItem
    {
        get => (object)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    #endregion

    public LanguageSelection()
    {
        InitializeComponent();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var popup = new PickerControlView<CultureInfo>
        {
            ItemsSource = ItemSource,
            ItemTemplate = ItemTemplate
        };

        var result = await Application.Current.MainPage.ShowPopupAsync<CultureInfo>(popup);

        if (result.WasDismissedByTappingOutsideOfPopup)
        {
            // popup dismissed by tapping outside
            lblDropDown.Text = $"Popup dismissed by tapping outside";
        }
        else if (result.Result is CultureInfo c)
        {
            // selected culture received
            lblDropDown.Text = $"Selected culture: {c.DisplayName}";

            // Update the SelectedItem property
            SelectedItem = c;
        }
        else
        {
            // no culture detected
            lblDropDown.Text = $"No selected culture";

            // Clear the SelectedItem if no culture is selected
            SelectedItem = null;
        }
    }
}