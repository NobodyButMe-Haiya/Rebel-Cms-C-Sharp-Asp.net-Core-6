using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Setting;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Setting;

public class DocumentNumberRepository
{
    private readonly SharedUtil _sharedUtil;

    public DocumentNumberRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public int Create(DocumentNumberModel documentNumberModel)
    {
        // okay next we create skeleton for the code
        int lastInsertKey;
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();

            var mySqlTransaction = connection.BeginTransaction();

            sql +=
                @"INSERT INTO document_number VALUES (null,@tenantId,@documentNumberCode, @documentNumber,@documentNumberDescription,0,@executeBy);";
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
                    Key = "@documentNumberCode",
                    Value = documentNumberModel.DocumentNumberCode
                },
                new()
                {
                    Key = "@documentNumber",
                    Value = documentNumberModel.DocumentNumber
                },
                new()
                {
                    Key = "@documentNumberDescription",
                    Value = documentNumberModel.DocumentNumberDescription
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

    public List<DocumentNumberModel> Read()
    {
        List<DocumentNumberModel> documentNumberModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
                SELECT      *
                FROM        document_number
                WHERE       isDelete != 1
                ORDER BY    documentNumberId DESC ";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                }
            };
            _sharedUtil.SetSqlSession(sql, parameterModels);

            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    documentNumberModels.Add(new DocumentNumberModel
                    {
                        DocumentNumberCode = reader["documentNumberCode"].ToString(),
                        DocumentNumber = reader["documentNumber"].ToString(),
                        DocumentNumberDescription = reader["documentNumberDescription"].ToString(),
                        DocumentNumberKey = Convert.ToInt32(reader["documentNumberId"])
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


        return documentNumberModels;
    }

    public List<DocumentNumberModel> Search(string search)
    {
        List<DocumentNumberModel> documentNumberModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
                SELECT  *
                FROM    document_number
                WHERE   tenantId = @tenantId
                AND     isDelete != 1
                AND     documentNumberCode LIKE CONCAT('%',@search,'%')
                OR      documentNumberDescription LIKE CONCAT('%',@search,'%'); ";
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
                    documentNumberModels.Add(new DocumentNumberModel
                    {
                        DocumentNumberCode = reader["documentNumberCode"].ToString(),
                        DocumentNumber = reader["documentNumber"].ToString(),
                        DocumentNumberDescription = reader["documentNumberDescription"].ToString(),
                        DocumentNumberKey = Convert.ToInt32(reader["documentNumberId"])
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


        return documentNumberModels;
    }

    public byte[] GetExcel()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Setting > Document Number");

        worksheet.Cell(1, 1).Value = "No";
        worksheet.Cell(1, 2).Value = "Code";
        worksheet.Cell(1, 3).Value = "Current Code";
        worksheet.Cell(1, 4).Value = "Description";


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
                    worksheet.Cell(currentRow, 4).Value = reader["documentNumberDescription"].ToString();
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

    public void Update(DocumentNumberModel documentNumberModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();

            sql += @"
                UPDATE  document_number 
                SET     documentNumberCode = @documentNumberCode, 
                        documentNumber = @documentNumber ,
                        documentNumberDescription = @documentNumberDescription 
                WHERE   documentNumberId = @documentNumberId ";
            MySqlCommand mySqlCommand = new(sql, connection);
            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@documentNumberCode",
                    Value = documentNumberModel.DocumentNumberCode
                },
                new()
                {
                    Key = "@documentNumber",
                    Value = documentNumberModel.DocumentNumber
                },
                new()
                {
                    Key = "@documentNumberDescription",
                    Value = documentNumberModel.DocumentNumberDescription
                },
                new()
                {
                    Key = "@executeBy",
                    Value = _sharedUtil.GetUserName()
                },
                new()
                {
                    Key = "@documentNumberId",
                    Value = documentNumberModel.DocumentNumberKey
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

    public void Delete(DocumentNumberModel documentNumberModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();

            sql += @"
                UPDATE  document_number
                SET     isDelete = 1
                WHERE   documentNumberId  = @documentNumberId;";
            MySqlCommand mySqlCommand = new(sql, connection);

            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@documentNumberId",
                    Value = documentNumberModel.DocumentNumberKey
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