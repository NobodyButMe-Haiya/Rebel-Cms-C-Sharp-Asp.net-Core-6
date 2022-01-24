using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Application;

public class ProductRepository
{
    private readonly SharedUtil _sharedUtil;

    public ProductRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public int Create(ProductModel productModel)
    {
        int lastInsertKey;
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            // we see here vendor id is missing .. 
            sql += @"
                INSERT INTO product (
                    productId,          tenantId,               productCategoryId,
                    productTypeId,      productName,            productDescription,
                    productCostPrice,   productSellingPrice,    isDelete,
                    executeBy
                ) VALUES (
                     null,              @tenantId,              @productCategoryId,
                    @productTypeId,     @productName,           @productDescription,
                    @productCostPrice,  @productSellingPrice,   0,
                    @executeBy
                );";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                },
                new()
                {
                    Key = "@productCategoryId",
                    Value = productModel.ProductCategoryKey
                },
                new()
                {
                    Key = "@productTypeId",
                    Value = productModel.ProductTypeKey
                },
                new()
                {
                    Key = "@vendorId",
                    Value = 0
                },
                new()
                {
                    Key = "@productName",
                    Value = productModel.ProductName
                },
                new()
                {
                    Key = "@productDescription",
                    Value = productModel.ProductDescription
                },
                new()
                {
                    Key = "@productCostPrice",
                    Value = productModel.ProductCostPrice
                },
                new()
                {
                    Key = "@productSellingPrice",
                    Value = productModel.ProductSellingPrice
                },
                new()
                {
                    Key = "@executeBy",
                    Value = _sharedUtil.GetUserName()
                }
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            mySqlCommand.ExecuteNonQuery();
            mySqlTransaction.Commit();
            mySqlCommand.Dispose();
            lastInsertKey = (int) mySqlCommand.LastInsertedId;
        }
        catch (MySqlException ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
            throw new Exception(ex.Message);
        }

        return lastInsertKey;
    }

    public List<ProductModel> Read()
    {
        List<ProductModel> productModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
                SELECT      *
                FROM        product
                JOIN        product_category
                USING       (tenantId,productCategoryId)
                JOIN        product_type
                USING       (tenantId,productCategoryId,productTypeId)
                WHERE       product.tenantId    = @tenantId
                ORDER BY    product.productId DESC ";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new("@tenantId", _sharedUtil.GetTenantId())
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            _sharedUtil.SetSqlSession(sql, parameterModels);

            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    productModels.Add(new ProductModel
                    {
                        ProductCategoryKey = Convert.ToInt32(reader["productCategoryId"]),
                        ProductTypeKey = Convert.ToInt32(reader["productTypeId"]),
                        ProductKey = Convert.ToInt32(reader["productId"]),
                        ProductName = reader["productName"].ToString(),
                        ProductDescription = reader["productDescription"].ToString(),
                        ProductCostPrice = Convert.ToDouble(reader["productCostPrice"]),
                        ProductSellingPrice = Convert.ToDouble(reader["productSellingPrice"])
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


        return productModels;
    }

    public List<ProductModel> Search(string search)
    {
        List<ProductModel> productModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
                SELECT  *
                FROM    product
                JOIN    product_category 
                USING   (productCategoryId,tenantId)
                JOIN    product_type 
                USING   (productCategoryId,productTypeId,tenantId)
                WHERE   product.tenantId    =   @tenantId
                AND     productName     LIKE CONCAT('%',@search,'%')
                OR      productDescription    LIKE CONCAT('%',@search,'%'); ";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                },
                new()
                {
                    Key = "@search",
                    Value = search
                }
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            _sharedUtil.SetSqlSession(sql, parameterModels);

            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    productModels.Add(new ProductModel
                    {
                        ProductCategoryKey = Convert.ToInt32(reader["productCategoryId"]),
                        ProductTypeKey = Convert.ToInt32(reader["productTypeId"]),
                        ProductKey = Convert.ToInt32(reader["productId"]),
                        ProductName = reader["productName"].ToString(),
                        ProductDescription = reader["productDescription"].ToString(),
                        ProductCostPrice = Convert.ToDouble(reader["productCostPrice"]),
                        ProductSellingPrice = Convert.ToDouble(reader["productSellingPrice"])
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


        return productModels;
    }

    public byte[] GetExcel()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Application >  Product");
        worksheet.Cell(1, 1).Value = "No";
        worksheet.Cell(1, 2).Value = "Code";
        worksheet.Cell(1, 2).Value = "Current";
        worksheet.Cell(1, 2).Value = "Description";


        var sql = _sharedUtil.GetSqlSession();
        var parameterModels = _sharedUtil.GetListSqlParameter();
        using var connection = SharedUtil.GetConnection();

        try
        {
            connection.Open();
            MySqlCommand mySqlCommand = new(sql, connection);
            if (parameterModels != null)
            {
                foreach (var parameter in parameterModels)
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
                    worksheet.Cell(currentRow, 2).Value = reader["documentNumberCode"].ToString();
                    worksheet.Cell(currentRow, 3).Value = reader["documentNumber"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["documentNumberDescription"].ToString();
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

    public void Update(ProductModel productModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();

            sql += @"
                UPDATE  product
                SET     productCategoryId   =   @productCategoryId,
                        productTypeId       =   @productTypeId,
                        productName         =   @productName,
                        productDescription  =   @productDescription,
                        productCostPrice    =   @productCostPrice,
                        productSellingPrice =   @productSellingPrice,
                        executeBy           =   @executeBy
                WHERE   productId           =   @productId;";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@productCategoryId",
                    Value = productModel.ProductCategoryKey
                },
                new()
                {
                    Key = "@productTypeId",
                    Value = productModel.ProductTypeKey
                },
                new()
                {
                    Key = "@productName",
                    Value = productModel.ProductName
                },
                new()
                {
                    Key = "@productDescription",
                    Value = productModel.ProductDescription
                },
                new()
                {
                    Key = "@productCostPrice",
                    Value = productModel.ProductCostPrice
                },
                new()
                {
                    Key = "@productSellingPrice",
                    Value = productModel.ProductSellingPrice
                },
                new()
                {
                    Key = "@executeBy",
                    Value = _sharedUtil.GetUserName()
                },
                new()
                {
                    Key = "@productId",
                    Value = productModel.ProductKey
                }
            };
            foreach (var parameter in parameterModels)
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

    public void Delete(ProductModel productModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();

            sql = @"
                UPDATE  product
                SET     isDelete = 1
                WHERE   productId  = @productId;";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@productId",
                    Value = productModel.ProductKey
                }
            };
            foreach (var parameter in parameterModels)
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