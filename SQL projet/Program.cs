
namespace SQL_projet
{
    class Prog
    {
        static void Main(string[] args)
        {
            string login = "SERVER=localhost;PORT=3306;DATABASE=Fleurs;UID=root;PASSWORD=root;"; //login pour se connecter à la database
            // Sql_fetcher fetcher = new Sql_fetcher(login);
            AffichageGraphique affichage = new AffichageGraphique();
            affichage.Affichage();
        }
    }
}