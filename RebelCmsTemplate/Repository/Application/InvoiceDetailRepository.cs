using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Application;

public class InvoiceDetailRepository
{
    private readonly SharedUtil _sharedUtil;

    public InvoiceDetailRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public int Create(InvoiceDetailModel invoiceDetailModel)
    {
        var lastInsertKey = 0;
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql +=
                @"INSERT INTO invoice_detail (invoiceDetailId,invoiceId,productId,invoiceDetailUnitPrice,invoiceDetailQuantity,invoiceDetailDiscount,isDelete) VALUES (null,@invoiceId,@productId,@invoiceDetailUnitPrice,@invoiceDetailQuantity,@invoiceDetailDiscount,@isDelete);";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@invoiceId",
                    Value = invoiceDetailModel.InvoiceKey
                },
                new()
                {
                    Key = "@productId",
                    Value = invoiceDetailModel.ProductKey
                },
                new()
                {
                    Key = "@invoiceDetailUnitPrice",
                    Value = invoiceDetailModel.InvoiceDetailUnitPrice
                },
                new()
                {
                    Key = "@invoiceDetailQuantity",
                    Value = invoiceDetailModel.InvoiceDetailQuantity
                },
                new()
                {
                    Key = "@invoiceDetailDiscount",
                    Value = invoiceDetailModel.InvoiceDetailDiscount
                },
                new()
                {
                    Key = "@isDelete",
                    Value = 0
                }
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            mySqlCommand.ExecuteNonQuery();
            mySqlTransaction.Commit();
            lastInsertKey = (int) mySqlCommand.LastInsertedId;
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

    public List<InvoiceDetailModel> Read()
    {
        List<InvoiceDetailModel> invoiceDetailModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql = @"
                SELECT      *
                FROM        invoice_detail 
                WHERE       isDelete !=1
                ORDER BY    invoiceDetailId DESC ";
            MySqlCommand mySqlCommand = new(sql, connection);
            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    invoiceDetailModels.Add(new InvoiceDetailModel
                    {
                        InvoiceDetailKey = Convert.ToInt32(reader["invoiceDetailId"]),
                        InvoiceKey = Convert.ToInt32(reader["invoiceId"]),
                        ProductKey = Convert.ToInt32(reader["productId"]),
                        InvoiceDetailUnitPrice = Convert.ToDecimal(reader["invoiceDetailUnitPrice"]),
                        InvoiceDetailQuantity = Convert.ToInt32(reader["invoiceDetailQuantity"]),
                        InvoiceDetailDiscount = Convert.ToDouble(reader["invoiceDetailDiscount"]),
                        IsDelete = Convert.ToInt32(reader["isDelete"])
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

        return invoiceDetailModels;
    }

    public List<InvoiceDetailModel> Search(string search)
    {
        List<InvoiceDetailModel> invoiceDetailModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
                SELECT  *
                FROM    invoice_detail 
	 JOIN invoice 
	 USING(invoiceId)
	 JOIN product 
	 USING(productId)
	 WHERE   invoice_detail.isDelete != 1
	 AND (	 invoice.invoiceId like concat('%',@search,'%') OR	 invoice.invoiceId like concat('%',@search,'%') OR	 invoice.invoiceId like concat('%',@search,'%') OR	 invoice.invoiceId like concat('%',@search,'%') OR	 invoice.invoiceId like concat('%',@search,'%') OR	 invoice.invoiceId like concat('%',@search,'%') OR	 invoice.invoiceId like concat('%',@search,'%') OR	 invoice.invoiceId like concat('%',@search,'%') OR	 invoice.invoiceId like concat('%',@search,'%') OR	 invoice.invoiceId like concat('%',@search,'%') OR	 invoice.invoiceId like concat('%',@search,'%') OR	 invoice.invoiceId like concat('%',@search,'%') OR	 invoice.invoiceId like concat('%',@search,'%') OR	 invoice.invoiceId like concat('%',@search,'%') OR	 invoice.invoiceId like concat('%',@search,'%') OR	 invoice.invoiceId like concat('%',@search,'%') OR	 product.productId like concat('%',@search,'%') OR	 product.productId like concat('%',@search,'%') OR	 product.productId like concat('%',@search,'%') OR	 product.productId like concat('%',@search,'%') OR	 product.productId like concat('%',@search,'%') OR	 product.productId like concat('%',@search,'%') OR	 product.productId like concat('%',@search,'%') OR	 product.productId like concat('%',@search,'%') OR	 product.productId like concat('%',@search,'%') OR	 product.productId like concat('%',@search,'%') OR	 product.productId like concat('%',@search,'%') OR	 product.productId like concat('%',@search,'%') OR	 product.productId like concat('%',@search,'%') OR	 product.productId like concat('%',@search,'%') OR	 product.productId like concat('%',@search,'%') OR
	 invoice_detail.invoiceDetailUnitPrice like concat('%',@search,'%') OR
	 invoice_detail.invoiceDetailQuantity like concat('%',@search,'%') OR
	 invoice_detail.invoiceDetailDiscount like concat('%',@search,'%') )";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
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
                    invoiceDetailModels.Add(new InvoiceDetailModel
                    {
                        InvoiceDetailKey = Convert.ToInt32(reader["invoiceDetailId"]),
                        InvoiceKey = Convert.ToInt32(reader["invoiceId"]),
                        ProductKey = Convert.ToInt32(reader["productId"]),
                        InvoiceDetailUnitPrice = Convert.ToDecimal(reader["invoiceDetailUnitPrice"]),
                        InvoiceDetailQuantity = Convert.ToInt32(reader["invoiceDetailQuantity"]),
                        InvoiceDetailDiscount = Convert.ToDouble(reader["invoiceDetailDiscount"]),
                        IsDelete = Convert.ToInt32(reader["isDelete"])
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

        return invoiceDetailModels;
    }

    public InvoiceDetailModel GetSingle(InvoiceDetailModel invoiceDetailModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
                SELECT  *
                FROM    invoice_detail 
	 JOIN invoice 
	 USING(invoiceId)
	 JOIN product 
	 USING(productId)
                WHERE   invoice_detail.isDelete != 1
                AND   invoice_detail.invoiceDetailId    =   @invoiceDetailId LIMIT 1";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@invoiceDetailId",
                    Value = invoiceDetailModel.InvoiceDetailKey
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
                    invoiceDetailModel = new InvoiceDetailModel
                    {
                        InvoiceDetailKey = Convert.ToInt32(reader["invoiceDetailId"]),
                        InvoiceKey = Convert.ToInt32(reader["invoiceId"]),
                        ProductKey = Convert.ToInt32(reader["productId"]),
                        InvoiceDetailUnitPrice = Convert.ToDecimal(reader["invoiceDetailUnitPrice"]),
                        InvoiceDetailQuantity = Convert.ToInt32(reader["invoiceDetailQuantity"]),
                        InvoiceDetailDiscount = Convert.ToDouble(reader["invoiceDetailDiscount"]),
                        IsDelete = Convert.ToInt32(reader["isDelete"])
                    };
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

        return invoiceDetailModel;
    }

    public byte[] GetExcel()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Administrator > InvoiceDetail ");
        worksheet.Cell(1, 1).Value = "invoiceDetailId";
        worksheet.Cell(1, 2).Value = "invoiceId";
        worksheet.Cell(1, 3).Value = "productId";
        worksheet.Cell(1, 4).Value = "invoiceDetailUnitPrice";
        worksheet.Cell(1, 5).Value = "invoiceDetailQuantity";
        worksheet.Cell(1, 6).Value = "invoiceDetailDiscount";
        worksheet.Cell(1, 7).Value = "isDelete";
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
                    worksheet.Cell(currentRow, 2).Value = reader["invoiceDetailId"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["invoiceId"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["productId"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["invoiceDetailUnitPrice"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["invoiceDetailQuantity"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["invoiceDetailDiscount"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["isDelete"].ToString();
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

    public void Update(InvoiceDetailModel invoiceDetailModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql = @"
                UPDATE  invoice_detail 
                SET     
invoiceId=@invoiceId,
productId=@productId,
invoiceDetailUnitPrice=@invoiceDetailUnitPrice,
invoiceDetailQuantity=@invoiceDetailQuantity,
invoiceDetailDiscount=@invoiceDetailDiscount,
isDelete=@isDelete

                WHERE   invoiceDetailId    =   @invoiceDetailId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@invoiceDetailId",
                    Value = invoiceDetailModel.InvoiceDetailKey
                },
                new()
                {
                    Key = "@invoiceId",
                    Value = invoiceDetailModel.InvoiceKey
                },
                new()
                {
                    Key = "@productId",
                    Value = invoiceDetailModel.ProductKey
                },
                new()
                {
                    Key = "@invoiceDetailUnitPrice",
                    Value = invoiceDetailModel.InvoiceDetailUnitPrice
                },
                new()
                {
                    Key = "@invoiceDetailQuantity",
                    Value = invoiceDetailModel.InvoiceDetailQuantity
                },
                new()
                {
                    Key = "@invoiceDetailDiscount",
                    Value = invoiceDetailModel.InvoiceDetailDiscount
                },
                new()
                {
                    Key = "@isDelete",
                    Value = 0
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

    public void Delete(InvoiceDetailModel invoiceDetailModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql = @"
                UPDATE  invoice_detail 
                SET     isDelete    =   1
                WHERE   invoiceDetailId    =   @invoiceDetailId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@invoiceDetailId",
                    Value = invoiceDetailModel.InvoiceDetailKey
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