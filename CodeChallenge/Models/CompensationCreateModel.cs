using CodeChallenge.JsonConverters;
using Newtonsoft.Json;
using System;

namespace CodeChallenge.Models
{
    /// <summary>
    /// This is the API model object for creating
    /// a new compensation for an employee
    /// </summary>
    public class CompensationCreateModel
    {
        /// <summary>
        /// id of the employee, we don't need the full employee entity
        /// for creating a new Compensation
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// new salary of the employee
        /// </summary>
        public double Salary { get; set; }

        /// <summary>
        /// date at which the salary takes effect
        /// </summary>
        [JsonConverter(converterType: typeof(DateOnlyConverter))]
        public DateOnly EffectiveDate { get; set; }
    }
}
