﻿
namespace Client {
	partial class FlightBooking {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.headerContainer = new System.Windows.Forms.TableLayoutPanel();
			this.button2 = new System.Windows.Forms.Button();
			this.aitrplaneNameLavel = new System.Windows.Forms.Label();
			this.flightNameLabel = new System.Windows.Forms.Label();
			this.departureLocationLabel = new System.Windows.Forms.Label();
			this.departureDatetimeLabel = new System.Windows.Forms.Label();
			this.classSelector = new System.Windows.Forms.ComboBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.passangersDisplayList = new System.Windows.Forms.TableLayoutPanel();
			this.button1 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.seatSelectTable = new Client.SeatsTable();
			this.selectedStatusLabel = new System.Windows.Forms.Label();
			this.seatHint = new System.Windows.Forms.ToolTip(this.components);
			this.headerContainer.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.passangersDisplayList.SuspendLayout();
			this.SuspendLayout();
			// 
			// headerContainer
			// 
			this.headerContainer.AutoSize = true;
			this.headerContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.headerContainer.BackColor = System.Drawing.Color.White;
			this.headerContainer.ColumnCount = 7;
			this.headerContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.headerContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.headerContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.headerContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.headerContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.headerContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
			this.headerContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.headerContainer.Controls.Add(this.button2, 6, 0);
			this.headerContainer.Controls.Add(this.aitrplaneNameLavel, 0, 2);
			this.headerContainer.Controls.Add(this.flightNameLabel, 0, 0);
			this.headerContainer.Controls.Add(this.departureLocationLabel, 1, 0);
			this.headerContainer.Controls.Add(this.departureDatetimeLabel, 1, 2);
			this.headerContainer.Controls.Add(this.classSelector, 4, 0);
			this.headerContainer.Dock = System.Windows.Forms.DockStyle.Top;
			this.headerContainer.Location = new System.Drawing.Point(0, 0);
			this.headerContainer.Name = "headerContainer";
			this.headerContainer.Padding = new System.Windows.Forms.Padding(10);
			this.headerContainer.RowCount = 3;
			this.headerContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.headerContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 3F));
			this.headerContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.headerContainer.Size = new System.Drawing.Size(1008, 57);
			this.headerContainer.TabIndex = 0;
			// 
			// button2
			// 
			this.button2.AutoSize = true;
			this.button2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.button2.BackColor = System.Drawing.Color.RoyalBlue;
			this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.button2.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
			this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.button2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.button2.Location = new System.Drawing.Point(902, 13);
			this.button2.Name = "button2";
			this.headerContainer.SetRowSpan(this.button2, 3);
			this.button2.Size = new System.Drawing.Size(93, 31);
			this.button2.TabIndex = 6;
			this.button2.Text = "Продолжить";
			this.button2.UseVisualStyleBackColor = false;
			// 
			// aitrplaneNameLavel
			// 
			this.aitrplaneNameLavel.AutoSize = true;
			this.aitrplaneNameLavel.Location = new System.Drawing.Point(13, 30);
			this.aitrplaneNameLavel.Name = "aitrplaneNameLavel";
			this.aitrplaneNameLavel.Size = new System.Drawing.Size(75, 13);
			this.aitrplaneNameLavel.TabIndex = 4;
			this.aitrplaneNameLavel.Text = "aitrplaneName";
			// 
			// flightNameLabel
			// 
			this.flightNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.flightNameLabel.AutoSize = true;
			this.flightNameLabel.Location = new System.Drawing.Point(13, 14);
			this.flightNameLabel.Name = "flightNameLabel";
			this.flightNameLabel.Size = new System.Drawing.Size(57, 13);
			this.flightNameLabel.TabIndex = 0;
			this.flightNameLabel.Text = "flightName";
			// 
			// departureLocationLabel
			// 
			this.departureLocationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.departureLocationLabel.AutoSize = true;
			this.departureLocationLabel.Location = new System.Drawing.Point(94, 14);
			this.departureLocationLabel.Name = "departureLocationLabel";
			this.departureLocationLabel.Size = new System.Drawing.Size(93, 13);
			this.departureLocationLabel.TabIndex = 1;
			this.departureLocationLabel.Text = "departureLocation";
			// 
			// departureDatetimeLabel
			// 
			this.departureDatetimeLabel.AutoSize = true;
			this.departureDatetimeLabel.Location = new System.Drawing.Point(94, 30);
			this.departureDatetimeLabel.Name = "departureDatetimeLabel";
			this.departureDatetimeLabel.Size = new System.Drawing.Size(94, 13);
			this.departureDatetimeLabel.TabIndex = 2;
			this.departureDatetimeLabel.Text = "departureDatetime";
			// 
			// classSelector
			// 
			this.classSelector.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.classSelector.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.classSelector.FormattingEnabled = true;
			this.classSelector.Location = new System.Drawing.Point(765, 18);
			this.classSelector.Name = "classSelector";
			this.headerContainer.SetRowSpan(this.classSelector, 3);
			this.classSelector.Size = new System.Drawing.Size(121, 21);
			this.classSelector.TabIndex = 5;
			this.classSelector.SelectedIndexChanged += new System.EventHandler(this.classSelector_SelectedIndexChanged);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoScroll = true;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.passangersDisplayList, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.seatSelectTable, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.selectedStatusLabel, 0, 5);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 57);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(10);
			this.tableLayoutPanel1.RowCount = 6;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1008, 396);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// passangersDisplayList
			// 
			this.passangersDisplayList.AutoScroll = true;
			this.passangersDisplayList.AutoSize = true;
			this.passangersDisplayList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.passangersDisplayList.ColumnCount = 2;
			this.passangersDisplayList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.passangersDisplayList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.passangersDisplayList.Controls.Add(this.button1, 0, 0);
			this.passangersDisplayList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.passangersDisplayList.Location = new System.Drawing.Point(10, 23);
			this.passangersDisplayList.Margin = new System.Windows.Forms.Padding(0);
			this.passangersDisplayList.Name = "passangersDisplayList";
			this.passangersDisplayList.RowCount = 1;
			this.passangersDisplayList.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.passangersDisplayList.Size = new System.Drawing.Size(988, 33);
			this.passangersDisplayList.TabIndex = 0;
			// 
			// button1
			// 
			this.button1.AutoSize = true;
			this.button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.button1.BackColor = System.Drawing.Color.Transparent;
			this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
			this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.button1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.button1.Location = new System.Drawing.Point(3, 3);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(77, 27);
			this.button1.TabIndex = 1;
			this.button1.Text = "Добавить";
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 10);
			this.label1.Margin = new System.Windows.Forms.Padding(0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Пассажиры:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 66);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(42, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Места:";
			// 
			// seatSelectTable
			// 
			this.seatSelectTable.AutoSize = true;
			this.seatSelectTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.seatSelectTable.ColumnCount = 1;
			this.seatSelectTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.seatSelectTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.seatSelectTable.Location = new System.Drawing.Point(10, 79);
			this.seatSelectTable.Margin = new System.Windows.Forms.Padding(0);
			this.seatSelectTable.Name = "seatSelectTable";
			this.seatSelectTable.Padding = new System.Windows.Forms.Padding(30, 20, 30, 20);
			this.seatSelectTable.RowCount = 1;
			this.seatSelectTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.seatSelectTable.Size = new System.Drawing.Size(988, 40);
			this.seatSelectTable.TabIndex = 3;
			// 
			// selectedStatusLabel
			// 
			this.selectedStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.selectedStatusLabel.AutoSize = true;
			this.selectedStatusLabel.Location = new System.Drawing.Point(894, 119);
			this.selectedStatusLabel.Margin = new System.Windows.Forms.Padding(0);
			this.selectedStatusLabel.Name = "selectedStatusLabel";
			this.selectedStatusLabel.Size = new System.Drawing.Size(104, 13);
			this.selectedStatusLabel.TabIndex = 4;
			this.selectedStatusLabel.Text = "selectedSeatsStatus";
			// 
			// FlightBooking
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1008, 453);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.headerContainer);
			this.Name = "FlightBooking";
			this.Text = "FlightBooking";
			this.Load += new System.EventHandler(this.FlightBooking_Load);
			this.headerContainer.ResumeLayout(false);
			this.headerContainer.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.passangersDisplayList.ResumeLayout(false);
			this.passangersDisplayList.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel headerContainer;
		private System.Windows.Forms.Label flightNameLabel;
		private System.Windows.Forms.Label departureLocationLabel;
		private System.Windows.Forms.Label departureDatetimeLabel;
		private System.Windows.Forms.Label aitrplaneNameLavel;
		private System.Windows.Forms.ComboBox classSelector;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel passangersDisplayList;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private SeatsTable seatSelectTable;
		private System.Windows.Forms.Label selectedStatusLabel;
		private System.Windows.Forms.ToolTip seatHint;
	}
}