using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.CRUD;
using Org.BouncyCastle.Tls;
using System;
namespace SQL_projet
{
    internal class Sql_fetcher //classe s'occupant d'afficher les requêtes sql faites sur le serveur 
    {
        private MySqlConnection sqlConnection;

        
        public Sql_fetcher(string connection)
        {
            sqlConnection = new MySqlConnection(connection);
        }
        public Sql_fetcher()
        {
            sqlConnection = null;
        }
        public void DisplayData( string commande) //affiche les résultats d'une recherche 
            {
                sqlConnection.Open();
                MySqlCommand command = sqlConnection.CreateCommand();
                Console.WriteLine(commande + "\n");
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
                            else Console.Write("null|");
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
        }
    }

