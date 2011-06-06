using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using derpirc.Data.Settings;
using Wintellect.Sterling.Database;

namespace derpirc.Data
{
    public class ModelDatabase : BaseDatabaseInstance
    {
        /// <summary>
        ///     The name of the database instance
        /// </summary>
        public override string Name
        {
            get { return "Type Database"; }
        }

        /// <summary>
        ///     Method called from the constructor to register tables
        /// </summary>
        /// <returns>The list of tables for the database</returns>
        protected override List<ITableDefinition> _RegisterTables()
        {
            return new List<ITableDefinition>
                       {
                           CreateTableDefinition<Client, bool>(c => true),
                           CreateTableDefinition<List<Server>, bool>(c => true),
                           CreateTableDefinition<List<Network>, bool>(c => true),
                           CreateTableDefinition<User, bool>(c => true),
                           CreateTableDefinition<Session, bool>(c => true),
                       };
        }
    }
}
