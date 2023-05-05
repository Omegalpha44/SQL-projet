using MySql.Data.MySqlClient;
using System.Text;

namespace SQL_projet
{
    internal class Sql_fetcher //classe s'occupant d'afficher les requêtes sql faites sur le serveur 
    {
        public MySqlConnection sqlConnection;


        public Sql_fetcher(string connection)
        {
            sqlConnection = new MySqlConnection(connection);
        }
        public Sql_fetcher()
        {
            sqlConnection = null;
        }
        public void DisplayData(string commande) //affiche les résultats d'une recherche 
        {
            sqlConnection.Open();
            MySqlCommand command = sqlConnection.CreateCommand();
            command.CommandText = commande;
            MySqlDataReader reader = command.ExecuteReader();
            bool foo2 = true;
            int i2 = 0;
            Console.Write("|");
            while (foo2)
            {
                try
                {
                    Console.Write(reader.GetName(i2) + "|");
                    i2 += 1;
                }
                catch
                {
                    foo2 = false;
                    Console.WriteLine();
                }
            }
            for (int j = 0; j < i2; j++)
            {
                Console.Write("-------");
            }
            Console.WriteLine();
            while (reader.Read())
            {
                bool foo = true;
                int i = 0;
                Console.Write("|");

                while (foo)
                {

                    try
                    {
                        if (reader.IsDBNull(i) == false) Console.Write(reader.GetString(i) + "|");
                        else Console.Write("|");
                        i += 1;
                    }
                    catch
                    {
                        Console.WriteLine();
                        foo = false;
                    }
                }
            }
            sqlConnection.Close();
            Console.WriteLine();
            Console.WriteLine();
        }


        public string ExecuterCommandeSqlString(string commandText)
        //permet d'exécuter des commandes sql et récupérer le contenu dans une string
        {
            sqlConnection.Open();
            MySqlCommand command = new MySqlCommand(commandText, sqlConnection);
            MySqlDataReader reader = command.ExecuteReader();
            StringBuilder result = new StringBuilder();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    result.Append(reader[i].ToString() + " ");
                }
                result.Append("\n");
            }
            sqlConnection.Close();
            return result.ToString();

        }
        public List<string[]> ExecuterCommandeSqlList(string commandText)
        {
            sqlConnection.Open();
            MySqlCommand command = new MySqlCommand(commandText, sqlConnection);
            MySqlDataReader reader = command.ExecuteReader();

            List<string[]> res = new List<string[]>();
            while (reader.Read())
            {
                string[] temp = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    temp[i] = reader[i].ToString();
                }
                res.Add(temp);
            }
            sqlConnection.Close();
            return res;
        }
        public void ExecuterCommande(string commandText)
        {
            sqlConnection.Open();
            MySqlCommand command = new MySqlCommand(commandText, sqlConnection);
            MySqlDataReader reader = command.ExecuteReader();
            reader.Read();
            sqlConnection.Close();
        }
    }
}
