using Google.Protobuf.WellKnownTypes;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Crypto;
using System;
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
        Sql_fetcher fetcher;
        string utilisateur; //permet de nommer le client (nom + prenom)
        int idClient; //permet d'identifier le client
        public AffichageGraphique()
        {
            fetcher = null;
        }
        public AffichageGraphique(string connection)
        {
            fetcher = new Sql_fetcher(connection);
        }
        public void Affichage()
        {
            ExceptionManager(FirstLogin);
            if (utilisateur == "admin admin \n") ExceptionManager(AdminMenu);
            else ExceptionManager(Menu);
    
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
        int GoodValue(int a, int b) // vérifie la valeur fournie en entrée
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
                utilisateur = fetcher.ExecuterCommandeSqlString("select nom, prenom from client where motDePasse = '" + password + "' and courriel = '" + login+"'");
                idClient = int.Parse(fetcher.ExecuterCommandeSqlString("select idclient from client where motDePasse = '" + password + "' and courriel = '" + login + "'"));
                Console.WriteLine("Connection réussi !");
                Console.WriteLine("bienvenu " + utilisateur);
            }
            else
            {
                Console.WriteLine("Connection échoué !");
                Console.Write("Voulez vous créer un compte ? (Y/N): ");
                string rep = Console.ReadLine();
                switch(rep)
                {
                    case "Y" or "y": AjouterClient(); Console.WriteLine("Votre compte est créé ! Veuillez vous reconnecter");FirstLogin() ; break;
                    case "N" or "n": Console.WriteLine("veuillez réessayer de rentrer vos identifiants");FirstLogin();break;
                    default: Console.WriteLine("veuillez réessayer de rentrer vos identifiants"); FirstLogin(); break;
                }
            }
        }
        public void AdminMenu()
        {
            Console.Clear();
            Console.WriteLine("Bonjour Admin");
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

        public void Menu()
        {
            List<string[]> panier = new List<string[]>();
            Console.Clear();
            Console.WriteLine("bienvenue chez Belle Fleur ! @ " + utilisateur);
            Console.WriteLine("Nous vous accueillons tout les jours dans nos magasins");
            Console.WriteLine("Que voulez vous faire :");
            Console.WriteLine("1.Voir mes commandes");
            Console.WriteLine("2.Faire une nouvelle commande");
            Console.WriteLine("3.Quitter");
            int res = GoodValue(1, 3);
            switch(res)
            {
                case 1: VoireCommande(); break;
                case 2: RemplissagePanier(new List<string[]>());break;
                case 3: break;
            }
            
            
        }
        public void RemplissagePanier(List<string[]> panier,bool isFlowerFilling = false)
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
                    Console.WriteLine(elem[0] + ": " + elem[1] +", quantité :" + elem[2] +", prix : " + elem[3]+" euros");
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

            else {r = 2; };
            switch(r)
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
                                tempPanier = new string[] { elem[0], elem[1] , elem[3], elem[2] };    
                            }
                        }
                        if(tempPanier == null)
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
                            if(quantiteRestante == 0)
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
                        string commande = String.Format("select idproduit,nom,quantite,prixIndiv from produits where isAlreadyComposed = 0 and {0} between dateA and dateB",dateAjd);
                        List<string[]> fleurs = fetcher.ExecuterCommandeSqlList(commande);
                        fetcher.DisplayData(commande);
                        Console.WriteLine("Veuillez indiquer le numéro de la fleur que vous voulez ajouter : ");
                        string reponse = Console.ReadLine();
                        Tuple<string,int>[] quantiteDejaFourni = new Tuple<string, int>[1000]; //on supposera que le client ne peut pas passer plus de 1000 commandes en simultané. 
                        
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
                        Console.WriteLine("Veuillez indiquer la note (45 caractères maximum):");
                        string note = Console.ReadLine();
                        Console.WriteLine("Veuillez indiquer le prix dépensé :");
                        float prix = float.Parse(Console.ReadLine());
                        Console.WriteLine("La commande a été enregistrée ! nous reviendrons vers vous une fois traité par un expert");
                        panier.Add(new string[5]{ "-1",note,"1",prix.ToString(),"CPAV"});
                        Console.ReadKey();
                        RemplissagePanier(panier);
                        break;
                    }
                case 4:
                    {
                        Console.WriteLine("Nous allons procéder au paiement, veuillez patienter...");
                        Console.WriteLine("la sommme totale à payer est : " + prixTotal);
                        Thread.Sleep(1000);
                        Console.WriteLine("succès ! Quel message souhaitez-vous inscrire sur la commande ? ");
                        string message = Console.ReadLine();
                        Console.WriteLine("Date de livraison ? : (format dd/mm/aaaa)");
                        DateTime livraison = DateTime.Parse(Console.ReadLine());
                        Console.WriteLine("Lieu de livraison ? :");
                        string lieuDeLivraison = Console.ReadLine();
                        //calcule le nombre de jour entre aujourd'hui et le jour de la livraison
                        int dayOfDifference = (livraison-DateTime.Now).Days; //renvoie la différence en jours
                        string note = "";
                        if(dayOfDifference>3)
                        {
                            foreach (string[] elem in panier)
                            {
                                if (elem[0] != "-1") /* -1 correspond à une demande */ fetcher.ExecuterCommande("update produits set quantite = quantite - " + elem[2] + " where idproduit = " + elem[0]);
                                else
                                {
                                    note += elem[1] + "; ";
                                    prixTotal += float.Parse(elem[3]);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Comme votre commande a été passé 3 jours avant la livraison, nous ne pouvons guarantir le stock");
                        }
                        string commandeAjout = String.Format("insert into commande(idclient, prix, livraisonAdresse, message, livraisonDate, commandeDate, note) value ({0},{1},'{2}','{3}','{4}','{5}','{6}')",idClient,prixTotal,lieuDeLivraison,message,livraison.ToString("yyyyMMdd"), DateTime.Now.ToString("yyyyMMdd"), note);
                        fetcher.ExecuterCommande(commandeAjout);
                        string idCommande = fetcher.ExecuterCommandeSqlList("select idCommande from commande where idclient = " + idClient+ " order by idCommande desc")[0][0];
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
            List<string[]> commandes = fetcher.ExecuterCommandeSqlList("select idcommande,prix,commandeDate,livraisonAdresse,livraisonDate,note, nom,nombre,(prixIndiv*nombre) as prixElement, composition,catégorie,message from commande natural join composition join produits p on composition.idproduit = p.idproduit where idclient = "+idClient+" order by idcommande");
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
            Console.Clear();
            Console.WriteLine("Lorem Ipsum");
            Console.ReadLine();
            Menu();
        }
        void AjouterClient()
        {
            
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
            Console.WriteLine("Module Commande :");
            Console.WriteLine("================");
            Console.WriteLine("1. Ajouter une commande");
            Console.WriteLine("2. Voir les commandes");
            int r = GoodValue(1, 2);
            switch(r)
            {
                case 1: break;
                case 2: break;
            }
            Console.ReadLine();
            Menu();
        }
        void AjouterCommande()
        {
            //Console.WriteLine("Vous avez choisi de rajouter une commande dans la base de donnée");
            //Console.Write("Nouveau client ? : (Y/N)");
            //string rep = Console.ReadLine();
            //switch(rep)
            //{
            //    case "Y" or "y":
            //        {
            //            Console.WriteLine("Vous allez entrer des informations sur le client :");
            //            Console.Write("nom :");
            //            string nom = Console.ReadLine();
            //            Console.Write("prenom :");
            //            string prenom = Console.ReadLine();
            //            bool goodphone = false;
            //            while (!goodphone)
            //            Console.Write("téléphone :");
            //            try
            //            {
            //                int phone = int.Parse(Console.ReadLine());
            //                goodphone = true;
            //            }
            //            catch
            //            {
            //            }
            //            Console.Write("courriel : ");
            //            string courriel = Console.ReadLine();
            //            Console.Write("mot de passe : ");
            //            string mdp = Console.ReadLine();
            //            Console.Write("addresse de facturation : ");
            //            string facturation = Console.ReadLine();
            //            break;
            //            bool goodcard = false;
            //            while (!goodcard)
            //                Console.Write("téléphone :");
            //            try
            //            {
            //                int card = int.Parse(Console.ReadLine());
            //                goodcard = true;
            //            }
            //            catch
            //            {
            //            }
                        

            //        }
            //    case "O" or "o":
            //        {
                        
            //            break;
            //        }
            //    default: break;
            //}
        }
        void ModuleStat()
        {
            Console.Clear();
            Console.WriteLine("Lorem Ipsum");
            Console.ReadLine();
            Menu();
        }
    }
}