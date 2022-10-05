﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using System.Diagnostics;
using System.Collections.Generic;

using Communication;
using System.Runtime.Remoting.Messaging;
using System.Reflection;
using System.Linq;
using FlightsOptions;

namespace AirlineTicketingServer {
	
	class Program {
		
		[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
		class MainMessageService : MessageService {
			private readonly Dictionary<int, string> flightClasses;
			private readonly List<City> cities;

			class A {
				public List<int> l;
			}

			public MainMessageService() {
				/*var options = new Options {
					baggageOptions = new BaggageOptions{
						baggage = new List<Baggage> {
							new Baggage(costRub: 2500, count: 1, maxWeightKg: 23),
							new Baggage(costRub: 5000, count: 2, maxWeightKg: 23)
						},
						handLuggage = new List<Baggage>{
							new Baggage(costRub: 0, count: 1, maxDim: new Size3{ x=55, y=40, z=20 }),
						},
					},
					termsOptions = new TermsOptions {
						changeFlightCostRub = 3250,
						refundCostRub = -1
					},
					servicesOptions = new ServicesOptions {
						seatChoiceCostRub = 450
					}
				};*/

				flightClasses = flightClasses = new Dictionary<int, string>();
				cities = new List<City>();

				using(
				var connection = new SqlConnection(Properties.Settings.Default.customersFlightsConnection)) {

				using(
				var selectClasses = new SqlCommand("select [Id], [Name] from [Flights].[Classes]")) {
				selectClasses.CommandType = System.Data.CommandType.Text;

				using(
				var selectCities = new SqlCommand("select [Id], [Name], [Country] from [Flights].[Cities]")) {
				selectCities.CommandType = System.Data.CommandType.Text;

				selectClasses.Connection = connection;
				connection.Open();
				using(
				var result = selectClasses.ExecuteReader()) {
				while(result.Read()) flightClasses.Add((int) result[0], (string) result[1]);
				}

				selectCities.Connection = connection;
				using(
				var result = selectCities.ExecuteReader()) {
				while(result.Read()) cities.Add(new City{ 
					code = (string) result[0], 
					name = (string) result[1],
					country = (string) result[2] 
				});
				}
				}
				}

				AvailableFlightsUpdate.checkAndUpdate(new SqlConnectionView(connection, true));
				}				
			}

			string testLoginPasswordValid(string login, string password) {
				if(login == null) login = "";
				if(password == null) password = "";
				var invalidLogin = LoginRegister.checkLogin(login);
				var invalidPassword = LoginRegister.checkPassword(password);

				if(!invalidLogin.ok || !invalidPassword.ok) {
					var loginError = invalidLogin.error;
					var passError = invalidPassword.error;
					return loginError + "\n" + passError;
				}
				else return null;
			}

			public AvailableOptionsResponse availableOptions() {
				return new AvailableOptionsResponse {
					flightClasses = flightClasses,
					cities = cities
				};
			}

			public List<AvailableFlight> matchingFlights(MatchingFlightsParams p) {
				if(p.fromCode == null) throw new FaultException<object>(null, "Город вылета должен быть заполнен");
				if(p.toCode == null) throw new FaultException<object>(null, "Город прилёта должен быть заполнен");
				if(p.when == null) throw new FaultException<object>(null, "Дата вылета должа быть заполнена");
				
				var list = new List<AvailableFlight>();
				using(
				var connection = new SqlConnection(Properties.Settings.Default.customersFlightsConnection)) {

				using(
				var selectClasses = new SqlCommand(
					@"select [AvailableFlight], [FlightName], [AirplaneName], [DepartureDatetime], [ArivalOffsetMinutes], [AvailableSeatsCount], [OptionsBin]
					from [Flights].[FindFlights](@fromCity, @toCity, @time, @class) 
					order by [DepartureDatetime] desc",
					connection
				)) {
				selectClasses.CommandType = CommandType.Text;
				selectClasses.Parameters.AddWithValue("@fromCity", p.fromCode);
				selectClasses.Parameters.AddWithValue("@toCity", p.toCode);
				selectClasses.Parameters.AddWithValue("@time", DateTime.Today.AddDays(2));
				selectClasses.Parameters.AddWithValue("@class", p.classId);

				connection.Open();
				using(
				var result = selectClasses.ExecuteReader()) {
				while(result.Read()) list.Add(new AvailableFlight{
					id = (int) result[0], flightName = (string) result[1],
					airplaneName = (string) result[2], departureTime = (DateTime) result[3],
					arrivalOffsteMinutes = (int) result[4],
					options = Binary.fromBytes((byte[])result[6]),
					availableSeatsCount = (int) result[5]
				});

				}
				}
				}

				return list;
			}

			public void register(string login, string password) {
				var error = testLoginPasswordValid(login, password);
				if(error != null) throw new FaultException<object>(null, error);

				var registered = LoginRegister.register(login, password);

				if(!registered) throw new FaultException<object>(
					null, 
					"Аккаунт с таким именем пользователя уже существует"
				);
			}

			public void logIn(string login, string password) {
				var error = testLoginPasswordValid(login, password);
				if(error != null) throw new FaultException<object>(null, error);

				var loggedIn = LoginRegister.login(login, password);

				if(loggedIn.status == LoginRegister.LoginResultStatus.USER_NOT_EXISTS) 
					 throw new FaultException<object>(null, "Пользователь с данным логином не найден");
				else if(loggedIn.status == LoginRegister.LoginResultStatus.WRONG_PASSWORD) 
					throw new FaultException<object>(null, "Неправильный пароль");

				//var userId = loggedIn.userID;
			}
		}

		[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
		public class LoggingProxy : System.Runtime.Remoting.Proxies.RealProxy {
		    private readonly MessageService instance;
		
		    private LoggingProxy(MessageService instance) : base(typeof(MessageService)) {
		        this.instance = instance;
		    }
		
		    public static MessageService Create(MessageService instance) {
		        return (MessageService) new LoggingProxy(instance).GetTransparentProxy();
		    }
		
		    public override IMessage Invoke(IMessage msg) {
				Stopwatch watch = null;
				try {
					var methodCall = (IMethodCallMessage) msg;
					var method = (MethodInfo) methodCall.MethodBase;
				
					watch = new Stopwatch();
					watch.Start();
		            var result = method.Invoke(instance, methodCall.InArgs);
					watch.Stop();
					Console.WriteLine(
						"Responding ({0}ms) to `{1}` with `{2}`",
						watch.Elapsed.TotalMilliseconds,
						methodCall.MethodName,
						result
					);
		            return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
		        }
		        catch (Exception e) {
					watch.Stop();
					Console.WriteLine(
						"Error while responding ({0} ms) to `{1}`: {2}",
						watch?.Elapsed.TotalMilliseconds,
						(msg as IMethodCallMessage)?.MethodName,
						e.ToString()
					);

		            if (e is TargetInvocationException && e.InnerException != null) {
						if(e.InnerException is FaultException<object>) {
							return new ReturnMessage(e.InnerException, msg as IMethodCallMessage);
						}
		                else return new ReturnMessage(new FaultException<object>(null, "Неизвестная ошибка"), msg as IMethodCallMessage);
					} 
					else throw e;
		        }
		    }
		}
		static void Main(string[] args) {
            try {
				string adress = "net.tcp://localhost:8080";
				var service = LoggingProxy.Create(new MainMessageService());
				
				ServiceHost host = new ServiceHost(service, new Uri[] { new Uri(adress) });
				var binding = new NetTcpBinding();
				host.AddServiceEndpoint(typeof(MessageService), binding, "client-query");
				host.Opened += (a, b) => Console.WriteLine("Server opened");
				host.Open();
			}
			catch(Exception e) {
				Console.Write("Error on startup:\n{0}", e);
			}

			Console.ReadKey();
		}
	}
}
