using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace MAUIBlurIssue
{
    public partial class MainPage : ContentPage
    {
        CustomGrid CustomGrid;
        Label label;
        int count = 1;
        int HeightRequest = 600;
        Android.Views.IWindowManager WindowManager;
        public MainPage()
        {
            InitializeComponent();
           
        }

        private void Button_Clicked(object? sender, EventArgs e)
        {
            CreateView();
        }

        private void View1_Clicked(object sender, EventArgs e)
        {
            CreateView();
        }
        private void CreateView()
        {
#if ANDROID
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                var button = new Button() { Text = "click" + count, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, WidthRequest = 200, BackgroundColor = Colors.Yellow, HeightRequest = HeightRequest, BorderWidth = 3, BorderColor = Colors.Green, TextColor = Colors.Black, FontSize = 20 };
                button.Clicked += Button_Clicked;
                CustomGrid = new CustomGrid();
                var tapgesture = new TapGestureRecognizer();
                tapgesture.Tapped += Tapgesture_Tapped;
                CustomGrid.GestureRecognizers.Add(tapgesture);
                CustomGrid.Children.Add(button);
                WindowManager = WindowHelper.GetPlatformWindow()!.WindowManager!;
                var WindowManagerLayoutParams = new Android.Views.WindowManagerLayoutParams();
                WindowManagerLayoutParams.Width = (int)(WindowHelper.Window!.Width * WindowHelper.Window.RequestDisplayDensity());
                WindowManagerLayoutParams.Height = (int)(WindowHelper.Window.Height * WindowHelper.Window.RequestDisplayDensity());
                WindowManagerLayoutParams.Format = Android.Graphics.Format.Translucent;
                WindowManagerLayoutParams.Flags = Android.Views.WindowManagerFlags.BlurBehind;
                WindowManagerLayoutParams.BlurBehindRadius = 21;
                var PlatformGrid = (Android.Views.ViewGroup)CustomGrid.ToPlatform(WindowHelper.Window.Handler!.MauiContext!);
                WindowManager!.AddView(PlatformGrid, WindowManagerLayoutParams);
                count++;
                HeightRequest -= 50;
            }
#endif
        }

        private void Tapgesture_Tapped(object? sender, TappedEventArgs e)
        {
            WindowManager.RemoveView((sender as CustomGrid)!.Handler!.PlatformView as Android.Views.ViewGroup);
            count--;
            HeightRequest += 50;
        }
    }

    public class CustomGrid : Grid
    {
        public CustomGrid()
        {
        
        }
    }

    public static class WindowHelper
    {
        public static IWindow Window => Application.Current!.Windows[Application.Current.Windows.Count - 1];

#if ANDROID
        public static Android.Views.Window? GetPlatformWindow()
        {
            if (Window != null && Window.Handler is WindowHandler windowHandler && windowHandler.PlatformView is Android.App.Activity platformActivity)
            {
                if (platformActivity == null || platformActivity.WindowManager == null
                    || platformActivity.WindowManager.DefaultDisplay == null)
                {
                    return null;
                }

                return platformActivity.Window;
            }

            return null;
        }
#endif

    }

}
