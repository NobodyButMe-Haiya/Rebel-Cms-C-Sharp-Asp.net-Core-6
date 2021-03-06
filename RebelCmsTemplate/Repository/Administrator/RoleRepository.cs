using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Administrator;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Administrator
{
    public class RoleRepository
    {
        private readonly SharedUtil _sharedUtil;

        public RoleRepository(IHttpContextAccessor httpContextAccessor)
        {
            _sharedUtil = new SharedUtil(httpContextAccessor);
        }

        public int Create(RoleModel roleModel)
        {
            // okay next we create skeleton for the code
            int lastInsertKey;
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {
                connection.Open();

                MySqlTransaction mySqlTransaction = connection.BeginTransaction();

                sql += @"
                INSERT INTO role (roleId,tenantId, roleName, isDelete, executeBy) VALUES (null,@tenantId,@roleName,0,@executeBy);";
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
                        Key = "@roleName",
                        Value = roleModel.RoleName
                    },
                    new()
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

              

                lastInsertKey = (int)mySqlCommand.LastInsertedId;

                mySqlCommand.Dispose();
                SetLeafAccess(connection,lastInsertKey);
                mySqlTransaction.Commit();
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _sharedUtil.SetQueryException(SharedUtil.GetSqlSessionValue(sql, parameterModels), ex);
                throw new Exception(ex.Message);
            }

            return lastInsertKey;
        }

        public List<RoleModel> Read()
        {
            List<RoleModel> roleModels = new();
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {
                connection.Open();
                sql += @"
                SELECT      *
                FROM        role
                WHERE       tenantId = @tenantId
                AND         isDelete !=1
                ORDER BY    roleId DESC ";
                MySqlCommand mySqlCommand = new(sql, connection);
                parameterModels = new List<ParameterModel>
                {
                    new()
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
                        roleModels.Add(new RoleModel
                        {
                            RoleName = reader["roleName"].ToString(),
                            RoleKey = Convert.ToInt32(reader["roleId"])
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


            return roleModels;
        }

        public List<RoleModel> Search(string search)
        {
            List<RoleModel> roleModels = new();
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {
                connection.Open();
                sql += @"
                SELECT  *
                FROM    role
                WHERE   tenantId = @tenantId
                AND     isDelete != 1
                AND     roleName like concat('%',@search,'%'); ";
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
                foreach (ParameterModel parameter in parameterModels)
                {
                    mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
                _sharedUtil.SetSqlSession(sql, parameterModels);

                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roleModels.Add(new RoleModel
                        {
                            RoleName = reader["roleName"].ToString(),
                            RoleKey = Convert.ToInt32(reader["roleId"])
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


            return roleModels;
        }

        public byte[] GetExcel()
        {

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Administrator > Role");
            worksheet.Cell(1, 1).Value = "No";
            worksheet.Cell(1, 2).Value = "Role";
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
                        worksheet.Cell(currentRow, 1).Value =  counter-1;
                        worksheet.Cell(currentRow, 2).Value = reader["roleName"].ToString();
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
        public void Update(RoleModel roleModel)
        {
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {
                connection.Open();
                MySqlTransaction mySqlTransaction = connection.BeginTransaction();

                sql += @"
                UPDATE  role
                SET     roleName = @roleName,
                        executeBy = @executeBy
                WHERE   roleId = @roleId ";
                MySqlCommand mySqlCommand = new(sql, connection);
                parameterModels = new List<ParameterModel>
                {
                    new()
                    {
                        Key = "@roleName",
                        Value = roleModel.RoleName
                    },
                    new()
                    {
                        Key = "@executeBy",
                        Value = _sharedUtil.GetUserName()
                    },
                    new()
                    {
                        Key = "@roleId",
                        Value = roleModel.RoleKey
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

        public void Delete(RoleModel roleModel)
        {
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {
                connection.Open();
                MySqlTransaction mySqlTransaction = connection.BeginTransaction();

                sql += @"
                UPDATE  role
                SET     isDelete = 1
                WHERE   roleId  = @roleId;";
                MySqlCommand mySqlCommand = new(sql, connection);

                parameterModels = new List<ParameterModel>
                {
                    new()
                    {
                        Key = "@roleId",
                        Value = roleModel.RoleKey
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
        private static void  SetLeafAccess(MySqlConnection connection,int roleId)
        {
            StringBuilder stringBuilder = new();
            try
            {
                const string sql = @"SELECT * FROM leaf WHERE isDelete != 1 ";
                MySqlCommand mySqlCommand = new(sql, connection);
                
                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stringBuilder.Append("(null, "+reader["leafId"]+", "+roleId+", 0, 0, 0, 0, 0, 0),");
                    }
                }

                mySqlCommand.Dispose();
                string sqlValues = stringBuilder.ToString().TrimEnd(',');
                // re loop  the access  
                string sqlConcat =
                    $@" INSERT INTO leaf_access (
                        leafAccessId,           leafId,                     roleId, 
                        leafAccessCreateValue,  leafAccessReadValue,        leafAccessUpdateValue,
                        leafAccessDeleteValue,  leafAccessExtraOneValue,    leafAccessExtraTwoValue
                    ) VALUES {sqlValues}";
                
                mySqlCommand = new MySqlCommand(sqlConcat, connection);
                mySqlCommand.ExecuteNonQuery();
                mySqlCommand.Dispose();
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}