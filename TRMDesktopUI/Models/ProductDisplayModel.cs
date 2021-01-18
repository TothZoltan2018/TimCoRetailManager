using System.ComponentModel;

namespace TRMDesktopUI.Models
{
    public class ProductDisplayModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal RetailPrice { get; set; }

        private int _quantityInStock;
        public int QuantityInStock
        {
            get { return _quantityInStock; }
            set
            {
                _quantityInStock = value;
                CallPropertyChanged(nameof(QuantityInStock));
            }
        }

        public bool IsTaxable { get; set; }

        // We have to fire this event off whenever a property changes
        public event PropertyChangedEventHandler PropertyChanged;

        public void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}