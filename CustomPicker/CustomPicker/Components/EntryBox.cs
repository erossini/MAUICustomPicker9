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
        #region Bindle Properties

        public static readonly BindableProperty ClearValidIconSourceProperty =
            BindableProperty.Create(nameof(EntryBox), typeof(ImageSource), typeof(EntryBox),
                ImageSource.FromFile("clear_icon_valid.png"));

        public static readonly BindableProperty ClearInvalidIconSourceProperty =
            BindableProperty.Create(nameof(EntryBox), typeof(ImageSource), typeof(EntryBox),
                ImageSource.FromFile("clear_icon_invalid.png"));

        public static readonly BindableProperty ClearUnknownIconSourceProperty =
            BindableProperty.Create(nameof(EntryBox), typeof(ImageSource), typeof(EntryBox),
                ImageSource.FromFile("clear_icon_unknown.png"));

        public static readonly BindableProperty IsValidProperty =
            BindableProperty.Create(nameof(IsValid), typeof(bool?), typeof(EntryBox),
                null, BindingMode.OneWayToSource);

        public static readonly BindableProperty MustNotEmptyProperty =
            BindableProperty.Create( nameof(MustNotEmpty), typeof(bool), typeof(EntryBox),
                false, propertyChanged: OnMustNotEmptyChanged);

        public static readonly BindableProperty PlaceholderColorProperty =
            BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(EntryBox), Colors.Gray);

        public static readonly BindableProperty PlaceholderTextProperty =
            BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(EntryBox), "Enter text...");

        public static readonly BindableProperty RequiredColorProperty =
            BindableProperty.Create(nameof(RequiredColor), typeof(Color), typeof(EntryBox), Colors.Red);

        public static readonly BindableProperty SelectedColorProperty =
            BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(EntryBox), Colors.Green);

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(EntryBox), default(string), BindingMode.TwoWay, 
                propertyChanged: OnTextChanged);

        public static readonly BindableProperty ValidationRuleProperty =
            BindableProperty.Create(nameof(ValidationRule), typeof(Func<string, bool>), typeof(EntryBox),
                null, propertyChanged: OnValidationRuleChanged);

        public ImageSource CurrentValidationIcon =>
            IsValid == true ? ClearValidIconSource : IsValid == false ? ClearInvalidIconSource : ClearUnknownIconSource;

        public ImageSource ClearValidIconSource
        {
            get => (ImageSource)GetValue(ClearValidIconSourceProperty);
            set => SetValue(ClearValidIconSourceProperty, value);
        }

        public ImageSource ClearInvalidIconSource
        {
            get => (ImageSource)GetValue(ClearInvalidIconSourceProperty);
            set => SetValue(ClearInvalidIconSourceProperty, value);
        }

        public ImageSource ClearUnknownIconSource
        {
            get => (ImageSource)GetValue(ClearUnknownIconSourceProperty);
            set => SetValue(ClearUnknownIconSourceProperty, value);
        }

        private bool? _isValid;
        public bool? IsValid
        {
            get => _isValid;
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged(nameof(IsValid));
                    OnPropertyChanged(nameof(CurrentValidationIcon));
                }
            }
        }

        public bool MustNotEmpty
        {
            get => (bool)GetValue(MustNotEmptyProperty);
            set => SetValue(MustNotEmptyProperty, value);
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

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Func<string, bool>? ValidationRule
        {
            get => (Func<string, bool>?)GetValue(ValidationRuleProperty);
            set => SetValue(ValidationRuleProperty, value);
        }

        #endregion Bindle Properties

        #region Variables

        private readonly Border _border;
        private readonly ImageButton _clearButton;
        private readonly Entry _entry;
        private bool _hasPulsedRequired = false;
        private bool _wasCleared = false;

        #endregion Variables

        public EntryBox()
        {
            _entry = new BorderlessEntry
            {
                Placeholder = PlaceholderText,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Colors.Transparent,
                Margin = new Thickness(3, 0, 0, 0)
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
            _clearButton.SetBinding(Image.SourceProperty, new Binding(nameof(CurrentValidationIcon), source: this));
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

        private static void OnMustNotEmptyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is EntryBox control)
                control.UpdateVisuals();
        }

        private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is EntryBox box)
                box.UpdateVisuals();
        }

        private static void OnValidationRuleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is EntryBox entryBox)
                entryBox.UpdateVisuals();
        }

        private async Task AnimateRequiredPulse()
        {
            await _border.ScaleTo(1.05, 150, Easing.CubicOut);
            await _border.ScaleTo(1.0, 150, Easing.CubicIn);
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
            bool isValid = ValidationRule?.Invoke(Text ?? string.Empty) ?? (MustNotEmpty ? hasText : true);

            IsValid = isValid;

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

            if (requiresValidation && !isValid && !_hasPulsedRequired)
            {
                _hasPulsedRequired = true;
                _ = AnimateRequiredPulse();
            }
            else if (isValid)
            {
                _hasPulsedRequired = false;
            }
        }
    }
}