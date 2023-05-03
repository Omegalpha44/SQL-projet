using MySql.Data.MySqlClient;
using Org.BouncyCastle.Tls;
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
        static int GoodValue(int a, int b) // vérifie la valeur fournie en entrée
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
        static void MenuClient(string courriel)
        {
            bool quit=false;
            while (!quit) 
            {
                Console.Clear();
                string connectionString = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT nom, prenom FROM client WHERE courriel = @courriel";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@courriel", courriel);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string nom = reader.GetString("nom");
                                string prenom = reader.GetString("prenom");

                                Console.WriteLine($"Bonjour {nom.ToUpper()} {prenom}");
                            }
                        }
                    }
                }
                Console.WriteLine("Que voulez vous faire ?");
                Console.WriteLine("1. Modifier mes informations");
                Console.WriteLine("2. Voir mon statut de fidélité");
                Console.WriteLine("3. Voir le catalogue du magasin");
                Console.WriteLine("4. Passer commande");
                Console.WriteLine("5. Quiter");
                int r = GoodValue(1, 5);
                string statut = StatutClient(courriel);
                switch (r)
                {
                    case 1: break;
                    case 2:
                        if (statut == " ") { Console.WriteLine("Vous n'avez pas de statut de fidélité actuellement"); }
                        else { Console.WriteLine("Votre statut de fidélité est " + statut); }
                        Console.ReadLine();
                        break;
                    case 3: break;
                    case 4: ChoixProduits(); break;
                    case 5:quit = true; break;
                }
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
                    command.CommandText = "INSERT INTO `Fleurs`.`client` (`nom`, `prenom`,`telephone`,`courriel`, `motDePasse`, `facturationAdresse`,`creditCard`) VALUES ('" + nom + "', '" + prenom + "','" +telephone+ "' ,'" + courriel + "','" + mdp + "', '" + adresse + "',    '" + cb + "');";

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
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT EXISTS (SELECT 1 FROM client WHERE courriel = @courriel AND motDePasse=@motDePasse)";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@courriel", courriel);
                        command.Parameters.AddWithValue("@motDePasse", mdp);
                        exist = Convert.ToInt32(command.ExecuteScalar());
                    }

                    connection.Close();
                }
                if(exist == 0) 
                {
                    Console.Clear();
                    Console.WriteLine("Courrier ou mot de passe éronné. Veuillez réessayer");
                }
                else 
                {
                    Console.Clear();
                    MenuClient(courriel);
                }

            }
            

        }
        
        static string StatutClient(string courriel) 
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";
            int count;
            DateTime commandeDate=DateTime.Now;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                
                string query = "SELECT COUNT(*) FROM commande INNER JOIN client ON commande.idclient = client.idclient WHERE client.courriel = @courriel AND MONTH(commande.commandeDate) = @month";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@courriel", courriel);
                    command.Parameters.AddWithValue("@month", commandeDate.Month);
                    count = Convert.ToInt32(command.ExecuteScalar());
                }

                connection.Close();
            }
            string statut;
            if (count < 1) 
            {
                statut = " ";
            }
            else if(count <5) { statut = "Bronze"; }
            else { statut = "Or"; }
            return statut;
        }

        static string ChoixProduits() 
        {
            Console.Clear ();
            Console.WriteLine("Quel type de commande souhaitez-vous ?");
            Console.WriteLine("1. Commande standard");
            Console.WriteLine("2. Commande personnalisée");
            Console.WriteLine("3. Quiter");
            int r = GoodValue(1, 3);
            string choix = "";
            switch (r)
            {
                case 1: choix=ChoixCommandeStandard(); break;
                case 2:break;
                case 3: break;
            }
            return choix;
        }
        static string ChoixCommandeStandard() 
        {
            Console.Clear ();
            Console.WriteLine("Quel bouquet souhaitez-vous"); 
            Console.WriteLine("1. Gros Merci, un arrangement floral avec marguerites et verdure parfait pour toute occasion pour un prix de 45 €");
            Console.WriteLine("2. L’amoureux, un arrangement floral avec roses blanches et roses rouges pour la St-Valentin pour un prix de 65 €");
            Console.WriteLine("3. L’Exotique, un arrangement floral avec ginger, oiseaux du paradis, roses et genet parfait pour toute occasion pour un prix de 40 €");
            Console.WriteLine("4. Maman, un arrangement  floral avec gerbera, roses blanches, lys et alstroméria pour la Fête des mères pour un prix de 80 €");
            Console.WriteLine("5. Vive la mariée, un arrangement  floral avec lys et orchidées parfait pour un mariage pour un prix de 120 €");
            Console.WriteLine("6. Quiter");
            string bouquet="";
            int r = GoodValue(1, 6);
            switch (r)
            {
                case 1: bouquet = "Gros Merci"; break;
                case 2: bouquet = "L’amoureux"; break;
                case 3: bouquet = "L’Exotique"; break;
                case 4: bouquet = "Maman"; break;
                case 5: bouquet = "Vive la mariée"; break;
                case 6: break;
            }
            return bouquet;
        }
    }
}