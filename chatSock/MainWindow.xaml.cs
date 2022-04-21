using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Windows.Threading;

namespace chatSock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Socket nomeSocket = null;
        DispatcherTimer dTimer = null;  //solo grazie allo using sysyem threading

        public MainWindow()
        {
            InitializeComponent();
            nomeSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress mittente_address = IPAddress.Any;
            IPEndPoint local_EndPoint = new IPEndPoint(mittente_address.MapToIPv4(), 65000);//porta scelta da noi ,scelta questa perche sennò firewall blocca comunicazione fra due macchine

            nomeSocket.Bind(local_EndPoint); //in pratica hai creato se hai creato il collegamento e avevi due utenti separati-stesso canale grazie al bind (quindi puoi inviare i dati perche il tuo processo è collegato al socket)

            dTimer = new DispatcherTimer();

            dTimer.Tick += new EventHandler(Aggiornamento_dTimer);  //con questo dico cosa deve fare,quindi visto che devo andare a leggere cosa,gli dico di andare all aggiormento(quindi quando scade il timer,andro a fare quello che ce nel metodo)quindi allo scadere del tick
            dTimer.Interval = new TimeSpan( 0, 0 ,0 ,0 ,250);//questo ti dice ogni quanto vai a fare quello che ce scritto nel metodo aggiornamento_Dtimer(sarebbe ogni 250 millisecondi in sto caso perche 0 giorni,0 ore,0 minuti,0 secodni,250 millisecondi
            dTimer.Start();
        
        }

        private void BTN_INVIA_Click(object sender, RoutedEventArgs e)
        {
            IPAddress remote_address = IPAddress.Parse(txtIP.Text); //in questo modo identifico ip destinatario

            IPEndPoint remote_endPoint = new IPEndPoint(remote_address, int.Parse(txtPorta.Text));

            byte[] messaggi = Encoding.UTF8.GetBytes(txtMessaggio.Text);

            nomeSocket.SendTo(messaggi, remote_endPoint);
        }

        private void Aggiornamento_dTimer(object sender,EventArgs e)
        {
            int nBytes = 0;

            if ((nBytes = nomeSocket.Available) > 0) //con questo mi chiedo se ogni 250 millisecondi qualcuno mi ha inviato qualcosa
            {
                //ricezione dei caratteri in attesa
                byte[] buffer = new byte[nBytes];

                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0); //any perche non sappiamo chi è che invia

                nBytes = nomeSocket.ReceiveFrom(buffer, ref remoteEndPoint);

                string from = ((IPEndPoint)remoteEndPoint).Address.ToString();  //serve per recuperare chi è che invia,faccio in sto modo e salvo l'IP

                string messaggio = Encoding.UTF8.GetString(buffer, 0, nBytes); //quello che ho ricevuto dal receiveFrom lo trasformo in un messaggio,dai byte che ho ricevuto dal ricevente sul buffer


                lstBOX.Items.Add(from + ":" + messaggio); //infine visualizzo nella listbox quello ricevuto dal receiveFrom,che avevo appena trasformato
            }
        }
    }
}
