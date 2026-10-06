namespace EmergencyDepartmentSystem
{
    internal class Program
    {
        // Queue = first in, first out. Perfect for a waiting room where the
        // person who arrived first should be seen first.
        private static readonly Queue<Patient> waitingList = new Queue<Patient>();

        // Stack = last in, first out. Perfect for undo, because the newest
        // action is always the one we want to reverse first.
        private static readonly Stack<PatientAction> actionHistory = new Stack<PatientAction>();

        // A few counters and trackers so the statistics screen has something useful to show.
        private static readonly HashSet<int> usedPatientNumbers = new HashSet<int>();
        private static int totalRegistered = 0;
        private static int totalCalled = 0;
        private static int totalUndone = 0;
        private static Patient? lastCalledPatient = null;

        private static void Main(string[] args)
        {
            bool running = true;

            while (running)
            {
                ShowMenu();
                string choice = (Console.ReadLine() ?? "").Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        RegisterPatient();
                        break;
                    case "2":
                        ViewWaitingPatients();
                        break;
                    case "3":
                        CallNextPatient();
                        break;
                    case "4":
                        RecordPatientAction();
                        break;
                    case "5":
                        ViewMostRecentAction();
                        break;
                    case "6":
                        UndoLastAction();
                        break;
                    case "7":
                        DisplayStatistics();
                        break;
                    case "8":
                        running = false;
                        Console.WriteLine("Shutting down the system. Goodbye!");
                        break;
                    default:
                        Console.WriteLine("That isn't a valid option, please pick a number from 1 to 8.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine();
                    Console.WriteLine("Press Enter to return to the menu...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }

        private static void ShowMenu()
        {
            Console.WriteLine("=================================");
            Console.WriteLine("   EMERGENCY DEPARTMENT SYSTEM");
            Console.WriteLine("=================================");
            Console.WriteLine("1. Register Patient");
            Console.WriteLine("2. View Waiting Patients");
            Console.WriteLine("3. Call Next Patient");
            Console.WriteLine("4. Record Patient Action");
            Console.WriteLine("5. View Most Recent Action");
            Console.WriteLine("6. Undo Last Action");
            Console.WriteLine("7. Display Statistics");
            Console.WriteLine("8. Exit");
            Console.WriteLine();
            Console.Write("Please select an option: ");
        }

        // 1. Register a new patient and add them to the back of the queue.
        private static void RegisterPatient()
        {
            Console.WriteLine("--- Register Patient ---");

            int number = ReadInt("Patient number: ", 1, int.MaxValue);
            if (usedPatientNumbers.Contains(number))
            {
                Console.WriteLine($"Patient number {number} has already been registered. Nothing was added.");
                return;
            }

            string name = ReadText("Patient name: ");
            int age = ReadInt("Age: ", 0, 120);
            string reason = ReadText("Reason for visit: ");

            Patient patient = new Patient(number, name, age, reason);
            waitingList.Enqueue(patient);
            usedPatientNumbers.Add(number);
            totalRegistered++;

            Console.WriteLine();
            Console.WriteLine($"{name} has been registered and added to the waiting list.");
            Console.WriteLine($"There are now {waitingList.Count} patient(s) waiting.");
        }

        // 2. Show everyone still waiting, in the order they will be called.
        private static void ViewWaitingPatients()
        {
            Console.WriteLine("--- Waiting Patients ---");

            if (waitingList.Count == 0)
            {
                Console.WriteLine("Nobody is waiting right now.");
                return;
            }

            int position = 1;
            foreach (Patient p in waitingList) // enumerating a queue doesn't remove anything
            {
                Console.WriteLine($"{position}. {p}");
                position++;
            }
        }

        // 3. Take the patient at the front of the queue.
        private static void CallNextPatient()
        {
            Console.WriteLine("--- Call Next Patient ---");

            if (waitingList.Count == 0)
            {
                Console.WriteLine("There are no patients waiting to be called.");
                return;
            }

            Patient next = waitingList.Dequeue();
            lastCalledPatient = next;
            totalCalled++;

            Console.WriteLine("Now calling:");
            Console.WriteLine(next);
            Console.WriteLine($"{waitingList.Count} patient(s) still waiting.");
        }

        // 4. Log something a nurse or doctor did to a patient's record.
        private static void RecordPatientAction()
        {
            Console.WriteLine("--- Record Patient Action ---");
            Console.WriteLine("Examples: Blood pressure updated, Medication added,");
            Console.WriteLine("          Allergy information updated, Doctor's notes added");
            Console.WriteLine();

            int number = ReadInt("Patient number: ", 1, int.MaxValue);

            // Only allow actions for patients we actually know about.
            if (!usedPatientNumbers.Contains(number))
            {
                Console.WriteLine($"No patient with number {number} has been registered.");
                return;
            }

            string description = ReadText("Action performed: ");

            actionHistory.Push(new PatientAction(number, description));
            Console.WriteLine("Action recorded.");
        }

        // 5. Peek at the top of the stack without removing it.
        private static void ViewMostRecentAction()
        {
            Console.WriteLine("--- Most Recent Action ---");

            if (actionHistory.Count == 0)
            {
                Console.WriteLine("No actions have been recorded yet.");
                return;
            }

            Console.WriteLine(actionHistory.Peek());
        }

        // 6. Pop the top of the stack so the newest action is reversed first.
        private static void UndoLastAction()
        {
            Console.WriteLine("--- Undo Last Action ---");

            if (actionHistory.Count == 0)
            {
                Console.WriteLine("There is nothing to undo.");
                return;
            }

            PatientAction undone = actionHistory.Pop();
            totalUndone++;

            Console.WriteLine("The following action has been undone:");
            Console.WriteLine(undone);
            Console.WriteLine($"{actionHistory.Count} action(s) left in the history.");
        }

        // 7. A quick snapshot of where the system is at.
        private static void DisplayStatistics()
        {
            Console.WriteLine("--- Statistics ---");
            Console.WriteLine($"Total patients registered : {totalRegistered}");
            Console.WriteLine($"Patients currently waiting: {waitingList.Count}");
            Console.WriteLine($"Patients called so far    : {totalCalled}");
            Console.WriteLine($"Actions in history        : {actionHistory.Count}");
            Console.WriteLine($"Actions undone            : {totalUndone}");

            string nextUp = waitingList.Count > 0
                ? $"{waitingList.Peek().Name} (Patient {waitingList.Peek().PatientNumber})"
                : "nobody";
            string lastCalled = lastCalledPatient != null
                ? $"{lastCalledPatient.Name} (Patient {lastCalledPatient.PatientNumber})"
                : "nobody yet";

            Console.WriteLine($"Next patient in line      : {nextUp}");
            Console.WriteLine($"Last patient called       : {lastCalled}");
        }

        // ---------- Small input helpers so the menu methods stay readable ----------

        // Keeps asking until the user types a whole number inside the allowed range.
        private static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = (Console.ReadLine() ?? "").Trim();

                if (int.TryParse(input, out int value) && value >= min && value <= max)
                {
                    return value;
                }

                Console.WriteLine($"Please enter a whole number between {min} and {max}.");
            }
        }

        // Keeps asking until the user types something that isn't blank.
        private static string ReadText(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = (Console.ReadLine() ?? "").Trim();

                if (input.Length > 0)
                {
                    return input;
                }

                Console.WriteLine("This can't be left empty, please try again.");
            }
        }
    }
}
