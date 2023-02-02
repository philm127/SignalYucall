using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalYucall.Domain.Entities
{
	public class Country
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string ShortName { get; set; }
		public DateTime CreatedDatedatetime { get; set; }
		public DateTime UpdatedDate { get; set; }
		public int Status { get; set; }
		public string TermAndConditionFileName { get; set; }
		public string CountryCode { get; set; }
	}
}