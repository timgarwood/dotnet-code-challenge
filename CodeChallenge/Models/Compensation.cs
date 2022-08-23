using CodeChallenge.JsonConverters;
using Newtonsoft.Json;
using System;

namespace CodeChallenge.Models
{
    /// <summary>
    /// This is the API model object for describing
    /// a compensation for an employee
    /// </summary>
    public class Compensation
    {
        /// <summary>
        /// compensation database id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// foreign key property to the Employee
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// navigation property to the Employee
        /// </summary>
        public Employee Employee { get; set; }

        /// <summary>
        /// salary
        /// </summary>
        public double Salary { get; set; }

        /// <summary>
        /// date at which the salary takes effect
        /// </summary>
        [JsonConverter(converterType: typeof(DateOnlyConverter))]
        public DateOnly EffectiveDate { get; set; }
    }
}
