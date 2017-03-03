using System;
using System.Data;
using BLToolkit.DataAccess;

namespace DbTestFramework.DemoApp.Model
{
    public interface IStkTestDbAccessor: IDisposable
    {
        IDataReader InsertOrUpdateProduct(int? id, string name, int weight);

        IDataReader SelectProducts(int? id);

        IDataReader SelectStockAcmounts(int? stockId);

        IDataReader UpdateStock(int productId, int amount);

        void DeleteProduct(int productId);
    }

	public abstract class StkTestDbAccessor : DataAccessor, IStkTestDbAccessor
    {
        public abstract IDataReader InsertOrUpdateProduct(int? id, string name, int weight);

        public abstract IDataReader SelectProducts(int? id);

        public abstract IDataReader SelectStockAcmounts(int? stockId);

        public abstract IDataReader UpdateStock(int productId, int amount);

        public abstract void DeleteProduct(int productId);

        public void Dispose()
        {
            GetDbManager().Dispose();
        }

        protected override string GetDefaultSpName(string typeName, string actionName)
        {
            return "usp_" + actionName;
        }
    }
}