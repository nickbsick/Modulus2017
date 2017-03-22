﻿using EaglesNestMobileApp.Core.Model;
using EaglesNestMobileApp.Core.Model.Food;
using GalaSoft.MvvmLight;
using System.Collections.Generic;

namespace EaglesNestMobileApp.Core.ViewModel.DiningViewModels
{
    public class FourWindsFragmentViewModel : ViewModelBase
    {
        /* All the items being served in Four Winds         */
        private List<FourWindsItem> _fourWinds;
        public FourWindsMenu FourWindsMenuItems
        {
            get { return _fourWindsMenuItems; }
            set { Set(() => FourWindsMenuItems, ref _fourWindsMenuItems, value); }
        }

        /* The FourWinds menu to be used by the view        */
        private FourWindsMenu _fourWindsMenuItems;
        public List<FourWindsItem> FourWinds
        {
            get { return _fourWinds; }
            set { Set(() => FourWinds, ref _fourWinds, value); }
        }

        ///* Command to be binded to the refresh event in the view */
        //private RelayCommand _refreshCommand;
        //public RelayCommand RefreshCommand => _refreshCommand ??
        //    (_refreshCommand = new RelayCommand(async () => await RefreshMenusAsync()));

        ///* Singleton instance of the database                    */
        //private readonly IAzureService Database;

        //public DiningViewModel(IAzureService database)
        //{
        //    Database = database;
        //}

        ///* Refresh the menus, pulling off of the remote db.                */
        ///* There needs to be something that indicates to the user          */
        ///* that the page is being refreshed like in a swipe refresh layout */
        //public async Task RefreshMenusAsync()
        //{
        //    try
        //    {
        //        /* Initialize the localDb if not already present and sync  */
        //        await Database.InitLocalStore();
        //        await Database.SyncAsync(pullData: true);

        //        /* Get all the items for the dining facilities              */
        //        FourWinds = await Database.GetFourWindsItemsAsync();
        //        Varsity = await Database.GetVarsityItemsAsync();
        //        GrabAndGo = await Database.GetGrabAndGoItemsAsync();

        //        /* Format the dining menus for the separate views          */
        //        GetDiningMenus();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("An error occurred in retrieving the menus:");
        //        Debug.WriteLine($"{ex.Message} in {ex.Source}");
        //    }
        //}

        //public void GetDiningMenus()
        //{
        //    /* Reset all the lists so that we're not adding to existing items */
        //    FourWindsMenuItems = new FourWindsMenu();
        //    VarsityMenuItems = new VarsityMenu();
        //    GrabAndGoMenuItems = new GrabAndGoMenu();

        //    /* Format the Four Winds dining menus                           */
        //    foreach (var item in FourWinds)
        //    {
        //        FourWindsMenuItems.AddItem(item);
        //    }

        //    /* Format the Varsity dining menus                              */
        //    foreach (var item in Varsity)
        //    {
        //        VarsityMenuItems.AddItem(item);
        //    }

        //    /* Format the Grab And Go dining menus                          */
        //    foreach (var item in GrabAndGo)
        //    {
        //        GrabAndGoMenuItems.AddItem(item);
        //    }
        //}

    }
}
