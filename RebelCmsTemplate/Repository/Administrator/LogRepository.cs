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
    public class LogRepository
    {
        private readonly SharedUtil _sharedUtil;
        public LogRepository(IHttpContextAccessor httpContextAccessor)
        {
            _sharedUtil = new SharedUtil(httpContextAccessor);
        }
        public List<LogModel> Read()
        {
            List<LogModel> logModels = new();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {

                connection.Open();
                const string sql = @"
                SELECT      * 
                FROM        log 
                ORDER BY    logId DESC ";
                // reporting purpose
                _sharedUtil.SetSqlSession(sql);
                MySqlCommand mySqlCommand = new(sql, connection);
                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logModels.Add(new LogModel
                        {
                            LogKey  = Convert.ToInt32(reader["logId"]),
                            LogUserName = reader["logUserName"].ToString(),
                            LogQuery = reader["logQuery"].ToString(),
                            LogError = reader["logError"].ToString(),
                            LogDateTime = Convert.ToDateTime(reader["logDateTime"])
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


            return logModels;
        }
        
        public List<LogModel> Search(string search)
        {
            List<LogModel> logModels = new();
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();
            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {

                connection.Open();
                sql += @"
                SELECT   * 
                FROM     log  
                WHERE    logUserName like concat('%',@search,'%') 
                OR       logQuery like concat('%',@search,'%')
                OR       logError like concat('%',@search,'%')";
                // reporting purpose
                
                
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
                _sharedUtil.SetSqlSession(sql,parameterModels);
                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logModels.Add(new LogModel
                        {
                            LogKey = Convert.ToInt32(reader["logId"]),
                            LogUserName = reader["logUserName"].ToString(),
                            LogQuery = reader["logQuery"].ToString(),
                            LogError = reader["logError"].ToString(),
                            LogDateTime = Convert.ToDateTime(reader["logDateTime"])
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
            return logModels;
        }
        public byte[] GetExcel()
        {

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Administrator > Log ");
            worksheet.Cell(1, 1).Value = "No";
            worksheet.Cell(1, 2).Value = "SQL";
            worksheet.Cell(1, 3).Value = "Message";
            worksheet.Cell(1, 4).Value = "Date Time";

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
                        worksheet.Cell(currentRow, 2).Value = reader["logQuery"].ToString();
                        worksheet.Cell(currentRow, 3).Value = reader["logError"].ToString();
                        worksheet.Cell(currentRow, 4).Value = reader["logDateTime"].ToString();

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
