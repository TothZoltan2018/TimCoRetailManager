using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class SaleData : ISaleData
    {        
        private readonly ISqlDataAccess _sql;
        private readonly IProductData _productData;
        public SaleData(ISqlDataAccess sql, IProductData productData)
        {
            _sql = sql;
            _productData = productData;
        }

        /// <summary>
        /// Saves the sale model to database
        /// </summary>
        /// <param name="saleInfo"> is a SaleModel received from the API</param>
        /// 
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            // Note, We do not trust the frontend, therefore we don't get all the infos from there.
            // TODO: Make this SOLID/DRY/Better
            // Start filling in the Sale Detail models we will save to the database
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            
            var taxRate = ConfigHelper.GetTaxRate() / 100;

            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                };

                //Get the info about this product                
                var productInfo = _productData.GetProductById(detail.ProductId);

                // Fill in the available information (SaleId is not get yet.)
                if (productInfo == null)
                {
                    throw new Exception($"The product Id of {detail.ProductId} could not be found in the database.");
                }

                detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);
                if (productInfo.IsTaxable)
                {
                    detail.Tax = (detail.PurchasePrice * taxRate);
                }

                details.Add(detail);
            }

            // Create the Sale model (Consits of several items in the shopping cart.)
            SaleDBModel sale = new SaleDBModel
            {
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
                CashierId = cashierId
            };

            sale.Total = sale.SubTotal + sale.Tax;

            // Saving into sale and saledetail tables happen in one transaction            
            try
            {
                _sql.StartTransaction("TRMData");
                // Save the Sale model
                _sql.SaveDataInTransaction("dbo.spSale_Insert", sale);

                // Get the Sale ID from the Sale model - dbo.spSale_Insert has an output variable Id, but "sale.id" do not get it.
                // Therefore an other query needs to be run.
                sale.Id = _sql.LoadDataInTransaction<int, dynamic>("spSale_Lookup", new { CashierId = sale.CashierId, SaleDate = sale.SaleDate }).FirstOrDefault();

                // Finish filling the Sale detail models
                foreach (var item in details)
                {
                    item.SaleId = sale.Id;
                    // Save the sale detail models
                    _sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                }

                _sql.CommitTransaction();
            }
            catch
            {
                _sql.RollbackTransaction();
                throw; // It throws the original exception, from the deep --> more info
            }            
        }

        public List<SaleReportModel> GetSaleReport()
        {
            var output = _sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "TRMData");

            return output;
        }
    }
}