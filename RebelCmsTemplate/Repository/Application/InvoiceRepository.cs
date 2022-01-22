using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Application;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;
namespace RebelCmsTemplate.Repository.Application;
public class InvoiceRepository
{
    private readonly SharedUtil _sharedUtil;
    public InvoiceRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }
    public int Create(InvoiceModel invoiceModel)
    {
        var lastInsertKey = 0;
        string sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using MySqlConnection connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            MySqlTransaction mySqlTransaction = connection.BeginTransaction();
            sql += @"INSERT INTO invoice (invoiceId,tenantId,customerId,shipperId,employeeId,invoiceOrderDate,invoiceRequiredDate,invoiceShippedDate,invoiceFreight,invoiceShipName,invoiceShipAddress,invoiceShipCity,invoiceShipRegion,invoiceShipPostalCode,invoiceShipCountry,isDelete) VALUES (null,@tenantId,@customerId,@shipperId,@employeeId,@invoiceOrderDate,@invoiceRequiredDate,@invoiceShippedDate,@invoiceFreight,@invoiceShipName,@invoiceShipAddress,@invoiceShipCity,@invoiceShipRegion,@invoiceShipPostalCode,@invoiceShipCountry,@isDelete);";
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
                        Key = "@customerId",
                        Value = invoiceModel.CustomerKey
                    },
                    new ()
                    {
                        Key = "@shipperId",
                        Value = invoiceModel.ShipperKey
                    },
                    new ()
                    {
                        Key = "@employeeId",
                        Value = invoiceModel.EmployeeKey
                    },
                    new ()
                    {
                        Key = "@invoiceOrderDate",
                        Value = invoiceModel.InvoiceOrderDate?.ToString("yyyy-MM-dd")
                    },
                    new ()
                    {
                        Key = "@invoiceRequiredDate",
                        Value = invoiceModel.InvoiceRequiredDate?.ToString("yyyy-MM-dd")
                    },
                    new ()
                    {
                        Key = "@invoiceShippedDate",
                        Value = invoiceModel.InvoiceShippedDate?.ToString("yyyy-MM-dd")
                    },
                    new ()
                    {
                        Key = "@invoiceFreight",
                        Value = invoiceModel.InvoiceFreight
                    },
                    new ()
                    {
                        Key = "@invoiceShipName",
                        Value = invoiceModel.InvoiceShipName
                    },
                    new ()
                    {
                        Key = "@invoiceShipAddress",
                        Value = invoiceModel.InvoiceShipAddress
                    },
                    new ()
                    {
                        Key = "@invoiceShipCity",
                        Value = invoiceModel.InvoiceShipCity
                    },
                    new ()
                    {
                        Key = "@invoiceShipRegion",
                        Value = invoiceModel.InvoiceShipRegion
                    },
                    new ()
                    {
                        Key = "@invoiceShipPostalCode",
                        Value = invoiceModel.InvoiceShipPostalCode
                    },
                    new ()
                    {
                        Key = "@invoiceShipCountry",
                        Value = invoiceModel.InvoiceShipCountry
                    },
                    new ()
                    {
                        Key = "@isDelete",
                        Value = 0
                    },

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
    public List<InvoiceModel> Read()
    {
        List<InvoiceModel> invoiceModels = new();
        string sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using MySqlConnection connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql = @"
                SELECT      *
                FROM        invoice 
	 JOIN customer 
	 USING(customerId)
	 JOIN shipper 
	 USING(shipperId)
	 JOIN employee 
	 USING(employeeId)
	 WHERE   invoice.isDelete != 1
                ORDER BY    invoiceId DESC ";
            MySqlCommand mySqlCommand = new(sql, connection);
            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    invoiceModels.Add(new InvoiceModel
                    {
                        InvoiceKey = Convert.ToInt32(reader["invoiceId"]),
                        CustomerName = reader["customerName"].ToString(),
                        CustomerKey = Convert.ToInt32(reader["customerId"]),
                        ShipperName = reader["shipperName"].ToString(),
                        ShipperKey = Convert.ToInt32(reader["shipperId"]),
                        EmployeeLastName = reader["employeeLastName"].ToString(),
                        EmployeeKey = Convert.ToInt32(reader["employeeId"]),
                        InvoiceOrderDate = (reader["invoiceOrderDate"] != DBNull.Value) ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["invoiceOrderDate"]) : null,
                        InvoiceRequiredDate = (reader["invoiceRequiredDate"] != DBNull.Value) ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["invoiceRequiredDate"]) : null,
                        InvoiceShippedDate = (reader["invoiceShippedDate"] != DBNull.Value) ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["invoiceShippedDate"]) : null,
                        InvoiceFreight = Convert.ToDecimal(reader["invoiceFreight"]),
                        InvoiceShipName = reader["invoiceShipName"].ToString(),
                        InvoiceShipAddress = reader["invoiceShipAddress"].ToString(),
                        InvoiceShipCity = reader["invoiceShipCity"].ToString(),
                        InvoiceShipRegion = reader["invoiceShipRegion"].ToString(),
                        InvoiceShipPostalCode = reader["invoiceShipPostalCode"].ToString(),
                        InvoiceShipCountry = reader["invoiceShipCountry"].ToString(),
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
        return invoiceModels;
    }
    public List<InvoiceModel> Search(string search)
    {
        List<InvoiceModel> invoiceModels = new();
        string sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using MySqlConnection connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
                SELECT  *
                FROM    invoice 
	 JOIN customer 
	 USING(customerId)
	 JOIN shipper 
	 USING(shipperId)
	 JOIN employee 
	 USING(employeeId)
	 WHERE   invoice.isDelete != 1
	 AND (	 customer.customerName LIKE CONCAT('%',@search,'%') OR	 customer.customerName LIKE CONCAT('%',@search,'%') OR	 customer.customerName LIKE CONCAT('%',@search,'%') OR	 customer.customerName LIKE CONCAT('%',@search,'%') OR	 customer.customerName LIKE CONCAT('%',@search,'%') OR	 customer.customerName LIKE CONCAT('%',@search,'%') OR	 customer.customerName LIKE CONCAT('%',@search,'%') OR	 customer.customerName LIKE CONCAT('%',@search,'%') OR	 customer.customerName LIKE CONCAT('%',@search,'%') OR	 customer.customerName LIKE CONCAT('%',@search,'%') OR	 customer.customerName LIKE CONCAT('%',@search,'%') OR	 customer.customerName LIKE CONCAT('%',@search,'%') OR	 customer.customerName LIKE CONCAT('%',@search,'%') OR	 customer.customerName LIKE CONCAT('%',@search,'%') OR	 shipper.shipperName LIKE CONCAT('%',@search,'%') OR	 shipper.shipperName LIKE CONCAT('%',@search,'%') OR	 shipper.shipperName LIKE CONCAT('%',@search,'%') OR	 shipper.shipperName LIKE CONCAT('%',@search,'%') OR	 shipper.shipperName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR	 employee.employeeLastName LIKE CONCAT('%',@search,'%') OR
	 invoice.invoiceOrderDate LIKE CONCAT('%',@search,'%') OR
	 invoice.invoiceRequiredDate LIKE CONCAT('%',@search,'%') OR
	 invoice.invoiceShippedDate LIKE CONCAT('%',@search,'%') OR
	 invoice.invoiceFreight LIKE CONCAT('%',@search,'%') OR
	 invoice.invoiceShipName LIKE CONCAT('%',@search,'%') OR
	 invoice.invoiceShipAddress LIKE CONCAT('%',@search,'%') OR
	 invoice.invoiceShipCity LIKE CONCAT('%',@search,'%') OR
	 invoice.invoiceShipRegion LIKE CONCAT('%',@search,'%') OR
	 invoice.invoiceShipPostalCode LIKE CONCAT('%',@search,'%') OR
	 invoice.invoiceShipCountry LIKE CONCAT('%',@search,'%') )";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
                {
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
                    invoiceModels.Add(new InvoiceModel
                    {
                        CustomerName = reader["customerName"].ToString(),
                        CustomerKey = Convert.ToInt32(reader["customerId"]),
                        ShipperName = reader["shipperName"].ToString(),
                        ShipperKey = Convert.ToInt32(reader["shipperId"]),
                        EmployeeLastName = reader["employeeLastName"].ToString(),
                        EmployeeKey = Convert.ToInt32(reader["employeeId"]),
                        InvoiceOrderDate = (reader["invoiceOrderDate"] != DBNull.Value) ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["invoiceOrderDate"]) : null,
                        InvoiceRequiredDate = (reader["invoiceRequiredDate"] != DBNull.Value) ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["invoiceRequiredDate"]) : null,
                        InvoiceShippedDate = (reader["invoiceShippedDate"] != DBNull.Value) ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["invoiceShippedDate"]) : null,
                        InvoiceFreight = Convert.ToDecimal(reader["invoiceFreight"]),
                        InvoiceShipName = reader["invoiceShipName"].ToString(),
                        InvoiceShipAddress = reader["invoiceShipAddress"].ToString(),
                        InvoiceShipCity = reader["invoiceShipCity"].ToString(),
                        InvoiceShipRegion = reader["invoiceShipRegion"].ToString(),
                        InvoiceShipPostalCode = reader["invoiceShipPostalCode"].ToString(),
                        InvoiceShipCountry = reader["invoiceShipCountry"].ToString(),
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
        return invoiceModels;
    }
    public InvoiceModel GetSingle(InvoiceModel invoiceModel)
    {
        string sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using MySqlConnection connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
                SELECT  *
                FROM    invoice 
	 JOIN customer 
	 USING(customerId)
	 JOIN shipper 
	 USING(shipperId)
	 JOIN employee 
	 USING(employeeId)
                WHERE   invoice.isDelete != 1
                AND   invoice.invoiceId    =   @invoiceId LIMIT 1";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@invoiceId",
                        Value = invoiceModel.InvoiceKey
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
                    invoiceModel = new InvoiceModel()
                    {
                        InvoiceKey = Convert.ToInt32(reader["invoiceId"]),
                        CustomerKey = Convert.ToInt32(reader["customerId"]),
                        ShipperKey = Convert.ToInt32(reader["shipperId"]),
                        EmployeeKey = Convert.ToInt32(reader["employeeId"]),
                        InvoiceOrderDate = (reader["invoiceOrderDate"] != DBNull.Value) ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["invoiceOrderDate"]) : null,
                        InvoiceRequiredDate = (reader["invoiceRequiredDate"] != DBNull.Value) ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["invoiceRequiredDate"]) : null,
                        InvoiceShippedDate = (reader["invoiceShippedDate"] != DBNull.Value) ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["invoiceShippedDate"]) : null,
                        InvoiceFreight = Convert.ToDecimal(reader["invoiceFreight"]),
                        InvoiceShipName = reader["invoiceShipName"].ToString(),
                        InvoiceShipAddress = reader["invoiceShipAddress"].ToString(),
                        InvoiceShipCity = reader["invoiceShipCity"].ToString(),
                        InvoiceShipRegion = reader["invoiceShipRegion"].ToString(),
                        InvoiceShipPostalCode = reader["invoiceShipPostalCode"].ToString(),
                        InvoiceShipCountry = reader["invoiceShipCountry"].ToString(),
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
        return invoiceModel;
    }
    public InvoiceModel GetSingleWithDetail(InvoiceModel invoiceModel)
    {
        string sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using MySqlConnection connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
                SELECT  *
                FROM    invoice 
	 JOIN customer 
	 USING(customerId)
	 JOIN shipper 
	 USING(shipperId)
	 JOIN employee 
	 USING(employeeId)
                WHERE   invoice.isDelete != 1
                AND   invoice.invoiceId    =   @invoiceId LIMIT 1";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@invoiceId",
                        Value = invoiceModel.InvoiceKey
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
                    invoiceModel = new InvoiceModel()
                    {
                        InvoiceKey = Convert.ToInt32(reader["invoiceId"]),
                        CustomerKey = Convert.ToInt32(reader["customerId"]),
                        ShipperKey = Convert.ToInt32(reader["shipperId"]),
                        EmployeeKey = Convert.ToInt32(reader["employeeId"]),
                        InvoiceOrderDate = (reader["invoiceOrderDate"] != DBNull.Value) ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["invoiceOrderDate"]) : null,
                        InvoiceRequiredDate = (reader["invoiceRequiredDate"] != DBNull.Value) ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["invoiceRequiredDate"]) : null,
                        InvoiceShippedDate = (reader["invoiceShippedDate"] != DBNull.Value) ? CustomDateTimeConvert.ConvertToDate((DateTime)reader["invoiceShippedDate"]) : null,
                        InvoiceFreight = Convert.ToDecimal(reader["invoiceFreight"]),
                        InvoiceShipName = reader["invoiceShipName"].ToString(),
                        InvoiceShipAddress = reader["invoiceShipAddress"].ToString(),
                        InvoiceShipCity = reader["invoiceShipCity"].ToString(),
                        InvoiceShipRegion = reader["invoiceShipRegion"].ToString(),
                        InvoiceShipPostalCode = reader["invoiceShipPostalCode"].ToString(),
                        InvoiceShipCountry = reader["invoiceShipCountry"].ToString(),
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
        List<InvoiceDetailModel> invoiceDetailModels = new();
        try
        {
            sql = @"
                SELECT      *
                FROM        invoice_detail 
	 JOIN invoice 
	 USING(invoiceId)
	 JOIN product 
	 USING(productId)
	 WHERE   invoice.isDelete != 1
                AND   invoice_detail.invoiceId    =   @invoiceId ";
            MySqlCommand mySqlCommand = new(sql, connection);
            foreach (ParameterModel parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }
            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    invoiceDetailModels.Add(new InvoiceDetailModel()
                    {
                        InvoiceDetailKey = Convert.ToInt32(reader["invoiceDetailId"]),
                        InvoiceKey = Convert.ToInt32(reader["invoiceId"]),
                        ProductKey = Convert.ToInt32(reader["productId"]),
                        InvoiceDetailUnitPrice = Convert.ToDecimal(reader["invoiceDetailUnitPrice"]),
                        InvoiceDetailQuantity = Convert.ToInt32(reader["invoiceDetailQuantity"]),
                        InvoiceDetailDiscount = Convert.ToDouble(reader["invoiceDetailDiscount"]),

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
        if (invoiceDetailModels != null)
        {
            invoiceModel.Data = invoiceDetailModels;
        }
        return invoiceModel;
    }
    public byte[] GetExcel()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Administrator > Invoice ");
        worksheet.Cell(1, 1).Value = "Customer";
        worksheet.Cell(1, 2).Value = "Shipper";
        worksheet.Cell(1, 3).Value = "Employee";
        worksheet.Cell(1, 4).Value = "Order Date";
        worksheet.Cell(1, 5).Value = "Required Date";
        worksheet.Cell(1, 6).Value = "Shipped Date";
        worksheet.Cell(1, 7).Value = "Freight";
        worksheet.Cell(1, 8).Value = "Ship Name";
        worksheet.Cell(1, 9).Value = "Ship Address";
        worksheet.Cell(1, 10).Value = "Ship City";
        worksheet.Cell(1, 11).Value = "Ship Region";
        worksheet.Cell(1, 12).Value = "Ship Postal Code";
        worksheet.Cell(1, 13).Value = "Ship Country";
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
                    worksheet.Cell(currentRow, 2).Value = reader["customerName"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["shipperName"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["employeeLastName"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["invoiceOrderDate"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["invoiceRequiredDate"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["invoiceShippedDate"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["invoiceFreight"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["invoiceShipName"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["invoiceShipAddress"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["invoiceShipCity"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["invoiceShipRegion"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["invoiceShipPostalCode"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["invoiceShipCountry"].ToString();
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
    public void Update(InvoiceModel invoiceModel)
    {
        string sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using MySqlConnection connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            MySqlTransaction mySqlTransaction = connection.BeginTransaction();
            sql = @"
                UPDATE  invoice 
                SET     
tenantId=@tenantId,
customerId=@customerId,
shipperId=@shipperId,
employeeId=@employeeId,
invoiceOrderDate=@invoiceOrderDate,
invoiceRequiredDate=@invoiceRequiredDate,
invoiceShippedDate=@invoiceShippedDate,
invoiceFreight=@invoiceFreight,
invoiceShipName=@invoiceShipName,
invoiceShipAddress=@invoiceShipAddress,
invoiceShipCity=@invoiceShipCity,
invoiceShipRegion=@invoiceShipRegion,
invoiceShipPostalCode=@invoiceShipPostalCode,
invoiceShipCountry=@invoiceShipCountry,
isDelete=@isDelete

                WHERE   invoiceId    =   @invoiceId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@invoiceId",
                        Value = invoiceModel.InvoiceKey
                   },
                    new ()
                    {
                        Key = "@tenantId",
                        Value = _sharedUtil.GetTenantId()
                    },
                    new ()
                    {
                        Key = "@customerId",
                        Value = invoiceModel.CustomerKey
                    },
                    new ()
                    {
                        Key = "@shipperId",
                        Value = invoiceModel.ShipperKey
                    },
                    new ()
                    {
                        Key = "@employeeId",
                        Value = invoiceModel.EmployeeKey
                    },
                    new ()
                    {
                        Key = "@invoiceOrderDate",
                        Value = invoiceModel.InvoiceOrderDate?.ToString("yyyy-MM-dd")
                    },
                    new ()
                    {
                        Key = "@invoiceRequiredDate",
                        Value = invoiceModel.InvoiceRequiredDate?.ToString("yyyy-MM-dd")
                    },
                    new ()
                    {
                        Key = "@invoiceShippedDate",
                        Value = invoiceModel.InvoiceShippedDate?.ToString("yyyy-MM-dd")
                    },
                    new ()
                    {
                        Key = "@invoiceFreight",
                        Value = invoiceModel.InvoiceFreight
                    },
                    new ()
                    {
                        Key = "@invoiceShipName",
                        Value = invoiceModel.InvoiceShipName
                    },
                    new ()
                    {
                        Key = "@invoiceShipAddress",
                        Value = invoiceModel.InvoiceShipAddress
                    },
                    new ()
                    {
                        Key = "@invoiceShipCity",
                        Value = invoiceModel.InvoiceShipCity
                    },
                    new ()
                    {
                        Key = "@invoiceShipRegion",
                        Value = invoiceModel.InvoiceShipRegion
                    },
                    new ()
                    {
                        Key = "@invoiceShipPostalCode",
                        Value = invoiceModel.InvoiceShipPostalCode
                    },
                    new ()
                    {
                        Key = "@invoiceShipCountry",
                        Value = invoiceModel.InvoiceShipCountry
                    },
                    new ()
                    {
                        Key = "@isDelete",
                        Value = 0
                    },

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
    public void Delete(InvoiceModel invoiceModel)
    {
        string sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using MySqlConnection connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            MySqlTransaction mySqlTransaction = connection.BeginTransaction();
            sql = @"
                UPDATE  invoice 
                SET     isDelete    =   1
                WHERE   invoiceId    =   @invoiceId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@invoiceId",
                        Value = invoiceModel.InvoiceKey
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