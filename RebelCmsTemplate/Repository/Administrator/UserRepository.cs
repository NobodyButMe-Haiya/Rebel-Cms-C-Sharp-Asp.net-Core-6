using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Administrator;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Administrator
{
    public partial class UserRepository
    {
        private readonly SharedUtil _sharedUtil;

        public UserRepository(IHttpContextAccessor httpContextAccessor)
        {
            _sharedUtil = new SharedUtil(httpContextAccessor);
        }

        public int Create(UserModel userModel)
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
                INSERT INTO `user` (
                    `userId`,       `tenantId`,     `roleId`,
                    `userName`,     `userPassword`, 
                    `userEmail`,    `isDelete`,     `isConform`,
                    `executeBy`,`executeTime`
                ) VALUES (
                    null,           @tenantId,      @roleId,
                    @userName,      @userPassword,  
                    @userEmail,     0,              0,
                    @executeBy,NOW()
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
                        Key = "@roleId",
                        Value = userModel.RoleKey
                    },
                    new ()
                    {
                        Key = "@userName",
                        Value = userModel.UserName
                    },
                    new ()
                    {
                        Key = "@userPassword",
                        Value = CheckAccessUtil.CalculateSha256(userModel.UserPassword)
                    },
                    new ()
                    {
                        Key = "@userEmail",
                        Value = userModel.UserEmail
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

             

                lastInsertKey = (int) mySqlCommand.LastInsertedId;

                mySqlCommand.Dispose();
                
                
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

        public List<UserModel> Read()
        {
            List<UserModel> userModels = new();
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {
                connection.Open();
                sql += @"
                SELECT      *
                FROM        user
                WHERE       tenantId = @tenantId
                AND         isDelete != 1
                ORDER BY    userId DESC ";
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
                        userModels.Add(new UserModel
                        {
                            RoleKey = Convert.ToInt32(reader["roleId"]),
                            UserName = reader["userName"].ToString(),
                            UserEmail = reader["userEmail"].ToString(),
                            UserPhone = reader["userPhone"].ToString(),
                            UserAddress = reader["userAddress"].ToString(),
                            UserKey = Convert.ToInt32(reader["userId"])
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


            return userModels;
        }

        public List<UserModel> Search(string search)
        {
            List<UserModel> userModels = new();
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {
                connection.Open();
                sql += @"
                SELECT  *
                FROM    user
                WHERE   tenantId = @tenantId
                AND     isDelete != 1
                AND     userName like concat('%',@search,'%')
                OR      userEmail like concat('%',@search,'%'); ";
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
                        userModels.Add(new UserModel
                        {
                            RoleKey = Convert.ToInt32(reader["roleId"]),
                            UserName = reader["userName"].ToString(),
                            UserEmail = reader["userEmail"].ToString(),
                            UserPhone = reader["userPhone"].ToString(),
                            UserAddress = reader["userAddress"].ToString(),
                            UserKey = Convert.ToInt32(reader["userId"])
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


            return userModels;
        }
        public byte[] GetExcel()
        {

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Administrator > User");
            worksheet.Cell(1, 1).Value = "No";
            worksheet.Cell(1, 2).Value = "Role";
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
                        worksheet.Cell(currentRow, 1).Value = counter-1;
                        worksheet.Cell(currentRow, 2).Value = reader["roleName"].ToString();
                        worksheet.Cell(currentRow, 3).Value = reader["userName"].ToString();

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

        public void Update(UserModel userModel)
        {
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {
                connection.Open();
                MySqlTransaction mySqlTransaction = connection.BeginTransaction();

                sql += userModel.UserPassword is not null ? @"
                    UPDATE  `user` 
                    SET     `roleId`=@roleId,
                            `userName`=@userName,
                            `userPassword`=@userPassword,
                            `userEmail`=@userEmail,
                            `executeBy`=@executeBy,
                            `executeTime`= NOW() 
                    WHERE   userId = @userId " : @"
                    UPDATE  `user` 
                    SET     `roleId`=@roleId,
                            `userName`=@userName,
                            `userEmail`=@userEmail,
                            `executeBy`=@executeBy,
                            `executeTime`= NOW() 
                    WHERE   `userId` = @userId ";

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
                        Key = "@roleId",
                        Value = userModel.RoleKey
                    },
                    new ()
                    {
                        Key = "@userName",
                        Value = userModel.UserName
                    },
                    new ()
                    {
                        Key = "@userEmail",
                        Value = userModel.UserEmail
                    },
                    new ()
                    {
                        Key = "@executeBy",
                        Value = _sharedUtil.GetUserName()
                    },
                    new ()
                    {
                        Key = "@userId",
                        Value = userModel.UserKey
                    }
                };
                if (userModel.UserPassword is not null)
                {
                    parameterModels.Add(new ParameterModel
                    {
                        Key = "@userPassword",
                        Value = CheckAccessUtil.CalculateSha256(userModel.UserPassword)
                    });
                }
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

        public void Delete(UserModel userModel)
        {
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {
                connection.Open();
                MySqlTransaction mySqlTransaction = connection.BeginTransaction();

                sql += @"
                UPDATE  user
                SET     isDelete = 1
                WHERE   userId  = @userId;";
                MySqlCommand mySqlCommand = new(sql, connection);

                mySqlCommand.Parameters.AddWithValue("@userId", userModel.UserKey);
                parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@userId",
                        Value = userModel.UserKey
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

    // non standard crud here 

    
}