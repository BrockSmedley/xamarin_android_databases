using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace Integrated_storage {
	public class Person {
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public Person(int ID, string firstname, string lastname) {
			this.ID = ID;
			this.FirstName = firstname;
			this.LastName = lastname;
		}

		public Person() { }

		public override string ToString() {
			return string.Format("[Person: ID={0}, FirstName={1}, LastName={2}]", ID, FirstName, LastName);
		}
	}
}