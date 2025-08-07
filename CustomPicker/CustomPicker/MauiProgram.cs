using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CustomPicker.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;

namespace CustomPicker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitCore()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Remove border for custom Entry on each platform
            EntryHandler.Mapper.AppendToMapping(nameof(BorderlessEntry), (handler, view) =>
            {
#if ANDROID
            handler.PlatformView.Background = null;
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#elif WINDOWS
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
#endif
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            EntryHandler.Mapper.AppendToMapping("CustomEntryStyling", (handler, view) =>
            {
#if ANDROID
            // Remove underline and set cursor color
            handler.PlatformView.Background = null;
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
            handler.PlatformView.TextCursorDrawable?.SetTint(Android.Graphics.Color.Black);
#elif IOS
            // Remove border and set cursor tint
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
            handler.PlatformView.TintColor = UIKit.UIColor.Red; // example color
#elif WINDOWS
            // Remove border and set caret brush
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            //handler.PlatformView.CaretBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Green);
#endif
            });

            return builder.Build();
        }
    }
}
