using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomPicker.Components
{
    public class DropDownButton<T> : ContentView
    {
        #region Bindable properties

        public static readonly BindableProperty ClearButtonSourceProperty =
            BindableProperty.Create(
                nameof(ClearButtonSource),
                typeof(ImageSource),
                typeof(DropDownButton<T>),
                ImageSource.FromFile("clear_icon.png")
            );

        public static readonly BindableProperty DisplayMemberProperty =
            BindableProperty.Create(nameof(DisplayMember), typeof(string), typeof(DropDownButton<T>));

        public static readonly BindableProperty ItemsSourceProperty =
                            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable<T>), typeof(DropDownButton<T>));

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(DropDownButton<T>));

        public static readonly BindableProperty PlaceholderColorProperty =
            BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(DropDownButton<T>), Colors.Gray);

        public static readonly BindableProperty PlaceholderTextProperty =
            BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(DropDownButton<T>), "Select...");

        public static readonly BindableProperty RequiredColorProperty =
            BindableProperty.Create(nameof(RequiredColor), typeof(Color), typeof(DropDownButton<T>), Colors.Red);

        public static readonly BindableProperty SelectedColorProperty =
            BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(DropDownButton<T>), Colors.Green);

        public static readonly BindableProperty SelectedItemProperty =
                BindableProperty.Create(nameof(SelectedItem), typeof(T), typeof(DropDownButton<T>),
                defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

        public ImageSource ClearButtonSource
        {
            get => (ImageSource)GetValue(ClearButtonSourceProperty);
            set => SetValue(ClearButtonSourceProperty, value);
        }

        public string DisplayMember
        {
            get => (string)GetValue(DisplayMemberProperty);
            set => SetValue(DisplayMemberProperty, value);
        }

        public IEnumerable<T> ItemsSource
        {
            get => (IEnumerable<T>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public Color PlaceholderColor
        {
            get => (Color)GetValue(PlaceholderColorProperty);
            set => SetValue(PlaceholderColorProperty, value);
        }

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public Color RequiredColor
        {
            get => (Color)GetValue(RequiredColorProperty);
            set => SetValue(RequiredColorProperty, value);
        }

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public T SelectedItem
        {
            get => (T)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        #endregion Bindable properties

        #region Variables

        private readonly Border _border;
        private readonly ImageButton _clearButton;
        private readonly Label _label;
        private bool _hasPulsedRequired = false;
        private bool _wasCleared = false;

        #endregion Variables

        public DropDownButton()
        {
            _label = new Label
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(10)
            };

            _clearButton = new ImageButton
            {
                BackgroundColor = Colors.Transparent,
                WidthRequest = 10,
                HeightRequest = 10,
                Padding = 10,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                IsVisible = false
            };

            _clearButton.SetBinding(Image.SourceProperty, new Binding(nameof(ClearButtonSource), source: this));

            _clearButton.Clicked += (s, e) =>
            {
                SelectedItem = default;
                _wasCleared = true;
                UpdateVisuals();
            };

            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                HeightRequest = 44
            };

            grid.Add(_label, 0, 0);
            grid.Add(_clearButton, 1, 0);

            _border = new Border
            {
                Stroke = Colors.Gray,
                StrokeThickness = 1,
                Padding = new Thickness(0, 5, 5, 5),
                StrokeShape = new RoundRectangle { CornerRadius = 10 },
                Content = grid
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) =>
            {
                var popup = new PickerControlView<T>
                {
                    ItemsSource = ItemsSource,
                    ItemTemplate = ItemTemplate
                };

                var result = await Application.Current.MainPage.ShowPopupAsync<T>(popup);
                if (!result.WasDismissedByTappingOutsideOfPopup && result.Result is T selected)
                {
                    SelectedItem = selected;
                    _label.Text = DisplayMember != null
                        ? selected?.GetType().GetProperty(DisplayMember)?.GetValue(selected)?.ToString()
                        : selected?.ToString();

                    _wasCleared = false;
                    UpdateVisuals();
                }
            };

            _label.GestureRecognizers.Add(tapGesture);
            _border.GestureRecognizers.Add(tapGesture);

            Content = _border;
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is DropDownButton<T> control)
            {
                control.UpdateVisuals();
            }
        }

        private async Task AnimateRequiredPulse()
        {
            await _border.ScaleTo(1.05, 150, Easing.CubicOut);
            await _border.ScaleTo(1.0, 150, Easing.CubicIn);
        }

        private void UpdateVisuals()
        {
            bool hasSelection = SelectedItem != null;

            if (!hasSelection && _wasCleared)
            {
                _label.Text = PlaceholderText;
                _label.TextColor = RequiredColor;
                _border.Stroke = RequiredColor;
                _clearButton.IsVisible = false;

                if (!_hasPulsedRequired)
                {
                    _hasPulsedRequired = true;
                    _ = AnimateRequiredPulse();
                }
            }
            else if (!hasSelection)
            {
                _label.Text = PlaceholderText;
                _label.TextColor = PlaceholderColor;
                _border.Stroke = PlaceholderColor;
                _clearButton.IsVisible = false;
            }
            else
            {
                var text = DisplayMember != null
                    ? SelectedItem?.GetType().GetProperty(DisplayMember)?.GetValue(SelectedItem)?.ToString()
                    : SelectedItem?.ToString();

                _label.Text = text;
                _label.TextColor = SelectedColor;
                _border.Stroke = SelectedColor;
                _clearButton.IsVisible = true;

                // Reset pulse state when a valid selection is made
                _hasPulsedRequired = false;
            }
        }
    }
}