using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        IProductEndpoint _productEndpoint;
        IConfigHelper _configHelper;
        public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper)
        {
            _productEndpoint = productEndpoint;
            _configHelper = configHelper;
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

        private ProductModel _selectedProduct;

        public ProductModel SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();

        public BindingList<CartItemModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        private int _itemQuantity = 1;

        // Althought ItemQuantity is a textbox, expecting string, auto-validation is done in the background to check and convert 
        // a numeric string to integer
        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        public string SubTotal
        {
            get
            {
                return CalculateSubTotal().ToString("C");
            }
        }

        private decimal CalculateSubTotal()
        {
            decimal subTotal = 0;

            foreach (var item in Cart)
            {
                subTotal += (item.Product.RetailPrice * item.QuantityInCart);
            }

            return subTotal;
        }

        public string Tax
        {
            get
            {
                return CalculateTax().ToString("C");
            }
        }

        private decimal CalculateTax()
        {
            decimal taxAmount = 0;
            decimal taxrate = _configHelper.GetTaxRate()/100;

            //foreach (var item in Cart)
            //{
            //    if (item.Product.IsTaxable)
            //    {
            //        taxAmount += (item.Product.RetailPrice * item.QuantityInCart * taxrate);
            //    }
            //}

            taxAmount = Cart
                .Where(x => x.Product.IsTaxable == true )
                .Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxrate);

            return taxAmount;
        }

        public string Total
        {
            get
            {
                decimal total = CalculateSubTotal() + CalculateTax();
                return total.ToString("C");
            }
        }

        public bool CanAddToCart
        {
            get
            {
                bool output = false;

                // make sure something is selected and item quantity has a value
                if (ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity) // "?": If NOT Null
                {
                    output = true;
                }

                return output;
            }
        }

        public void AddToCart()
        {
            // Compares the two objects (not the values).
            CartItemModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct); 
            // We already have this utem in the cart
            if (existingItem != null)
            {
                // This is executed on the Cart. (existingItem points to an address of an item on the cart)
                existingItem.QuantityInCart += ItemQuantity;
                // TODO: This hack needs to be fixed to properly refresh the cart ListBox.
                Cart.Remove(existingItem);
                Cart.Add(existingItem);
            }
            // New item to the cart
            else
            {
                CartItemModel item = new CartItemModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };

                Cart.Add(item);
            }            

            SelectedProduct.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);            
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
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
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
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
