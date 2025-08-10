using CustomPicker.Converters;
using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CustomPicker.Components
{
    public class MenuAction
    {
        public string? Text { get; set; }
        public string? Icon { get; set; }
        public bool ShowImageBorder { get; set; } = true;

        public ICommand? Command { get; set; }
        public object? CommandParameter { get; set; }

        public Action? Clicked { get; set; }
    }

    public class MenuOption
    {
        public string? Title { get; set; }
        public string? Icon { get; set; }
        public bool ShowImageBorder { get; set; } = true;
        public List<MenuAction> Actions { get; set; } = new();
    }

    public class MenuListView : ContentView
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable<MenuOption>), typeof(MenuListView));

        public static readonly BindableProperty ActionTextColorProperty =
            BindableProperty.Create(
                nameof(ActionTextColor),
                typeof(Color),
                typeof(MenuListView),
                Colors.White); // default color

        public Color ActionTextColor
        {
            get => (Color)GetValue(ActionTextColorProperty);
            set => SetValue(ActionTextColorProperty, value);
        }

        public IEnumerable<MenuOption> ItemsSource
        {
            get => (IEnumerable<MenuOption>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public MenuListView()
        {
            var collectionView = new CollectionView
            {
                ItemTemplate = new DataTemplate(() =>
                {
                    var titleLayout = new HorizontalStackLayout
                    {
                        Spacing = 8,
                        VerticalOptions = LayoutOptions.Center
                    };

                    var titleIconBorder = new Border
                    {
                        Stroke = Colors.Gray,
                        StrokeThickness = 1,
                        BackgroundColor = Colors.Transparent,
                        WidthRequest = 28,
                        HeightRequest = 28,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(6) }
                    };
                    // Bind Content dynamically
                    titleIconBorder.SetBinding(Border.ContentProperty, new Binding("Icon", converter: new IconToImageConverter()));

                    // Also bind IsVisible to ensure layout exclusion
                    titleIconBorder.SetBinding(Border.IsVisibleProperty, new Binding("Icon", converter: new HasIconConverter()));

                    titleIconBorder.SetBinding(Border.StrokeThicknessProperty, new MultiBinding
                    {
                        Bindings =
                        {
                            new Binding("Icon"),
                            new Binding("ShowImageBorder")
                        },
                        Converter = new IconAndVisibilityConverter()
                    });

                    var titleLabel = new Label
                    {
                        FontSize = 16,
                        VerticalOptions = LayoutOptions.Center
                    };
                    titleLabel.SetBinding(Label.TextProperty, "Title");

                    // Show icon only if it's set
                    titleLayout.Children.Add(titleIconBorder);
                    titleLayout.Children.Add(titleLabel);

                    var actionsLayout = new HorizontalStackLayout
                    {
                        Spacing = 8,
                        VerticalOptions = LayoutOptions.Center
                    };

                    // Bindable layout for dynamic buttons
                    BindableLayout.SetItemTemplate(actionsLayout, new DataTemplate(() =>
                    {
                        var icon = new Image
                        {
                            WidthRequest = 16,
                            HeightRequest = 16,
                            VerticalOptions = LayoutOptions.Center
                        };
                        icon.SetBinding(Image.SourceProperty, "Icon");

                        var label = new Label
                        {
                            FontSize = 14,
                            VerticalOptions = LayoutOptions.Center,
                            TextColor = Colors.Blue
                        };
                        label.SetBinding(Label.TextProperty, "Text");
                        label.SetBinding(Label.TextColorProperty, new Binding
                        {
                            Source = this,
                            Path = nameof(ActionTextColor),
                            Mode = BindingMode.OneWay
                        });

                        var content = new HorizontalStackLayout
                        {
                            Spacing = 4,
                            Children = { icon, label }
                        };

                        var tapGesture = new TapGestureRecognizer();
                        tapGesture.SetBinding(TapGestureRecognizer.CommandProperty, "Command");
                        tapGesture.SetBinding(TapGestureRecognizer.CommandParameterProperty, "CommandParameter");
                        tapGesture.Tapped += (s, e) =>
                        {
                            if (content.BindingContext is MenuAction action)
                                action.Clicked?.Invoke();
                        };
                        content.GestureRecognizers.Add(tapGesture);

                        return new Border
                        {
                            Stroke = Colors.Transparent,
                            Padding = new Thickness(8, 4),
                            Content = content
                        };
                    }));
                    actionsLayout.SetBinding(BindableLayout.ItemsSourceProperty, "Actions");

                    var grid = new Grid
                    {
                        Padding = new Thickness(16, 12),
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = GridLength.Auto }
                        }
                    };

                    grid.Add(titleLayout, 0, 0);
                    grid.Add(actionsLayout, 1, 0);

                    var separator = new BoxView
                    {
                        HeightRequest = 1,
                        BackgroundColor = Colors.LightGray,
                        HorizontalOptions = LayoutOptions.Fill
                    };

                    var tapGesture = new TapGestureRecognizer();
                    tapGesture.Tapped += async (s, e) =>
                    {
                        if (grid.BindingContext is MenuOption option && option.Actions?.Count == 1)
                        {
                            await grid.ScaleTo(0.97, 50, Easing.CubicOut);
                            await grid.ScaleTo(1.0, 50, Easing.CubicIn);

                            var action = option.Actions[0];
                            if (action.Command?.CanExecute(action.CommandParameter) == true)
                                action.Command.Execute(action.CommandParameter);
                            action.Clicked?.Invoke();
                        }
                    };
                    grid.GestureRecognizers.Add(tapGesture);

                    return new VerticalStackLayout
                    {
                        Spacing = 0,
                        Children = { grid, separator }
                    };
                })
            };

            collectionView.SetBinding(ItemsView.ItemsSourceProperty, new Binding(nameof(ItemsSource), source: this));

            Content = new Border
            {
                Stroke = Colors.LightGray,
                StrokeThickness = 1,
                Padding = 0,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(12) },
                Content = collectionView
            };
        }
    }
}