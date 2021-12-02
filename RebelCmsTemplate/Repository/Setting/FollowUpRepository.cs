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
    public class FollowUpRepository
    {

        private readonly SharedUtil _sharedUtil;
        public FollowUpRepository(IHttpContextAccessor httpContextAccessor)
        {
            _sharedUtil = new SharedUtil(httpContextAccessor);
        }
        public int Create(FollowUpModel followUpModel)
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
                INSERT INTO follow_up  (
                    followUpId,      tenantId,
                    followUpName,   isDelete,
                    executeBy
                ) VALUES (
                    null,                   @tenantId,
                    @followUpName,   0,
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
                        Key = "@followUpName",
                        Value = followUpModel.FollowUpName
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
        public List<FollowUpModel> Read()
        {
            List<FollowUpModel> followUpModels = new();
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {

                connection.Open();
                sql += @"
                SELECT      *
                FROM        follow_up
                WHERE       isDelete != 1
                AND         tenantId = @tenantId
                ORDER BY    followUpId DESC ";
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
                        followUpModels.Add(new FollowUpModel
                        {

                            FollowUpName = reader["followUpName"].ToString(),
                            FollowUpKey = Convert.ToInt32(reader["followUpId"])
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


            return followUpModels;
        }
        public List<FollowUpModel> Search(string search)
        {
            List<FollowUpModel> followUpModels = new();
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {

                connection.Open();
                sql += @"
                SELECT  *
                FROM    follow_up
                WHERE   tenantId = @tenantId
                AND     isDelete != 1
                AND     followUpName like concat('%',@search,'%') ";
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
                        followUpModels.Add(new FollowUpModel
                        {
                            FollowUpName = reader["followUpName"].ToString(),
                            FollowUpKey = Convert.ToInt32(reader["followUpId"])
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


            return followUpModels;
        }
        public byte[] GetExcel()
        {

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Setting > Follow Up ");
            worksheet.Cell(1, 1).Value = "No";
            worksheet.Cell(1, 2).Value = "Name";
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
                        worksheet.Cell(currentRow, 1).Value =  counter -1;
                        worksheet.Cell(currentRow, 2).Value = reader["followUpName"].ToString();
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
        public void Update(FollowUpModel followUpModel)
        {
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {

                connection.Open();
                MySqlTransaction mySqlTransaction = connection.BeginTransaction();

                sql += @"
                UPDATE  follow_up
                SET     followUpName =   @followUpName,
                        executeBy           =   @executeBy
                WHERE   followUpId   =   @followUpId ";
                MySqlCommand mySqlCommand = new(sql, connection);

                parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@followUpName",
                        Value = followUpModel.FollowUpName
                    },
                    new ()
                    {
                        Key = "@executeBy",
                        Value = _sharedUtil.GetUserName()
                    },
                    new ()
                    {
                        Key = "@followUpId",
                        Value = followUpModel.FollowUpKey
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
        public void Delete(FollowUpModel followUpModel)
        {
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {

                connection.Open();
                MySqlTransaction mySqlTransaction = connection.BeginTransaction();

                sql += @"
                UPDATE  follow_up
                SET     isDelete = 1
                WHERE   followUpId  = @followUpId;";
                MySqlCommand mySqlCommand = new(sql, connection);

                mySqlCommand.Parameters.AddWithValue("@followUpId", followUpModel.FollowUpKey);
                parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@followUpId",
                        Value = followUpModel.FollowUpKey
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
