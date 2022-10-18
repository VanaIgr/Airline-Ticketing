﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Client {
	public partial class FlightDetailsFill : Form {
		private Communication.MessageService service;
		private CustomerData customer;

		private List<PassangerDisplay> curPassangersDisplays;
		private List<BookingPassanger> bookingPassangers;

		private Dictionary<int, string> classesNames;
		private FlightAndCities flightAndCities;

		public FlightAndCities CurrentFlight{ get{ return this.flightAndCities; } }

		public FlightDetailsFill(Communication.MessageService service, CustomerData customer) {
			this.service = service;
			this.customer = customer;

			InitializeComponent();

			Misc.unfocusOnEscape(this);
			passangersPanel.AutoScrollMargin = new System.Drawing.Size(SystemInformation.HorizontalScrollBarHeight, SystemInformation.VerticalScrollBarWidth);

			this.seatSelectTable.BackColor2 = Color.LightGray;

			//tableLayoutPanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			//tableLayoutPanel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			Misc.addBottomDivider(headerContainer);

			Misc.fixFlowLayoutPanelHeight(passangersPanel);

			bookingPassangers = new List<BookingPassanger>();
			curPassangersDisplays = new List<PassangerDisplay>();
		}

		public bool setFromFlight(
			Dictionary<int, string> classesNames,
			FlightAndCities flightAndCities
		) {
			this.classesNames = new Dictionary<int, string>();
			foreach(var classId in flightAndCities.flight.optionsForClasses.Keys) {
				this.classesNames.Add(classId, classesNames[classId]);
			}

			//this.classesNames = classesNames;
			this.flightAndCities = flightAndCities;

			var flight = flightAndCities.flight;

			headerContainer.SuspendLayout();
			flightNameLabel.Text = flight.flightName;
			aitrplaneNameLavel.Text = flight.airplaneName;
			departureDatetimeLabel.Text = flight.departureTime.ToString("d MMMM, dddd, h:mm");
			departureLocationLabel.Text = flightAndCities.fromCityCode;
			headerContainer.ResumeLayout(false);
			headerContainer.PerformLayout();

			seatSelectTable.update(flightAndCities.flight.seats, addOrUpdatePassanger);

			updateSeatsStatusText();

			return true;
		}

		private void FlightBooking_Load(object sender, EventArgs e) {
			ActiveControl = Misc.addDummyButton(this);
		}

        private void selectCurrentPassanger(int index) {
			var passanger = bookingPassangers[index];

			var seatHandling = new SeatHandlingFromScheme(
				bookingPassangers, flightAndCities.flight.seats,
				index
			);

			var newBookingPassanger = passanger.Copy();

            var selectionForm = new PassangerSettings(
				service, customer,
				seatHandling, newBookingPassanger,
				flightAndCities.flight.optionsForClasses, classesNames
			);
			var result = selectionForm.ShowDialog();
			
			if(result == DialogResult.OK) {
				var customerPassangerIndex = newBookingPassanger.passangerIndex;

				if(passanger.useIndex) {
					var seatLoc = seatSelectTable.getSeatLocation(passanger.seatIndex);
					var seat = (SeatButton) seatSelectTable.GetControlFromPosition(seatLoc.X, seatLoc.Y);
					seat.Value = null;
				}
				
				if(newBookingPassanger.useIndex) {
					var seatLoc = seatSelectTable.getSeatLocation(newBookingPassanger.seatIndex);
					var seat = (SeatButton) seatSelectTable.GetControlFromPosition(seatLoc.X, seatLoc.Y);
					seat.Value = index;
				}

				bookingPassangers[index] = newBookingPassanger;
			}
			else if(result == DialogResult.Abort) {
				deletePassanger(index);
			}

			for(int i = 0; i < bookingPassangers.Count; i++) {
				var passangerData = bookingPassangers[i];
				var display = curPassangersDisplays[i];
				if(passangerData.passangerIndex != null) {
					Communication.Passanger p;
					var exists = customer.passangers.TryGetValue((int) passangerData.passangerIndex, out p);
					if(exists) {
						display.Passanger = p;
					}
					else {
						passangerData.passangerIndex = null;
						display.Passanger = null;
						bookingPassangers[i] = passangerData;
					}		
				}
			}

			updateSeatsStatusText();
        }

		private void addOrUpdatePassanger(SeatButton button, SeatsScheme.Point location) {
			int index;

			if(button.Value == null) index = addPassanger();
			else index = (int) button.Value;

			var passanger = bookingPassangers[index];
			passanger.useIndex = true;
			passanger.seatIndex = flightAndCities.flight.seats.Scheme.coordToIndex(location.x, location.z);
			button.Value = index;

			updateSeatsStatusText();
			
			selectCurrentPassanger(index);
		}

		private void updateSeatsStatusText() {
			var autofilledCount = 0;
			foreach(var passanger in bookingPassangers) {
				if(!passanger.useIndex) autofilledCount++;
			}

			var sb = new StringBuilder();
			if(autofilledCount != bookingPassangers.Count) {
				if(sb.Length != 0) sb.Append(", в");
				else sb.Append("В");
				sb.Append("ыбрано вручную: ").Append(bookingPassangers.Count - autofilledCount);
			}
			if(autofilledCount != 0) {
				if(sb.Length != 0) sb.Append(", в");
				else sb.Append("В");
				sb.Append("ыбрано автоматически: ").Append(autofilledCount);
			}

			selectedStatusLabel.Text = sb.ToString();
		}

		private void удалитьToolStripMenuItem_Click(object sender, EventArgs e) {
			var pass = (PassangerDisplay) passangerMenu.SourceControl;
			var number = pass.Number;
			deletePassanger(number);

			updateSeatsStatusText();
		}

		private void continueButton_Click(object sender, EventArgs e) {
			foreach(var passanger in bookingPassangers) if(passanger.passangerIndex == null) {
				MessageBox.Show(
					"Данные для всех пассажиров должны быть заданы", "", MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				return;
			}
		}

		private void addAutoseat_Click(object sender, EventArgs e) {
			addPassanger();
			updateSeatsStatusText();
		}

		private int addPassanger() {
			var index = bookingPassangers.Count;

			var enumerator = classesNames.Keys.GetEnumerator();
			enumerator.MoveNext();

			var display = new PassangerDisplay() { Number = index };
			bookingPassangers.Add(new BookingPassanger(enumerator.Current));
			curPassangersDisplays.Add(display);

			enumerator.Dispose();
			
			display.ContextMenuStrip = passangerMenu;
			display.Click += (a, b) => selectCurrentPassanger(((PassangerDisplay) a).Number);
			display.ShowNumber = true;
			display.ToolTip = passangerTooltip;

			var passangersDisplayList = passangersPanel;

			passangersDisplayList.SuspendLayout();
			passangersDisplayList.Controls.Add(display);
			passangersDisplayList.ResumeLayout(false);
			passangersDisplayList.PerformLayout();

			return index;
		}

		private void deletePassanger(int index) {
			passangersPanel.SuspendLayout();
			seatSelectTable.SuspendLayout(); //is this really needed here?

			for(int i = index+1; i < bookingPassangers.Count; i++) {
				var display = curPassangersDisplays[i];
				display.Number = i-1;
			}
			foreach(var seat in seatSelectTable) {
				if(seat.Value > index) seat.Value--;
				else if(seat.Value == index) seat.Value = null;
			}
			curPassangersDisplays[index].Dispose();
			bookingPassangers.RemoveAt(index);

			passangersPanel.ResumeLayout(false);
			seatSelectTable.ResumeLayout(false);
			passangersPanel.PerformLayout();
			seatSelectTable.PerformLayout();
		}

		class SeatHandlingFromScheme : PassangerSettings.SeatHandling {
			private List<BookingPassanger> passangers;
			private SeatsScheme.Seats seats;
			private int baggagePassangerIndex;

			public SeatHandlingFromScheme(
				List<BookingPassanger> passangers,
				SeatsScheme.Seats seats,
				int baggagePassangerIndex
			) {
				this.passangers = passangers;
				this.seats = seats;
				this.baggagePassangerIndex = baggagePassangerIndex;
			}

			int PassangerSettings.SeatHandling.classAt(int index) {
				return seats.Class(index);
			}

			public string getSeatString(int index) {
				var coord = seats.Scheme.indexToCoord(index);
				var width = seats.Scheme.WidthForRow(coord.z);
				return SeatsScheme.WidthsNaming.widthsNaming[width][coord.x] + "" + (coord.z + 1);
			}

			int? PassangerSettings.SeatHandling.indexFromSeatString(string t) {
				try {
					var z = int.Parse(t.Substring(1)) - 1;
					var width = seats.Scheme.WidthForRow(z);
					var naming = SeatsScheme.WidthsNaming.widthsNaming[width];
					var x = 0;
					for(; x < naming.Length; x++) if(char.ToLowerInvariant(naming[x]) == char.ToLowerInvariant(t[0])) break;

					return seats.Scheme.coordToIndex(x, z);
				}
				catch(Exception ex) {
					return null;
				}
			}

			bool PassangerSettings.SeatHandling.canPlaceAt(int index) {
				for(int i = 0; i < passangers.Count; i++) {
					if(i != baggagePassangerIndex && (passangers[i].useIndex && passangers[i].seatIndex == index)) {
						return false;
					}
				}
				return true;
			}
		}
	}
	

	class SeatButton : Label {
		private int? value;

		public int? Value {
			get{ return value; }
			set{
				if(value == null) Text = "";
				else Text = "" + value;
				this.value = value;
				setColors();
			}
		}

		public SeatButton() : base() {
			TextAlign = ContentAlignment.MiddleCenter;
			Dock = DockStyle.Fill;
			Margin = new Padding(3);
			Value = null;
			AutoSize = true;
		}

		private void setColors() {
			 if(!Enabled) {
				BackColor = Color.FromArgb(unchecked((int) 0xff98a3b8u));
				ForeColor = SystemColors.ControlText;
			}
			else if(Value != null) {
				BackColor = FlightDisplay.freeColor; 
				ForeColor = SystemColors.ControlLightLight;
			}
			else {
				BackColor = Color.CornflowerBlue;
				ForeColor = SystemColors.ControlLightLight;
			}
		}
	}

	class SeatsTable : TableLayoutPanel {
		public Color BackColor2;

		private SeatsScheme.SeatsScheme seatsScheme;
		private Dictionary<int, Point> seatsIndexToTableLocation;

		public SeatsTable() : base() { 
			DoubleBuffered = true; 
			seatsIndexToTableLocation = new Dictionary<int, Point>();
		}

		public Point getSeatLocation(SeatsScheme.Point seatPos) {
			return seatsIndexToTableLocation[seatsScheme.coordToIndex(seatPos)];
		}

		public Point getSeatLocation(int seatIndex) {
			return seatsIndexToTableLocation[seatIndex];
		}

		public IEnumerator<SeatButton> GetEnumerator() {
			return new SeatsEnumerator(this);
		}

		private class SeatsEnumerator : IEnumerator<SeatButton> {
			private Dictionary<int, Point>.Enumerator enumerator;
			private SeatsTable table;

			public SeatsEnumerator(SeatsTable table) {
				this.table = table;
				enumerator = this.table.seatsIndexToTableLocation.GetEnumerator();
			}

			public SeatButton Current { get{
				var loc = enumerator.Current.Value;
				return (SeatButton) table.GetControlFromPosition(loc.X, loc.Y);
			} }

			object System.Collections.IEnumerator.Current => Current;

			public void Dispose() {
				enumerator.Dispose();
			}

			public bool MoveNext() {
				return enumerator.MoveNext();
			}

			public void Reset() {
				throw new NotImplementedException();
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e) {
			base.OnPaintBackground(e);
			var g = e.Graphics;

			var size = this.Size;
			var pad = this.Padding;

			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			using(var b = new SolidBrush(BackColor2)) {
			var start = new PointF((pad.Left + this.GetColumnWidths()[0]) * 1.5f, pad.Top * 0.7f  + this.GetRowHeights()[0]);
			var end = new PointF(size.Width - pad.Right * 2f, size.Height - pad.Bottom * 0.7f);
			
			//draw main body
			g.FillRectangle(b, point4(start.X, start.Y, end.X, end.Y));

			/*draw back side*/ {
				var cirDiam = end.Y - start.Y;
				var cirRad = cirDiam * 0.5f;
				g.FillEllipse(b, point4(end.X - cirRad, start.Y, end.X + cirRad, end.Y));
			}

			/*draw front side*/ {
				var cirDiamX = (end.Y - start.Y) * 3;
				var cirRadX = cirDiamX * 0.5f;
				g.FillEllipse(b, point4(start.X - cirRadX, start.Y, start.X + cirRadX, end.Y));
			}

			/*draw wings*/ {
				var center = end.X * 0.6f + start.X * 0.4f;
				var rad = (end.X - start.X) * 0.5f * 0.5f;
				using(
				var path = new System.Drawing.Drawing2D.GraphicsPath(System.Drawing.Drawing2D.FillMode.Winding)) {
				path.StartFigure();
				path.AddLine(-rad, 0, 0, -rad * 5);
				path.AddLine(0, -rad * 5, rad * 0.7f, -rad * 5);
				path.AddLine(rad * 0.7f, -rad * 5, 0, 0);
				//path.AddLine(0, 0, -rad, 0);
				path.CloseFigure();

				using(
				var p2 = (System.Drawing.Drawing2D.GraphicsPath) path.Clone()) {
				p2.Transform(new System.Drawing.Drawing2D.Matrix(
					1, 0, 0, 1, center, start.Y+1
				));
				g.FillPath(b, p2);
				}

				path.Transform(new System.Drawing.Drawing2D.Matrix(
					1, 0, 0, -1, center, end.Y-1
				));
				g.FillPath(b, path);
				}
			}
			}
		}

		public delegate void ButtonClicked(SeatButton button, SeatsScheme.Point seatLoc);

		public void update(SeatsScheme.Seats seats, ButtonClicked clicked) {
			seatsScheme = seats.Scheme;

			seatsIndexToTableLocation.Clear();

			this.SuspendLayout();

			this.Controls.Clear();
			this.RowStyles.Clear();
			this.ColumnStyles.Clear();

			var seatsWidthLCM = 1;
			for(int i = 0; i < seatsScheme.SizesCount; i++) {
				var size = seatsScheme.sizeAtIndex(i);
				seatsWidthLCM = Math2.lcm(seatsWidthLCM, size.x);
			}

			this.ColumnCount = seatsScheme.TotalLength + seatsScheme.SizesCount;
			this.RowCount = seatsWidthLCM + 1;

			//columns
			for(int z = 0; z < ColumnCount; z++) {
				this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1));
			}
			//https://stackoverflow.com/q/36169745/18704284
			//hack around the fact that last column tekes more space than others
			this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0));
			this.ColumnCount++;


			//rows
			this.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			for(int x = 1; x < RowCount; x++) {
				this.RowStyles.Add(new RowStyle(SizeType.Percent, 1));
			}

			/*add seats*/ { 
				int z = 0;
				int tableZ = 0;
				for(int i = 0; i < seatsScheme.SizesCount; i++) {
					var size = seatsScheme.sizeAtIndex(i);

					var xSpan = seatsWidthLCM / size.x;

					//add row names
					var widthNaming = SeatsScheme.WidthsNaming.widthsNaming[size.x];
					for(int x = 0; x < size.x; x++) {
						var label = new Label{ 
							Text = widthNaming[x].ToString(), 
							TextAlign = ContentAlignment.MiddleCenter, 
							Dock = DockStyle.Fill, 
							BackColor = Color.Transparent 
						};
						this.Controls.Add(label, tableZ, 1+x*xSpan);
						this.SetRowSpan(label, xSpan);
					}
					tableZ++;

					for(int zO = 0; zO < size.z; zO++, z++) {
						//add column name
						this.Controls.Add(new Label{ 
							Text = "" + (1+z), TextAlign = ContentAlignment.MiddleCenter, 
							Dock = DockStyle.Fill, BackColor = Color.Transparent 
						}, tableZ, 0);

						for(int x = 0; x < size.x; x++) {
							var seatLoc = new SeatsScheme.Point{x=x,z=z};
							var seat = seats[x, z];

							var it = new SeatButton();
							if(seat.Occupied) it.Enabled = false;
							it.Click += (a, b) => clicked(it, seatLoc);

							var tablePos = new Point(tableZ, 1 + x*xSpan);
							this.Controls.Add(it, tablePos.X, tablePos.Y);
							this.SetRowSpan(it, xSpan);
							seatsIndexToTableLocation.Add(this.seatsScheme.coordToIndex(seatLoc), tablePos);
						}
						tableZ++;
					}
				}
			}

			this.ResumeLayout(false);
			this.PerformLayout();
		}

		static RectangleF point4(float x1, float y1, float x2, float y2) {
			return new RectangleF(x1, y1, x2 - x1, y2 - y1);
		}
	}
}