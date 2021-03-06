using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Menu;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Menu
{
    public class FolderAccessRepository
    {
        private readonly SharedUtil _sharedUtil;
        public FolderAccessRepository(IHttpContextAccessor httpContextAccessor)
        {
            _sharedUtil = new SharedUtil(httpContextAccessor);
        }
        public List<FolderAccessModel> Read(int roleId = 0, int folderId = 0)
        {
            List<FolderAccessModel> folderAccessModels = new();
            string sql = string.Empty;
            List<ParameterModel> parameterModels = new ();

            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {

                connection.Open();
                sql += @"
                SELECT  * 
                FROM    folder_access 

                JOIN    role 
                USING(roleId) 

                JOIN folder 
                USING(folderId)

                WHERE folder.tenantId  = @tenantId ";
                if (roleId > 0)
                {
                    sql += " AND roleId  = @roleId ";
                }
                if (folderId > 0)
                {
                    sql += " AND folderId  = @folderId ";
                }
              
                MySqlCommand mySqlCommand = new(sql, connection);

                if (roleId > 0)
                {
                    parameterModels.Add(new ParameterModel
                    {
                        Key = "@roleId",
                        Value = roleId
                    });

                }
                if (folderId > 0)
                {
                    parameterModels.Add(new ParameterModel
                    {
                        Key = "@folderId",
                        Value = folderId
                    });

                }
                parameterModels.Add(new ParameterModel
                {
                    Key = "@tenantId",
                    Value = _sharedUtil.GetTenantId()
                });
                foreach (ParameterModel parameter in parameterModels)
                {
                    mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
                using (var reader = mySqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        folderAccessModels.Add(new FolderAccessModel
                        {
                            FolderAccessKey = Convert.ToInt32(reader["folderAccessId"]),
                            FolderAccessValue = Convert.ToInt32(reader["folderAccessValue"]),
                            FolderKey = Convert.ToInt32(reader["folderId"]),
                            RoleKey = Convert.ToInt32(reader["roleId"]),
                            RoleName = reader["roleName"].ToString(),
                            FolderName = reader["folderName"].ToString()
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


            return folderAccessModels;
        }

        public void Update(List<FolderAccessModel> folderAccessModels)
        {
            using MySqlConnection connection = SharedUtil.GetConnection();
            try
            {

                connection.Open();
                MySqlTransaction mySqlTransaction = connection.BeginTransaction();

                string sql = @" UPDATE  `folder_access` " +
                " SET     `folderAccessValue`			= CASE `folderAccessId` ";
                List<int> primaryKeyAll = new();
                foreach (FolderAccessModel folderAccessModel in folderAccessModels)
                {
                    sql += " WHEN " + folderAccessModel.FolderAccessKey;
                    sql += " THEN " + folderAccessModel.FolderAccessValue;
                    primaryKeyAll.Add(folderAccessModel.FolderAccessKey);                    
                }
                sql += "	END ";

                sql += " WHERE 	`folderAccessId`		IN	(" + string.Join(",", primaryKeyAll) + ")";
                MySqlCommand mySqlCommand = new(sql, connection);

                mySqlCommand.ExecuteNonQuery();
                mySqlTransaction.Commit();
                mySqlCommand.Dispose();

            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _sharedUtil.SetSystemException(ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
