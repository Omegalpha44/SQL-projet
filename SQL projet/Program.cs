
namespace SQL_projet
{
    class Prog
    {
        static void Main(string[] args)
        {
            string login = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root"; //login pour se connecter à la base de donnée
            AffichageGraphique affichage = new AffichageGraphique(login);
            affichage.Affichage();
        }
    }
}