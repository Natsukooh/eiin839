class Client
{

    static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        Console.WriteLine("Client pour l'exercice 3.");
        Console.WriteLine("La requête envoyée sera de la forme http://localhost:8080/exercice3/<nom_méthode>?param1=<paramètre>");
        Console.WriteLine("C'est vous qui décidez de <nom_méthode> ainsi que de <paramètre>.");
        Console.WriteLine("Note : dans le cadre de ce TD, il n'existe qu'une seule méthode : incr. Vous pouvez en essayer d'autre, mais le serveur vous renverra (proporement) une erreur.\n");

        while(true)
        {
            Console.WriteLine("Veuillez entrer un nom de méthode : ");
            string method_name = Console.ReadLine();
            Console.WriteLine("Veuillez entrer un paramètre : ");
            string param = Console.ReadLine();

            string request = $"http://localhost:8080/exercice3/{method_name}?param1={param}";
            Console.WriteLine($"La requête qui va être envoyée est : {request}. Envoi imminent.");

            HttpResponseMessage response = await client.GetAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Le serveur a répondu : {responseBody}\n");
        }
    }
}
