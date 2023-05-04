using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
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
        public static void Menu()
        {
            Console.Clear();
            Console.WriteLine("Bienvenue chez Belle Fleure");
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
                Console.WriteLine("5. Voir mes commandes passées");
                Console.WriteLine("6. Quiter");
                int r = GoodValue(1, 6);
                string statut = StatutClient(courriel);
                switch (r)
                {
                    case 1: ModificationsDonnées(courriel); break;
                    case 2:
                        if (statut == " ") { Console.WriteLine("Vous n'avez pas de statut de fidélité actuellement"); }
                        else { Console.WriteLine("Votre statut de fidélité est " + statut); }
                        Console.ReadLine();
                        break;
                    case 3: ProduitsMagasin(); break;
                    case 4: ChoixProduits(); break;
                    case 5: AffichageCommandesPassées(courriel); break;
                    case 6:quit = true; break;
                }
            }
            
        }
        static void ModuleClient()
        {
            Console.Clear();
            Console.WriteLine("Que voulez vous faire ?");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Sign up");
            Console.WriteLine("3. Quiter");
            int r = GoodValue(1, 3);
            switch (r)
            {
                case 1: ConnectionClient(); break;
                case 2: AjouterClient(); break;
                case 3: AffichageClients(); break;
            }
            Console.ReadLine();
            Menu();
        }
        static void ModuleProduit()
        {
            Console.Clear();
            Console.WriteLine("Lorem Ipsum");
            Console.ReadLine();
            Menu();
        }
        static void ModuleCommande()
        {
            Console.Clear();
            Console.WriteLine("Lorem Ipsum");
            Console.ReadLine();
            Menu();

        }
        static void ModuleStat()
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
                    Console.WriteLine("Carte de crédit");
                    string cb = Console.ReadLine();
                    MySqlConnection connection = new MySqlConnection(connectionString);
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO `Fleurs`.`client` (`idclient`,`nom`, `prenom`,`telephone`,`courriel`, `motDePasse`, `facturationAdresse`,`creditCard`) VALUES (9,'" + nom + "', '" + prenom + "','" +telephone+ "' ,'" + courriel + "','" + mdp + "', '" + adresse + "',    '" + cb + "');";

                    MySqlDataReader reader;
                    reader = command.ExecuteReader();
                    connection.Close();
                    ConnectionClient();
                    
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
        
        static string StatutClient(string courriel) //renvoie le statut de fidélité du client ayant ce courrie
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";
            int count;
            DateTime commandeDate=DateTime.Now;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                
                string query = "SELECT COUNT(*) FROM commande JOIN client ON commande.idclient = client.idclient WHERE client.courriel = @courriel AND MONTH(commande.commandeDate) = @month";
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
        }//renvoie le bouquet demandé par le client
        static string ChoixCommandeStandard() 
        {
            Console.Clear ();
            Console.WriteLine("Quel bouquet souhaitez-vous"); 
            Console.WriteLine("1. Gros Merci, un arrangement floral avec marguerites et verdure parfait pour toute occasion pour un prix de 45 euros");
            Console.WriteLine("2. L’amoureux, un arrangement floral avec roses blanches et roses rouges pour la St-Valentin pour un prix de 65 euros");
            Console.WriteLine("3. L’Exotique, un arrangement floral avec ginger, oiseaux du paradis, roses et genet parfait pour toute occasion pour un prix de 40 euros");
            Console.WriteLine("4. Maman, un arrangement  floral avec gerbera, roses blanches, lys et alstroméria pour la Fête des mères pour un prix de 80 euros");
            Console.WriteLine("5. Vive la mariée, un arrangement  floral avec lys et orchidées parfait pour un mariage pour un prix de 120 euros");
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
        }//renvoie le bouquet standard choisi par le client

        static void ProduitsMagasin() 
        {
            Console.Clear();
            Console.WriteLine("Fleur        Prix(unité)");
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT nom,prixIndiv FROM produits;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string nom = reader.GetString("nom");
                            string prixIndiv = reader.GetString("prixIndiv");

                            Console.WriteLine($"{nom}        {prixIndiv} euros");
                        }
                    }
                }
            }
            Console.ReadLine();
        }
        static void AffichageClients() 
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT c.nom, c.prenom, c.courriel, COUNT(co.idcommande) AS nombre_commandes FROM client c LEFT JOIN commande co ON c.idclient = co.idclient WHERE c.courriel!=@admin GROUP BY c.idclient;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@admin", "admin");
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine("Nom\tPrénom\tCourriel\tNombre de commandes\tStatut");

                        while (reader.Read())
                        {
                            string nom = reader.GetString("nom");
                            string prenom = reader.GetString("prenom");
                            string courriel = reader.GetString("courriel");
                            int nombreCommandes = reader.GetInt32("nombre_commandes");

                            Console.WriteLine($"{nom}\t{prenom}\t{courriel}\t{nombreCommandes}\t{StatutClient(courriel)}");
                        }
                    }
                }
            }
            Console.ReadLine();
        }
        static void AffichageClient(string courriel) 
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM client WHERE courriel = @courriel";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@courriel", courriel);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())                           // parcours ligne par ligne
                        {
                            string currentRowAsString = "";

                            for (int i = 0; i < reader.FieldCount; i++)    // parcours cellule par cellule
                            {
                                string valueAsString = reader.GetValue(i).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                                currentRowAsString += valueAsString + ", ";
                            }
                            Console.WriteLine(currentRowAsString);    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
                        }
                    }
                }
            }

        }
        static void ModificationsDonnées(string courriel) 
        {
            Console.Clear();
            AffichageClient(courriel);
            Console.WriteLine("Que voulez vous modifier ?");
            Console.WriteLine("1. Nom");
            Console.WriteLine("2. Telephone");
            Console.WriteLine("3. Courriel");
            Console.WriteLine("4. Mot de passe");
            Console.WriteLine("5. Adresse de facturation");
            Console.WriteLine("6. Carte de crédit");
            Console.WriteLine("7. Quiter");
            int r = GoodValue(1, 7);
            string statut = StatutClient(courriel);
            switch (r)
            {
                case 1: ModificationClient("nom",courriel); break;
                case 2:ModificationClient("telephone", courriel);
                    break;
                case 3: ModificationClient("courriel", courriel); break;
                case 4: ModificationClient("motDePasse", courriel); break;
                case 5: ModificationClient("adresseFacturation", courriel); break;
                case 6: ModificationClient("creditCard", courriel);break;
            }
            Console.ReadLine();
            
        }
        static void ModificationClient(string info,string courriel) 
        {
            Console.WriteLine("Veuillez rentrer la nouvelle donnée");
            int valeurInt=-1;
            string valeur=null;
            if (info == "telephone" || info=="creditCard") 
            {
                valeurInt=Convert.ToInt32(Console.ReadLine());
            }
            else { valeur = Console.ReadLine(); }
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = $"UPDATE client SET {info} = @nouvelleValeur WHERE courriel = @courriel";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    if (valeur != null) { command.Parameters.AddWithValue("@nouvelleValeur", valeur); }
                    else { command.Parameters.AddWithValue("@nouvelleValeur", valeurInt); }
                    command.Parameters.AddWithValue("@courriel", courriel);
                    int rowsAffected = command.ExecuteNonQuery();
                }
                connection.Close();
            }

        }

        static void AffichageCommandesPassées(string courriel) 
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT prix,livraisonDate FROM commande JOIN client ON commande.idclient = client.idclient WHERE client.courriel = @courriel;";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@courriel", courriel);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            float prix = reader.GetFloat("prix");
                            DateTime livraisonDate = reader.GetDateTime("livraisonDate");
                            if (livraisonDate < DateTime.Now) { Console.WriteLine($"{prix}        {livraisonDate.ToShortDateString()}"); }
                            else { Console.WriteLine($"{prix}        En cours de livraison"); }
                        }
                    }
                }
            }
            Console.ReadLine();
        }

        static void MenuAdmin() 
        {
            bool quit = false;
            while (!quit)
            {
                Console.Clear();
                Console.WriteLine("Que voulez vous faire ?");
                Console.WriteLine("1. Module Client");
                Console.WriteLine("2. Module Commande");
                Console.WriteLine("3. Module Produit");
                Console.WriteLine("4. Module Statistiques");
                Console.WriteLine("5. Quiter");
                int r = GoodValue(1, 5);
                switch (r)
                {
                    case 1: ModuleClient(); break;
                    case 2:ModuleCommande();
                        break;
                    case 3: ModuleProduit(); break;
                    case 4: ModuleStat(); break;
                    case 5: quit = true; break;
                }
            }
        }

    }
}