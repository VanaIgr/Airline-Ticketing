﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ClientCommunication;
using Communication;

namespace ClientCommunication {
	public class CustomerData {
		public Customer? customer;

		public int newPassangerIndex = 0;
		public Dictionary<int, PassangerIdData> passangerIds;
		public Dictionary<int, Passanger> passangers;

		//public List<BookedFlight> localBookedFlights;
		//public List<BookedFlightDetails> localBookedFlightsDetails;

		//public List<BookedFlight> bookedFlights;
		public int newBookedFlightIndex = 0;
		public Dictionary<int, BookedFlightDetails> bookedFlightsDetails;
		public Dictionary<int, BookedFlight> flightsBooked;

		public CustomerData() { 
			customer = null;
			newPassangerIndex = 0;
			passangerIds = new Dictionary<int, PassangerIdData>();
			passangers = new Dictionary<int, Passanger>();
			bookedFlightsDetails = new Dictionary<int, BookedFlightDetails>();
			flightsBooked = new Dictionary<int, BookedFlight>();
		}

		public CustomerData(string login, string password) : this() {
			this.customer = new Customer{
				login = login,
				password = password
			};
		}

		public Customer Get() { return (Customer) customer; }
		
		public void unlogin() {
			customer = null;
			newPassangerIndex = 0;
			passangerIds.Clear();
			passangers.Clear();
			bookedFlightsDetails.Clear();
			flightsBooked.Clear();
		}

		public void setFrom(Customer o) {
			unlogin();
			this.customer = o;
		}

		public bool LoggedIn{
			get{ return customer != null; }
		}

		public int? findPasangerIndexByDatabaseId(int databaseId) {
			foreach(var pair in passangerIds) {
				if(!pair.Value.IsLocal && pair.Value.DatabaseId == databaseId) return pair.Key;
			}
			return null;
		}
	}

	public struct PassangerIdData {
		private int? databaseId;

		public PassangerIdData(int? databaseId) {
			this.databaseId = databaseId;
		}

		public bool IsLocal{ get{ return databaseId == null; } }

		public int DatabaseId{ get{
			if(IsLocal) throw new InvalidOperationException();
			else return (int) databaseId;
		}}
	}

	/*public struct PassangerId {
		private bool? isLocal;
		private int index;

		public static PassangerId fromLocalIndex(int value) {
			return new PassangerId{ isLocal = true, index = value };
		}

		public static PassangerId fromDatabaseIndex(int value) {
			return new PassangerId{ isLocal = false, index = value };
		}

		public bool isValid{ get{ return isLocal != null; } }
		public bool IsLocal{ get{ if(!isValid) throw new InvalidOperationException(); return (bool) isLocal; } }

		public int Index{ get{ if(!isValid) throw new InvalidOperationException(); return index; } }

		public override bool Equals(object obj) {
			if(obj == null || !(obj is PassangerId)) return false;
			var o = (PassangerId) obj;
			return isLocal == o.isLocal && index == o.index;
		}

		public override int GetHashCode() {
			return index * (isLocal == null ? 0 : (isLocal == true ? 1 : -1));
		}
	}*/
}
