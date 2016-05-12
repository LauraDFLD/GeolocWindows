using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace Géoloc
{
    class AsyncServer
    {
        // Port d'écoute du serveur
        private int port;

        private TcpListener listener;
        private bool token;

        private List<Telephone> listeTels;
        private Form1 formParent;
 
        /**
         * Constructeur
         * @param port Port utilisé pour la communication
         */
        public AsyncServer(int port, List<Telephone> listeTels, Form1 formParent)
        {
            this.port = port;
            this.listeTels = listeTels;
            this.formParent = formParent;
            token = true;
        }

        /**
         * Démarrage du serveur
         */
        public async void Start()
        {
            // Adresse IP sur laquelle écoute le serveur
            IPAddress ip = IPAddress.Loopback;
            
            // Création du socket en écoute
            listener = new TcpListener(ip, port);
            listener.Start();
            Console.WriteLine("Serveur démarré sur le port {0}", port);

            // Boucle d'écoute tant que le serveur est actif
            while(token)
            {
                Console.WriteLine("En attente...");
                try
                {
                    // On attend la connexion d'un client de façon asynchrone et
                    // non bloquante (await)
                    TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                    // Une fois une connexion établie on traite va recevoir les données
                    // de ce client
                    HandleConnectionAsync(tcpClient);
                }
                // Exception levée lors de l'arrêt du listener (listener.Stop())
                // Ce n'est pas très propre pour arrêter le serveur mais c'est court !
                catch (ObjectDisposedException e)
                {
                    Console.WriteLine("Exception : {0}", e.ToString());
                    token = false;
                }
            }
            Console.WriteLine("Serveur arrêté");
        }

        /**
         * Arret du serveur
         */
        public void Stop()
        {
            listener.Stop();
        }

        /**
         * Gestion de la connexion sur le socket et traitement des données
         * reçues
         */
        private async void HandleConnectionAsync(TcpClient tcpClient)
        {
            // Récupération d'infos de connexion pour affichage uniquement
            string clientInfo = tcpClient.Client.RemoteEndPoint.ToString();
            Console.WriteLine("Demande de connexion de {0}", clientInfo);

            // Les données sont récupérées de façon brute dans un tableau d'octets (bytes)
            // puis seront converties en texte (string)
            Byte[] bytes = new Byte[256];
            string data = null;

            try
            {
                // On déclare le flux sur lequel on va recevoir les données et
                // qui provient du socket
                NetworkStream stream = tcpClient.GetStream();

                int i;
                // Boucle pour recevoir toutes les données émises
                // On essaie de lire (de façon asynchrone et non bloquante -await- les 
                // données du flux (de la position 0 à la taille maxi déclarée)
                while ((i = await stream.ReadAsync(bytes, 0, bytes.Length))!=0) {
                    // On traduit les données brutes en chaîne de caractères
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Reçu : {0}", data);


                    Telephone tel = new Telephone(data);
                    
                    // Mise à jour de la liste (on regarde si le téléphone est déjà dans la liste
                    if (listeTels.Exists(elt => elt.id == tel.id))
                    {
                        int pos = listeTels.FindIndex(elt => elt.id == tel.id);
                        listeTels[pos] = tel;
                    }
                    else
                        listeTels.Add(tel);
                    formParent.affTel();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : {0}", e.ToString());
            }
            finally
            {
                Console.WriteLine("Fin de la connexion de {0}", clientInfo);
                tcpClient.Close();
            }
        }
    } 
}
