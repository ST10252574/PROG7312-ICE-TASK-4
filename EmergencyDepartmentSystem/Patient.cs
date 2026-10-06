namespace EmergencyDepartmentSystem
{
    // Holds the details we collect when someone walks into the emergency department.
    public class Patient
    {
        public int PatientNumber { get; }
        public string Name { get; }
        public int Age { get; }
        public string ReasonForVisit { get; }
        public DateTime ArrivalTime { get; }

        public Patient(int patientNumber, string name, int age, string reasonForVisit)
        {
            PatientNumber = patientNumber;
            Name = name;
            Age = age;
            ReasonForVisit = reasonForVisit;
            ArrivalTime = DateTime.Now; // stamped automatically at registration
        }

        public override string ToString()
        {
            return $"{ArrivalTime:HH:mm} - Patient {PatientNumber} - {Name} (Age {Age}) - {ReasonForVisit}";
        }
    }
}
