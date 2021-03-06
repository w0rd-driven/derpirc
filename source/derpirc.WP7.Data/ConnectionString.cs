﻿
namespace derpirc.Data
{
    public class ContextConnectionString
    {
        #region Properties

        public string ConnectionString { get; set; }
        public string DataSource { get; set; }
        public string Password { get; set; }
        public int? MaxDatabaseSize { get; set; }
        public int? MaxBufferSize { get; set; }
        public DatabaseMode DatabaseMode { get; set; }
        public bool? CaseSensitive { get; set; }

        #endregion

        public ContextConnectionString()
        {
            DataSource = "isostore:/IRC.sdf";
            DatabaseMode = DatabaseMode.Default;
        }

        public new string ToString()
        {
            var result = string.Empty;

            if (!string.IsNullOrEmpty(ConnectionString))
            {
                result = ConnectionString;
                return result;
            }

            var dataSource = string.Empty;
            var password = string.Empty;
            var maxDatabaseSize = string.Empty;
            var maxBufferSize = string.Empty;
            var fileMode = string.Empty;
            var culturalIdentifier = string.Empty;
            var caseSensitive = string.Empty;

            if (!string.IsNullOrEmpty(DataSource))
                dataSource = string.Format("Data Source='{0}';", DataSource);
            if (!string.IsNullOrEmpty(Password))
                password = string.Format("Password={0};", Password);
            if (MaxDatabaseSize.HasValue)
                maxDatabaseSize = string.Format("Max Database Size={0};", MaxDatabaseSize);
            if (MaxBufferSize.HasValue)
                maxBufferSize = string.Format("Max Buffer Size={0};", MaxBufferSize);

            switch (DatabaseMode)
            {
                case DatabaseMode.ReadWrite:
                    fileMode = "File Mode=read write;";
                    break;
                case DatabaseMode.ReadOnly:
                    fileMode = "File Mode=read only;";
                    break;
                case DatabaseMode.Exclusive:
                    fileMode = "File Mode=exclusive;";
                    break;
                case DatabaseMode.SharedRead:
                    fileMode = "File Mode=shared read;";
                    break;
            }

            if (CaseSensitive.HasValue)
                caseSensitive = string.Format("Case Sensitive={0};", CaseSensitive);

            result = dataSource + password + fileMode + maxDatabaseSize + maxBufferSize + culturalIdentifier + caseSensitive;

            return result;
        }
    }
}
