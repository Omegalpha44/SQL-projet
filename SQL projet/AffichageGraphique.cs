using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_projet
{
    public delegate void Ui();
    internal class AffichageGraphique
    {
        public AffichageGraphique()
        {
            
        }
        public void Affichage()
        {
            ExceptionManager(Menu);
        }
        void ExceptionManager(Ui func) // permet, si une erreur est détecté, de relancer la méthode
        {
            try { func(); }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Une erreur est survenue, veuillez vérifier vos réponses");
                Console.WriteLine("===============");
                Console.WriteLine("Appuyez sur une touche ...");
                Console.ReadKey();
                Console.Clear();
                ExceptionManager(func);
            }
        }
        int GoodValue(int a, int b) // vérifie la valeur fournie en entrée
        {
            int r = int.Parse(Console.ReadLine());
            if (a <= r && r <= b)
            {
                return r;
            }
            else
            {
                Console.WriteLine("Valeur incorrecte");
                return GoodValue(a, b);
            }
        }
        public void Menu()
        {
            Console.Clear();
            Console.WriteLine("Bienvenu chez Belle Fleure");
            Console.WriteLine("Que voulez vous faire ?");
            Console.WriteLine("1. Module Client");
            Console.WriteLine("2. Module Produit");
            Console.WriteLine("3. Module Commande");
            Console.WriteLine("4. Module Statistiques");
            Console.WriteLine("5. Quiter");
            int r = GoodValue(1, 5);
            switch(r)
            {
                case 1 : ModuleClient();break;
                case 2: ModuleProduit();break;
                case 3: ModuleCommande();break;
                case 4: ModuleStat();break;
                case 5: break;
            }
        }
        void ModuleClient()
        {
            Console.Clear();
            Console.WriteLine("Lorem Ipsum");
            Console.WriteLine("Bienvenu chez Belle Fleure");
            Console.WriteLine("Que voulez vous faire ?");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Sign up");
            Console.WriteLine("3. Quiter");
            int r = GoodValue(1, 3);
            switch (r)
            {
                case 1: ConnectionClient(); break;
                case 2: AjouterClient(); break;
                case 3: break;
            }
            Console.ReadLine();
            Menu();
        }
        void ModuleProduit()
        {
            Console.Clear();
            Console.WriteLine("Lorem Ipsum");
            Console.ReadLine();
            Menu();
        }
        void ModuleCommande()
        {
            Console.Clear();
            Console.WriteLine("Lorem Ipsum");
            Console.ReadLine();
            Menu();

        }
        void ModuleStat()
        {
            Console.Clear();
            Console.WriteLine("Lorem Ipsum");
            Console.ReadLine();
            Menu();
        }
        static void AjouterClient()
        {
            Console.Clear();
            Console.WriteLine("Prénom : ");
            string prenom = Console.ReadLine();
            Console.WriteLine("Nom : ");
            string nom = Console.ReadLine();
            Console.WriteLine("Adresse : ");
            string adresse = Console.ReadLine();
            Console.WriteLine("Mail : ");
            string courriel = Console.ReadLine();
            Console.WriteLine("Mot de passe");
            string mdp = Console.ReadLine();

            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";
            int exist = 1;
            while (exist==1)
            {
                using (MySqlConnection connection1 = new MySqlConnection(connectionString))
                {
                    connection1.Open();

                    string query = "SELECT EXISTS (SELECT 1 FROM client WHERE courriel = @courriel)";
                    using (MySqlCommand command1 = new MySqlCommand(query, connection1))
                    {
                        command1.Parameters.AddWithValue("@courriel", courriel);

                        exist = Convert.ToInt32(command1.ExecuteScalar());
                    }

                    connection1.Close();
                }
                if (exist != 1)
                {
                    Console.WriteLine("Téléphone : ");
                    int telephone = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine(telephone);
                    Console.WriteLine("Carte de crédit");
                    string cb = Console.ReadLine();
                    MySqlConnection connection = new MySqlConnection(connectionString);
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO `Fleurs`.`client` (`idclient`,`nom`, `prenom`,`telephone`,`courriel`, `motDePasse`, `facturationAdresse`,`creditCard`) VALUES (8,'" + nom + "', '" + prenom + "','" +telephone+ "' ,'" + courriel + "','" + mdp + "', '" + adresse + "',    '" + cb + "');";

                    MySqlDataReader reader;
                    reader = command.ExecuteReader();
                    connection.Close();
                    
                }
                else
                {
                    Console.WriteLine("Ce mail est déjà associé à un autre client. Veuillez saisir un autre mail et saisir de nouveau le mot de passe");
                    Console.WriteLine("Mail : ");
                    courriel = Console.ReadLine();
                    Console.WriteLine("Mot de passe");
                    mdp = Console.ReadLine();
                }
                break;
            }


        }
        static void ConnectionClient() 
        {
            Console.Clear();
            int exist = 0;
            while (exist == 0) 
            {
                Console.WriteLine("Courriel :");
                string courriel = Console.ReadLine();
                Console.WriteLine("Mot de passe :");
                string mdp = Console.ReadLine();
                string connectionString = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";
                using (MySqlConnection connection1 = new MySqlConnection(connectionString))
                {
                    connection1.Open();

                    string query = "SELECT EXISTS (SELECT 1 FROM client WHERE courriel = @courriel)";
                    using (MySqlCommand command1 = new MySqlCommand(query, connection1))
                    {
                        command1.Parameters.AddWithValue("@courriel", courriel);

                        exist = Convert.ToInt32(command1.ExecuteScalar());
                    }

                    connection1.Close();
                }
                if(exist == 0) 
                {
                    Console.Clear();
                    Console.WriteLine("Courrier ou mot de passe éronné. Veuillez réessayer");
                }
                else 
                {
                    Console.Clear();
                    Console.WriteLine("Bienvenue");
                }

            }
            

        }
    }
}