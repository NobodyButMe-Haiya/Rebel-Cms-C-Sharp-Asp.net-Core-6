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
    public class LogSystemRepository
    {
        private readonly SharedUtil _sharedUtil;

        public LogSystemRepository(IHttpContextAccessor httpContextAccessor)
        {
            _sharedUtil = new SharedUtil(httpContextAccessor);
        }

        public List<LogSystemModel> Read()
        {
            List<LogSystemModel> logSystemModels = new();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {
                connection.Open();
                const string sql = @"
                SELECT      * 
                FROM        log_system 
                ORDER BY    logSystemId DESC ";
                MySqlCommand mySqlCommand = new(sql, connection);
                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logSystemModels.Add(new LogSystemModel
                        {
                            LogSystemKey = Convert.ToInt32(reader["logSystemId"]),
                            LogSystemQuery = reader["logQuery"].ToString(),
                            LogSystemDateTime = Convert.ToDateTime(reader["logDateTime"])
                        });
                    }
                }

                mySqlCommand.Dispose();
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _sharedUtil.SetSystemException(ex);
                throw new Exception(ex.Message);
            }


            return logSystemModels;
        }

        public List<LogSystemModel> Search(string search)
        {
            List<LogSystemModel> logSystemModels = new();
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {
                connection.Open();
                sql += @"
                SELECT  * 
                FROM    log_system  
                WHERE   logSystemQuery  LIKE CONCAT('%',@search,'%')";

                MySqlCommand mySqlCommand = new(sql, connection);

                parameterModels = new List<ParameterModel>
                {
                    new ()
                    {
                        Key = "@search",
                        Value = search
                    }
                };

                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logSystemModels.Add(new LogSystemModel
                        {
                            LogSystemKey = Convert.ToInt32(reader["logSystemId"]),
                            LogSystemQuery = reader["logQuery"].ToString(),
                            LogSystemDateTime = Convert.ToDateTime(reader["logDateTime"])
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
            return logSystemModels;
        }
        public byte[] GetExcel()
        {

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Administrator > Log System");
            worksheet.Cell(1, 1).Value = "No";
            worksheet.Cell(1, 2).Value = "Information";
            worksheet.Cell(1, 3).Value = "Date Time";


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
                        worksheet.Cell(currentRow, 1).Value = counter -1 ;
                        worksheet.Cell(currentRow, 2).Value = reader["logSystemQuery"].ToString();
                        worksheet.Cell(currentRow, 3).Value = reader["logSystemDateTime"].ToString();


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
    }
}