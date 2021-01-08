using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        IProductEndpoint _productEndpoint;
        public SalesViewModel(IProductEndpoint productEndpoint)
        {
            _productEndpoint = productEndpoint;            
        }

        protected override async void OnViewLoaded(object view) // Although, it's async, it can be void, because this is an eventhandler
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }


        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAll();
            Products = new BindingList<ProductModel>(productList);        
        }

        private BindingList<ProductModel> _products;

        public BindingList<ProductModel> Products
        {
            get { return _products; }
            // Note, that set is not fired when adding elemnts to the List. Only fired when overriding the whole list.
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private BindingList<string> _cart;

        public BindingList<string> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }


        private int _itemQuantity;

        // Althought ItemQuantity is a textbox, expecting string, auto-validation is done in the background to check and convert 
        // a numeric string to integer
        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
            }
        }

        public string SubTotal
        {
            get
            {
                // TODO - Replace with calculation
                return "$0.00";
            }
        }

        public string Total
        {
            get
            {
                // TODO - Replace with calculation
                return "$0.00";
            }
        }

        public string Tax
        {
            get
            {
                // TODO - Replace with calculation
                return "$0.00";
            }
        }


        public bool CanAddToCart
        {
            get
            {
                bool output = false;

                if (false) // "?": If NOT Null
                {
                    // make sure something is selected and item quantity has a value
                    output = true;
                }

                return output;
            }
        }

        public void AddToCart()
        {
        
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                if (false) // "?": If NOT Null
                {
                    // make sure something is selected
                    output = true;
                }

                return output;
            }
        }

        public void RemoveFromCart()
        {

        }

        public bool CanCheckout
        {
            get
            {
                bool output = false;

                if (false) // "?": If NOT Null
                {
                    // make sure there is something in the cart
                    output = true;
                }

                return output;
            }
        }

        public void Checkout()
        {

        }
    }
}
