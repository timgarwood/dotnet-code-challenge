namespace CodeChallenge.Models
{
    /// <summary>
    /// This is the API model object for describing the number of 
    /// direct reports for a given employee
    /// </summary>
    public class ReportingStructureModel
    {
        /// <summary>
        /// the employee at the top of the reporting structure
        /// </summary>
        public Employee Employee { get; set; }

        /// <summary>
        /// total number of reports below this employee in the org
        /// </summary>
        public int NumberOfReports { get; set; }
    }
}
