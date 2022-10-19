﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client {
	public partial class PassangerOptions : UserControl {
		private Communication.MessageService service;
		private int flightId;

		private Dictionary<int, FlightsOptions.Options> optionsForClasses;

		private Dictionary<int, BaggageOption> baggageDisplays;
		private Dictionary<int, BaggageOption> handLuggageDisplays;

		private BookingPassanger passanger;
		private int curClassId;

		public PassangerOptions() {
			InitializeComponent();
			Misc.fixFlowLayoutPanelHeight(this.baggageOptionsPanel);
			Misc.fixFlowLayoutPanelHeight(this.handLuggageOptionsPanel);

			this.baggageDisplays = new Dictionary<int, BaggageOption>();
			this.handLuggageDisplays = new Dictionary<int, BaggageOption>();
		}

		public void init(
			Communication.MessageService service,
			int flightId,
			Dictionary<int, FlightsOptions.Options> optionsForClasses,
			BookingPassanger passanger
		) {
			this.service = service;
			this.flightId = flightId;
			this.optionsForClasses = optionsForClasses;
			this.passanger = passanger;
		}

		private BaggageOption addBaggageOption(FlightsOptions.Baggage b, int optionIndex) {
			var it = new BaggageOption {
				Baggage = b,
				Margin = new Padding(3),
				Index = optionIndex,
			};
			it.Cursor = Cursors.Hand;
			it.Click += (a, c) => { baggageOptionClicked((BaggageOption) a); };
			baggageOptionsPanel.Controls.Add(it);
			baggageDisplays.Add(optionIndex, it);
			return it;
		}

		private BaggageOption addHandLuggageOption(FlightsOptions.Baggage b, int optionIndex) {
			var it = new BaggageOption {
				Baggage = b,
				Margin = new Padding(3),
				Index = optionIndex,
			};
			it.Cursor = Cursors.Hand;
			it.Click += (a, c) => { handLuggageOptionClicked((BaggageOption) a); };
			handLuggageOptionsPanel.Controls.Add(it);
			handLuggageDisplays.Add(optionIndex, it);
			return it;
		}

		public void updateForClassAndSeat(int classId) {
			baggageDisplays.Clear();
			handLuggageDisplays.Clear();
			curClassId = classId;

			var options = optionsForClasses[classId];
			

			{
				baggageOptionsPanel.SuspendLayout();
				baggageOptionsPanel.Controls.Clear();

				var baggages = options.baggageOptions.baggage;

				for(int i = 0; i < baggages.Count; i++) {
					var b = baggages[i];
					if(b.IsFree) addBaggageOption(b, i);
				}

				for(int i = 0; i < baggages.Count; i++) {
					var b = baggages[i];
					if(!b.IsFree) addBaggageOption(b, i);
				}

				updateCurBaggageOptionDisplay();

				baggageOptionsPanel.ResumeLayout(false);
				baggageOptionsPanel.PerformLayout();
			}

			{
				handLuggageOptionsPanel.SuspendLayout();
				handLuggageOptionsPanel.Controls.Clear();

				var baggages = options.baggageOptions.handLuggage;

				for(int i = 0; i < baggages.Count; i++) {
					var b = baggages[i];
					if(b.IsFree) addHandLuggageOption(b, i);
				}

				for(int i = 0; i < baggages.Count; i++) {
					var b = baggages[i];
					if(!b.IsFree) addHandLuggageOption(b, i);
				}

				updateCurhandLuggageOptionDisplay();

				handLuggageOptionsPanel.ResumeLayout(false);
				handLuggageOptionsPanel.PerformLayout();
			}

			recalculatePrice();
		}

		private void baggageOptionClicked(BaggageOption option) {
			var newIndex = option.Index;

			int prevIndex;
			var hasPrevIndex = passanger.baggageOptionIndexForClass.TryGetValue(curClassId, out prevIndex);
			if(hasPrevIndex) {
				baggageDisplays[prevIndex].BackColor = Color.White;
			}

			passanger.baggageOptionIndexForClass[curClassId] = newIndex;
			updateCurBaggageOptionDisplay();
			recalculatePrice();
		}

		private void updateCurBaggageOptionDisplay() {
			int index;
			var anySelected = passanger.baggageOptionIndexForClass.TryGetValue(curClassId, out index);
			if(!anySelected) return;
			var option = baggageDisplays[index];
			option.BackColor =  Misc.selectionColor3;
		}

		private void handLuggageOptionClicked(BaggageOption option) {
			var newIndex = option.Index;

			int prevIndex;
			var hasPrevIndex = passanger.handLuggageOptionIndexForClass.TryGetValue(curClassId, out prevIndex);
			if(hasPrevIndex) {
				handLuggageDisplays[prevIndex].BackColor = Color.White;
			}

			passanger.handLuggageOptionIndexForClass[curClassId] = newIndex;
			updateCurhandLuggageOptionDisplay();
			recalculatePrice();
		}

		private void updateCurhandLuggageOptionDisplay() {
			int index;
			var anySelected = passanger.handLuggageOptionIndexForClass.TryGetValue(curClassId, out index);
			if(!anySelected) return;
			var option = handLuggageDisplays[index];
			option.BackColor =  Misc.selectionColor3;
		}

		public void recalculatePrice() {
			var options = optionsForClasses[curClassId];
			
			totalCostLabel.Text = "";
			totalCostLabel.ForeColor = SystemColors.ControlText;

			basePriceLabel.Text = options.basePriceRub + " руб.";

			int seatCost;
			if(passanger.manualSeatSelected) 
				seatCost = options.servicesOptions.seatChoiceCostRub; // can be == 0
			else seatCost = 0;
			
			if(seatCost == 0) seatPriceLabel.Text = "бесплатно";
			else seatPriceLabel.Text = seatCost + " руб.";


			int baggageIndex;
			bool isBaggageSelected = passanger.baggageOptionIndexForClass.TryGetValue(curClassId, out baggageIndex);

			int handLuggageIndex;
			bool isHandLuggageSelected = passanger.handLuggageOptionIndexForClass.TryGetValue(curClassId, out handLuggageIndex);

			if(!isBaggageSelected || !isHandLuggageSelected) {
				totalCostLabel.Text = "недоступно, так как не выбраны опции багажа или ручной клади";
			}
			else { 
				try {
					var result = service.seatsData(flightId, new Communication.SeatAndOptions[]{ new Communication.SeatAndOptions{
						useSeatIndex = passanger.manualSeatSelected,
						seatIndex = passanger.seatIndex,
						seatClassId = passanger.seatClassId,
						selectedOptions = new FlightsOptions.SelectedOptions(new FlightsOptions.SelectedBaggageOptions(
							baggageIndex, handLuggageIndex
						))
					} });

					if(result) {
						var seatData = result.s[0];

						totalCostLabel.Text = seatData.totalCost + " руб.";
					}
					else {
						totalCostLabel.ForeColor = Color.Firebrick;
						totalCostLabel.Text = result.f.message;
						statusTooltip.SetToolTip(totalCostLabel, result.f.message);
					}
				} catch(Exception e) {
					totalCostLabel.ForeColor = Color.Firebrick;
					totalCostLabel.Text = "Неизвестная ошибка";
					statusTooltip.SetToolTip(totalCostLabel, e.ToString());
				}
			}			
		}
	}
}
