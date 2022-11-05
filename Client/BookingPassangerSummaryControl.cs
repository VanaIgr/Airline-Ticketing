﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClientCommunication {
	public partial class BookingPassangerSummaryControl : UserControl {
		public BookingPassangerSummaryControl() {
			InitializeComponent();
		}

		public void set(
			Customer customer, BookingPassanger bookingP,
			FlightsSeats.Seats seats, Dictionary<int, FlightsOptions.Options> optionsForClasses, 
			ClientCommunication.BookedSeatInfo? bookedSeatInfo, ClientCommunication.SeatCost seatCost,
			Dictionary<int, string> classesNames
		) {
			var passanger = customer.passangers[(int) bookingP.passangerIndex];

			var sb = new StringBuilder();
			
			//full name
			this.passangerNameLabel.Text = sb.Clear()
				.Append("ФИО: ")
				.Append(passanger.surname)
				.Append(" ")
				.Append(passanger.name)
				.Append(" ")
				.Append(passanger.middleName)
				.ToString();

			var seatClass = bookingP.ClassId(seats);
			var options = optionsForClasses[seatClass];

			//base brice
			basePriceLabel.Text = sb.Clear().Append("Базовая цена: ").Append(seatCost.basePrice).Append(" руб.").ToString();

			//seat
			sb.Clear().Append("Место: ");
			if(bookingP.manualSeatSelected) {
				sb.Append(seats.Scheme.ToName(bookingP.seatIndex))
					.Append(" (")
					.Append(classesNames[seatClass])
					.Append(")");
			}
			else { 
				sb.Append("автовыбор (")
					.Append(classesNames[seatClass]).Append(")");
			}

			if(bookedSeatInfo != null) {
				sb.Append("[").Append(seats.Scheme.ToName(bookedSeatInfo.Value.selectedSeat)).Append("]");
			}
			sb.Append(", ");
			sb.Append(seatCost.seatCost).Append(" руб.");

			this.seatLabel.Text = sb.ToString();

			//baggage
			var b = options.baggageOptions.baggage[bookingP.baggageOptionIndexForClass[seatClass]];
			sb.Clear().Append("Багаж: ");
			if(b.count == 0) sb.Append("без багажа");
			else {
				sb.Append(b.count).Append(" x ");
				if(b.RestrictionSize) sb.Append(b.maxWeightKg).Append("кг");
				else sb.Append("сумка");
			}
			sb.Append(", ").Append(b.costRub).Append(" руб.").ToString();
			baggageLabel.Text = sb.ToString();

			//hand luggage
			var h = options.baggageOptions.handLuggage[bookingP.handLuggageOptionIndexForClass[seatClass]];
			sb.Clear().Append("Ручная кладь: ");
			if(h.count == 0) sb.Append("без ручной клади");
			else {
				sb.Append(h.count).Append(" x ");
				if(h.RestrictionSize) sb.Append(h.maxWeightKg).Append("кг");
				else sb.Append("сумка");
			}
			sb.Append(", ").Append(h.costRub).Append(" руб.").ToString();
			handLuggageLabel.Text = sb.ToString();

			this.totalPriceLabel.Text = sb.Clear().Append("Итого: ").Append(seatCost.totalCost).Append(" руб.").ToString();
		}
	}
}
