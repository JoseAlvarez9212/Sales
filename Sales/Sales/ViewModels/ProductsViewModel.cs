﻿using GalaSoft.MvvmLight.Command;
using Sales.Common.Models;
using Sales.Helpers;
using Sales.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Sales.ViewModels
{
    public class ProductsViewModel : BaseViewModel
    {
        private ApiService apiService;

        private ObservableCollection<Product> products;

        public ObservableCollection<Product> Products
        {
            get { return this.products; }
            set { this.SetValue(ref this.products, value); }
        }

        private bool isRefreshing;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { this.SetValue(ref this.isRefreshing, value); }
        }

        public ProductsViewModel()
        {
            this.apiService = new ApiService();
            this.LoadProducts();
        }

        private async void LoadProducts()
        {
            IsRefreshing = true;

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message,Languages.Accept);
                return;
            }

            //var url = Application.Current.Resources["UrlAPI"].ToString();
            //var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            //var controller = Application.Current.Resources["UrlProductsController"].ToString();

            //var response = await this.apiService.GetList<Product>(url, prefix, controller);
            var response = await apiService.GetList<Product>("https://salesapiservice.azurewebsites.net", "/api", "/products");
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }
            var list = (List<Product>)response.Result;
            this.Products = new ObservableCollection<Product>(list);
            IsRefreshing = false;
        }

        public ICommand RefreshCommand
        {
            get
            {
               return new RelayCommand(LoadProducts);
            }
        }
    }
}
