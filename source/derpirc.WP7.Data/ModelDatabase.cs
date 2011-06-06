using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dIRca.Data.Settings;
using Wintellect.Sterling.Database;

namespace dIRca.Data
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
                           CreateTableDefinition<List<IrcServer>, bool>(c => true),
                           CreateTableDefinition<List<IrcNetwork>, bool>(c => true),
                           CreateTableDefinition<IrcUser, bool>(c => true),
                           CreateTableDefinition<LastConnection, int>(last => last.Id),
                       };
        }
    }
}
