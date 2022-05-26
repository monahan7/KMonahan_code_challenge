using System;

namespace challenge.Models
{
	public class Compensation
	{
		public string CompensationId { get; set; }
		public Employee employee { get; set; }
		public int salary { get; set; }
		public DateTime effectiveDate { get; set; }
	}
}
