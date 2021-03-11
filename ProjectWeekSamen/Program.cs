using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectWeekSamen
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = "gegevens.txt";
            using (StreamWriter writer = new StreamWriter(file, true))
            {
            }

            while (true)
            {
                switch (SelectMenu("Gebruiker toevoegen", "Gebruiker bewerken", "Gebruiker verwijderen", "Inloggen"))
                {
                    case 1:
                        GebruikerToevoegen();
                        break;
                    case 2:
                        GebruikerBewerken();
                        break;
                    case 3:
                        GebruikerVerwijderen();
                        break;
                    case 4:
                        SpelMenu(Inloggen());
                        break;
                    default:
                        DefaultErrorMessage();
                        break;
                }
            }
        }
        static int SelectMenu(params string[] menu)
        {
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;
            Console.Clear();

            int selection = 1;
            bool selected = false;
            ConsoleColor selectionForeground = Console.BackgroundColor;
            ConsoleColor selectionBackground = Console.ForegroundColor;

            while (!selected)
            {
                for (int i = 0; i < menu.Length; i++)
                {
                    if (selection == i + 1)
                    {
                        Console.ForegroundColor = selectionForeground;
                        Console.BackgroundColor = selectionBackground;
                    }
                    Console.WriteLine((i + 1) + ": " + menu[i]);
                    Console.ResetColor();
                }

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        selection--;
                        break;
                    case ConsoleKey.DownArrow:
                        selection++;
                        break;
                    case ConsoleKey.Enter:
                        selected = true;
                        break;
                }

                selection = Math.Min(Math.Max(selection, 1), menu.Length);
                Console.SetCursorPosition(0, 0);
            }

            Console.Clear();
            Console.CursorVisible = true;

            return selection;
        }
        static void SpelMenu(string username)
        {
            DateTime loginSinds = DateTime.Now;
            bool loggedIn = true;
            int saldo = 200;

            while (loggedIn)
            {
                Console.Clear();
                Console.WriteLine($"Beste {username}. Het is {DateTime.Now.ToString("dd/MM HH:mm:ss")}.");
                int aantalMinuten = (DateTime.Now - loginSinds).Minutes;

                if (aantalMinuten == 1)
                    Console.WriteLine($"Je bent al {aantalMinuten} minuut online.");
                else
                    Console.WriteLine($"Je bent al {aantalMinuten} minuten online.");

                Console.WriteLine($"Jouw saldo is ${saldo}.");

                Console.ReadLine();

                switch (SelectMenu("BlackJack", "Slot Machine", "Memory", "Uitloggen"))
                {
                    case 1:
                        if (saldo >= 10)
                        {
                            saldo -= 10;
                            saldo += BlackJack();
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Niet genoeg geld. BlackJack kost $10.");
                            Console.ReadLine();
                        }
                        break;
                    case 2:
                        do
                        {
                            if (saldo >= 5)
                            {
                                saldo -= 5;
                                saldo += SlotMachine();
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Niet genoeg geld. Slot Machine kost $5.");
                                Console.ReadLine();
                            }
                        }
                        while (SelectMenu("Nog eens spelen", "Verlaten") == 1);
                        break;
                    case 3:
                        if (saldo >= 20)
                        {
                            saldo -= 20;
                            saldo += Memory();
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Niet genoeg geld. Memory kost $20.");
                            Console.ReadLine();
                        }
                        break;
                    case 4:
                        loggedIn = false;
                        break;

                    default:
                        DefaultErrorMessage();
                        break;
                }
            }
        }
        static int BlackJack()
        {
            Console.Clear();
            int[] deck = new int[52];

            for (int i = 0; i < deck.Length; i++)
            {
                deck[i] = i;
            }

            deck = Shuffle(deck);

            int winst = 0;
            int wijzer = 0;
            int mijnHandWijzer = 0;
            int aces = 0;
            int totaal = 0;

            int[] mijnHand = new int[21];

            for (int i = 0; i < mijnHand.Length; i++)
            {
                mijnHand[i] = -1;
            }

            Console.Write("Je krijgt een ");
            mijnHand[mijnHandWijzer++] = VisualizeCard(deck[wijzer++]);
            Console.Write(" en een ");
            mijnHand[mijnHandWijzer++] = VisualizeCard(deck[wijzer++]);
            Console.WriteLine(".");

            bool stop = false;
            int handTeller = 0;
            while (!stop)
            {
                handTeller = 0;
                aces = 0;
                totaal = 0;
                Console.Write("Jouw hand: ");
                while (mijnHand[handTeller] != -1)
                {
                    if (ValueCard(mijnHand[handTeller]) == 11)
                        aces++;
                    totaal += ValueCard(mijnHand[handTeller]);
                    VisualizeCard(mijnHand[handTeller++]);
                    Console.Write(' ');
                }

                for (int i = aces; i > 0; i--)
                {
                    if (totaal > 21)
                        totaal -= 10;
                }

                Console.WriteLine($"voor een totaal van {totaal}.");
                Console.ReadLine();

                if (totaal == 21 && handTeller == 2)
                {
                    Console.WriteLine("Proficiat! Je wint $25.");
                    Console.ReadLine();
                    winst = 25;
                    stop = true;
                }
                else if (totaal == 21)
                {
                    stop = true;
                }
                else if (totaal > 21)
                {
                    Console.WriteLine("Jammer, je bent verloren!");
                    Console.ReadLine();
                    stop = true;
                }
                else if (SelectMenu("Trek nog een kaart bij", "Wait") == 1)
                {
                    Console.Clear();
                    Console.Write("Je trekt een ");
                    mijnHand[mijnHandWijzer++] = VisualizeCard(deck[wijzer++]);
                    Console.WriteLine(" bij.");
                }
                else
                {
                    stop = true;
                }
            }

            if (totaal <= 21 && !(handTeller == 2 && totaal == 21))
            {
                int dealerTotaal = 0;
                int dealerAces = 0;
                int dealerWijzer = 0;
                int[] dealerHand = new int[21];

                for (int i = 0; i < dealerHand.Length; i++)
                {
                    dealerHand[i] = -1;
                }

                Console.Write("De dealer trekt een ");
                dealerHand[dealerWijzer++] = VisualizeCard(deck[wijzer++]);
                Console.Write(" en een ");
                dealerHand[dealerWijzer++] = VisualizeCard(deck[wijzer++]);
                Console.WriteLine(".");

                while (dealerTotaal < 17)
                {
                    dealerTotaal = 0;
                    dealerAces = 0;
                    int dealerHandTeller = 0;
                    Console.Write("De dealer zijn hand: ");

                    while (dealerHand[dealerHandTeller] != -1)
                    {
                        if (ValueCard(dealerHand[dealerHandTeller]) == 11)
                            dealerAces++;
                        dealerTotaal += ValueCard(dealerHand[dealerHandTeller]);
                        VisualizeCard(dealerHand[dealerHandTeller++]);
                        Console.Write(" ");
                    }

                    for (int i = dealerAces; i > 0; i--)
                    {
                        if (dealerTotaal > 21)
                            dealerTotaal -= 10;
                    }

                    Console.WriteLine($"voor een totaal van {dealerTotaal}.");
                    Console.ReadLine();

                    if (dealerTotaal < 17)
                    {
                        Console.Clear();
                        Console.Write("De dealer trekt een ");
                        dealerHand[dealerWijzer++] = VisualizeCard(deck[wijzer++]);
                        Console.Write(" bij.");
                        Console.WriteLine();
                    }
                }

                Console.WriteLine($"Jij hebt {totaal} en de dealer heeft {dealerTotaal}.");
                if (totaal == dealerTotaal)
                {
                    winst = 10;
                    Console.WriteLine("Gelijk! Je krijgt je geld terug.");
                    Console.ReadLine();
                }
                else if (totaal > dealerTotaal || dealerTotaal > 21)
                {
                    winst = 20;
                    Console.WriteLine("Proficiat! Gewonnen! Je verdient $20.");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Jammer. Verloren.");
                    Console.ReadLine();
                }
            }

            return winst;
        }
        static int VisualizeCard(int getal)
        {
            string kaart = "";

            switch (getal / 13)
            {
                case 0:
                    Console.ForegroundColor = ConsoleColor.Red;
                    kaart += "♥";
                    break;
                case 1:
                    Console.ForegroundColor = ConsoleColor.Red;
                    kaart += "♦";
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.White;
                    kaart += "♣";
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.White;
                    kaart += "♠";
                    break;
                default:
                    DefaultErrorMessage();
                    break;
            }

            switch (getal % 13)
            {
                case 1:
                    kaart += "A";
                    break;
                case 11:
                    kaart += "J";
                    break;
                case 12:
                    kaart += "Q";
                    break;
                case 0: //13e kaart
                    kaart += "K";
                    break;

                default:
                    kaart += getal % 13;
                    break;
            }

            Console.Write(kaart);
            Console.ResetColor();

            return getal;
        }
        static int ValueCard(int getal)
        {
            switch (getal % 13)
            {
                case 1:
                    return 11;
                case 11:
                case 12:
                case 0:
                    return 10;
                default:
                    return getal % 13;
            }
        }
        static char ColorSign(char sign)
        {
            switch (sign)
            {
                case '☻':
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case '♠':
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case '♣':
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case '♦':
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case '♥':
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case 'A':
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case '7':
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
            return sign;
        }
        static int ValueSign(char sign)
        {
            int winst = 0;
            switch (sign)
            {
                case '☻':
                    winst = 3;
                    break;
                case '♠':
                    winst = 5;
                    break;
                case '♣':
                    winst = 7;
                    break;
                case '♦':
                    winst = 10;
                    break;
                case '♥':
                    winst = 20;
                    break;
                case 'A':
                    winst = 50;
                    break;
                case '7':
                    winst = 100;
                    break;
            }
            return winst;
        }
        static int SlotMachine()
        {
            Console.Clear();
            Console.CursorVisible = false;
            string row1 = "☻☻☻☻☻♠♠♠♠♣♣♣♣♦♦♦♥♥♥AA7";
            string row2 = "☻☻☻☻☻♠♠♠♠♣♣♣♣♦♦♦♥♥♥AA7";
            string row3 = "☻☻☻☻☻♠♠♠♠♣♣♣♣♦♦♦♥♥♥AA7";

            Random rng = new Random();
            row1 = Shuffle(row1, rng);
            row2 = Shuffle(row2, rng);
            row3 = Shuffle(row3, rng);

            int teller = 0;
            int duration = 50;

            while (teller < duration)
            {
                Console.SetCursorPosition(0, 0);

                if (teller < duration / 3)
                {
                    row1 += row1[0];
                    row1 = row1.Substring(1, row1.Length - 1);
                }

                if (teller < duration / 3 * 2)
                {
                    row2 += row2[0];
                    row2 = row2.Substring(1, row2.Length - 1);
                }

                row3 += row3[0];
                row3 = row3.Substring(1, row3.Length - 1);

                for (int i = 0; i <= 2; i++)
                {
                    Console.Write(ColorSign('['));
                    Console.Write(ColorSign(row1[i]) + " ");
                    Console.Write(ColorSign(row2[i]) + " ");
                    Console.Write(ColorSign(row3[i]));
                    Console.WriteLine(ColorSign(']'));
                }

                System.Threading.Thread.Sleep(100 + Math.Max(0, 10 + teller - duration) * duration * 2);
                teller++;
            }

            Console.CursorVisible = true;

            int winst = 0;
            if (row1[0] == row2[0] && row2[0] == row3[0])
                winst += ValueSign(row1[0]);
            if (row1[1] == row2[1] && row2[1] == row3[1])
                winst += ValueSign(row1[1]);
            if (row1[2] == row2[2] && row2[2] == row3[2])
                winst += ValueSign(row1[2]);
            if (row1[0] == row2[1] && row2[1] == row3[2])
                winst += ValueSign(row1[0]);
            if (row1[2] == row2[1] && row2[1] == row3[0])
                winst += ValueSign(row1[2]);

            if (winst > 0)
                Console.WriteLine($"Hoera! Je wint {winst}.");
            else
                Console.WriteLine("Jammer. Je wint niets.");
            Console.ReadLine();

            return winst;
        }
        static int Memory()
        {
            Console.Clear();

            int winst = 0;
            string kaartenKeuzes = "♥♥♦♦♣♣♠♠AA";
            kaartenKeuzes = Shuffle(kaartenKeuzes, new Random());

            Console.WriteLine(" 0   1   2   3   4   5   6   7   8   9");
            for (int i = 0; i < kaartenKeuzes.Length; i++)
            {
                Console.Write(ColorSign('['));
                Console.Write(ColorSign(kaartenKeuzes[i]));
                Console.Write(ColorSign(']') + " ");
            }
            Console.WriteLine();
            Console.WriteLine("Onthoud deze combinatie!");
            System.Threading.Thread.Sleep(5000);

            char[] geraden = new char[10];

            for (int i = 0; i < geraden.Length; i++)
            {
                geraden[i] = '?';
            }

            bool finished = false;
            while (!finished)
            {
                Console.Clear();
                Console.WriteLine(" 0   1   2   3   4   5   6   7   8   9");
                for (int i = 0; i < geraden.Length; i++)
                {
                    Console.Write(ColorSign('['));
                    Console.Write(ColorSign(geraden[i]));
                    Console.Write(ColorSign(']') + " ");
                }

                bool test = true;
                for (int i = 0; i < kaartenKeuzes.Length; i++)
                {
                    if (kaartenKeuzes[i] != geraden[i])
                        test = false;
                }

                Console.WriteLine();

                if (test)
                {
                    finished = true;
                    winst = 30;
                    Console.WriteLine("Gefeliciteerd! Gewonnen!");
                    Console.ReadLine();
                }
                else
                {
                    int kaart1 = Convert.ToInt32(VraagInput("Kaart 1"));
                    int kaart2 = Convert.ToInt32(VraagInput("Kaart 2"));

                    if (kaartenKeuzes[kaart1] == kaartenKeuzes[kaart2])
                    {
                        geraden[kaart1] = kaartenKeuzes[kaart1];
                        geraden[kaart2] = kaartenKeuzes[kaart2];
                    }
                    else
                    {
                        finished = true;
                        Console.WriteLine("Dat is fout.");
                        Console.ReadLine();
                    }
                }
            }
            return winst;
        }
        static void GebruikerToevoegen()
        {
            string file = "gegevens.txt";

            string naam = VraagNaam();
            string wachtwoord = VraagWachtwoord();

            using (StreamWriter writer = new StreamWriter(file, true))
            {
                writer.WriteLine(naam + " " + Encryptor(wachtwoord));
                Console.WriteLine("De gebruiker werd toegevoegd.");
            }
            Console.ReadLine();
        }
        static void GebruikerBewerken()
        {
            string[] gebruikersLijst = GebruikersOphalen();

            if (gebruikersLijst == null || gebruikersLijst.Length == 0)
            {
                Console.WriteLine("Geen gebruikers gevonden.");
                Console.ReadLine();
            }
            else
            {
                string gekozenUsername = gebruikersLijst[SelectMenu(gebruikersLijst) - 1];


                switch (SelectMenu(gekozenUsername + " zijn/haar username aanpassen", gekozenUsername + " zijn/haar wachtwoord aanpassen"))
                {
                    case 1:
                        {
                            string nieuweNaam = VraagNaam();
                            string[] gebruikers = AllesOphalen();
                            string gevonden = string.Empty;

                            for (int i = 0; i < gebruikers.Length; i++)
                            {
                                gevonden = gebruikers[i].Split(' ')[0];
                                if (gevonden == gekozenUsername)
                                {
                                    gebruikers[i] = gevonden.Replace(gevonden, nieuweNaam) + " " + gebruikers[i].Split(' ')[1];
                                    break;
                                }
                            }

                            using (StreamWriter writer = new StreamWriter("gegevens.txt"))
                            {
                                for (int i = 0; i < gebruikers.Length; i++)
                                {
                                    writer.WriteLine(gebruikers[i]);
                                }
                            }

                            Console.WriteLine("Username aangepast.");
                            Console.ReadLine();
                        }
                        break;

                    case 2:
                        {
                            string nieuwWachtwoord = VraagWachtwoord();
                            string[] gebruikers = AllesOphalen();
                            string gevonden = string.Empty;

                            for (int i = 0; i < gebruikers.Length; i++)
                            {
                                gevonden = gebruikers[i].Split(' ')[0];
                                if (gevonden == gekozenUsername)
                                {
                                    string wachtwoord = gebruikers[i].Split(' ')[1];
                                    gebruikers[i] = gevonden + " " + Encryptor(nieuwWachtwoord);
                                    break;
                                }
                            }

                            using (StreamWriter writer = new StreamWriter("gegevens.txt"))
                            {
                                for (int i = 0; i < gebruikers.Length; i++)
                                {
                                    writer.WriteLine(gebruikers[i]);
                                }
                            }

                            Console.WriteLine("Wachtwoord aangepast.");
                            Console.ReadLine();
                        }
                        break;

                    default:
                        DefaultErrorMessage();
                        break;
                }
            }
        }
        static void GebruikerVerwijderen()
        {
            string[] gebruikersLijst = GebruikersOphalen();

            if (gebruikersLijst == null || gebruikersLijst.Length == 0)
            {
                Console.WriteLine("Geen gebruikers gevonden.");
                Console.ReadLine();
            }
            else
            {
                string gekozenUsername = gebruikersLijst[SelectMenu(gebruikersLijst) - 1];

                string[] gebruikers = AllesOphalen();
                string gevonden = "";

                int index = -1;

                for (int i = 0; i < gebruikers.Length; i++)
                {
                    if (gebruikers[i].Split(' ')[0] == gekozenUsername)
                    {
                        gevonden = gebruikers[i];
                        index = i;
                        break;
                    }
                }

                gebruikers[index] = gebruikers[gebruikers.Length - 1];

                using (StreamWriter writer = new StreamWriter("gegevens.txt"))
                {
                    for (int i = 0; i < gebruikers.Length - 1; i++)
                    {
                        writer.WriteLine(gebruikers[i]);
                    }
                }
            }
        }
        static string Inloggen()
        {
            string[] gebruikers = AllesOphalen();
            bool loggedIn = false;
            string username;

            do
            {
                Console.Clear();
                username = VraagInput("Username");
                string wachtwoord = VraagInput("Wachtwoord");

                for (int i = 0; i < gebruikers.Length; i++)
                {
                    if (gebruikers[i].Split(' ')[0].ToLower() == username.ToLower() && gebruikers[i].Split(' ')[1] == Encryptor(wachtwoord))
                    {
                        loggedIn = true;
                    }
                }

                if (!loggedIn)
                {
                    Console.WriteLine("Foute combinatie");
                    Console.ReadLine();
                }
            } while (!loggedIn);

            return username;
        }
        static string VraagNaam()
        {
            string naam = string.Empty;
            bool passed = false;
            bool isUnique = true;

            do
            {
                Console.Clear();
                Console.WriteLine("Geef een username in.\nDe username mag alleen letters en cijfers bevatten.");

                if (!isUnique)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("De naam bestaat al. Gelieve een andere naam in te geven.");
                    Console.ResetColor();
                    passed = false;
                }

                if (passed)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("De gekozen naam voldoet niet aan de voorwaarden.");
                    Console.WriteLine("Gelieve bovenstaande voorwaarden toe te passen.");
                    Console.ResetColor();
                }

                passed = true;
                naam = VraagInput("Naam");

                string[] checkUnique = GebruikersOphalen();

                isUnique = true;
                for (int i = 0; i < checkUnique.Length; i++)
                {
                    if (checkUnique[i].ToLower() == naam.ToLower())
                    {
                        isUnique = false;
                    }
                }
            }
            while (!Regex.IsMatch(naam, @"^[a-zA-Z0-9]+$")
            || string.IsNullOrEmpty(naam)
            || !isUnique);

            return naam;
        }
        static string VraagWachtwoord()
        {
            string wachtwoord = string.Empty;
            bool passed = true;

            do
            {
                Console.Clear();
                Console.WriteLine("Geef een wachtwoord in.\nMinstens 1 kleine letter, 1 hoofdletter, 1 cijfer, 1 teken 8-20 characters");

                if (!passed)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Het wachtwoord voldoet niet aan de regels.");
                    if (!wachtwoord.Any(char.IsUpper))
                        Console.WriteLine("! Het bevat geen hoofdletters.");
                    if (!wachtwoord.Any(char.IsLower))
                        Console.WriteLine("! Het bevat geen kleine letters.");
                    if (!wachtwoord.Any(char.IsDigit))
                        Console.WriteLine("! Het bevat geen cijfers.");
                    if (wachtwoord.Length < 8)
                        Console.WriteLine("! Het wachtwoord is te kort.");
                    if (wachtwoord.Length > 20)
                        Console.WriteLine("! Het wachtwoord is te lang.");
                    if (Regex.IsMatch(wachtwoord, @"^[a-zA-Z0-9]+$"))
                        Console.WriteLine("! Het wachtwoord bevat geen vreemd teken.");
                    Console.ResetColor();
                }
                passed = false;
                wachtwoord = VraagInput("Wachtwoord");
            } while (!wachtwoord.Any(char.IsUpper)
            || !wachtwoord.Any(char.IsLower)
            || !wachtwoord.Any(char.IsDigit)
            || wachtwoord.Length < 8
            || wachtwoord.Length > 20
            || Regex.IsMatch(wachtwoord, @"^[a-zA-Z0-9]+$"));

            return wachtwoord;
        }
        static string VraagInput(string vraag)
        {
            Console.Write(vraag + ": ");
            return Console.ReadLine();
        }
        static string[] GebruikersOphalen()
        {
            string[] gebruikersArray;

            gebruikersArray = File.ReadAllLines("gegevens.txt");

            for (int i = 0; i < gebruikersArray.Length; i++)
            {
                gebruikersArray[i] = gebruikersArray[i].Split(' ')[0];
            }

            return gebruikersArray;
        }
        static string[] AllesOphalen()
        {
            return File.ReadAllLines("gegevens.txt");
        }
        static string Encryptor(string wachtwoord)
        {
            string encrypted = "";

            for (int i = 0; i < wachtwoord.Length; i++)
            {
                {
                    encrypted += 255 - wachtwoord[i];
                }
            }

            return encrypted;
        }
        static int[] Shuffle(int[] list)
        {
            Random rng = new Random();
            int n = list.Length;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int temp = list[k];
                list[k] = list[n];
                list[n] = temp;
            }

            return list;
        }
        static string Shuffle(string list, Random rng)
        {
            int n = list.Length;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                char value = list[k];
                list = list.Remove(k, 1).Insert(k, list[n].ToString());
                list = list.Remove(n, 1).Insert(n, value.ToString());
            }

            return list;
        }
        static void DefaultErrorMessage()
        {
            Console.WriteLine("Ge hebt het grat verpest.");
            Console.ReadLine();
        }
    }
}
