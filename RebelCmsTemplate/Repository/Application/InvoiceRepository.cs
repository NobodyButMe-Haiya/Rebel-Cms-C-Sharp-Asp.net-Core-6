using ClosedXML.Excel;
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
        int lastInsertKey;
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql +=
                @"INSERT INTO invoice (invoiceId,tenantId,customerId,shipperId,employeeId,invoiceOrderDate,invoiceRequiredDate,invoiceShippedDate,invoiceFreight,invoiceShipName,invoiceShipAddress,invoiceShipCity,invoiceShipRegion,invoiceShipPostalCode,invoiceShipCountry,isDelete) VALUES (null,@tenantId,@customerId,@shipperId,@employeeId,@invoiceOrderDate,@invoiceRequiredDate,@invoiceShippedDate,@invoiceFreight,@invoiceShipName,@invoiceShipAddress,@invoiceShipCity,@invoiceShipRegion,@invoiceShipPostalCode,@invoiceShipCountry,@isDelete);";
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
                    Key = "@customerId",
                    Value = invoiceModel.CustomerKey
                },
                new()
                {
                    Key = "@shipperId",
                    Value = invoiceModel.ShipperKey
                },
                new()
                {
                    Key = "@employeeId",
                    Value = invoiceModel.EmployeeKey
                },
                new()
                {
                    Key = "@invoiceOrderDate",
                    Value = invoiceModel.InvoiceOrderDate?.ToString("yyyy-MM-dd")
                },
                new()
                {
                    Key = "@invoiceRequiredDate",
                    Value = invoiceModel.InvoiceRequiredDate?.ToString("yyyy-MM-dd")
                },
                new()
                {
                    Key = "@invoiceShippedDate",
                    Value = invoiceModel.InvoiceShippedDate?.ToString("yyyy-MM-dd")
                },
                new()
                {
                    Key = "@invoiceFreight",
                    Value = invoiceModel.InvoiceFreight
                },
                new()
                {
                    Key = "@invoiceShipName",
                    Value = invoiceModel.InvoiceShipName
                },
                new()
                {
                    Key = "@invoiceShipAddress",
                    Value = invoiceModel.InvoiceShipAddress
                },
                new()
                {
                    Key = "@invoiceShipCity",
                    Value = invoiceModel.InvoiceShipCity
                },
                new()
                {
                    Key = "@invoiceShipRegion",
                    Value = invoiceModel.InvoiceShipRegion
                },
                new()
                {
                    Key = "@invoiceShipPostalCode",
                    Value = invoiceModel.InvoiceShipPostalCode
                },
                new()
                {
                    Key = "@invoiceShipCountry",
                    Value = invoiceModel.InvoiceShipCountry
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

    public List<InvoiceModel> Read()
    {
        List<InvoiceModel> invoiceModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        // the reason limit avoid hang . dom browser can't process a lot of record. Want more paging or ajax paging
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
                ORDER BY    invoiceId DESC LIMIT 100 ";
            MySqlCommand mySqlCommand = new(sql, connection);
            _sharedUtil.SetSqlSession(sql, parameterModels);
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
                        InvoiceOrderDate = reader["invoiceOrderDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime) reader["invoiceOrderDate"])
                            : null,
                        InvoiceRequiredDate = reader["invoiceRequiredDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime) reader["invoiceRequiredDate"])
                            : null,
                        InvoiceShippedDate = reader["invoiceShippedDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime) reader["invoiceShippedDate"])
                            : null,
                        InvoiceFreight = Convert.ToDecimal(reader["invoiceFreight"]),
                        InvoiceShipName = reader["invoiceShipName"].ToString(),
                        InvoiceShipAddress = reader["invoiceShipAddress"].ToString(),
                        InvoiceShipCity = reader["invoiceShipCity"].ToString(),
                        InvoiceShipRegion = reader["invoiceShipRegion"].ToString(),
                        InvoiceShipPostalCode = reader["invoiceShipPostalCode"].ToString(),
                        InvoiceShipCountry = reader["invoiceShipCountry"].ToString()
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
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
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
            AND     invoice.tenantId = @tenantId
     		AND (				customer. LIKE CONCAT('%',@search,'%') OR
				customer.tenantName LIKE CONCAT('%',@search,'%') OR
				customer.customerCode LIKE CONCAT('%',@search,'%') OR
				customer.customerName LIKE CONCAT('%',@search,'%') OR
				customer.customerContactName LIKE CONCAT('%',@search,'%') OR
				customer.customerContactTitle LIKE CONCAT('%',@search,'%') OR
				customer.customerAddress LIKE CONCAT('%',@search,'%') OR
				customer.customerCity LIKE CONCAT('%',@search,'%') OR
				customer.customerRegion LIKE CONCAT('%',@search,'%') OR
				customer.customerPostalCode LIKE CONCAT('%',@search,'%') OR
				customer.customerCountry LIKE CONCAT('%',@search,'%') OR
				customer.customerPhone LIKE CONCAT('%',@search,'%') OR
				customer.customerFax LIKE CONCAT('%',@search,'%') OR
				customer.isDelete LIKE CONCAT('%',@search,'%') OR
				customer.isDefault LIKE CONCAT('%',@search,'%') OR
				shipper. LIKE CONCAT('%',@search,'%') OR
				shipper.tenantName LIKE CONCAT('%',@search,'%') OR
				shipper.shipperName LIKE CONCAT('%',@search,'%') OR
				shipper.shipperPhone LIKE CONCAT('%',@search,'%') OR
				shipper.isDelete LIKE CONCAT('%',@search,'%') OR
				shipper.isDefault LIKE CONCAT('%',@search,'%') OR
				employee. LIKE CONCAT('%',@search,'%') OR
				employee.tenantName LIKE CONCAT('%',@search,'%') OR
				employee.employeeFirstName LIKE CONCAT('%',@search,'%') OR
				employee.employeeLastName LIKE CONCAT('%',@search,'%') OR
				employee.employeeTitle LIKE CONCAT('%',@search,'%') OR
				employee.employeeTitleOfCourtesy LIKE CONCAT('%',@search,'%') OR
				employee.employeeBirthDate LIKE CONCAT('%',@search,'%') OR
				employee.employeeHireDate LIKE CONCAT('%',@search,'%') OR
				employee.employeeAddress LIKE CONCAT('%',@search,'%') OR
				employee.employeeCity LIKE CONCAT('%',@search,'%') OR
				employee.employeeRegion LIKE CONCAT('%',@search,'%') OR
				employee.employeePostalCode LIKE CONCAT('%',@search,'%') OR
				employee.employeeCountry LIKE CONCAT('%',@search,'%') OR
				employee.employeeHomePhone LIKE CONCAT('%',@search,'%') OR
				employee.employeeExtension LIKE CONCAT('%',@search,'%') OR
				employee.employeePhoto LIKE CONCAT('%',@search,'%') OR
				employee.employeeNotes LIKE CONCAT('%',@search,'%') OR
				employee.employeePhotoPath LIKE CONCAT('%',@search,'%') OR
				employee.employeeSalary LIKE CONCAT('%',@search,'%') OR
				employee.isDelete LIKE CONCAT('%',@search,'%') OR
				employee.isDefault LIKE CONCAT('%',@search,'%') OR
				invoice.invoiceOrderDate LIKE CONCAT('%',@search,'%') OR
				invoice.invoiceRequiredDate LIKE CONCAT('%',@search,'%') OR
				invoice.invoiceShippedDate LIKE CONCAT('%',@search,'%') OR
				invoice.invoiceFreight LIKE CONCAT('%',@search,'%') OR
				invoice.invoiceShipName LIKE CONCAT('%',@search,'%') OR
				invoice.invoiceShipAddress LIKE CONCAT('%',@search,'%') OR
				invoice.invoiceShipCity LIKE CONCAT('%',@search,'%') OR
				invoice.invoiceShipRegion LIKE CONCAT('%',@search,'%') OR
				invoice.invoiceShipPostalCode LIKE CONCAT('%',@search,'%') OR
				invoice.invoiceShipCountry LIKE CONCAT('%',@search,'%')
            ) LIMIT 100 ";
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
                    invoiceModels.Add(new InvoiceModel
                    {
                        CustomerName = reader["customerName"].ToString(),
                        CustomerKey = Convert.ToInt32(reader["customerId"]),
                        ShipperName = reader["shipperName"].ToString(),
                        ShipperKey = Convert.ToInt32(reader["shipperId"]),
                        EmployeeLastName = reader["employeeLastName"].ToString(),
                        EmployeeKey = Convert.ToInt32(reader["employeeId"]),
                        InvoiceOrderDate = reader["invoiceOrderDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime) reader["invoiceOrderDate"])
                            : null,
                        InvoiceRequiredDate = reader["invoiceRequiredDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime) reader["invoiceRequiredDate"])
                            : null,
                        InvoiceShippedDate = reader["invoiceShippedDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime) reader["invoiceShippedDate"])
                            : null,
                        InvoiceFreight = Convert.ToDecimal(reader["invoiceFreight"]),
                        InvoiceShipName = reader["invoiceShipName"].ToString(),
                        InvoiceShipAddress = reader["invoiceShipAddress"].ToString(),
                        InvoiceShipCity = reader["invoiceShipCity"].ToString(),
                        InvoiceShipRegion = reader["invoiceShipRegion"].ToString(),
                        InvoiceShipPostalCode = reader["invoiceShipPostalCode"].ToString(),
                        InvoiceShipCountry = reader["invoiceShipCountry"].ToString()
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
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
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

            WHERE   invoice.isDelete    !=  1
            AND     invoice.tenantId    =   @tenantId
            AND   invoice.invoiceId     =   @invoiceId LIMIT 1";
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
                    Key = "@invoiceId",
                    Value = invoiceModel.InvoiceKey
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
                    invoiceModel = new InvoiceModel
                    {
                        InvoiceKey = Convert.ToInt32(reader["invoiceId"]),
                        CustomerKey = Convert.ToInt32(reader["customerId"]),
                        ShipperKey = Convert.ToInt32(reader["shipperId"]),
                        EmployeeKey = Convert.ToInt32(reader["employeeId"]),
                        InvoiceOrderDate = reader["invoiceOrderDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime) reader["invoiceOrderDate"])
                            : null,
                        InvoiceRequiredDate = reader["invoiceRequiredDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime) reader["invoiceRequiredDate"])
                            : null,
                        InvoiceShippedDate = reader["invoiceShippedDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime) reader["invoiceShippedDate"])
                            : null,
                        InvoiceFreight = Convert.ToDecimal(reader["invoiceFreight"]),
                        InvoiceShipName = reader["invoiceShipName"].ToString(),
                        InvoiceShipAddress = reader["invoiceShipAddress"].ToString(),
                        InvoiceShipCity = reader["invoiceShipCity"].ToString(),
                        InvoiceShipRegion = reader["invoiceShipRegion"].ToString(),
                        InvoiceShipPostalCode = reader["invoiceShipPostalCode"].ToString(),
                        InvoiceShipCountry = reader["invoiceShipCountry"].ToString()
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
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
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

            WHERE   invoice.isDelete !  =   1
            AND     invoice.tenantId    =   @tenantId
            AND     invoice.invoiceId   =   @invoiceId
            LIMIT 1";
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
                    Key = "@invoiceId",
                    Value = invoiceModel.InvoiceKey
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
                    invoiceModel = new InvoiceModel
                    {
                        InvoiceKey = Convert.ToInt32(reader["invoiceId"]),
                        CustomerKey = Convert.ToInt32(reader["customerId"]),
                        ShipperKey = Convert.ToInt32(reader["shipperId"]),
                        EmployeeKey = Convert.ToInt32(reader["employeeId"]),
                        InvoiceOrderDate = reader["invoiceOrderDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime) reader["invoiceOrderDate"])
                            : null,
                        InvoiceRequiredDate = reader["invoiceRequiredDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime) reader["invoiceRequiredDate"])
                            : null,
                        InvoiceShippedDate = reader["invoiceShippedDate"] != DBNull.Value
                            ? CustomDateTimeConvert.ConvertToDate((DateTime) reader["invoiceShippedDate"])
                            : null,
                        InvoiceFreight = Convert.ToDecimal(reader["invoiceFreight"]),
                        InvoiceShipName = reader["invoiceShipName"].ToString(),
                        InvoiceShipAddress = reader["invoiceShipAddress"].ToString(),
                        InvoiceShipCity = reader["invoiceShipCity"].ToString(),
                        InvoiceShipRegion = reader["invoiceShipRegion"].ToString(),
                        InvoiceShipPostalCode = reader["invoiceShipPostalCode"].ToString(),
                        InvoiceShipCountry = reader["invoiceShipCountry"].ToString()
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

            WHERE   invoice.isDelete        !=  1
            AND     invoice.tenantId        =   @tenantId
            AND     invoice_detail.isDelete != 1
            AND   invoice_detail.invoiceId  =   @invoiceId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                }
            };
            foreach (var parameter in parameterModels)
            {
                mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

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
                        InvoiceDetailDiscount = Convert.ToDouble(reader["invoiceDetailDiscount"])
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


        invoiceModel.Data = invoiceDetailModels;


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
                    worksheet.Cell(currentRow, 1).Value = reader["customerName"].ToString();
                    worksheet.Cell(currentRow, 2).Value = reader["shipperName"].ToString();
                    worksheet.Cell(currentRow, 3).Value = reader["employeeLastName"].ToString();
                    worksheet.Cell(currentRow, 4).Value = reader["invoiceOrderDate"].ToString();
                    worksheet.Cell(currentRow, 5).Value = reader["invoiceRequiredDate"].ToString();
                    worksheet.Cell(currentRow, 6).Value = reader["invoiceShippedDate"].ToString();
                    worksheet.Cell(currentRow, 7).Value = reader["invoiceFreight"].ToString();
                    worksheet.Cell(currentRow, 8).Value = reader["invoiceShipName"].ToString();
                    worksheet.Cell(currentRow, 9).Value = reader["invoiceShipAddress"].ToString();
                    worksheet.Cell(currentRow, 10).Value = reader["invoiceShipCity"].ToString();
                    worksheet.Cell(currentRow, 11).Value = reader["invoiceShipRegion"].ToString();
                    worksheet.Cell(currentRow, 12).Value = reader["invoiceShipPostalCode"].ToString();
                    worksheet.Cell(currentRow, 13).Value = reader["invoiceShipCountry"].ToString();
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
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql = @"
            UPDATE  invoice 
            SET     customerId              =   @customerId,
                    shipperId               =   @shipperId,
                    employeeId              =   @employeeId,
                    invoiceOrderDate        =   @invoiceOrderDate,
                    invoiceRequiredDate     =   @invoiceRequiredDate,
                    invoiceShippedDate      =   @invoiceShippedDate,
                    invoiceFreight          =   @invoiceFreight,
                    invoiceShipName         =   @invoiceShipName,
                    invoiceShipAddress      =   @invoiceShipAddress,
                    invoiceShipCity         =   @invoiceShipCity,
                    invoiceShipRegion       =   @invoiceShipRegion,
                    invoiceShipPostalCode   =   @invoiceShipPostalCode,
                    invoiceShipCountry      =   @invoiceShipCountry

            WHERE   invoiceId    =   @invoiceId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@invoiceId",
                    Value = invoiceModel.InvoiceKey
                },
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                },
                new()
                {
                    Key = "@customerId",
                    Value = invoiceModel.CustomerKey
                },
                new()
                {
                    Key = "@shipperId",
                    Value = invoiceModel.ShipperKey
                },
                new()
                {
                    Key = "@employeeId",
                    Value = invoiceModel.EmployeeKey
                },
                new()
                {
                    Key = "@invoiceOrderDate",
                    Value = invoiceModel.InvoiceOrderDate?.ToString("yyyy-MM-dd")
                },
                new()
                {
                    Key = "@invoiceRequiredDate",
                    Value = invoiceModel.InvoiceRequiredDate?.ToString("yyyy-MM-dd")
                },
                new()
                {
                    Key = "@invoiceShippedDate",
                    Value = invoiceModel.InvoiceShippedDate?.ToString("yyyy-MM-dd")
                },
                new()
                {
                    Key = "@invoiceFreight",
                    Value = invoiceModel.InvoiceFreight
                },
                new()
                {
                    Key = "@invoiceShipName",
                    Value = invoiceModel.InvoiceShipName
                },
                new()
                {
                    Key = "@invoiceShipAddress",
                    Value = invoiceModel.InvoiceShipAddress
                },
                new()
                {
                    Key = "@invoiceShipCity",
                    Value = invoiceModel.InvoiceShipCity
                },
                new()
                {
                    Key = "@invoiceShipRegion",
                    Value = invoiceModel.InvoiceShipRegion
                },
                new()
                {
                    Key = "@invoiceShipPostalCode",
                    Value = invoiceModel.InvoiceShipPostalCode
                },
                new()
                {
                    Key = "@invoiceShipCountry",
                    Value = invoiceModel.InvoiceShipCountry
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

    public void Delete(InvoiceModel invoiceModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();
            sql = @"
            UPDATE  invoice 
            SET     isDelete    =   1
            WHERE   invoiceId    =   @invoiceId";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@invoiceId",
                    Value = invoiceModel.InvoiceKey
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