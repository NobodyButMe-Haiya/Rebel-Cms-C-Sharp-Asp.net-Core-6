using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Setting;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Setting
{
    public class ProductCategoryRepository
    {

        private readonly SharedUtil _sharedUtil;
        public ProductCategoryRepository(IHttpContextAccessor httpContextAccessor)
        {
            _sharedUtil = new SharedUtil(httpContextAccessor);
        }
        public int Create(ProductCategoryModel productCategoryModel)
        {
            // okay next we create skeleton for the code
            int lastInsertKey;
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {

                connection.Open();

                MySqlTransaction mySqlTransaction = connection.BeginTransaction();

                sql += @"
                INSERT INTO product_category  (
                    productCategoryId,      tenantId,
                    productCategoryName,   isDelete,
                    executeBy
                ) VALUES (
                    null,                   @tenantId,
                    @productCategoryName,   0,
                    @executeBy
                );";
                MySqlCommand mySqlCommand = new(sql, connection);

                parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@tenantId",
                        Value = _sharedUtil.GetTenantId()
                    },
                    new ()
                    {
                        Key = "@productCategoryName",
                        Value = productCategoryModel.ProductCategoryName
                    },
                    new ()
                    {
                        Key = "@executeBy",
                        Value = _sharedUtil.GetUserName()
                    }
                };
                foreach (ParameterModel parameter in parameterModels)
                {
                    mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
                mySqlCommand.ExecuteNonQuery();

                mySqlTransaction.Commit();

                lastInsertKey = (int)mySqlCommand.LastInsertedId;

                mySqlCommand.Dispose();

            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
                throw new Exception(ex.Message);
            }

            return lastInsertKey;


        }
        public List<ProductCategoryModel> Read()
        {
            List<ProductCategoryModel> productCategoryModels = new();
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {

                connection.Open();
                sql += @"
                SELECT      *
                FROM        product_category
                WHERE       isDelete != 1
                AND         tenantId = @tenantId
                ORDER BY    productCategoryId DESC ";
                MySqlCommand mySqlCommand = new(sql, connection);
                parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@tenantId",
                        Value = _sharedUtil.GetTenantId()
                    }
                };
                foreach (ParameterModel parameter in parameterModels)
                {
                    mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
                _sharedUtil.SetSqlSession(sql, parameterModels);

                using (var reader = mySqlCommand.ExecuteReader())
                {
                    
                    while (reader.Read())
                    {
                        productCategoryModels.Add(new ProductCategoryModel
                        {

                            ProductCategoryName = reader["productCategoryName"].ToString(),
                            ProductCategoryKey = Convert.ToInt32(reader["productCategoryId"])
                        });
                    }
                }

                mySqlCommand.Dispose();

            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
                throw new Exception(ex.Message);
            }


            return productCategoryModels;
        }
        public List<ProductCategoryModel> Search(string search)
        {
            List<ProductCategoryModel> productCategoryModels = new();
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {

                connection.Open();
                sql += @"
                SELECT  *
                FROM    product_category
                WHERE   tenantId = @tenantId
                AND     isDelete != 1
                AND     productCategoryName like concat('%',@search,'%') ";
                MySqlCommand mySqlCommand = new(sql, connection);
                parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@tenantId",
                        Value = _sharedUtil.GetTenantId()
                    },
                    new ()
                    {
                        Key = "@search",
                        Value = search
                    }
                };
                foreach (ParameterModel parameter in parameterModels)
                {
                    mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
                _sharedUtil.SetSqlSession(sql, parameterModels);

                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productCategoryModels.Add(new ProductCategoryModel
                        {
                            ProductCategoryName = reader["productCategoryName"].ToString(),
                            ProductCategoryKey = Convert.ToInt32(reader["productCategoryId"])
                        });
                    }
                }
                mySqlCommand.Dispose();
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
                throw new Exception(ex.Message);
            }


            return productCategoryModels;
        }
        public byte[] GetExcel()
        {

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Setting > Product Category");
  
            worksheet.Cell(1, 1).Value = "No";
            worksheet.Cell(1, 2).Value = "Category";
            var sql = _sharedUtil.GetSqlSession();
            var parameterModels = _sharedUtil.GetListSqlParameter();
            using MySqlConnection connection = SharedUtil.GetConnection();

            try
            {
                connection.Open();
                MySqlCommand mySqlCommand = new(sql, connection);
                if (parameterModels != null)
                {
                    foreach (ParameterModel parameter in parameterModels)
                    {
                        mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }
                }
                using (var reader = mySqlCommand.ExecuteReader())
                {
                    var counter = 1;
                    while (reader.Read())
                    {
                        var currentRow = counter++;
                        worksheet.Cell(currentRow, 1).Value = counter -1 ;
                        worksheet.Cell(currentRow, 2).Value = reader["productCategoryName"].ToString();
                    }
                }

                mySqlCommand.Dispose();
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        public void Update(ProductCategoryModel productCategoryModel)
        {
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {

                connection.Open();
                MySqlTransaction mySqlTransaction = connection.BeginTransaction();

                sql += @"
                UPDATE  product_category
                SET     productCategoryName =   @productCategoryName,
                        executeBy           =   @executeBy
                WHERE   productCategoryId   =   @productCategoryId ";
                MySqlCommand mySqlCommand = new(sql, connection);

                parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@productCategoryName",
                        Value = productCategoryModel.ProductCategoryName
                    },
                    new ()
                    {
                        Key = "@executeBy",
                        Value = _sharedUtil.GetUserName()
                    },
                    new ()
                    {
                        Key = "@productCategoryId",
                        Value = productCategoryModel.ProductCategoryKey
                    }
                };
                foreach (ParameterModel parameter in parameterModels)
                {
                    mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
                mySqlCommand.ExecuteNonQuery();
                mySqlTransaction.Commit();
                mySqlCommand.Dispose();

            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
                throw new Exception(ex.Message);
            }
        }
        public void Delete(ProductCategoryModel productCategoryModel)
        {
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {

                connection.Open();
                MySqlTransaction mySqlTransaction = connection.BeginTransaction();

                sql += @"
                UPDATE  product_category
                SET     isDelete = 1
                WHERE   productCategoryId  = @productCategoryId;";
                MySqlCommand mySqlCommand = new(sql, connection);

                parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@productCategoryId",
                        Value = productCategoryModel.ProductCategoryKey
                    }
                };
                foreach (ParameterModel parameter in parameterModels)
                {
                    mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
                mySqlCommand.ExecuteNonQuery();

                mySqlTransaction.Commit();

                mySqlCommand.Dispose();

            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
