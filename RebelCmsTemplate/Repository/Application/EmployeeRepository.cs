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
    public class EmployeeRepository
    {
        private readonly SharedUtil _sharedUtil;
        public EmployeeRepository(IHttpContextAccessor httpContextAccessor)
        {
            _sharedUtil = new SharedUtil(httpContextAccessor);
        }
        public int Create(EmployeeModel employeeModel)
        {
            var lastInsertKey = 0;
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();
            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {
                connection.Open();
                MySqlTransaction mySqlTransaction = connection.BeginTransaction();
                sql += @"INSERT INTO employee (employeeId,tenantId,employeeLastName,employeeFirstName,employeeTitle,employeeTitleOfCourtesy,employeeBirthDate,employeeHireDate,employeeAddress,employeeCity,employeeRegion,employeePostalCode,employeeCountry,employeeHomePhone,employeeExtension,employeePhoto,employeeNotes,employeePhotoPath,employeeSalary,isDelete) VALUES (null,@tenantId,@employeeLastName,@employeeFirstName,@employeeTitle,@employeeTitleOfCourtesy,@employeeBirthDate,@employeeHireDate,@employeeAddress,@employeeCity,@employeeRegion,@employeePostalCode,@employeeCountry,@employeeHomePhone,@employeeExtension,@employeePhoto,@employeeNotes,@employeePhotoPath,@employeeSalary,@isDelete);";
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
                        Key = "@employeeLastName",
                        Value = employeeModel.EmployeeLastName
                    },
                    new ()
                    {
                        Key = "@employeeFirstName",
                        Value = employeeModel.EmployeeFirstName
                    },
                    new ()
                    {
                        Key = "@employeeTitle",
                        Value = employeeModel.EmployeeTitle
                    },
                    new ()
                    {
                        Key = "@employeeTitleOfCourtesy",
                        Value = employeeModel.EmployeeTitleOfCourtesy
                    },
                    new ()
                    {
                        Key = "@employeeBirthDate",
                        Value = employeeModel.EmployeeBirthDate?.ToString("yyyy-MM-dd")
                    },
                    new ()
                    {
                        Key = "@employeeHireDate",
                        Value = employeeModel.EmployeeHireDate?.ToString("yyyy-MM-dd")
                    },
                    new ()
                    {
                        Key = "@employeeAddress",
                        Value = employeeModel.EmployeeAddress
                    },
                    new ()
                    {
                        Key = "@employeeCity",
                        Value = employeeModel.EmployeeCity
                    },
                    new ()
                    {
                        Key = "@employeeRegion",
                        Value = employeeModel.EmployeeRegion
                    },
                    new ()
                    {
                        Key = "@employeePostalCode",
                        Value = employeeModel.EmployeePostalCode
                    },
                    new ()
                    {
                        Key = "@employeeCountry",
                        Value = employeeModel.EmployeeCountry
                    },
                    new ()
                    {
                        Key = "@employeeHomePhone",
                        Value = employeeModel.EmployeeHomePhone
                    },
                    new ()
                    {
                        Key = "@employeeExtension",
                        Value = employeeModel.EmployeeExtension
                    },
                    new ()
                    {
                        Key = "@employeePhoto",
                        Value = employeeModel.EmployeePhoto
                    },
                    new ()
                    {
                        Key = "@employeeNotes",
                        Value = employeeModel.EmployeeNotes
                    },
                    new ()
                    {
                        Key = "@employeePhotoPath",
                        Value = employeeModel.EmployeePhotoPath
                    },
                    new ()
                    {
                        Key = "@employeeSalary",
                        Value = employeeModel.EmployeeSalary
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
        public List<EmployeeModel> Read()
        {
            List<EmployeeModel> employeeModels = new();
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();
            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {
                connection.Open();
                sql = @"
                SELECT      *
                FROM        employee 
                WHERE       isDelete !=1
                ORDER BY    employeeId DESC ";
                MySqlCommand mySqlCommand = new(sql, connection);
                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employeeModels.Add(new EmployeeModel
                       {
                            EmployeeKey = Convert.ToInt32(reader["employeeId"]),
                            TenantKey = Convert.ToInt32(reader["tenantId"]),
                            EmployeeLastName = reader["employeeLastName"].ToString(),
                            EmployeeFirstName = reader["employeeFirstName"].ToString(),
                            EmployeeTitle = reader["employeeTitle"].ToString(),
                            EmployeeTitleOfCourtesy = reader["employeeTitleOfCourtesy"].ToString(),
                            EmployeeBirthDate = CustomDateTimeConvert.ConvertToDate(reader["employeeBirthDate"].ToString()),
                            EmployeeHireDate = CustomDateTimeConvert.ConvertToDate(reader["employeeHireDate"].ToString()),
                            EmployeeAddress = reader["employeeAddress"].ToString(),
                            EmployeeCity = reader["employeeCity"].ToString(),
                            EmployeeRegion = reader["employeeRegion"].ToString(),
                            EmployeePostalCode = reader["employeePostalCode"].ToString(),
                            EmployeeCountry = reader["employeeCountry"].ToString(),
                            EmployeeHomePhone = reader["employeeHomePhone"].ToString(),
                            EmployeeExtension = reader["employeeExtension"].ToString(),
                            EmployeePhoto = (byte[])reader["employeePhoto"],
                            EmployeePhotoBase64String = Convert.ToBase64String((byte[])reader["employeePhoto"]),
                            EmployeeNotes = reader["employeeNotes"].ToString(),
                            EmployeePhotoPath = reader["employeePhotoPath"].ToString(),
                            EmployeeSalary = Convert.ToDouble(reader["employeeSalary"]),
                            IsDelete = Convert.ToInt32(reader["isDelete"]),
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
            return employeeModels;
        }
        public List<EmployeeModel> Search(string search)
       {
            List<EmployeeModel> employeeModels = new();
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();
            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {
                connection.Open();
                sql += @"
                SELECT  *
                FROM    employee 
	 WHERE   employee.isDelete != 1
	 AND (
	 employee.employeeLastName like concat('%',@search,'%') OR
	 employee.employeeFirstName like concat('%',@search,'%') OR
	 employee.employeeTitle like concat('%',@search,'%') OR
	 employee.employeeTitleOfCourtesy like concat('%',@search,'%') OR
	 employee.employeeBirthDate like concat('%',@search,'%') OR
	 employee.employeeHireDate like concat('%',@search,'%') OR
	 employee.employeeAddress like concat('%',@search,'%') OR
	 employee.employeeCity like concat('%',@search,'%') OR
	 employee.employeeRegion like concat('%',@search,'%') OR
	 employee.employeePostalCode like concat('%',@search,'%') OR
	 employee.employeeCountry like concat('%',@search,'%') OR
	 employee.employeeHomePhone like concat('%',@search,'%') OR
	 employee.employeeExtension like concat('%',@search,'%') OR
	 employee.employeePhoto like concat('%',@search,'%') OR
	 employee.employeeNotes like concat('%',@search,'%') OR
	 employee.employeePhotoPath like concat('%',@search,'%') OR
	 employee.employeeSalary like concat('%',@search,'%') )";
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
                        employeeModels.Add(new EmployeeModel
                       {
                            EmployeeKey = Convert.ToInt32(reader["employeeId"]),
                            TenantKey = Convert.ToInt32(reader["tenantId"]),
                            EmployeeLastName = reader["employeeLastName"].ToString(),
                            EmployeeFirstName = reader["employeeFirstName"].ToString(),
                            EmployeeTitle = reader["employeeTitle"].ToString(),
                            EmployeeTitleOfCourtesy = reader["employeeTitleOfCourtesy"].ToString(),
                            EmployeeBirthDate = CustomDateTimeConvert.ConvertToDate(reader["employeeBirthDate"].ToString()),
                            EmployeeHireDate = CustomDateTimeConvert.ConvertToDate(reader["employeeHireDate"].ToString()),
                            EmployeeAddress = reader["employeeAddress"].ToString(),
                            EmployeeCity = reader["employeeCity"].ToString(),
                            EmployeeRegion = reader["employeeRegion"].ToString(),
                            EmployeePostalCode = reader["employeePostalCode"].ToString(),
                            EmployeeCountry = reader["employeeCountry"].ToString(),
                            EmployeeHomePhone = reader["employeeHomePhone"].ToString(),
                            EmployeeExtension = reader["employeeExtension"].ToString(),
                            EmployeePhoto = (byte[])reader["employeePhoto"],
                            EmployeeNotes = reader["employeeNotes"].ToString(),
                            EmployeePhotoPath = reader["employeePhotoPath"].ToString(),
                            EmployeeSalary = Convert.ToDouble(reader["employeeSalary"]),
                            IsDelete = Convert.ToInt32(reader["isDelete"]),
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
            return employeeModels;
        }
        public EmployeeModel  GetSingle(EmployeeModel employeeModel)
        {
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();
            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {
                connection.Open();
                sql += @"
                SELECT  *
                FROM    employee 
	            WHERE   employee.isDelete != 1
                AND   employee.employeeId    =   @employeeId LIMIT 1";
                MySqlCommand mySqlCommand = new(sql, connection);
                parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@employeeId",
                        Value = employeeModel.EmployeeKey
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
employeeModel = new EmployeeModel() { 
EmployeeKey = Convert.ToInt32(reader["employeeId"]),
TenantKey = Convert.ToInt32(reader["tenantId"]),
EmployeeLastName = reader["employeeLastName"].ToString(),
EmployeeFirstName = reader["employeeFirstName"].ToString(),
EmployeeTitle = reader["employeeTitle"].ToString(),
EmployeeTitleOfCourtesy = reader["employeeTitleOfCourtesy"].ToString(),
EmployeeBirthDate = CustomDateTimeConvert.ConvertToDate(reader["employeeBirthDate"].ToString()),
EmployeeHireDate = CustomDateTimeConvert.ConvertToDate(reader["employeeHireDate"].ToString()),
EmployeeAddress = reader["employeeAddress"].ToString(),
EmployeeCity = reader["employeeCity"].ToString(),
EmployeeRegion = reader["employeeRegion"].ToString(),
EmployeePostalCode = reader["employeePostalCode"].ToString(),
EmployeeCountry = reader["employeeCountry"].ToString(),
EmployeeHomePhone = reader["employeeHomePhone"].ToString(),
EmployeeExtension = reader["employeeExtension"].ToString(),
EmployeePhoto = (byte[])reader["employeePhoto"],
EmployeeNotes = reader["employeeNotes"].ToString(),
EmployeePhotoPath = reader["employeePhotoPath"].ToString(),
EmployeeSalary = Convert.ToDouble(reader["employeeSalary"]),
IsDelete = Convert.ToInt32(reader["isDelete"]),
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
            return employeeModel;
        }
        public byte[] GetExcel()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Administrator > Employee ");
            worksheet.Cell(1, 1).Value = "employeeId";
            worksheet.Cell(1, 2).Value = "tenantId";
            worksheet.Cell(1, 3).Value = "employeeLastName";
            worksheet.Cell(1, 4).Value = "employeeFirstName";
            worksheet.Cell(1, 5).Value = "employeeTitle";
            worksheet.Cell(1, 6).Value = "employeeTitleOfCourtesy";
            worksheet.Cell(1, 7).Value = "employeeBirthDate";
            worksheet.Cell(1, 8).Value = "employeeHireDate";
            worksheet.Cell(1, 9).Value = "employeeAddress";
            worksheet.Cell(1, 10).Value = "employeeCity";
            worksheet.Cell(1, 11).Value = "employeeRegion";
            worksheet.Cell(1, 12).Value = "employeePostalCode";
            worksheet.Cell(1, 13).Value = "employeeCountry";
            worksheet.Cell(1, 14).Value = "employeeHomePhone";
            worksheet.Cell(1, 15).Value = "employeeExtension";
            worksheet.Cell(1, 16).Value = "employeePhoto";
            worksheet.Cell(1, 17).Value = "employeeNotes";
            worksheet.Cell(1, 18).Value = "employeePhotoPath";
            worksheet.Cell(1, 19).Value = "employeeSalary";
            worksheet.Cell(1, 20).Value = "isDelete";
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
                        worksheet.Cell(currentRow, 2).Value = reader["employeeId"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["tenantId"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["employeeLastName"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["employeeFirstName"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["employeeTitle"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["employeeTitleOfCourtesy"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["employeeBirthDate"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["employeeHireDate"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["employeeAddress"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["employeeCity"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["employeeRegion"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["employeePostalCode"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["employeeCountry"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["employeeHomePhone"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["employeeExtension"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["employeePhoto"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["employeeNotes"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["employeePhotoPath"].ToString();
                        worksheet.Cell(currentRow, 2).Value = reader["employeeSalary"].ToString();
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
        public void Update(EmployeeModel employeeModel)
        {
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();
            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {
                connection.Open();
                MySqlTransaction mySqlTransaction = connection.BeginTransaction();
                sql = @"
                UPDATE  employee 
                SET     
tenantId=@tenantId,
employeeLastName=@employeeLastName,
employeeFirstName=@employeeFirstName,
employeeTitle=@employeeTitle,
employeeTitleOfCourtesy=@employeeTitleOfCourtesy,
employeeBirthDate=@employeeBirthDate,
employeeHireDate=@employeeHireDate,
employeeAddress=@employeeAddress,
employeeCity=@employeeCity,
employeeRegion=@employeeRegion,
employeePostalCode=@employeePostalCode,
employeeCountry=@employeeCountry,
employeeHomePhone=@employeeHomePhone,
employeeExtension=@employeeExtension,
employeePhoto=@employeePhoto,
employeeNotes=@employeeNotes,
employeePhotoPath=@employeePhotoPath,
employeeSalary=@employeeSalary,
isDelete=@isDelete

                WHERE   employeeId    =   @employeeId";
                MySqlCommand mySqlCommand = new(sql, connection);
                parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@employeeId",
                        Value = employeeModel.EmployeeKey
                   },
                    new ()
                    {
                        Key = "@tenantId",
                        Value = _sharedUtil.GetTenantId()
                    },
                    new ()
                    {
                        Key = "@employeeLastName",
                        Value = employeeModel.EmployeeLastName
                    },
                    new ()
                    {
                        Key = "@employeeFirstName",
                        Value = employeeModel.EmployeeFirstName
                    },
                    new ()
                    {
                        Key = "@employeeTitle",
                        Value = employeeModel.EmployeeTitle
                    },
                    new ()
                    {
                        Key = "@employeeTitleOfCourtesy",
                        Value = employeeModel.EmployeeTitleOfCourtesy
                    },
                    new ()
                    {
                        Key = "@employeeBirthDate",
                        Value = employeeModel.EmployeeBirthDate?.ToString("yyyy-MM-dd")
                    },
                    new ()
                    {
                        Key = "@employeeHireDate",
                        Value = employeeModel.EmployeeHireDate?.ToString("yyyy-MM-dd")
                    },
                    new ()
                    {
                        Key = "@employeeAddress",
                        Value = employeeModel.EmployeeAddress
                    },
                    new ()
                    {
                        Key = "@employeeCity",
                        Value = employeeModel.EmployeeCity
                    },
                    new ()
                    {
                        Key = "@employeeRegion",
                        Value = employeeModel.EmployeeRegion
                    },
                    new ()
                    {
                        Key = "@employeePostalCode",
                        Value = employeeModel.EmployeePostalCode
                    },
                    new ()
                    {
                        Key = "@employeeCountry",
                        Value = employeeModel.EmployeeCountry
                    },
                    new ()
                    {
                        Key = "@employeeHomePhone",
                        Value = employeeModel.EmployeeHomePhone
                    },
                    new ()
                    {
                        Key = "@employeeExtension",
                        Value = employeeModel.EmployeeExtension
                    },
                    new ()
                    {
                        Key = "@employeePhoto",
                        Value = employeeModel.EmployeePhoto
                    },
                    new ()
                    {
                        Key = "@employeeNotes",
                        Value = employeeModel.EmployeeNotes
                    },
                    new ()
                    {
                        Key = "@employeePhotoPath",
                        Value = employeeModel.EmployeePhotoPath
                    },
                    new ()
                    {
                        Key = "@employeeSalary",
                        Value = employeeModel.EmployeeSalary
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
        public void Delete(EmployeeModel employeeModel)
        {
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();
            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {
                connection.Open();
                MySqlTransaction mySqlTransaction = connection.BeginTransaction();
                sql = @"
                UPDATE  employee 
                SET     isDelete    =   1
                WHERE   employeeId    =   @employeeId";
                MySqlCommand mySqlCommand = new(sql, connection);
                parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@employeeId",
                        Value = employeeModel.EmployeeKey
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
