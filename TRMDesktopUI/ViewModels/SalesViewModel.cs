using AutoMapper;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Models;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        IProductEndpoint _productEndpoint;
        ISaleEndpoint _saleEndpoint;
        IConfigHelper _configHelper;
        IMapper _mapper;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;

        public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper,
            ISaleEndpoint saleEndpoint, IMapper mapper, StatusInfoViewModel status, IWindowManager window)
        {
            _productEndpoint = productEndpoint;
            _saleEndpoint = saleEndpoint;
            _configHelper = configHelper;
            _mapper = mapper;
            _status = status;
            _window = window;
        }

        protected override async void OnViewLoaded(object view) // Although, it's async, it can be void, because this is an eventhandler
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadProducts();
            }
            catch (Exception ex)
            {
                // Creating a dialog box starts
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";

                if (ex.Message == "Unauthorized")
                {
                    _status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales Form.");                    
                }
                else
                {
                    _status.UpdateMessage("Fatal Exception", ex.Message);                    
                }

                await _window.ShowDialogAsync(_status, null, settings);
                // Creating a dialog box ends

                TryCloseAsync();
            }
        }

        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAll();
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);
            Products = new BindingList<ProductDisplayModel>(products);
        }

        private BindingList<ProductDisplayModel> _products;

        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            // Note, that set is not fired when adding elemnts to the List. Only fired when overriding the whole list.
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private ProductDisplayModel _selectedProduct;

        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        private async Task ResetSalesViewModelAsync()
        {
            Cart = new BindingList<CartItemDisplayModel>();
            // TODO: clearing if needed _selectedCartItem
            await LoadProducts();

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckout);
        }

        private CartItemDisplayModel _selectedCartItem;
        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
            }
        }

        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();

        public BindingList<CartItemDisplayModel> Cart
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
            CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct); 
            // We already have this utem in the cart
            if (existingItem != null)
            {
                // This is executed on the Cart. (existingItem points to an address of an item on the cart)
                existingItem.QuantityInCart += ItemQuantity;
            }
            // New item to the cart
            else
            {
                CartItemDisplayModel item = new CartItemDisplayModel
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
            NotifyOfPropertyChange(() => CanCheckout);
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;
                                
                if (SelectedCartItem != null && SelectedCartItem?.QuantityInCart > 0) // TODO Zoli: null check not needed
                {                    
                    output = true;
                }

                return output;                
            }
        }

        public void RemoveFromCart()
        {
            SelectedCartItem.Product.QuantityInStock += 1;

            if (SelectedCartItem.QuantityInCart > 1)
            {
                SelectedCartItem.QuantityInCart -= 1;                
            }
            else
            {
                Cart.Remove(SelectedCartItem);
            }

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckout);
            NotifyOfPropertyChange(() => CanAddToCart);
        }

        public bool CanCheckout
        {
            get
            {
                bool output = false;

                // make sure there is something in the cart
                if (Cart.Count > 0)  // TODO: "?": If NOT Null
                {                    
                    output = true;
                }

                return output;
            }
        }

        public async Task Checkout()
        {
            // Create a SaleModel from the items in the Cart and post to the API
            SaleModel sale = new SaleModel();

            foreach (var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });
            }

            await _saleEndpoint.PostSale(sale);

            await ResetSalesViewModelAsync();
        }
    }
}
