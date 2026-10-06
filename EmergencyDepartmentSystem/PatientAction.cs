namespace EmergencyDepartmentSystem
{
    // One thing a staff member did to a patient's record, e.g. "Medication added".
    public class PatientAction
    {
        public int PatientNumber { get; }
        public string Description { get; }
        public DateTime RecordedAt { get; }

        public PatientAction(int patientNumber, string description)
        {
            PatientNumber = patientNumber;
            Description = description;
            RecordedAt = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{RecordedAt:HH:mm:ss} - Patient {PatientNumber} - {Description}";
        }
    }
}
