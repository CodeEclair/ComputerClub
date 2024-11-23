namespace ComputerClub;

class Program
{
    static void Main(string[] args)
    {
        ComputerClub computerClub = new ComputerClub(8);
        computerClub.Work();
        computerClub.GameOver();
        Console.WriteLine("Press any key to finish...");
        Console.ReadKey();
    }
}

class ComputerClub
{
    private int _money = 0;
    private List<Computer> _computers = new List<Computer>();
    private Queue<Client> _clients = new Queue<Client>();

    public ComputerClub(int computersCount)
    {
        Random random = new Random();

        for (int i = 0; i < computersCount; i++)
        {
            _computers.Add(new Computer(random.Next(5, 15)));
        }
        
        CreateNewClients(25, random);
    }

    public void CreateNewClients(int clientsCount, Random random)
    {
        for (int i = 0; i < clientsCount; i++)
        {
            _clients.Enqueue(new Client(random.Next(100, 250), random));
        }
    }

    public void Work()
    {
        while (_clients.Count > 0)
        {
            Client newClient = _clients.Dequeue();
            Console.WriteLine($"Balance of the Computer Club is {_money} dollars. We're waiting for a new client.\n");
            Console.WriteLine($"You have a new client. They'd like to buy {newClient.DesiredMinutes} minutes.");
            ShowAllComputersState();

            Console.Write("\nYou offer them computer number: ");
            string userInput = Console.ReadLine();

            if (int.TryParse(userInput, out int computerNumber))
            {
                computerNumber -= 1;

                if (computerNumber >= 0 && computerNumber < _computers.Count)
                {
                    if (_computers[computerNumber].IsTaken)
                    {
                        Console.WriteLine("This computer is currently taken. The client just left!");
                    }
                    else
                    {
                        if (newClient.CheckSolvency(_computers[computerNumber]))
                        {
                            Console.WriteLine($"The client has payed and took the computer number {computerNumber + 1} for {newClient.DesiredMinutes} minutes.");
                            _money += newClient.Pay();
                            _computers[computerNumber].BecomeTaken(newClient);
                        }
                        else
                        {
                            Console.WriteLine("The client doesn't have enough money and they left.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("There is no computer with that number! If you were thinking better you wouldn't loose the client.");
                }
            }
            else
            {
                CreateNewClients(1, new Random());
                Console.WriteLine("Invalid input. Try again.");
            }

            Console.WriteLine("Press any key to move to the next client...");
            Console.ReadKey();
            Console.Clear();
            SpendOneMinute();
        }
    }

    private void ShowAllComputersState()
    {
        Console.WriteLine("\nList of all computers: ");
        for (int i = 0; i < _computers.Count; i++)
        {
            Console.Write(i + 1 + " - ");
            _computers[i].ShowState();
        }
    }

    private void SpendOneMinute()
    {
        foreach (var computer in _computers)
        {
            computer.SpendOneMinute();
        }
    }

    public void GameOver()
    {
        Console.WriteLine($"There are no more clients. You've earned ${_money}.");
    }
}

class Computer
{
    private Client _client;
    private int _minutesRemaining;

    public bool IsTaken
    {
        get
        {
            return _minutesRemaining > 0;
        }
    }
    
    public int PricePerMinute { get; private set; }

    public Computer(int pricePerMinute)
    {
        PricePerMinute = pricePerMinute;
    }

    public void BecomeTaken(Client client)
    {
        _client = client;
        _minutesRemaining = _client.DesiredMinutes;
    }

    public void BecomeEmpty()
    {
        _client = null;
    }

    public void SpendOneMinute()
    {
        _minutesRemaining -= 1;
    }

    public void ShowState()
    {
        if (IsTaken)
        {
            Console.WriteLine($"This computer is currently taken. Minutes remaining: {_minutesRemaining}");
        }
        else
        {
            Console.WriteLine($"This computer is currently available. Price per minute is {PricePerMinute}");
        }
    }
}

class Client
{
    private int _money;
    private int _moneyToPay;
    public int DesiredMinutes { get; private set; }

    public Client(int money, Random random)
    {
        _money = money;
        DesiredMinutes = random.Next(10, 30);
    }

    public bool CheckSolvency(Computer computer)
    {
        _moneyToPay = DesiredMinutes * computer.PricePerMinute;

        if (_money >= _moneyToPay)
        {
            return true;
        }
        else
        {
            _moneyToPay = 0;
            return false;
        }
    }

    public int Pay()
    {
        _money -= _moneyToPay;
        return _moneyToPay;
    }
}