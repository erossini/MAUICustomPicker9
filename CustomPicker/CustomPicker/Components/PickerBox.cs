using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomPicker.Components
{
    public class PickerBox<T> : ContentView
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable<T>), typeof(PickerBox<T>), 
                default(IEnumerable<T>), propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(T), typeof(PickerBox<T>), default(T), BindingMode.TwoWay);

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(PickerBox<T>), string.Empty);

        public static readonly BindableProperty IsValidProperty =
            BindableProperty.Create(nameof(IsValid), typeof(bool?), typeof(PickerBox<T>), null, propertyChanged: OnValidationChanged);

        public static readonly BindableProperty DisplayMemberPathProperty =
    BindableProperty.Create(nameof(DisplayMemberPath), typeof(string), typeof(PickerBox<T>), default(string));

        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        public IEnumerable<T> ItemsSource
        {
            get => (IEnumerable<T>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public T SelectedItem
        {
            get => (T)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public bool? IsValid
        {
            get => (bool?)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }

        private readonly Picker picker;
        private readonly Image validationIcon;

        public PickerBox()
        {
            picker = new Picker
            {
                Title = Placeholder,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            picker.SelectedIndexChanged += (s, e) =>
            {
                if (picker.SelectedIndex >= 0 && ItemsSource is IList<T> list)
                    SelectedItem = list[picker.SelectedIndex];
            };

            picker.SetBinding(Picker.ItemsSourceProperty, new Binding(nameof(ItemsSource), source: this));
            picker.SetBinding(Picker.SelectedItemProperty, new Binding(nameof(SelectedItem), BindingMode.TwoWay, source: this));

            validationIcon = new Image
            {
                WidthRequest = 20,
                HeightRequest = 20,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(5, 0)
            };

            var layout = new Grid
            {
                ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            }
            };
            layout.Add(picker);
            layout.Add(validationIcon, 1, 0);

            var border = new Border
            {
                Stroke = Colors.Gray,
                StrokeThickness = 1,
                Padding = 5,
                Content = layout
            };

            Content = border;
            UpdateValidationIcon();
        }

        private static void OnValidationChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is PickerBox<T> box)
                box.UpdateValidationIcon();
        }

        private void UpdateValidationIcon()
        {
            validationIcon.Source = IsValid switch
            {
                true => "clear_icon_valid.png",
                false => "clear_icon_invalid.png",
                _ => "clear_icon_unknown.png"
            };
        }
        
        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is PickerBox<T> box)
                box.UpdateValidationIcon();
        }
    }
}