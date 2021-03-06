﻿using HeroExplorer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HeroExplorer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Character> marvelCharacters { get; set; }
        public ObservableCollection<ComicResult> marvelComics { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            marvelCharacters = new ObservableCollection<Character>();
            marvelComics = new ObservableCollection<ComicResult>();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(
                new Uri("ms-appx:///VoiceCommandDictionary.xml"));
            await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager
                .InstallCommandDefinitionsFromStorageFileAsync(storageFile);

            Refresh();
        }

        public async void Refresh()
        {
            MyProgressRing.IsActive = true;
            MyProgressRing.Visibility = Visibility.Visible;

            marvelCharacters.Clear();
            while (marvelCharacters.Count < 10)
            {
                Task task = MarvelFacade.populateMarvelCharactersAsync(marvelCharacters);
                await task;
            }
            MyProgressRing.IsActive = false;
            MyProgressRing.Visibility = Visibility.Collapsed;
        }

        private async void MasterListview_ItemClick(object sender, ItemClickEventArgs e)
        {
            MyProgressRing.IsActive = true;
            MyProgressRing.Visibility = Visibility.Visible;

            ComicDetailDescription.Text = "";
            ComicDetailNameTextBlock.Text = "";
            ComicDetailImage.Source = null;

            var selectedCharacter = (Character) e.ClickedItem;
            DetailNameTextBlock.Text = selectedCharacter.name;
            DetailDescription.Text = selectedCharacter.description;

            var largeImage = new BitmapImage();
            Uri uri = new Uri(selectedCharacter.thumbnail.large, UriKind.Absolute);
            largeImage.UriSource = uri;
            DetailImage.Source = largeImage;

            marvelComics.Clear();
            await MarvelFacade.populateMarvelComicsAsync(marvelComics,selectedCharacter.id);
            MyProgressRing.IsActive = false;
            MyProgressRing.Visibility = Visibility.Collapsed;
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void ComicsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedComic = (ComicResult) e.ClickedItem;
            ComicDetailNameTextBlock.Text = selectedComic.title;
            if (selectedComic.description!=null)
            {
                ComicDetailDescription.Text = selectedComic.description;
            }

            var largeImage = new BitmapImage();
            Uri uri = new Uri(selectedComic.thumbnail.large, UriKind.Absolute);
            largeImage.UriSource = uri;
            ComicDetailImage.Source = largeImage;

        }
    }
}
