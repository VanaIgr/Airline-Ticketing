﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Communication {
	
	[Serializable] public class Customer {
		public string login, password;

		public Customer() { }

		public Customer(string login, string password) {
			this.login = login;
			this.password = password;
		}

		internal void unlogin() {
			login = null; password = null;
		}

		internal void setFrom(Customer o) {
			login = o.login;
			password = o.password;
		}
	}

	[Serializable] public struct AvailableOptionsResponse {
		public Dictionary<int, string> flightClasses;
		public List<City> cities;
	}

	[Serializable] public class MatchingFlightsParams {
		public string fromCode;
		public string toCode;
		public DateTime when;
	}

	[Serializable] public class AvailableFlight {
		public int id;
		public DateTime departureTime;
		public int arrivalOffsteMinutes;

		public string flightName;
		public string airplaneName;

		public Dictionary<int, FlightsOptions.Options> optionsForClasses;
		public SeatsScheme.Seats seatsScheme;

		public override string ToString() {
			var sb = new StringBuilder();

			sb.AppendFormat(
				"{{ id={0}, date={1}, name={2}, airplane={3}, arrivalOffsteMinutes={4} }}",
				id, departureTime, flightName, airplaneName, arrivalOffsteMinutes
			);

			return sb.ToString();
		}
	}

	[Serializable] public struct City {
		public string country, code;
		public string name { get; set; } //display in combobox requires property
	}

	[Serializable] public class Passanger {
		public string name;
		public string surname;
		public string middleName;
		public DateTime birthday;
		public Documents.Document document;
	}

	[ServiceContract]
	public interface MessageService {
		[FaultContract(typeof(object))] [OperationContract] 
		void register(Customer customer);


		[FaultContract(typeof(object))] [OperationContract] 
		void logIn(Customer customer);		
		

		[FaultContract(typeof(object))] [OperationContract] 
		AvailableOptionsResponse availableOptions();	


		[FaultContract(typeof(object))] [OperationContract] 
		List<AvailableFlight> matchingFlights(MatchingFlightsParams p);	

		[FaultContract(typeof(object))] [OperationContract] 
		Dictionary<int, Passanger> getPassangers(Customer customer);

		[FaultContract(typeof(object))] [OperationContract] 
		int addPassanger(Customer customer, Passanger passanger);

		[FaultContract(typeof(object))] [OperationContract] 
		int replacePassanger(Customer customer, int index, Passanger passanger);
	}
}
