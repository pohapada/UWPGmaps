﻿using GoogleMapsUnofficial.View;
using GoogleMapsUnofficial.View.OnMapControls;
using GoogleMapsUnofficial.ViewModel.OfflineMapDownloader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GoogleMapsUnofficial
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static Frame RootFrame { get; set; }
        public static Grid Grid { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            this.Loaded += MainPage_Loaded;
            RootFrame = Fr;
            Grid = Gr;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Fr.Navigate(typeof(View.MapView));
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            if (Fr.CanGoBack) Fr.GoBack();
            else App.Current.Exit();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Fr.Navigate(typeof(View.OfflineMapDownloader.MapDownloaderView));
        }

        private async void DirBtn_Click(object sender, RoutedEventArgs e)
        {
            var dir = Gr.FindName("DirectionUC") as View.DirectionsControls.DirectionsMainUserControl;
            if (dir == null)
            {
                if (MapView.MapControl.FindName("OrDesSelector") as DraggablePin == null)
                {
                    Gr.Children.Add(new View.DirectionsControls.DirectionsMainUserControl() { Name = "DirectionUC", VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left });
                    await new MessageDialog("Navigate the point added on your screen to the Origin point and click on it. Then move it to Destination point and click on it again. Select navigation mode from top left menu and hit the navigate button.").ShowAsync();
                }
            }
            else
                Gr.Children.Remove(dir);
        }

        private void Fr_Navigated(object sender, NavigationEventArgs e)
        {
            if (Fr.CanGoBack)
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            else SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

        }

        private void FavPlaces_Click(object sender, RoutedEventArgs e)
        {
            var FavPlaces = Gr.FindName("FavPlaces") as SavedPlacesUserControl;
            if (FavPlaces == null)
            {
                if (MapView.MapControl.FindName("OrDesSelector") as DraggablePin == null)
                {
                    Gr.Children.Add(new SavedPlacesUserControl() { Name = "FavPlaces", Width = 180, VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Left });
                }
            }
            else
            {
                Gr.Children.Remove(FavPlaces);
            }
        }
    }
}
