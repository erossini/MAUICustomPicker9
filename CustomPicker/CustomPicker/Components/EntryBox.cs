using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        public static readonly BindableProperty RightImageSourceProperty =
            BindableProperty.Create(nameof(RightImageSource), typeof(ImageSource), typeof(EntryBox), 
                default(ImageSource), propertyChanged: OnRightImageSourceChanged);

        public static readonly BindableProperty RightImageCommandProperty =
            BindableProperty.Create(nameof(RightImageCommand), typeof(ICommand), typeof(EntryBox));

        public static readonly BindableProperty RightImageCommandParameterProperty =
            BindableProperty.Create(nameof(RightImageCommandParameter), typeof(object), typeof(EntryBox));

        public static readonly BindableProperty MultilineProperty =
            BindableProperty.Create(nameof(Multiline), typeof(bool), typeof(EntryBox), false,
                propertyChanged: OnMultilineChanged);

        public static readonly BindableProperty ShowCharacterCountProperty =
            BindableProperty.Create(nameof(ShowCharacterCount), typeof(bool), typeof(EntryBox), false,
                propertyChanged: OnShowCharacterCountChanged);

        public static readonly BindableProperty MinimumHeightProperty =
            BindableProperty.Create(nameof(MinimumHeight), typeof(double), typeof(EntryBox), 40.0);

        public static readonly BindableProperty MaximumHeightProperty =
            BindableProperty.Create(nameof(MaximumHeight), typeof(double), typeof(EntryBox), 100.0);

        public double MinimumHeight
        {
            get => (double)GetValue(MinimumHeightProperty);
            set => SetValue(MinimumHeightProperty, value);
        }

        public double MaximumHeight
        {
            get => (double)GetValue(MaximumHeightProperty);
            set => SetValue(MaximumHeightProperty, value);
        }


        public bool ShowCharacterCount
        {
            get => (bool)GetValue(ShowCharacterCountProperty);
            set => SetValue(ShowCharacterCountProperty, value);
        }

        public bool Multiline
        {
            get => (bool)GetValue(MultilineProperty);
            set => SetValue(MultilineProperty, value);
        }

        public object RightImageCommandParameter
        {
            get => GetValue(RightImageCommandParameterProperty);
            set => SetValue(RightImageCommandParameterProperty, value);
        }

        public ICommand RightImageCommand
        {
            get => (ICommand)GetValue(RightImageCommandProperty);
            set => SetValue(RightImageCommandProperty, value);
        }

        public ImageSource RightImageSource
        {
            get => (ImageSource)GetValue(RightImageSourceProperty);
            set => SetValue(RightImageSourceProperty, value);
        }

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

        private readonly Grid _grid;
        private readonly Border _border;
        private readonly ImageButton _clearButton;
        private readonly ImageButton _rightImageButton;
        private readonly Label _characterCountLabel;
        private readonly VerticalStackLayout _verticalLayout;

        private Entry _entry;
        private Editor _editor;
        private View _inputControl => Multiline ? _editor : _entry;

        private bool _hasPulsedRequired = false;
        private bool _wasCleared = false;

        #endregion Variables
        #region Events

        public event EventHandler RightImageClicked;

        #endregion

        public EntryBox()
        {
            _entry = new BorderlessEntry
            {
                Placeholder = PlaceholderText,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Colors.Transparent,
                Margin = new Thickness(3, 0, 0, 0),
                IsVisible = true
            };
            _entry.HandlerChanged += (s, e) => UpdateCursorColor(null);
            _entry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), source: this, mode: BindingMode.TwoWay));
            _entry.TextChanged += (s, e) =>
            {
                RightImageCommandParameter = e.NewTextValue;
                _wasCleared = false;
                UpdateVisuals();
            };

            _characterCountLabel = new Label
            {
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Start,
                TextColor = Colors.Gray,
                Text = "0 characters",
                Margin = new Thickness(5, 0, 0, 0)
            };
            _characterCountLabel.SetBinding(Label.IsVisibleProperty, new Binding(nameof(ShowCharacterCount), 
                source: this, mode: BindingMode.TwoWay));

            _editor = new Editor { 
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                BackgroundColor = Colors.Transparent,
                Margin = new Thickness(3, 0, 0, 0),
                IsVisible = false
            };
            _editor.HandlerChanged += (s, e) => UpdateCursorColor(null);
            _editor.SetBinding(Editor.TextProperty, new Binding(nameof(Text), source: this, mode: BindingMode.TwoWay));
            _editor.TextChanged += (s, e) =>
            {
                RightImageCommandParameter = e.NewTextValue;

                _characterCountLabel.Text = $"{_editor.Text?.Length ?? 0} characters";
                _characterCountLabel.IsVisible = ShowCharacterCount;

                _wasCleared = false;
                UpdateVisuals();
            };

            _clearButton = new ImageButton
            {
                BackgroundColor = Colors.Transparent,
                WidthRequest = 10,
                HeightRequest = 10,
                Padding = new Thickness(10,10,0,10),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start
            };
            _clearButton.SetBinding(Image.SourceProperty, new Binding(nameof(CurrentValidationIcon), source: this));
            _clearButton.Clicked += (s, e) =>
            {
                Text = string.Empty;
                _wasCleared = true;
                UpdateVisuals();
            };

            _rightImageButton = new ImageButton
            {
                BackgroundColor = Colors.Transparent,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start,
                WidthRequest = 10,
                HeightRequest = 10,
                Padding = new Thickness(0, 10, 0, 10),
                Margin = new Thickness(5, 0),
                Opacity = 0.5
            };
            _rightImageButton.Clicked += (s, e) =>
            {
                var parameter = RightImageCommandParameter ?? _entry?.Text;
                if (RightImageCommand?.CanExecute(parameter) == true)
                    RightImageCommand.Execute(parameter);

                OnRightImageClicked();
            };

            _grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                HeightRequest = 40
            };

            _verticalLayout = new VerticalStackLayout
            {
                Spacing = 4,
                Padding = new Thickness(0),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center
            };

            _verticalLayout.Children.Add(Multiline ? _editor : _entry);
            if (ShowCharacterCount)
            {
                _characterCountLabel.IsVisible = true;
                _verticalLayout.Children.Add(_characterCountLabel);
            }

            _grid.Add(_verticalLayout, 0, 0);
            _grid.Add(_clearButton, 1, 0);

            _border = new Border
            {
                Stroke = Colors.Gray,
                StrokeThickness = 1,
                Padding = new Thickness(5, 5, 5, 5),
                StrokeShape = new RoundRectangle { CornerRadius = 10 },
                Content = _grid
            };

            Content = _border;
            UpdateVisuals();
        }

        double GetPlatformLineHeight()
        {
            if (DeviceInfo.Platform == DevicePlatform.macOS)
                return 22;
            if (DeviceInfo.Platform == DevicePlatform.iOS)
                return 22;
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return 20;
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
                return 24;
            return 20; // Default fallback
        }

        private static void OnMustNotEmptyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is EntryBox control)
                control.UpdateVisuals();
        }

        private static void OnMultilineChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is EntryBox box)
                box.SwitchInputControl();
        }

        private static void OnShowCharacterCountChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is EntryBox box)
                box.SwitchInputControl();
        }

        private void SwitchInputControl()
        {
            if (_entry == null || _editor == null || _grid == null)
                return;

            _verticalLayout.Children.Clear();
            _verticalLayout.Children.Add(Multiline ? _editor : _entry);

            _entry.IsVisible = !Multiline;
            _editor.IsVisible = Multiline;

            if (ShowCharacterCount)
            {
                _characterCountLabel.IsVisible = true;
                _verticalLayout.Children.Add(_characterCountLabel);
            }

            UpdateVisuals();
        }

        protected virtual void OnRightImageClicked()
        {
            RightImageClicked?.Invoke(this, EventArgs.Empty);
        }

        private static void OnRightImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is EntryBox box)
                box.UpdateVisuals();
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

            var input = _inputControl;
            if (input is Entry entry)
            {
                entry.Placeholder = PlaceholderText;
                entry.PlaceholderColor = currentColor;
                entry.TextColor = currentColor;
            }
            else if (input is Editor editor)
            {
                editor.TextColor = currentColor;
                editor.HeightRequest = MaximumHeight - (ShowCharacterCount ? GetPlatformLineHeight() : 0);
            }

            _border.Stroke = currentColor;

            _clearButton.Opacity = hasText ? 1 : 0;
            _clearButton.InputTransparent = !hasText;

            if (RightImageSource != null && !_grid.Children.Contains(_rightImageButton))
            {
                _rightImageButton.Source = RightImageSource;
                _grid.Add(_rightImageButton, 2, 0);
            }

            _rightImageButton.Source = RightImageSource;
            _rightImageButton.Opacity = RightImageSource != null ? 1 : 0;
            _rightImageButton.IsEnabled = IsValid == true;
            _rightImageButton.Opacity = IsValid == true ? 1 : 0.5;

            _grid.HeightRequest = Multiline ? MaximumHeight : MinimumHeight;
            _grid.MaximumHeightRequest = Multiline ? MaximumHeight : MinimumHeight;

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