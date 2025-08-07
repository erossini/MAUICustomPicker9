using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace CustomPicker.Components
{
    public class EntryBox : Microsoft.Maui.Controls.ContentView
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(EntryBox), default(string), BindingMode.TwoWay, propertyChanged: OnTextChanged);

        public static readonly BindableProperty PlaceholderTextProperty =
            BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(EntryBox), "Enter text...");

        public static readonly BindableProperty PlaceholderColorProperty =
            BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(EntryBox), Colors.Gray);

        public static readonly BindableProperty SelectedColorProperty =
            BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(EntryBox), Colors.Green);

        public static readonly BindableProperty RequiredColorProperty =
            BindableProperty.Create(nameof(RequiredColor), typeof(Color), typeof(EntryBox), Colors.Red);

        public static readonly BindableProperty ClearButtonSourceProperty =
            BindableProperty.Create(nameof(ClearButtonSource), typeof(ImageSource), typeof(EntryBox), ImageSource.FromFile("clear_icon.png"));

        public static readonly BindableProperty ValidationRuleProperty =
            BindableProperty.Create(
                nameof(ValidationRule),
                typeof(Func<string, bool>),
                typeof(EntryBox),
                null,
                propertyChanged: OnValidationRuleChanged);

        public static readonly BindableProperty MustNotEmptyProperty =
    BindableProperty.Create(
        nameof(MustNotEmpty),
        typeof(bool),
        typeof(EntryBox), // Replace with your actual control class name
        false,
        propertyChanged: OnMustNotEmptyChanged);

        public bool MustNotEmpty
        {
            get => (bool)GetValue(MustNotEmptyProperty);
            set => SetValue(MustNotEmptyProperty, value);
        }

        public Func<string, bool>? ValidationRule
        {
            get => (Func<string, bool>?)GetValue(ValidationRuleProperty);
            set => SetValue(ValidationRuleProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public Color PlaceholderColor
        {
            get => (Color)GetValue(PlaceholderColorProperty);
            set => SetValue(PlaceholderColorProperty, value);
        }

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public Color RequiredColor
        {
            get => (Color)GetValue(RequiredColorProperty);
            set => SetValue(RequiredColorProperty, value);
        }

        public ImageSource ClearButtonSource
        {
            get => (ImageSource)GetValue(ClearButtonSourceProperty);
            set => SetValue(ClearButtonSourceProperty, value);
        }

        private readonly Entry _entry;
        private readonly Border _border;
        private readonly ImageButton _clearButton;
        private bool _hasPulsedRequired = false;
        private bool _wasCleared = false;

        public EntryBox()
        {
            _entry = new BorderlessEntry
            {
                Placeholder = PlaceholderText,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Colors.Transparent
            };
            _entry.HandlerChanged += (s, e) => UpdateCursorColor(null);
            _entry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), source: this, mode: BindingMode.TwoWay));
            _entry.TextChanged += (s, e) =>
            {
                _wasCleared = false;
                UpdateVisuals();
            };

            _clearButton = new ImageButton
            {
                BackgroundColor = Colors.Transparent,
                WidthRequest = 10,
                HeightRequest = 10,
                Padding = 10,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };
            _clearButton.SetBinding(Image.SourceProperty, new Binding(nameof(ClearButtonSource), source: this));
            _clearButton.Clicked += (s, e) =>
            {
                Text = string.Empty;
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
                HeightRequest = 40
            };

            grid.Add(_entry, 0, 0);
            grid.Add(_clearButton, 1, 0);

            _border = new Border
            {
                Stroke = Colors.Gray,
                StrokeThickness = 1,
                Padding = new Thickness(5, 5, 5, 5),
                StrokeShape = new RoundRectangle { CornerRadius = 10 },
                Content = grid
            };

            Content = _border;
            UpdateVisuals();
        }

        private static void OnValidationRuleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is EntryBox entryBox)
            {
                entryBox.UpdateVisuals();
            }
        }

        private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is EntryBox box)
                box.UpdateVisuals();
        }

        private static void OnMustNotEmptyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is EntryBox control)
            {
                control.UpdateVisuals();
            }
        }

        private void UpdateCursorColor(Color? color)
        {
            if (color == null)
                color = SelectedColor;

#if ANDROID
    if (_entry.Handler is EntryHandler handler)
    {
        var editText = handler.PlatformView;
        editText.TextCursorDrawable?.SetTint(color.ToInt());
    }
#elif IOS
    if (_entry.Handler is EntryHandler handler)
    {
        handler.PlatformView.TintColor = color.ToPlatform();
    }
#elif WINDOWS
    if (_entry.Handler is EntryHandler handler)
    {
        //handler.PlatformView.CaretBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(color.ToWindowsColor());
    }
#endif
        }

        private void UpdateVisuals()
        {
            bool hasText = !string.IsNullOrWhiteSpace(Text);
            bool requiresValidation = MustNotEmpty || ValidationRule != null;
            bool isValid = ValidationRule?.Invoke(Text ?? string.Empty) ?? hasText;

            Color currentColor = requiresValidation
                ? (isValid ? SelectedColor : RequiredColor)
                : (hasText ? SelectedColor : PlaceholderColor);

            _entry.Placeholder = PlaceholderText;
            _entry.PlaceholderColor = currentColor;
            _entry.TextColor = currentColor;
            _border.Stroke = currentColor;

            _clearButton.Opacity = hasText ? 1 : 0;
            _clearButton.InputTransparent = !hasText;

            UpdateCursorColor(currentColor);

            if (requiresValidation && !isValid)
            {
                if (!_hasPulsedRequired)
                {
                    _hasPulsedRequired = true;
                    _ = AnimateRequiredPulse();
                }
            }
            else
                _hasPulsedRequired = false;
        }

        private async Task AnimateRequiredPulse()
        {
            await _border.ScaleTo(1.05, 150, Easing.CubicOut);
            await _border.ScaleTo(1.0, 150, Easing.CubicIn);
        }
    }
}