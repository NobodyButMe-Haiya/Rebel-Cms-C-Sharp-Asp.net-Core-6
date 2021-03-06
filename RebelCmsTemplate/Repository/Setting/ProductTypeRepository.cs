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
    public class ProductTypeRepository
    {

        private readonly SharedUtil _sharedUtil;
        public ProductTypeRepository(IHttpContextAccessor httpContextAccessor)
        {
            _sharedUtil = new SharedUtil(httpContextAccessor);
        }
        public int Create(ProductTypeModel productTypeModel)
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
                INSERT INTO product_type VALUES (null,@tenantId,@productCategoryId,@productTypeName,0,@executeBy);";
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
                        Key = "@productCategoryId",
                        Value = productTypeModel.ProductCategoryKey
                    },
                    new ()
                    {
                        Key = "@productTypeName",
                        Value = productTypeModel.ProductTypeName
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
        public List<ProductTypeModel> Read()
        {
            List<ProductTypeModel> productTypeModels = new();
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {

                connection.Open();
                sql += @"
                SELECT      *
                FROM        product_type
                JOIN        product_category              
                USING       (productCategoryId,tenantId)
                JOIN        tenant
                USING       (tenantId)
                WHERE       product_category.tenantId = @tenantId
                AND         product_type.isDelete != 1
                ORDER BY    productTypeId DESC ";
                // reporting purpose
                _sharedUtil.SetSqlSession(sql);
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
                        productTypeModels.Add(new ProductTypeModel
                        {

                            ProductCategoryName = reader["productCategoryName"].ToString(),
                            ProductTypeName = reader["productTypeName"].ToString(),
                            ProductCategoryKey = Convert.ToInt32(reader["productCategoryId"]),
                            ProductTypeKey = Convert.ToInt32(reader["productTypeId"]),
                            TenantKey = Convert.ToInt32(reader["tenantId"]),
                            TenantName = reader["tenantName"].ToString()
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


            return productTypeModels;
        }
        public List<ProductTypeModel> Search(string search)
        {
            List<ProductTypeModel> productTypeModels = new();
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {

                connection.Open();
                sql += @"
                SELECT  *
                FROM    product_type
                JOIN    product_category
                USING   (productCategoryId,tenantId)
                JOIN    tenant
                USING   (tenantId)
                WHERE   product_category.tenantId = @tenantId
                AND     product_type.isDelete != 1
                AND     product_category.isDelete != 1            
                AND     productTypeName like concat('%',@search,'%')
                OR      productCategoryName like concat('%',@search,'%'); ";
                // reporting purpose
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
                        productTypeModels.Add(new ProductTypeModel
                        {
                            ProductCategoryName = reader["productCategoryName"].ToString(),
                            ProductTypeName = reader["productTypeName"].ToString(),
                            ProductCategoryKey = Convert.ToInt32(reader["productCategoryId"]),
                            ProductTypeKey = Convert.ToInt32(reader["productTypeId"]),
                            TenantKey = Convert.ToInt32(reader["tenantId"]),
                            TenantName = reader["tenantName"].ToString()
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


            return productTypeModels;
        }
        public byte[] GetExcel()
        {

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Setting > Product Type");
            
            worksheet.Cell(1, 1).Value = "No";
            worksheet.Cell(1, 2).Value = "Category";
            worksheet.Cell(1, 3).Value = "Name";

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
                        worksheet.Cell(currentRow, 1).Value = counter - 1;
                        worksheet.Cell(currentRow, 2).Value = reader["productCategoryName"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["productTypeName"].ToString();

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
        public void Update(ProductTypeModel productTypeModel)
        {
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {

                connection.Open();
                MySqlTransaction mySqlTransaction = connection.BeginTransaction();

                sql += @"
                UPDATE  product_type
                SET     productCategoryId   =   @productCategoryId,
                        productTypeName     =   @productTypeName,
                        executeBy           =   @executeBy
                WHERE   productTypeId       =   @productTypeId ";
                MySqlCommand mySqlCommand = new(sql, connection);

                parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@productCategoryId",
                        Value = productTypeModel.ProductCategoryKey
                    },
                    new ()
                    {
                        Key = "@productTypeName",
                        Value = productTypeModel.ProductTypeName
                    },
                    new ()
                    {
                        Key = "@executeBy",
                        Value = _sharedUtil.GetUserName()
                    },
                    new ()
                    {
                        Key = "@productTypeId",
                        Value = productTypeModel.ProductTypeKey
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
        public void Delete(ProductTypeModel productTypeModel)
        {
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();
            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {

                connection.Open();
                MySqlTransaction mySqlTransaction = connection.BeginTransaction();

                sql += @"
                UPDATE  product_type
                SET     isDelete = 1,
                        executeBy = @executeBy
                WHERE   productTypeId  = @productTypeId;";
                MySqlCommand mySqlCommand = new(sql, connection);

                parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@productTypeId",
                        Value = productTypeModel.ProductTypeKey
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
