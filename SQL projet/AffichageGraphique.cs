using System;
using System.Collections.Generic;
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
    }
}