using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Threading;

namespace BBEEGInteger.Log
{
    public sealed class MyLogListener : TraceListener
    {
        #region objets, variable privé
        //surveillance rapprocher du fichier de log
        private FileSystemWatcher watcher = null;
        //chemin complet du fichier de log
        private string _logPath;
        // object pour le lock concernant l'ecriture dans le fichier
        private Object fileLock = new Object();
        // flag si messagebox deja affiché encas d'erreur (histoire de pas en avoir 15000
        private bool alreadyMsgBox = false;
        //Pile FIFO qui contient les message de log a écrire dans un fichier
        private static Stack StackDeLogAEcrire;
        //Singleton
        public static MyLogListener instance = null;
        static readonly object padlock = new object();
        #endregion

        #region Singleton
        public static MyLogListener Instance(string logPath)
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new MyLogListener(logPath);
                }
                return instance;
            }
        }
        #endregion

        #region Propriétés
        /// <summary>
        /// Chemin vers le fichier de log
        /// </summary>
        public string LogPath
        {
            set
            {
                _logPath = value;
                string fileName = Path.GetFileName(_logPath);
                if (Path.GetExtension(_logPath) == string.Empty)
                    throw new Exception("Bad log filename, extension require!");
                string directoryLogPath = Path.GetDirectoryName(_logPath);
                // on vérifie que le répertoire existe
                if (directoryLogPath != string.Empty)
                    if (!Directory.Exists(directoryLogPath))
                        Directory.CreateDirectory(directoryLogPath);
            }
            get { return _logPath; }
        }

        private int _maxLogInWait;
        /// <summary>
        /// Limitateur de la pile de message en attente (element de securité), à laisser à 50
        /// car plus la taille de message en attente augmente plus le logger prend des ressources systems et nuie à l'ensemble
        /// Mettre 0 pour desactiver cette protection (pour debuggage par exemple).
        /// </summary>
        public int MaxLogInWait
        {
            set { _maxLogInWait = value; }
            get { return _maxLogInWait; }
        }

        private long _maxLogSize;
        /// <summary>
        /// Taille max du fichier de log (en oct). Mettre 0 desactive la limitation
        /// </summary>
        public long MaxLogSize
        {
            set { _maxLogSize = value; }
            get { return _maxLogSize; }
        }

        private bool _showFatalErrorInMessageBox;
        /// <summary>
        /// Active l'affichage d'erreur fatal dans une boite de message
        /// il est conseillé de désactiver cette propriete pour des processus serveurs
        /// </summary>
        public bool ShowFatalErrorInMessageBox
        {
            set { _showFatalErrorInMessageBox = value; }
            get { return _showFatalErrorInMessageBox; }
        }


        private bool _WriteDateInfo;
        /// <summary>
        /// Indique si la date et l'heure est ecrite lors de chaque ecriture de log
        /// </summary>
        public bool WriteDateInfo
        {
            set { _WriteDateInfo = value; }
            get { return _WriteDateInfo; }
        }

        private bool _indicateDate;
        /// <summary>
        /// Indique si la date et l'heure est ecrite lors de chaque ecriture de log
        /// </summary>
        public bool IndicateDate
        {
            set { _indicateDate = value; }
            get { return _indicateDate; }
        }


        private bool _IsErrorDetected = false;
        /// <summary>
        /// Indique si une exception a été recu
        /// </summary>
        public bool IsErrorDetected
        {
            private set { _IsErrorDetected = value; }
            get { return _IsErrorDetected; }
        }

        private Exception _LastException = null;
        /// <summary>
        /// Pointe sur la derniere exception recu
        /// </summary>
        public Exception LastException
        {
            private set { _LastException = value; }
            get { return _LastException; }
        }

        private string _LastErrMsg = null;
        /// <summary>
        /// Indique le dernier message d'erreur recu
        /// </summary>
        public string LastErrorMsg
        {
            private set { _LastErrMsg = value; }
            get { return _LastErrMsg; }
        }
        #endregion

        #region Methodes publiques

        /// <summary>
        /// Constructeur
        /// </summary>
        /// /// <param name="logPath">
        /// [in] Spécifie le chemin du fichier de log
        /// </param>
        public MyLogListener(string logPath)
        {
            if ((logPath == null) || (logPath == string.Empty))
                throw new Exception("The path of the log file is required.");
            LogPath = logPath;
            MaxLogSize = 10000000; //1 Mo;
            IndicateDate = true;
            WriteDateInfo = true;
            StackDeLogAEcrire = new Stack();
            ShowFatalErrorInMessageBox = true;
            MaxLogInWait = 50;

            //lance le detecteur de changement sur le fichier de logs - cf.WriteInFicThreadStart()
            watcher = new FileSystemWatcher();
            watcher.Path = System.IO.Path.GetDirectoryName(this.LogPath);
            // surveille que notre fichier.
            watcher.Filter = System.IO.Path.GetFileName(this.LogPath); // "*.txt";
            // surveille les changements dernier acces, dernieère ecriture, renommage et repertoire
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName; //| NotifyFilters.DirectoryName;
            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
            watcher.Created += new FileSystemEventHandler(watcher_Changed);
            watcher.Deleted += new FileSystemEventHandler(watcher_Changed);
            watcher.Renamed += new RenamedEventHandler(watcher_Changed);
        }

        //Method below couldn't be used in Compact framework!
        //public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        //{

        //    //si de type information et ecriture log info non demandé : on sort
        //    if (eventType == TraceEventType.Information)
        //        if (this.WriteDateInfo == false)
        //            return;

        //    if (message == string.Empty)
        //        message = "";

        //    if (eventType == TraceEventType.Error)
        //    {
        //        //envoi l'evenement
        //        RaiseExceptionDetectedEvent(message, null);
        //    }

        //    message = " Type : " + eventType.ToString() + " - message : " + message + "\r\n";
        //    WriteLine(message);
        //}

        //public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message, params object[] args)
        //{
        //    //si de type information et ecriture log info non demandé : on sort
        //    if (eventType == TraceEventType.Information)
        //        if (this.WriteDateInfo == false)
        //            return;

        //    if (args.Length > 0)
        //    {
        //        if (args[0] is Exception)
        //        {
        //            Exception ex = (Exception)args[0];

        //            if (message == string.Empty)
        //                message = "";

        //            if (eventType == TraceEventType.Error)
        //            {
        //                //envoi de l'evenement
        //                RaiseExceptionDetectedEvent(message, ex);
        //            }

        //            //formatage du message
        //            string messageErr = " Type : " + eventType.ToString() + " - message : " + message + "\r\n";
        //            messageErr += String.Format("EXCEPTION type : {0} \r\n   Message d'erreur: {1} \r\n   Origine : {2} \r\n", ex.GetType().ToString(), ex.Message, ex.StackTrace);
        //            WriteLine(messageErr);
        //        }
        //    }
        //}

        /// <summary>
        /// Evénement indiquant la reception d'une erreur provenant de Trace.TraceError
        /// </summary>
        public event EventHandler ErrorDetectedEvent;
        // Methode pour envoyer un evenement SearchContactEvent
        protected void RaiseExceptionDetectedEvent(string messageCourt, Exception ex)
        {
            this.LastException = ex;
            this.LastErrorMsg = messageCourt;
            this.IsErrorDetected = true;

            EventHandler eventHandler = ErrorDetectedEvent;
            if (eventHandler != null)
                eventHandler(this, new EventArgs());
        }

        public override void WriteLine(string message, string cate)
        {
            WriteInFic(message + "\r\n");
        }

        public override void WriteLine(string message)
        {
            Write(message + "\r\n");
        }
        public override void Write(string message)
        {
            if (this.IndicateDate == true)
                message = DateTime.Now.ToString()
                    + ":" + DateTime.Now.Millisecond.ToString()
                    + " -> " + message;
            WriteInFic(message);
        }

        public override void WriteLine(object o)
        {
            base.WriteLine(o); //renvoi au pere, object non traité
        }

        #endregion

        #region Methodes privées
        private void WriteInFic(string message)
        {
            bool runThread = false;
            lock (StackDeLogAEcrire)
            {
                //si la pile de message est vide on lancera le thread d'ecriture
                if (StackDeLogAEcrire.Count == 0)
                    runThread = true;
                StackDeLogAEcrire.Push(message);
            }
            if (runThread)
            {
                //lancement du thread
                Thread WriteThread = new Thread(WriteInFicThreadStart);
                WriteThread.Start();
            }
        }

        private void WriteInFicThreadStart()
        {
            //Fonction coeur
            //Cette fonction est lancé dans un thread car elle permet l'ecriture inversé des log dans un fichier
            //l'opération consiste à 
            // 1- Prise de controle du fichier de log
            // 2- recopie de son contenu dans un fichier temporaire
            // 3- ecriture du message
            // 4- ajoute les ancien log present dans le fichier temporaire

            try
            {
                //verrouillage en cas de multi-thread/multi acces interne appli (à priori inutile mais bon y a le mauvais developpeur et le BON developpeur, le mauvais fait plein de bug et le bon ... aussi :))
                lock (fileLock)
                {
                    long tailleFic = 0; // compteur de taille du fichier pour appliquer le limitateur de taille des logs
                    //si un fichier de log existe, on faudra rajouter son contenu apres notre message de log
                    string oldFilePath = this.LogPath + ".old";
                    bool bRecopieNedd = false;
                    if (File.Exists(this.LogPath))
                        bRecopieNedd = true;

                    //1- tentative de prise de controle du fichier de log
                    //Permet de prend en compte le cas de plusieurs appli/process accèdant au même fichier de log
                    FileStream fsw = null;

                    try
                    {
                        fsw = new FileStream(this.LogPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                    }
                    catch
                    {
                        //if (watcher == null)
                        //{
                        //}
                        // commence la surveillance.
                        if (watcher.EnableRaisingEvents == false)
                            watcher.EnableRaisingEvents = true;
                        return;
                    }

                    //2- recopie de son contenu dans un fichier temporaire
                    if (File.Exists(oldFilePath))
                        File.Delete(oldFilePath);
                    if (bRecopieNedd)
                        File.Copy(this.LogPath, oldFilePath);

                    //3- ecriture du message
                    //ouverture du flux en ecriture
                    System.IO.StreamWriter sw = new StreamWriter(fsw);
                    //ecriture du message
                    lock (StackDeLogAEcrire)
                    {
                        while (StackDeLogAEcrire.Count > 0)
                        {
                            string message = (string)StackDeLogAEcrire.Pop();
                            sw.Write(message);
                            tailleFic += message.Length; //1car = 1oct
                        }
                    }

                    //4- ajoute les ancien log present dans le fichier temporaire
                    if (bRecopieNedd)
                    {
                        using (StreamReader sr = new StreamReader(oldFilePath))
                        {
                            String line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                tailleFic += line.Length; //1car = 1oct
                                //si taille max defini des log est atteint on arrete la recopie
                                if (tailleFic < this.MaxLogSize || this.MaxLogSize == 0)
                                    sw.WriteLine(line);
                                else
                                    break;
                            }
                        }
                        //suppression du fichier temporaire
                        File.Delete(oldFilePath);
                    }

                    //relachement du flux et fichier
                    sw.Close();
                    fsw.Close();


                    //si entre temps il y a des nouveaux element à ecrire on relance le thread
                    lock (StackDeLogAEcrire)
                    {
                        if (StackDeLogAEcrire.Count > 0)
                        {
                            //Protection : Limitation de la pile et capacité de traitement du logger
                            //si autant de message en attente = ralentissement machine, process ... pas forcement judicieux
                            if (StackDeLogAEcrire.Count > MaxLogInWait && MaxLogInWait != 0)
                            {
                                StackDeLogAEcrire.Clear();
                                StackDeLogAEcrire.Push("==== DEPASSEMENT DU NOMBRE DE MESSAGE QUE LE GESTIONNAIRE DE LOG PEUT TRAITER !!! ===");
                                StackDeLogAEcrire.Push("==== CERTAINS LOGS N'ONT PAS ETE ECRIT. ===");
                            }
                            Console.WriteLine("Le thread d'ecriture a ete relance");
                            Thread WriteThread = new Thread(WriteInFicThreadStart);
                            WriteThread.Start();
                        }
                    }
                }

            }
            catch
            {
                //si une erreur -- affichage de l'erreur dans une box (si propriété autorisée)
                if (this.ShowFatalErrorInMessageBox)
                {
                    if (!this.alreadyMsgBox)
                    {
                        //System.Windows.Forms.MessageBox.Show("Erreur Ecriture de log impossible!", System.Reflection.Assembly.GetEntryAssembly().FullName, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        alreadyMsgBox = true;
                    }
                }
            }
        }


        private void watcher_Changed(object source, FileSystemEventArgs e)
        {
            // l'etat du fichier de log a changer, on relance l'ecriture
            // arrete la surveillance.
            if (watcher.EnableRaisingEvents == true)
                watcher.EnableRaisingEvents = false;
            WriteInFicThreadStart();
        }

        #endregion

    }
}
