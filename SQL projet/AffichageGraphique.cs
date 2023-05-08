using MySql.Data.MySqlClient;
using System.Globalization;

namespace SQL_projet
{
    public delegate void Ui();
    internal class AffichageGraphique
    {
        public Sql_fetcher fetcher;
        string utilisateur; //permet de nommer le client (nom + prenom)
        int idClient; //permet d'identifier le client
        string status;
        public AffichageGraphique()
        {
            fetcher = null;
        }
        public AffichageGraphique(string connection)
        {
            fetcher = new Sql_fetcher(connection);
        }
        public void Affichage()// méthode principale permettant l'affichage
        { 
            ExceptionManager(FirstLogin); // pour le first login, on demande à l'utilisateur / admin de taper ses credentials
            if (utilisateur == "admin admin \n") ExceptionManager(AdminMenu); // si admin --> connection admin
            else ExceptionManager(Menu); // --> sinon connection client simple

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
                fetcher.sqlConnection.Close();
                ExceptionManager(func);
            }
        }
        static int GoodValue(int a, int b) // vérifie la valeur fournie en entrée
        {
            string temp = Console.ReadLine();
            int r = -1000;
            int.TryParse(temp, out r);
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
        public void FirstLogin()
        //pour la primo connection
        {
            Console.Clear();
            Console.WriteLine("bienvenue chez Belle Fleur");
            Console.WriteLine("================");
            Console.WriteLine("Veuillez vous identifier : ");
            Console.Write("login : ");
            string login = Console.ReadLine();
            Console.Write("password : ");
            string password = Console.ReadLine();
            string coincidence = fetcher.ExecuterCommandeSqlString("select * from client where motDePasse ='" + password + "' and courriel = '" + login + "'");
            if (coincidence != "")
            {
                utilisateur = fetcher.ExecuterCommandeSqlString("select nom, prenom from client where motDePasse = '" + password + "' and courriel = '" + login + "'");
                idClient = int.Parse(fetcher.ExecuterCommandeSqlString("select idclient from client where motDePasse = '" + password + "' and courriel = '" + login + "'"));
                Console.WriteLine("Connection réussi !");
                Console.WriteLine("bienvenu " + utilisateur);
            }
            else
            {
                Console.WriteLine("Connection échoué !");
                Console.Write("Voulez vous créer un compte ? (Y/N): ");
                string rep = Console.ReadLine();
                switch (rep)
                {
                    case "Y" or "y": AjouterClient(); Console.WriteLine("Votre compte est créé ! Veuillez vous reconnecter"); FirstLogin(); break;
                    case "N" or "n": Console.WriteLine("veuillez réessayer de rentrer vos identifiants"); FirstLogin(); break;
                    default: Console.WriteLine("veuillez réessayer de rentrer vos identifiants"); FirstLogin(); break;
                }
            }
        }
        public void AdminMenu()
        //affichage de l'admin
        {
            Console.Clear();
            Console.WriteLine("Bonjour Admin");
            Console.WriteLine("===============");
            Stock(); // affiche si des produits sont en manque de stocks
            Console.WriteLine("===============");
            Console.WriteLine("Que voulez vous faire ?");
            Console.WriteLine("1. Module Client");
            Console.WriteLine("2. Module Produit");
            Console.WriteLine("3. Module Commande");
            Console.WriteLine("4. Module Statistiques");
            Console.WriteLine("5. Quitter");
            int r = GoodValue(1, 5);
            switch (r)
            {
                case 1: ModuleClient(); break;
                case 2: ModuleProduit(); break;
                case 3: ModuleCommande(); break;
                case 4: ModuleStat(); break;
                case 5: break;
            }
        }
        public void Stock()
        //gestion des stocks
        {
            List<string[]> produits = fetcher.ExecuterCommandeSqlList("select nom,quantite,isAlreadyComposed from produits");
            foreach (string[] elem in produits)
            {
                if (elem[2] == "1") // pour les bouquets
                {
                    if (int.Parse(elem[1]) < 30)
                    {
                        Console.WriteLine("produit : " + elem[0] + " est en quantité insuffisante");
                        Console.WriteLine("Quantité : " + elem[1]);
                    }
                }
                else // pour les fleurs
                {
                    if (int.Parse(elem[1]) < 100)
                    {
                        Console.WriteLine("produit : " + elem[0] + " est en quantité insuffisante");
                        Console.WriteLine("Quantité : " + elem[1]);
                    }
                }
            }
        }
        static void MenuClient(string courriel) //obsolete
        {
            bool quit = false;
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
                Console.WriteLine("5. Quitter");
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
                    case 5: quit = true; break;
                }
            }

        } // à supprimer
        public void Menu()
        {
            List<string[]> panier = new List<string[]>();
            Console.Clear();
            status = StatutClient();
            Console.WriteLine("bienvenue chez Belle Fleur ! @ " + utilisateur);
            Console.WriteLine("Statut actuel : " + status);
            Console.WriteLine("Nous vous accueillons tout les jours dans nos magasins");
            Console.WriteLine("Que voulez vous faire :");
            Console.WriteLine("1.Voir mes commandes");
            Console.WriteLine("2.Faire une nouvelle commande");
            Console.WriteLine("3.Modifier mes informations");
            Console.WriteLine("4.Quitter");
            int res = GoodValue(1, 4);
            switch (res)
            {
                case 1: VoireCommande(); break;
                case 2: RemplissagePanier(new List<string[]>()); break;
                case 3: ModificationDonnées(); break;
                case 4: break;
            }


        }
        public void RemplissagePanier(List<string[]> panier, bool isFlowerFilling = false)
        {
            Console.Clear();
            float prixTotal = 0f;
            Console.WriteLine("Voici votre panier actuel :");
            Console.WriteLine("===============");
            if (panier != null)
            {
                prixTotal = 0f;
                foreach (string[] elem in panier)
                {
                    Console.WriteLine(elem[0] + ": " + elem[1] + ", quantité :" + elem[2] + ", prix : " + elem[3] + " euros");
                    prixTotal += float.Parse(elem[3]);
                    Console.WriteLine("---------------");
                }
                Console.WriteLine("===============");
                Console.WriteLine("Prix total : " + prixTotal + " euros");
                Console.WriteLine();

            }
            else Console.WriteLine("Votre panier est vide");
            int r;
            if (!isFlowerFilling)
            {
                Console.WriteLine("Que voulez-vous faire ? :");
                Console.WriteLine("1. ajouter un bouquet composé");
                Console.WriteLine("2. ajouter un bouquet manuellement");
                Console.WriteLine("3. faire une demande de bouquet customisée");
                Console.WriteLine("4. passer au paiement");
                Console.WriteLine("5. quitter");
                r = GoodValue(1, 5);
            }

            else { r = 2; };
            switch (r)
            {
                case 1:
                    {
                        Console.Clear();
                        Console.WriteLine("Voici les bouquets disponibles :");
                        List<string[]> bouquets = fetcher.ExecuterCommandeSqlList("select idproduit,nom,quantite,prixIndiv,composition,catégorie from produits where isAlreadyComposed = 1");
                        fetcher.DisplayData("select idproduit,nom,quantite,prixIndiv,composition,catégorie from produits where isAlreadyComposed = 1");
                        Console.Write("veuillez indiquer quel bouquet vous intéresse (avec son numéro de produit) :");
                        string reponse = Console.ReadLine();
                        string[] tempPanier = null;
                        foreach (string[] elem in bouquets)
                        {
                            if (elem[0] == reponse)
                            {
                                tempPanier = new string[] { elem[0], elem[1], elem[3], elem[2] };
                            }
                        }
                        if (tempPanier == null)
                        {
                            Console.WriteLine("le bouquet n'a pas été trouvé. Veuillez réessayer.");
                            Console.ReadKey();
                            RemplissagePanier(panier);
                        }
                        else
                        {
                            Console.Write("combien en voulez-vous ? : ");
                            int quantiteDejaCommande = 0;
                            foreach (string[] elem in panier)
                            {
                                if (elem[0] == tempPanier[0]) quantiteDejaCommande += int.Parse(elem[2]);
                            }
                            int quantiteRestante = int.Parse(tempPanier[3]) - quantiteDejaCommande;
                            Console.WriteLine(quantiteRestante + " restants");
                            if (quantiteRestante == 0)
                            {
                                Console.WriteLine("Il n'y a plus de cet élément restant");
                                Console.ReadKey();
                                RemplissagePanier(panier);
                            }
                            else
                            {
                                int i = GoodValue(1, quantiteRestante);
                                string l = i.ToString();
                                float prix = float.Parse(l) * float.Parse(tempPanier[2]); // peut causer des erreurs
                                panier.Add(new string[5] { tempPanier[0], tempPanier[1], l, prix.ToString(), "VINV" });
                                Console.WriteLine("succès !");
                                Console.ReadKey();
                                RemplissagePanier(panier);
                            }


                        }


                        break;
                    }
                case 2:
                    {
                        Console.WriteLine("Voici les fleurs disponibles (seulement celles disponibles dans la période actuelle) :");
                        int dateAjd = DateTime.Now.Month;
                        string commande = String.Format("select idproduit,nom,quantite,prixIndiv from produits where isAlreadyComposed = 0 and {0} between dateA and dateB", dateAjd);
                        List<string[]> fleurs = fetcher.ExecuterCommandeSqlList(commande);
                        fetcher.DisplayData(commande);
                        Console.WriteLine("Veuillez indiquer le numéro de la fleur que vous voulez ajouter : ");
                        string reponse = Console.ReadLine();
                        Tuple<string, int>[] quantiteDejaFourni = new Tuple<string, int>[1000]; //on supposera que le client ne peut pas passer plus de 1000 commandes en simultané. 

                        string[] tempPanier = null;
                        foreach (string[] elem in fleurs)
                        {
                            if (elem[0] == reponse)
                            {
                                tempPanier = new string[] { elem[0], elem[1], elem[3], elem[2] };
                            }
                        }
                        if (tempPanier == null)
                        {
                            Console.WriteLine("la fleur n'a pas été trouvée. Veuillez réessayer.");
                            Console.ReadKey();
                            RemplissagePanier(panier);
                        }
                        else
                        {
                            Console.Write("combien en voulez-vous ? : ");
                            int quantiteDejaCommande = 0;
                            foreach (string[] elem in panier)
                            {
                                if (elem[0] == tempPanier[0]) quantiteDejaCommande += int.Parse(elem[2]);
                            }
                            int quantiteRestante = int.Parse(tempPanier[3]) - quantiteDejaCommande;
                            Console.WriteLine(quantiteRestante + " restants");
                            if (quantiteRestante == 0)
                            {
                                Console.WriteLine("Il n'y a plus de cet élément restant");
                                Console.ReadKey();
                                RemplissagePanier(panier);
                            }
                            else
                            {
                                int i = GoodValue(1, int.Parse(tempPanier[3]) - quantiteDejaCommande);
                                string l = i.ToString();
                                float prix = float.Parse(l) * float.Parse(tempPanier[2]); // peut causer des erreurs
                                panier.Add(new string[5] { tempPanier[0], tempPanier[1], l, prix.ToString(), "CPAV" });
                                Console.WriteLine("succès !");

                                Console.WriteLine("voulez vous continuer la composition du bouquet ? (Y/N):");
                                string rep = Console.ReadLine();
                                if (rep == "Y" || rep == "y")
                                {
                                    RemplissagePanier(panier, true);
                                }
                                else
                                {
                                    RemplissagePanier(panier);
                                }
                            }

                        }
                        break;
                    }
                case 3:
                    {
                        Console.WriteLine("Vous avez choisi de faire un bouquet sur note");
                        Console.WriteLine("Veuillez indiquer la note (200 caractères maximum & ne pas utiliser de ' ):");
                        string note = Console.ReadLine();
                        Console.WriteLine("Veuillez indiquer le prix dépensé :");
                        float prix = float.Parse(Console.ReadLine());
                        Console.WriteLine("La commande a été enregistrée ! nous reviendrons vers vous une fois traité par un expert");
                        panier.Add(new string[5] { "-1", note, "1", prix.ToString(), "CPAV" });
                        Console.ReadKey();
                        RemplissagePanier(panier);
                        break;
                    }
                case 4:
                    {
                        Console.WriteLine("Nous allons procéder au paiement, veuillez patienter...");
                        float prixDef = 0f;
                        Console.WriteLine(status);
                        if (status == "Or")
                        {
                            prixDef = prixTotal * 0.85f;
                        }
                        else if (status == "Bronze")
                        {
                            prixDef = prixTotal * 0.95f;
                        }
                        else
                        {
                            prixDef = prixTotal;
                        }
                        Console.WriteLine("la sommme totale à payer est : " + prixDef + " euros. Une réduction est appliqué en fonction de votre statut");
                        Thread.Sleep(1000);
                        Console.WriteLine("succès ! Quel message souhaitez-vous inscrire sur la commande ? ");
                        string message = Console.ReadLine();
                        Console.WriteLine("Date de livraison ? : (format dd/mm/aaaa)");
                        DateTime livraison = DateTime.Parse(Console.ReadLine());
                        Console.WriteLine("Lieu de livraison ? :");
                        string lieuDeLivraison = Console.ReadLine();
                        //calcule le nombre de jour entre aujourd'hui et le jour de la livraison
                        int dayOfDifference = (livraison - DateTime.Now).Days; //renvoie la différence en jours
                        string note = "";
                        if (dayOfDifference > 3)
                        {
                            foreach (string[] elem in panier)
                            {
                                if (elem[0] != "-1") /* -1 correspond à une demande */ fetcher.ExecuterCommande("update produits set quantite = quantite - " + elem[2] + " where idproduit = " + elem[0]);
                                else
                                {
                                    note += elem[1] + ";" + elem[4];
                                    prixTotal += float.Parse(elem[3]);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Comme votre commande a été passé 3 jours avant la livraison, nous ne pouvons garantir le stock");
                            foreach (string[] elem in panier) // alors on convertit tout les éléments en l'état VINV
                            {
                                elem[4] = "VINV";
                                if (elem[0] == "-1")
                                {
                                    note += elem[1] + ";" + "VINV";
                                }
                            }
                        }
                        Console.WriteLine("magasin de rattachement (pour récupérer votre commande alternativement ):");
                        string magasin = Console.ReadLine();
                        string commandeAjout = String.Format("insert into commande(idclient, prix, livraisonAdresse, message, livraisonDate, commandeDate, note,magasin) value ({0},{1},'{2}','{3}','{4}','{5}','{6}','{7}')", idClient, prixDef.ToString("N2", new CultureInfo("en-US")), lieuDeLivraison, message, livraison.ToString("yyyyMMdd"), DateTime.Now.ToString("yyyyMMdd"), note, magasin);
                        fetcher.ExecuterCommande(commandeAjout);
                        string idCommande = fetcher.ExecuterCommandeSqlList("select idCommande from commande where idclient = " + idClient + " order by idCommande desc")[0][0];
                        string commandeAjoutProduit = "";
                        foreach (string[] elem in panier) // on lis chaque item à sa commande
                        {
                            commandeAjoutProduit = String.Format("insert into composition(idcommande,idproduit,nombre,etat) value ({0}, {1}, {2}, '{3}')", idCommande, elem[0], elem[2], elem[4]);
                            if (elem[0] != "-1") fetcher.ExecuterCommande(commandeAjoutProduit);
                        }
                        Console.WriteLine("la commande est passé ! Merci de votre patience, et à très bientôt chez Belle Fleur !");
                        Console.WriteLine("===================================");
                        Console.WriteLine("Veuillez presser un bouton ... ");
                        Console.ReadKey();
                        Menu();
                        break;
                    }
                case 5:
                    {
                        panier = null;
                        Console.WriteLine("votre panier a été vidé. Nous vous remercions de votre visite.");
                        Console.WriteLine("===================================");
                        Console.WriteLine("Veuillez presser un bouton ... ");
                        Console.ReadKey();
                        Menu();
                        break;
                    }
            }
        }

        public void VoireCommande()
        {
            Console.Clear();
            Console.WriteLine("Voici vos commandes en cours :");
            List<string[]> commandes = fetcher.ExecuterCommandeSqlList("select idcommande,prix,commandeDate,livraisonAdresse,livraisonDate,note, nom,nombre,(prixIndiv*nombre) as prixElement, composition,catégorie,message from commande natural join composition join produits p on composition.idproduit = p.idproduit where idclient = " + idClient + " order by idcommande");
            if (commandes != null)
            {
                List<string> indices = new List<string>();
                foreach (string[] elem in commandes)
                {
                    if (indices.Find(x => elem[0] == x) == null) //nouvel indice de commande
                    {
                        Console.WriteLine();
                        Console.WriteLine(String.Format("Commande numéro {0} : fait le {1}. \n Livraison prévu / effectué le {2} à {3} \n Prix total : {4} \n Note de bouquet custom : {5} \n Message : {6} \n Items :", elem[0], elem[2], elem[4], elem[3], elem[1], elem[5], elem[11]));
                        indices.Add(elem[0]);
                    }
                    Console.WriteLine(String.Format("------------ {0}: quantite : {1} ; prix : {2} ; composition : {3}", elem[6], elem[7], elem[8], elem[9]));
                    if (elem[5] != "") Console.WriteLine("------------ Et un bouquet customisé !");

                }

                Console.WriteLine("Que voulez vous faire ? :");
                Console.WriteLine("1. supprimer une commande");
                Console.WriteLine("2. quitter");
                int r = GoodValue(1, 2);
                switch (r)
                {
                    case 1:
                        {
                            Console.Write("laquelle ? : ");
                            int ligne = int.Parse(Console.ReadLine());

                            if (indices.Find(x => ligne.ToString() == x) != null)
                            {
                                fetcher.ExecuterCommande("delete commande from commande where idcommande = " + ligne);
                                Console.WriteLine("Suppression faite !");
                            }
                            else
                            {
                                Console.WriteLine("Ceci n'est pas l'une de vos commandes !!");
                            }
                            Console.ReadKey();
                            Menu();
                            break;
                        }
                    case 2:
                        Menu();
                        break;
                }
            }
            else
            {
                Console.WriteLine("pas de commande dans la base de donnée à votre nom");
                Console.ReadKey();
                Menu();
            }
        }
        void ModuleClient()
        {
            bool quit = false;
            while (!quit)
            {
                Console.Clear();
                Console.WriteLine("Que voulez vous faire ?");
                Console.WriteLine("1. Afficher les clients");
                Console.WriteLine("2. Ajouter un client");
                Console.WriteLine("3. Supprimer un client");
                Console.WriteLine("4. Exporter les informations des clients qui ont fait plusieurs commandes ce mois");
                Console.WriteLine("5. Quitter");
                int r = GoodValue(1, 5);
                switch (r)
                {
                    case 1: AffichageClients(); break;
                    case 2: AjouterClient(); break;
                    case 3: SupprimerClient(); break;
                    case 4: ExportClientsMois(); break;
                    case 5: quit = true; break;
                }
                Console.ReadLine();
            }
            AdminMenu();

        }

        void ModuleProduit()
        {
            bool quit = false;
            while (!quit)
            {
                Console.Clear();
                Console.WriteLine("Que voulez vous faire ?");
                Console.WriteLine("1. Afficher les produits");
                Console.WriteLine("2. Ajouter un produit");
                Console.WriteLine("3. Supprimer un produit");
                Console.WriteLine("4. Ajouter du stock pour un produit");
                Console.WriteLine("5. Quitter");
                int r = GoodValue(1, 5);
                switch (r)
                {
                    case 1: ProduitsMagasin(); break;
                    case 2: AjouterProduit(); break;
                    case 3: SupprimerProduit(); break;
                    case 4: AjoutStock(); break;
                    case 5: quit = true; AdminMenu(); break;
                }
                Console.ReadKey();
            }
            AdminMenu();
        }
        void ModuleCommande()
        {
            Console.Clear();
            Console.WriteLine("Module Commande :");
            Console.WriteLine("================");
            Console.WriteLine("1. Voir toutes les commandes");
            Console.WriteLine("2. voir les commandes d'un client");
            Console.WriteLine("3. voir l'état des commandes");
            Console.WriteLine("4. Changer l'etat d'une commande");
            Console.WriteLine("5. Quitter");
            int r = GoodValue(1, 5);
            switch (r)
            {
                case 1:
                    {
                        fetcher.DisplayData("select * from commande");
                        Console.ReadKey();
                        ModuleCommande();
                        break;
                    }
                case 2:
                    {
                        Console.WriteLine("Veuillez indiquer le numéro du client en question : ");
                        string value = Console.ReadLine();
                        List<string[]> commandeClient = fetcher.ExecuterCommandeSqlList("select * from commande where idclient =" + value);
                        if (commandeClient[0] != null)
                        {
                            fetcher.DisplayData("select * from commande where idclient =" + value);
                        }
                        else
                        {
                            Console.WriteLine("Aucun client ne correspond à ce numéro");
                        }
                        Console.ReadKey();
                        ModuleCommande();
                        break;
                    }
                case 3:
                    {
                        List<string[]> données = fetcher.ExecuterCommandeSqlList("select c.idcommande,nom,etat,note from commande join composition c on commande.idcommande = c.idcommande natural join produits;");
                        if (données[0] != null)
                        {
                            List<string> indices = new List<string>();
                            foreach (string[] elem in données)
                            {
                                if (indices.Find(x => elem[0] == x) == null) //nouvel indice de commande
                                {
                                    Console.WriteLine("Commande n°" + elem[0]);
                                    indices.Add(elem[0]);
                                    if (elem[3] != "") // présence d'un bouquet customisé
                                    {
                                        string statut = elem[3].Split(';')[1]; //le statut 
                                        string nom = elem[3].Split(";")[0]; // le nom de la demande
                                        Console.WriteLine("Demande customisée : " + nom + "; etat : " + statut);
                                    }
                                }
                                Console.WriteLine("element : " + elem[1] + "; etat :" + elem[2]);
                            }
                            List<string[]> données2 = fetcher.ExecuterCommandeSqlList("select idcommande, note from commande where idcommande not in (select distinct idcommande from composition)");
                            foreach (string[] elem in données2)
                            {
                                if (elem[1] != "")
                                {
                                    Console.WriteLine("Commande n°" + elem[0]);
                                    string statut = elem[1].Split(';')[1]; //le statut 
                                    string nom = elem[1].Split(";")[0]; // le nom de la demande
                                    Console.WriteLine("Demande customisée : " + nom + "; etat : " + statut);
                                }
                            }
                        }

                        else
                        {
                            Console.WriteLine("aucune donnée à afficher");
                        }
                        Console.ReadKey();
                        ModuleCommande();
                        break;
                    }
                case 4:
                    {
                        List<string[]> données = fetcher.ExecuterCommandeSqlList("select c.idcommande,nom,etat,note from commande join composition c on commande.idcommande = c.idcommande natural join produits;");
                        if (données[0] != null)
                        {
                            List<string> indices = new List<string>();
                            foreach (string[] elem in données)
                            {
                                if (indices.Find(x => elem[0] == x) == null) //nouvel indice de commande
                                {
                                    Console.WriteLine("Commande n°" + elem[0]);
                                    indices.Add(elem[0]);
                                    if (elem[3] != "") // présence d'un bouquet customisé
                                    {
                                        string statut = elem[3].Split(';')[1]; //le statut 
                                        string nom = elem[3].Split(";")[0]; // le nom de la demande
                                        Console.WriteLine("Demande customisée : " + nom + "; etat : " + statut);
                                    }
                                }
                                Console.WriteLine("element : " + elem[1] + "; etat :" + elem[2]);
                            }
                            List<string[]> données2 = fetcher.ExecuterCommandeSqlList("select idcommande, note from commande where idcommande not in (select distinct idcommande from composition)");
                            foreach (string[] elem in données2)
                            {
                                if (elem[1] != "")
                                {
                                    indices.Add(elem[0]);
                                    Console.WriteLine("Commande n°" + elem[0]);
                                    string statut = elem[1].Split(';')[1]; //le statut 
                                    string nom = elem[1].Split(";")[0]; // le nom de la demande
                                    Console.WriteLine("Demande customisée : " + nom + "; etat : " + statut);
                                }
                            }

                            Console.WriteLine();
                            Console.WriteLine("Quel commande voulez-vous modifier l'état d'une sous-commande ?");
                            string rep = Console.ReadLine();
                            if (indices.Find(x => rep == x) != null)
                            {
                                fetcher.DisplayData("select idproduit,nom,etat from commande join composition c on commande.idcommande = c.idcommande natural join produits where c.idcommande = " + rep);
                                string Custom = fetcher.ExecuterCommandeSqlString("select note from commande where idcommande =" + rep);
                                if (Custom != "")
                                {
                                    Console.WriteLine("-1 : " + Custom);
                                }
                                Console.WriteLine("Quel sous commande ? :");
                                string rep2 = Console.ReadLine();
                                Console.WriteLine("Entrer le nouveau état :");
                                string etat = Console.ReadLine();
                                if (rep2 != "-1") fetcher.ExecuterCommande(String.Format("update composition set etat = '{0}' where idcommande = {1} and idproduit = {2};", etat, rep, rep2));
                                else fetcher.ExecuterCommande(String.Format("update commande set note = '{0}' where idcommande = {1}", Custom.Split(";")[0] + ";" + etat, rep));
                                Console.WriteLine("Modification faite !");
                            }
                        }
                        else
                        {
                            Console.WriteLine("aucune donnée à modifier");
                        }

                        Console.ReadKey();
                        ModuleCommande();
                        break;
                    }
                case 5:
                    {
                        AdminMenu();
                        break;
                    }
            }
        }
        void ModuleStat()
        {
            Console.Clear();
            Console.WriteLine("Bienvenue dans le menu statistiques");
            Console.WriteLine("Que souhaitez-vous connaitre ? ");
            Console.WriteLine("1. prix moyen du bouquet");
            Console.WriteLine("2. prix moyen d'une commande");
            Console.WriteLine("3. le meilleur client d'une période donnée");
            Console.WriteLine("4. le bouquet standard le plus populaire");
            Console.WriteLine("5. Le magasin ayant généré le plus de chiffre d'affaire");
            Console.WriteLine("6. Revenir au menu principal");
            int r = GoodValue(1, 6);
            switch (r)
            {
                case 1:
                    {
                        float prixMoyen = float.Parse(fetcher.ExecuterCommandeSqlList("select avg(prixIndiv) from produits where isAlreadyComposed = 1")[0][0]);
                        Console.WriteLine("le prix moyen des bouquet est : " + prixMoyen);
                        Console.ReadKey();
                        ModuleStat();
                        break;
                    }
                case 2:
                    {
                        float prixMoyenCommande = float.Parse(fetcher.ExecuterCommandeSqlList("select avg(prix) from commande")[0][0]);
                        Console.WriteLine("le prix moyen des commandes est : " + prixMoyenCommande);
                        Console.ReadKey();
                        ModuleStat();
                        break;
                    }
                case 3:
                    {
                        Console.WriteLine("entrez les dates limites à la suite : (format dd/mm/yyyy)");
                        DateTime dateA = DateTime.Parse(Console.ReadLine());
                        DateTime dateB = DateTime.Parse(Console.ReadLine());
                        string[] meilleur_client = fetcher.ExecuterCommandeSqlList(String.Format("select nom,prenom,sum(prix) from client join (select prix,idclient from commande where commandeDate between '{0}' and '{1}') c on client.idclient = c.idclient  group by c.idclient order by sum(prix) desc limit 1", dateA.ToString("yyyyMMdd"), dateB.ToString("yyyyMMdd")))[0];
                        if (meilleur_client != null)
                        {
                            Console.WriteLine("Le meilleur client est " + meilleur_client[0] + " " + meilleur_client[1]);
                            Console.WriteLine("total payé : " + meilleur_client[2]);
                        }
                        else Console.WriteLine("Pas de client dans cette timezone.");
                        Console.ReadKey();
                        ModuleStat();
                        break;
                    }
                case 4:
                    {
                        string[] meilleur_bouquet = fetcher.ExecuterCommandeSqlList("select nom,sum(nombre) from produits join composition c on produits.idproduit = c.idproduit where isAlreadyComposed = 1 group by c.idproduit order by sum(nombre) desc limit 1")[0];
                        Console.WriteLine("Meilleur bouquet : " + meilleur_bouquet[0]);
                        Console.WriteLine("total vendu : " + meilleur_bouquet[1]);
                        Console.ReadKey();
                        ModuleStat();
                        break;
                    }
                case 5:
                    {
                        string[] meilleur_magasin = fetcher.ExecuterCommandeSqlList("select magasin,sum(prix) from commande group by magasin order by sum(prix) desc limit 1")[0];
                        Console.WriteLine("Meilleur magasin : " + meilleur_magasin[0]);
                        Console.WriteLine("total vendu : " + meilleur_magasin[1]);
                        Console.ReadKey();
                        ModuleStat();
                        break;
                    }
                case 6:
                    {
                        AdminMenu();
                        break;
                    }
            }
        }
        #region vieux code, non utilisé
        void AjouterClient() // permet d'ajouter un client
        { 
            Console.Clear();
            Console.WriteLine("Vous allez vous inscrire à belle fleure. Nous reccueillerons les données indiquées uniquement dans un but d'inscription");
            Console.WriteLine("Veuillez appuyer sur un bouton pour indiquer votre accord :");
            Console.ReadKey();
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
            while (exist == 1)
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
                    command.CommandText = "INSERT INTO `Fleurs`.`client` (`nom`, `prenom`,`telephone`,`courriel`, `motDePasse`, `facturationAdresse`,`creditCard`) VALUES ('" + nom + "', '" + prenom + "','" + telephone + "' ,'" + courriel + "','" + mdp + "', '" + adresse + "',    '" + cb + "');";

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
            FirstLogin();
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
                if (exist == 0)
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


        } // aucune référence
        static string StatutClient(string courriel)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";
            int count;
            DateTime commandeDate = DateTime.Now;
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
            else if (count < 5) { statut = "Bronze"; }
            else { statut = "Or"; }
            return statut;
        } //obsolete
        string StatutClient() // permet de connaitre le statut du client 
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;";
            int count;
            DateTime commandeDate = DateTime.Now;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM commande INNER JOIN client ON commande.idclient = client.idclient WHERE client.idclient = @client AND MONTH(commande.commandeDate) = @month";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@client", idClient);
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
            else if (count < 5) { statut = "Bronze"; }
            else { statut = "Or"; }
            return statut;
        }
        static string ChoixProduits()
        {
            Console.Clear();
            Console.WriteLine("Quel type de commande souhaitez-vous ?");
            Console.WriteLine("1. Commande standard");
            Console.WriteLine("2. Commande personnalisée");
            Console.WriteLine("3. Quitter");
            int r = GoodValue(1, 3);
            string choix = "";
            switch (r)
            {
                case 1: choix = ChoixCommandeStandard(); break;
                case 2: break;
                case 3: break;
            }
            return choix;
        } // obsolete
        static string ChoixCommandeStandard() // obsolete
        {
            Console.Clear();
            Console.WriteLine("Quel bouquet souhaitez-vous");
            Console.WriteLine("1. Gros Merci, un arrangement floral avec marguerites et verdure parfait pour toute occasion pour un prix de 45 €");
            Console.WriteLine("2. L’amoureux, un arrangement floral avec roses blanches et roses rouges pour la St-Valentin pour un prix de 65 €");
            Console.WriteLine("3. L’Exotique, un arrangement floral avec ginger, oiseaux du paradis, roses et genet parfait pour toute occasion pour un prix de 40 €");
            Console.WriteLine("4. Maman, un arrangement  floral avec gerbera, roses blanches, lys et alstroméria pour la Fête des mères pour un prix de 80 €");
            Console.WriteLine("5. Vive la mariée, un arrangement  floral avec lys et orchidées parfait pour un mariage pour un prix de 120 €");
            Console.WriteLine("6. Quitter");
            string bouquet = "";
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
        #endregion
        void AffichageClients() // pour l'admin
        {
            fetcher.DisplayData("SELECT c.nom, c.prenom, c.courriel, COUNT(co.idcommande) AS nombre_commandes FROM client c LEFT JOIN commande co ON c.idclient = co.idclient WHERE c.idclient!=1 GROUP BY c.idclient;");
        }
        void AffichageClient() // pour le client
        {

            fetcher.DisplayData($"SELECT * FROM client WHERE idclient = {idClient}");
        }
        void ModificationDonnées() // permet de modifier les paramètres d'un client
        {
            Console.Clear();
            AffichageClient();
            Console.WriteLine("Que voulez vous modifier ?");
            Console.WriteLine("1. Nom");
            Console.WriteLine("2. Telephone");
            Console.WriteLine("3. Courriel");
            Console.WriteLine("4. Mot de passe");
            Console.WriteLine("5. Adresse de facturation");
            Console.WriteLine("6. Carte de crédit");
            Console.WriteLine("7. Quitter");
            int r = GoodValue(1, 7);
            switch (r)
            {
                case 1: ModificationClient("nom"); break;
                case 2:
                    ModificationClient("telephone");
                    break;
                case 3: ModificationClient("courriel"); break;
                case 4: ModificationClient("motDePasse"); break;
                case 5: ModificationClient("facturationAdresse"); break;
                case 6: ModificationClient("creditCard"); break;
                case 7: Menu(); break;
            }
            Console.ReadLine();
        }
        void ModificationClient(string info)
        {
            Console.WriteLine("Veuillez rentrer la nouvelle donnée");
            int valeurInt = 0;
            string valeur = null;
            if (info == "telephone" || info == "creditCard")
            {
                valeurInt = Convert.ToInt32(Console.ReadLine());
            }
            else
            {
                valeur = Console.ReadLine();
            }
            string query;
            if (valeur != null)
            {
                query = $"UPDATE client SET {info} = '{valeur}' WHERE idclient = {idClient}";
            }
            else
            {
                query = $"UPDATE client SET {info} = {valeurInt} WHERE idclient = {idClient}";
            }
            fetcher.ExecuterCommande(query);
        }

        void SupprimerClient()
        {
            Console.WriteLine("Veuillez rentrer le courriel du client que vous souhaitez supprimer");
            string mail = Console.ReadLine();
            fetcher.ExecuterCommande("DELETE FROM client WHERE courriel = '" + mail + "';");
        }
        void ExportClientsMois()
        {
            fetcher.Export2Xml("test1.xml", "SELECT c.*, COUNT(co.idcommande) AS nombre_commandes FROM client c LEFT JOIN commande co ON c.idclient = co.idclient WHERE c.idclient != 1 AND YEAR(co.commandeDate) = YEAR(CURDATE()) AND MONTH(co.commandeDate) = MONTH(CURDATE()) GROUP BY c.idclient, c.nom, c.prenom, c.courriel, c.telephone, c.creditCard HAVING COUNT(co.idcommande) != 0;");
        }
        void AjouterProduit()
        {
            Console.Clear();
            Console.WriteLine("Quel type de produit voulez-vous ajouter ?");
            Console.WriteLine("1. Fleurs individuelles");
            Console.WriteLine("2. Bouquets");
            Console.WriteLine("3. Quitter");
            int r = GoodValue(1, 3);
            Console.WriteLine("Entrez les informations pour le nouveau produit :");
            Console.Write("Nom : ");
            string nom = Console.ReadLine();
            Console.Write("Quantité : ");
            int quantite = int.Parse(Console.ReadLine());
            Console.Write("Prix individuel (format u,d): ");
            float prixIndiv = float.Parse(Console.ReadLine());
            Console.Write("Date A : ");
            DateTime dateA = DateTime.Parse(Console.ReadLine());
            Console.Write("Date B : ");
            int dateB = int.Parse(Console.ReadLine());
            string query;
            switch (r)
            {
                case 1:
                    query = $"INSERT INTO produits (nom, quantite, prixIndiv, dateA, dateB, isAlreadyComposed, composition, catégorie) VALUES ('{nom}', {quantite}, {prixIndiv.ToString("N2", new CultureInfo("en-US"))}, {dateA.ToString("yyyyMMdd")}, {dateB.ToString("yyyyMMdd")}, {0}, NULL, NULL);";
                    fetcher.ExecuterCommande(query);
                    break;
                case 2:
                    Console.Write("Composition : ");
                    string composition = Console.ReadLine();
                    Console.Write("Catégorie : ");
                    string categorie = Console.ReadLine();
                    query = $"INSERT INTO produits (nom, quantite, prixIndiv, dateA, dateB, isAlreadyComposed, composition, catégorie) VALUES ('{nom}', {quantite}, {prixIndiv.ToString("N2", new CultureInfo("en-US"))}, {dateA.ToString("yyyyMMdd")}, {dateB.ToString("yyyyMMdd")}, {1}, '{composition}', '{categorie}');";
                    fetcher.ExecuterCommande(query);
                    break;
                case 3: break;
            }
        }
        void SupprimerProduit() 
        {
            Console.Clear();
            Console.WriteLine("Quel type de produit voulez-vous supprimer ?");
            Console.WriteLine("1. Fleurs individuelles");
            Console.WriteLine("2. Bouquets");
            Console.WriteLine("3. Quitter");
            int idProduit = 0;
            int r = GoodValue(1, 3);
            switch (r)
            {
                case 1: fetcher.DisplayData("SELECT idproduit,nom,prixIndiv,quantite FROM produits WHERE isAlreadyComposed=0;"); break;
                case 2: fetcher.DisplayData("SELECT idproduit,nom,prixIndiv,quantite FROM produits WHERE isAlreadyComposed=1;"); break;
                case 3: break;
            }
            if (r == 1)
            {
                List<string[]> listeIdProduits = fetcher.ExecuterCommandeSqlList("SELECT idproduit FROM produits WHERE isAlreadyComposed=0");
                while (!listeIdProduits.SelectMany(array => array).ToList().Contains(idProduit.ToString()))
                {
                    Console.WriteLine("Veuillez rentrer un idProduit valide :");
                    idProduit = Convert.ToInt32(Console.ReadLine());
                }
            }
            else if (r == 2)
            {
                List<string[]> listeIdProduits = fetcher.ExecuterCommandeSqlList("SELECT idproduit FROM produits WHERE isAlreadyComposed=1");
                while (!listeIdProduits.SelectMany(array => array).ToList().Contains(idProduit.ToString()))
                {
                    Console.WriteLine("Veuillez rentrer un idProduit valide :");
                    idProduit = Convert.ToInt32(Console.ReadLine());
                }
            }
            fetcher.ExecuterCommande($"DELETE FROM produits WHERE idproduit ={idProduit} ");
        }
        void ProduitsMagasin()
        {
            bool quit = false;
            while (!quit)
            {
                Console.Clear();
                Console.WriteLine("Que voulez vous faire ?");
                Console.WriteLine("1. Afficher les fleurs individuelles");
                Console.WriteLine("2. Ajouter les bouquets");
                Console.WriteLine("3. Quitter");
                int r = GoodValue(1, 3);
                switch (r)
                {
                    case 1: fetcher.DisplayData("SELECT idproduit,nom,prixIndiv,quantite FROM produits WHERE isAlreadyComposed=0;"); break;
                    case 2: fetcher.DisplayData("SELECT idproduit,nom,prixIndiv,quantite FROM produits WHERE isAlreadyComposed=1;"); break;
                    case 3: quit = true; break;
                }
                Console.ReadLine();
            }

        }
        void AjoutStock()
        {
            Console.Clear();
            Console.WriteLine("De quel type de produit voulez-vous augmenter le stock ?");
            Console.WriteLine("1. Afficher les fleurs individuelles");
            Console.WriteLine("2. Ajouter les bouquets");
            Console.WriteLine("3. Quitter");
            int idProduit = 0;
            int r = GoodValue(1, 3);
            switch (r)
            {
                case 1: fetcher.DisplayData("SELECT idproduit,nom,prixIndiv,quantite FROM produits WHERE isAlreadyComposed=0;"); break;
                case 2: fetcher.DisplayData("SELECT idproduit,nom,prixIndiv,quantite FROM produits WHERE isAlreadyComposed=1;"); break;
                case 3: break;
            }
            if (r == 1)
            {
                List<string[]> listeIdProduits = fetcher.ExecuterCommandeSqlList("SELECT idproduit FROM produits WHERE isAlreadyComposed=0");
                while (!listeIdProduits.SelectMany(array => array).ToList().Contains(idProduit.ToString()))
                {
                    Console.WriteLine("Veuillez rentrer un idProduit valide :");
                    idProduit = Convert.ToInt32(Console.ReadLine());
                }
            }
            else if (r == 2)
            {
                List<string[]> listeIdProduits = fetcher.ExecuterCommandeSqlList("SELECT idproduit FROM produits WHERE isAlreadyComposed=1");
                while (!listeIdProduits.SelectMany(array => array).ToList().Contains(idProduit.ToString()))
                {
                    Console.WriteLine("Veuillez rentrer un idProduit valide :");
                    idProduit = Convert.ToInt32(Console.ReadLine());
                }
            }
            Console.WriteLine("Combien de produits voulez-vous ajouter ?");
            int quantiteAjoutee = Convert.ToInt32(Console.ReadLine());

            string query = $"UPDATE produits SET quantite = quantite + {quantiteAjoutee} WHERE idproduit = {idProduit}";

            fetcher.ExecuterCommande(query);

            Console.WriteLine($"{quantiteAjoutee} produits ont été ajoutés au produit n°{idProduit}");
            Console.ReadLine();
        }
    }
}