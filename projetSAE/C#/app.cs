using System;
using System.IO;
using System.Collections.Generic;

class App
{
    struct Reservation
    {
        public string nom;
        public string idLiaison;
        public string date;
        public string heure;
        public string horodatage;
        public List<Passager> passager;
        public List<Vehicule> vehicule;

        public Reservation(string n, string id, string d, string he, string ho, List<Passager> p, List<Vehicule> v)
        {
            nom = n;
            idLiaison = id;
            date = d;
            heure = he;
            horodatage = ho;
            passager = p;
            vehicule = v;
        }
    }

    struct Passager
    {
        public string nom;
        public string prenom;
        public string codeCategorie;
        
        public Passager(string n, string pn,string cC)
        {
            nom = n;
            prenom = pn;
            codeCategorie = cC;
        }
    }

    struct Vehicule
    {
        public string codeCategorie;
        public string quantite;

        public Vehicule(string cC, string qte)
        {
            codeCategorie = cC;
            quantite = qte;
        }
    }


    static void Main()
    {
        List<Reservation> listReserv;
        listReserv = new List<Reservation>();

        int c=0,nb,i;
        bool stop = false;
        string r="";

        while(!stop)
        {
            while(c < 1 || c > 3)
            {
                Console.WriteLine("]========================================================[MENU]========================================================[");
                Console.WriteLine("\n\t-Faire une nouvelle réservation".PadRight(110)+"1");
                Console.WriteLine("\n\t-Faire le json des réservations".PadRight(110)+"2");
                Console.WriteLine("\n\t-Quitter".PadRight(110)+"3");
                Console.WriteLine("\n]======================================================================================================================[");
                c = int.Parse(Console.ReadLine());
            }

            switch (c)
            {
                case 1:
                    listReserv.Add(nouvelleReservation());
                    Console.WriteLine("----------------------------------------------------------------------------------------\nVotre réservation a été créé, combien de personne et animaux de compagnie compte venir ? ");
                    nb = int.Parse(Console.ReadLine());
                    for(i=0 ; i < nb ; i++)
                    {
                        listReserv[listReserv.Count-1].passager.Add(nouveauPassager()); //dans le dernier élément de listReserv ajoute un nouveau passager
                    }
                    Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------\nLes personnes et les animaux de compagnie ont été ajouté à votre réservation, combien de type de véhicule comptez vous rammener ? ");
                    nb = int.Parse(Console.ReadLine());
                    for(i=0 ; i < nb ; i++)
                    {
                        listReserv[listReserv.Count-1].vehicule.Add(nouveauVehicule()); //dans le dernier élément de listReserv ajoute un nouveau véhicule
                    }
                    recape(listReserv[listReserv.Count-1]); //fait un récape de la dernière réservation
                    while(r != "o" && r != "n")
                    {
                        Console.WriteLine("Voulez-vous réserver le retour ? (o/n)");
                        r = Console.ReadLine();
                    }
                    if(r == "o")
                    {
                        listReserv.Add(retour(listReserv[listReserv.Count-1]));
                    }
                    break;
                case 2:
                    nouveauJson(listReserv);
                    break;
                case 3:
                    stop = true;
                    break;
            }
            c = 0;
        }
    }

    static Reservation retour(Reservation res)
    {
        string heure="",id = "";
        int date=0;
        switch (res.idLiaison)
        {
            case "1":
                id = "2";
                break;
            case "2":
                id = "1";
                break;
            case "3":
                id = "4";
                break;
            case "4":
                id = "3";
                break;
        }
        while(date < int.Parse(res.date.Substring(8)) || date > 30)
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Donc pour quelle jour voulez vous rentrer");
            date = int.Parse(Console.ReadLine());
        }
        while(!toutesLesDates(int.Parse(id))[date-1].Contains(heure))
        {
            AfficherHeuresSelonJour(int.Parse(id),date);
            Console.WriteLine("Donnez une des heures de départ au format (hh:mm): ");
            heure=Console.ReadLine();
        }

        return(new Reservation(res.nom,id,bonFormat(date),heure,res.horodatage,res.passager,res.vehicule));
    }

    static Reservation nouvelleReservation()
    {
        string[] reserv;
        int date=0;
    
        reserv = new string[5];
        DateTime thisDay = DateTime.Now; 

        Console.WriteLine("Donnez un nom de réservation : ");
        reserv[0]=Console.ReadLine(); 
        while(reserv[1]!="1" && reserv[1]!="2" && reserv[1]!="3" && reserv[1]!="4")
        {
            Console.WriteLine("------------------------------------");
            Console.WriteLine("* Lorien-Groix (id 1)");
            Console.WriteLine("* Groix-Lorient (id 2)");
            Console.WriteLine("* Quiberon-Le Palais (id 3)");
            Console.WriteLine("* Le Pailais-Quiberon (id 4)");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("Donnez l'id de votre liaison : ");
            reserv[1]=Console.ReadLine();
        }
        while(date < 1 || date > 30)
        {
            Console.WriteLine("Donnez un jour de réservation compris entre le 1er et le 30 novembre 2025 : ");
            date=int.Parse(Console.ReadLine());
        }
        reserv[2]=bonFormat(date);
        while(!toutesLesDates(int.Parse(reserv[1]))[date-1].Contains(reserv[3]))
        {
            AfficherHeuresSelonJour(int.Parse(reserv[1]),date);
            Console.WriteLine("Donnez une des heures de départ au format (hh:mm): ");
            reserv[3]=Console.ReadLine();
        }
        reserv[4]=thisDay.ToString("yyyy-MM-dd HH:mm:ss");
        return new Reservation(reserv[0],reserv[1],reserv[2],reserv[3],reserv[4],new List<Passager>(), new List<Vehicule>());
    }
    
    static string bonFormat(int d)
    {
        string vrai_date = "2025-11-";

        if(d < 10)
        {
            vrai_date += "0" + d;
        }
        else
        {
            vrai_date += d;
        }

        return vrai_date;
    }

    static Passager nouveauPassager()
    {
        string[] passager;
        passager = new string[3];
        
        Console.WriteLine("------------------------------------------");
        Console.WriteLine("Donnez votre nom :");
        passager[0] = Console.ReadLine();
        Console.WriteLine("Donnez votre prénom :");
        passager[1] = Console.ReadLine();
        while(passager[2] != "1" && passager[2] != "2" && passager[2] != "3" && passager[2] != "4" && passager[2] != "5")
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("* Adulte (26 ans ou plus)              [1]");
            Console.WriteLine("* Jeune (18 à 25 ans inclus)           [2]");
            Console.WriteLine("* Enfant (4 à 17 ans inclus)           [3]");
            Console.WriteLine("* Bébé (mois de 4 ans)                 [4]");
            Console.WriteLine("* Animal de compagnie                  [5]");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("Donnez le numéro de votre catégorie :");
            passager[2] = Console.ReadLine();
        }
        switch (int.Parse(passager[2]))
        {
            case 1:
                passager[2] = "adu26p";
                break;
            case 2:
                passager[2] = "jeu1825";
                break;
            case 3:
                passager[2] = "enf417";
                break;
            case 4:
                passager[2] = "bebe";
                break;
            case 5:
                passager[2] = "ancomp";
                break;
        }

        return new Passager(passager[0],passager[1],passager[2]);
    }

    static Vehicule nouveauVehicule()
    {
        string[] vehicule;
        vehicule = new string[2];
       
        while(vehicule[0] != "1" && vehicule[0] != "2" && vehicule[0] != "3" && vehicule[0] != "4" && vehicule[0] != "5")
        {
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("* Trotinette électrique                            [1]");
            Console.WriteLine("* Vélo                                             [2]");
            Console.WriteLine("* Deux-roues                                       [3]");
            Console.WriteLine("* Voiture                                          [4]");
            Console.WriteLine("* Camping-car (véhicule de plus de 2,10m de haut)  [5]");
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("Donnez le numéro de votre catégorie :");
            vehicule[0] = Console.ReadLine();
        }
        switch (int.Parse(vehicule[0]))
        {
            case 1:
                vehicule[0] = "trot";
                break;
            case 2:
                vehicule[0] = "0";
                while(vehicule[0] != "1" && vehicule[0] != "2" && vehicule[0] != "3")
                {
                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine("* Vélo ou remorque à vélo                          [1]");
                    Console.WriteLine("* Vélo électrique                                  [2]");
                    Console.WriteLine("* Vélo cargo ou tande                              [3]");
                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine("Donnez le numéro de votre catégorie :");
                    vehicule[0] = Console.ReadLine();
                }
                switch (int.Parse(vehicule[0]))
                {
                    case 1:
                        vehicule[0] = "velo";
                        break;
                    case 2:
                        vehicule[0] = "velelec";
                        break;
                    case 3:
                        vehicule[0] = "cartand";
                        break;
                }
                break;
            case 3:
                vehicule[0] = "0";
                while(vehicule[0] != "1" && vehicule[0] != "2" && vehicule[0] != "3")
                {
                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine("* Deux-roues <= 125 cm3                            [1]");
                    Console.WriteLine("* Deux-roues > 125 cm3                             [2]");
                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine("Donnez le numéro de votre catégorie :");
                    vehicule[0] = Console.ReadLine();
                }
                switch (int.Parse(vehicule[0]))
                {
                    case 1:
                        vehicule[0] = "mobil";
                        break;
                    case 2:
                        vehicule[0] = "moto";
                        break;
                }
                break;
            case 4:
                vehicule[0] = "0";
                while(vehicule[0] != "1" && vehicule[0] != "2" && vehicule[0] != "3" && vehicule[0] != "4")
                {
                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine("* Voiture moins de 4 m                             [1]");
                    Console.WriteLine("* Voiture de 4 m à 4.39 m                          [2]");
                    Console.WriteLine("* Voiture de 4.40 m à 4.79 m                       [3]");
                    Console.WriteLine("* Voiture 4.80 m et plus                           [4]");
                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine("Donnez le numéro de votre catégorie :");
                    vehicule[0] = Console.ReadLine();
                }
                switch (int.Parse(vehicule[0]))
                {
                    case 1:
                        vehicule[0] = "cat1";
                        break;
                    case 2:
                        vehicule[0] = "cat2";
                        break;
                    case 3:
                        vehicule[0] = "cat3";
                        break;
                    case 4:
                        vehicule[0] = "cat4";
                        break;
                }
                break;
            case 5:
                vehicule[0] = "camp";
                break;
        }
        int nb;
        while(int.TryParse(vehicule[1], out nb)==false || nb<1)
        {
            Console.WriteLine("Donne une quantité du type de véhicule donné :");
            vehicule[1] = Console.ReadLine();
        }

        return new Vehicule(vehicule[0],vehicule[1]);
    }

    
    static void recape(Reservation r)
    {
        float prix=0f;
        Console.WriteLine("------------------------------------------------------");
        Console.WriteLine("La reservation au nom de "+r.nom+" a été pris en compte le "+r.horodatage+" pour le "+ r.date+" à "+r.heure);

        foreach(Passager p in r.passager)
        {
            switch (p.codeCategorie)
            {
                case "adu26p" :
                    if(r.idLiaison == "1" || r.idLiaison == "2")
                    {
                        prix += 18.75f;
                    }
                    else
                    {
                        prix += 13.8f;
                    }
                    break;
                case "jeu1825" :
                    if(r.idLiaison == "1" || r.idLiaison == "2")
                    {
                        prix += 13.8f;
                    }
                    else
                    {
                        prix += 14.1f;
                    }
                    break;
                case "enf417" :
                    if(r.idLiaison == "1" || r.idLiaison == "2")
                    {
                        prix += 11.25f;
                    }
                    else
                    {
                        prix += 11.65f;
                    }
                    break;
                case "ancomp" :
                    prix += 3.35f;
                    break;
            }
        }
        foreach(Vehicule v in r.vehicule)
        {
            switch (v.codeCategorie)
            {
                case "trot" :
                    prix += float.Parse(v.quantite)*4.7f;
                    break;
                case "velo" :
                    prix += float.Parse(v.quantite)*8.2f;
                    break;
                case "velelec" :
                    prix += float.Parse(v.quantite)*11f;
                    break;
                case "cartand" :
                    prix += float.Parse(v.quantite)*16.45f;
                    break;
                case "mobil" :
                    if(r.idLiaison == "1" || r.idLiaison == "2")
                    {
                        prix += float.Parse(v.quantite)*23.10f;
                    }
                    else
                    {
                        prix += float.Parse(v.quantite)*23.35f;
                    }
                    break;
                case "moto" :
                    if(r.idLiaison == "1" || r.idLiaison == "2")
                    {
                        prix += float.Parse(v.quantite)*66.05f;
                    }
                    else
                    {
                        prix += float.Parse(v.quantite)*66.4f;
                    }
                    break;
                case "cat1" :
                    if(r.idLiaison == "1" || r.idLiaison == "2")
                    {
                        prix += float.Parse(v.quantite)*96.05f;
                    }
                    else
                    {
                        prix += float.Parse(v.quantite)*98.5f;
                    }
                    break;
                case "cat2" :
                    if(r.idLiaison == "1" || r.idLiaison == "2")
                    {
                        prix += float.Parse(v.quantite)*114.8f;
                    }
                    else
                    {
                        prix += float.Parse(v.quantite)*117.2f;
                    }
                    break;
                case "cat3" :
                    if(r.idLiaison == "1" || r.idLiaison == "2")
                    {
                        prix += float.Parse(v.quantite)*174.45f;
                    }
                    else
                    {
                        prix += float.Parse(v.quantite)*176.9f;
                    }
                    break;
                case "cat4" :
                    if(r.idLiaison == "1" || r.idLiaison == "2")
                    {
                        prix += float.Parse(v.quantite)*210.9f;
                    }
                    else
                    {
                        prix += float.Parse(v.quantite)*213.35f;
                    }
                    break;
                case "camp" :
                    if(r.idLiaison == "1" || r.idLiaison == "   ")
                    {
                        prix += float.Parse(v.quantite)*330.2f;
                    }
                    else
                    {
                        prix += float.Parse(v.quantite)*332.7f;
                    }
                    break;
            }
        }

        Console.WriteLine("Donc le prix total est de "+prix+"€");
    }

    static void nouveauJson(List<Reservation> r) 
    {
        FileStream fs = new FileStream("Reservation.json", FileMode.Create, FileAccess.Write);
        StreamWriter Reservation = new StreamWriter(fs);
        Reservation.Write("[");
        foreach(Reservation res in r)
        {
            // partie reservation
            Reservation.WriteLine("{\n\t\"reservation\":"); 
            Reservation.WriteLine("\t\t{\n\t\t\t\"nom\": \""+res.nom+"\",");
            Reservation.WriteLine("\t\t\t\"idLiaison\": "+res.idLiaison+",");
            Reservation.WriteLine("\t\t\t\"date\": \""+res.date+"\",");
            Reservation.WriteLine("\t\t\t\"heure\": \""+res.heure+"\",");
            Reservation.WriteLine("\t\t\t\"horodatage\": \""+res.horodatage+"\"\n\t\t},");
            //partie passager
            Reservation.WriteLine("\t\"passagers\": [");
            foreach(Passager p in res.passager)
            {
                Reservation.WriteLine("\t\t{\n\t\t\t\"nom\": \""+p.nom+"\",");
                Reservation.WriteLine("\t\t\t\"prenom\": \""+p.prenom+"\",");
                Reservation.WriteLine("\t\t\t\"codeCategorie\": \""+p.codeCategorie+"\"");
                if(res.passager.IndexOf(p) == res.passager.Count - 1)
                {
                    Reservation.WriteLine("\t\t}");
                }
                else
                {
                    Reservation.WriteLine("\t\t},"); 
                }
            }
            Reservation.WriteLine("\t],");
            Reservation.WriteLine("\t\"vehicules\": [");
            foreach(Vehicule v in res.vehicule)
            {
                Reservation.WriteLine("\t\t{\n\t\t\t\"codeCategorie\": \""+v.codeCategorie+"\",");
                Reservation.WriteLine("\t\t\t\"quantite\": \""+v.quantite+"\"");
                if(res.vehicule.IndexOf(v) == res.vehicule.Count - 1)
                {
                    Reservation.WriteLine("\t\t}");
                }
                else
                {
                    Reservation.WriteLine("\t\t},"); 
                }
            }
            Reservation.WriteLine("\t]");
            if(r.IndexOf(res) == r.Count -1)
            {
                Reservation.WriteLine("}]");
            }
            else
            {
                Reservation.WriteLine("},");
            }
        }
        Reservation.Close();
    }

    static List<string>[] toutesLesDates(int id)
    {
        List<string>[] mois = new List<string>[30]; 
        for(int i = 0; i < 30; i++)
        {
            switch(id)
            {
                case 1:
                    if(i == 0 || i == 10 || i%7 == 1 ){
                        mois[i] = new List<string> {"09:45","12:15","17:00","19:30"};
                    }else{
                        mois[i] = new List<string> {"08:05","11:00","13:45","16:15","18:45"};
                    }
                    break;
                case 2:
                    if(i == 0 || i == 10 || i%7 == 1 ){
                        mois[i] = new List<string> {"08:30","11:45","15:45","18:15"};
                    }else{
                        mois[i] = new List<string> {"06:50","09:30","12:30","15:00","17:30"};
                    }
                    break;
            }
        }
        switch(id)
        {
            case 3: // QUIBERON -> LE PALAIS

                mois[0] = new List<string> { "08:15", "09:30", "11:15", "12:45", "14:45", "17:15", "19:00", "20:00" };
                mois[1] = new List<string> { "08:00", "09:30", "11:15", "12:30", "14:15", "15:45", "17:15", "19:00", "20:00" };
                mois[2] = new List<string> { "08:00", "09:30", "11:15", "14:30", "17:30", "19:30" };
                mois[3] = new List<string> { "07:30", "09:30", "10:30", "13:30", "18:15", "19:30" };
                mois[4] = new List<string> { "08:00", "10:30", "12:00", "17:15", "19:00", "20:00" };
                mois[5] = new List<string> { "07:30", "08:30", "11:45", "14:45", "18:00", "19:45" };
                mois[6] = new List<string> { "08:00", "09:30", "12:30", "15:30", "18:15", "20:00" };
                mois[7] = new List<string> { "08:15", "09:45", "11:00", "14:15", "17:15", "19:30" };
                mois[8] = new List<string> { "08:30", "11:15", "14:15", "16:15", "17:30", "19:30" };
                mois[9] = new List<string> { "08:00", "09:30", "11:15", "14:15", "17:30", "19:30" };
                mois[10] = new List<string> { "08:15", "11:15", "14:15", "16:15", "17:30", "19:30" };
                mois[11] = new List<string> { "08:00", "09:30", "11:15", "14:15", "16:15", "17:15", "20:00" };
                mois[12] = new List<string> { "08:00", "09:30", "11:15", "14:15", "16:15", "17:15", "20:00" };
                mois[13] = new List<string> { "08:00", "09:30", "11:15", "14:15", "17:15", "19:15", "20:15" };
                mois[14] = new List<string> { "08:00", "09:30", "11:15", "14:15", "17:30", "19:30" };
                mois[15] = new List<string> { "08:15", "11:15", "14:15", "16:15", "17:30", "19:30" };
                mois[16] = new List<string> { "08:00", "09:30", "11:15", "14:15", "16:15", "17:15", "20:00" };
                mois[17] = new List<string> { "08:00", "09:30", "11:15", "14:15", "16:15", "17:15", "20:00" };
                mois[18] = new List<string> { "08:00", "09:30", "11:15", "14:15", "16:15", "17:15", "20:00" };
                mois[19] = new List<string> { "08:00", "09:30", "11:15", "14:15", "16:15", "17:15", "20:00" };
                mois[20] = new List<string> { "08:00", "09:30", "11:15", "14:15", "17:15", "19:15", "20:15" };
                mois[21] = new List<string> { "08:00", "09:30", "11:15", "14:15", "17:30", "19:30" };
                mois[22] = new List<string> { "08:15", "11:15", "14:15", "16:15", "17:30", "19:30" };
                mois[23] = new List<string> { "08:00", "09:30", "11:15", "14:15", "16:15", "17:15", "20:00" };
                mois[24] = new List<string> { "08:00", "09:30", "11:15", "14:15", "16:15", "17:15", "20:00" };
                mois[25] = new List<string> { "08:00", "09:30", "11:15", "14:15", "16:15", "17:15", "20:00" };
                mois[26] = new List<string> { "08:00", "09:30", "11:15", "14:15", "16:15", "17:15", "20:00" };
                mois[27] = new List<string> { "08:00", "09:30", "11:15", "14:15", "17:15", "19:15", "20:15" };
                mois[28] = new List<string> { "08:00", "09:30", "11:15", "14:15", "17:30", "19:30" };
                mois[29] = new List<string> { "08:15", "11:15", "14:15", "16:15", "17:30", "19:30" };
                break;
            case 4: // LE PALAIS -> QUIBERON
                mois[0] = new List<string> { "06:45", "07:45", "09:45", "11:00", "12:45", "14:45", "17:30", "18:45" };
                mois[1] = new List<string> { "06:30", "07:45", "09:45", "11:00", "12:45", "14:15", "15:45", "17:15", "18:40" };
                mois[2] = new List<string> { "06:30", "07:45", "09:45", "12:45", "16:00", "18:00" };
                mois[3] = new List<string> { "06:00", "07:45", "09:00", "12:00", "16:45", "18:00" };
                mois[4] = new List<string> { "06:30", "07:45", "10:30", "13:30", "17:30", "18:30" };
                mois[5] = new List<string> { "06:00", "07:00", "09:00", "13:15", "16:15", "18:30" };
                mois[6] = new List<string> { "06:30", "07:45", "09:45", "14:00", "16:45", "18:45" };
                mois[7] = new List<string> { "07:00", "08:00", "09:45", "12:45", "15:45", "17:45" };
                mois[8] = new List<string> { "07:15", "09:45", "12:45", "14:45", "15:45", "18:00" };
                mois[9] = new List<string> { "06:30", "07:45", "09:45", "12:45", "15:45", "18:00" };
                mois[10] = new List<string> { "07:00", "09:45", "12:45", "14:45", "15:45", "18:00" };
                mois[11] = new List<string> { "06:30", "07:45", "09:45", "12:45", "14:45", "15:45", "18:30" };
                mois[12] = new List<string> { "06:30", "07:45", "09:45", "12:45", "14:45", "15:45", "18:30" };
                mois[13] = new List<string> { "06:30", "07:45", "09:45", "12:45", "15:45", "17:45", "18:45" };
                mois[14] = new List<string> { "06:30", "07:45", "09:45", "12:45", "15:45", "18:00" };
                mois[15] = new List<string> { "07:00", "09:45", "12:45", "14:45", "15:45", "18:00" };
                mois[16] = new List<string> { "06:30", "07:45", "09:45", "12:45", "14:45", "15:45", "18:30" };
                mois[17] = new List<string> { "06:30", "07:45", "09:45", "12:45", "14:45", "15:45", "18:30" };
                mois[18] = new List<string> { "06:30", "07:45", "09:45", "12:45", "14:45", "15:45", "18:30" };
                mois[19] = new List<string> { "06:30", "07:45", "09:45", "12:45", "14:45", "15:45", "18:30" };
                mois[20] = new List<string> { "06:30", "07:45", "09:45", "12:45", "15:45", "17:45", "18:45" };
                mois[21] = new List<string> { "06:30", "07:45", "09:45", "12:45", "15:45", "18:00" };
                mois[22] = new List<string> { "07:00", "09:45", "12:45", "14:45", "15:45", "18:00" };
                mois[23] = new List<string> { "06:30", "07:45", "09:45", "12:45", "14:45", "15:45", "18:30" };
                mois[24] = new List<string> { "06:30", "07:45", "09:45", "12:45", "14:45", "15:45", "18:30" };
                mois[25] = new List<string> { "06:30", "07:45", "09:45", "12:45", "14:45", "15:45", "18:30" };
                mois[26] = new List<string> { "06:30", "07:45", "09:45", "12:45", "14:45", "15:45", "18:30" };
                mois[27] = new List<string> { "06:30", "07:45", "09:45", "12:45", "15:45", "17:45", "18:45" };
                mois[28] = new List<string> { "06:30", "07:45", "09:45", "12:45", "15:45", "18:00" };
                mois[29] = new List<string> { "07:00", "09:45", "12:45", "14:45", "15:45", "18:00" };
                break;
        }
        return mois; 
    }


    static void AfficherHeuresSelonJour(int id, int jour)
    {
        List<string>[] mois = toutesLesDates(id);
        Console.WriteLine("}-----------------------------------------------------{");
        Console.WriteLine("Heures disponibles pour le " + jour + " Novembre :");

        // On accède directement à l'indice (jour - 1)
        foreach (string heure in mois[jour - 1])
        {
            Console.WriteLine("\t"+heure);
        }
        Console.WriteLine("}-----------------------------------------------------{");
    }
}