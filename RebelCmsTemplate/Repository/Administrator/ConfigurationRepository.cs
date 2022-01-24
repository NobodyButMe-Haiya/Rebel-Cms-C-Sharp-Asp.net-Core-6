using MySql.Data.MySqlClient;
using RebelCmsTemplate.Models.Administrator;
using RebelCmsTemplate.Models.Shared;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Repository.Administrator;

public class ConfigurationRepository
{
    private readonly SharedUtil _sharedUtil;

    public ConfigurationRepository(IHttpContextAccessor httpContextAccessor)
    {
        _sharedUtil = new SharedUtil(httpContextAccessor);
    }

    public List<ConfigurationModel> Read()
    {
        List<ConfigurationModel> configurationModels = new();
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();

        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            sql += @"
                SELECT      * 
                FROM        configuration 
                ORDER BY    configurationId DESC ";

            // reporting purpose
            _sharedUtil.SetSqlSession(sql);
            MySqlCommand mySqlCommand = new(sql, connection);
            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    configurationModels.Add(new ConfigurationModel
                    {
                        ConfigurationKey = Convert.ToInt32(reader["configurationId"]),
                        ConfigurationPortal = reader["configurationPortal"].ToString(),
                        ConfigurationPortalLocal = reader["configurationPortalLocal"].ToString(),
                        ConfigurationEmail = reader["configurationEmail"].ToString(),
                        ConfigurationEmailHost = reader["configurationEmailHost"].ToString(),
                        ConfigurationEmailPassword = reader["configurationEmailPassword"].ToString(),
                        ConfigurationEmailPort = reader["configurationEmailPort"].ToString(),
                        ConfigurationEmailSecure = reader["configurationEmailSecure"].ToString()
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


        return configurationModels;
    }

    public void Update(ConfigurationModel configurationModel)
    {
        var sql = string.Empty;
        List<ParameterModel> parameterModels = new();
        using var connection = SharedUtil.GetConnection();
        try
        {
            connection.Open();
            var mySqlTransaction = connection.BeginTransaction();

            sql += @"
                UPDATE  configuration 
                SET     configurationPortal         =   @configurationPortal,
                        configurationPortalLocal    =   @configurationPortalLocal,
                        configurationEmail          =   @configurationEmail,
                        configurationEmailHost      =   @configurationEmailHost,
                        configurationEmailPassword  =   @configurationEmailPassword,
                        configurationEmailPort      =   @configurationEmailPort,
                        configurationEmailSecure    =   @configurationEmailSecure
                WHERE   configurationId             =   @configurationId ";
            MySqlCommand mySqlCommand = new(sql, connection);

            parameterModels = new List<ParameterModel>
            {
                new()
                {
                    Key = "@configurationPortal",
                    Value = configurationModel.ConfigurationPortal
                },
                new()
                {
                    Key = "@configurationPortalLocal",
                    Value = configurationModel.ConfigurationPortalLocal
                },
                new()
                {
                    Key = "@configurationEmail",
                    Value = configurationModel.ConfigurationEmail
                },
                new()
                {
                    Key = "@configurationEmailPassword",
                    Value = configurationModel.ConfigurationEmailPassword
                },
                new()
                {
                    Key = "@configurationEmailPort",
                    Value = configurationModel.ConfigurationEmailPort
                },
                new()
                {
                    Key = "@configurationEmailSecure",
                    Value = configurationModel.ConfigurationEmailSecure
                },
                new()
                {
                    Key = "@configurationId",
                    Value = configurationModel.ConfigurationKey
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