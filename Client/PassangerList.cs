﻿using Common;
using Communication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;

namespace ClientCommunication {
	public partial class PassangerList : UserControl {
		private ClientCommunication.MessageService service;
		private CustomerData customer;
		private BookingStatus status;

		private Dictionary<int, PassangerDisplay> passangersDisplays;

		private BookingPassanger passanger;

		private DocumentFields generalDataFields;
		private DocumentFields documentFields;

		private sealed class FormPassanger {
			public string name, surname, middleName;
			public DateTime birthday;
			public int selectedDocument;
			public Dictionary<int, Documents.Document> documents;
		}
		private FormPassanger formPassanger;

		private int? currentPassangerIndex {
			get { return passanger.passangerIndex; }
			set { passanger.passangerIndex = value; }
		}

		private enum State {
			none, select, add, edit
		}

		private State curState;
		private void setStateAdd() {
			setStatus(false, null);

			if(status.booked) throw new InvalidOperationException();

			curState = State.add;
			
			deleteButton.Enabled = false;
			editButton.Enabled = false;
			addButton.Enabled = false;
			passangerDataTable.Enabled = true;

			if(currentPassangerIndex != null) passangersDisplays[(int) currentPassangerIndex].BackColor = Color.White;
			currentPassangerIndex = null;

			clearPassangerData();

			saveButton.Visible = true;
			saveButton.Text = "Добавить";
		}
				
		private void setStateSelect(int newSelectedPassangerId) {
			setStatus(false, null);

			curState = State.select;

			deleteButton.Enabled = true;
			editButton.Enabled = true;
			addButton.Enabled = true;
			passangerDataTable.Enabled = false;

			if(currentPassangerIndex != null) passangersDisplays[(int) currentPassangerIndex].BackColor = Color.White;
			currentPassangerIndex = newSelectedPassangerId;
			passangersDisplays[newSelectedPassangerId].BackColor = Misc.selectionColor3;
			
			setDataFromPassanger(customer.passangers[newSelectedPassangerId]);

			saveButton.Visible = false;
		}

		private void setStateEdit() {
			setStatus(false, null);

			if(status.booked) throw new InvalidOperationException();

			curState = State.edit;

			deleteButton.Enabled = true;
			editButton.Enabled = false;
			addButton.Enabled = true;
			passangerDataTable.Enabled = true;

			saveButton.Visible = true;

			var passanger = customer.passangers[(int) currentPassangerIndex];
			if(passanger.archived) saveButton.Text = "Сохранить копию";
			else saveButton.Text = "Сохранить";
		}

		public void setStateNone() {
			setStatus(false, null);

			curState = State.none;

			deleteButton.Enabled = false;
			editButton.Enabled = false;
			passangerDataTable.Enabled = false;

			if(currentPassangerIndex != null) passangersDisplays[(int) currentPassangerIndex].BackColor = Color.White;
			currentPassangerIndex = null;

			clearPassangerData();

			saveButton.Visible = false;
		}

		public PassangerList() {
			InitializeComponent();

			generalDataFields = new DocumentFields(() => this.ActiveControl = null, setStatus, generalDataTooltip, generalDataPanel, startFieldIndex: 0);
			documentFields = new DocumentFields(() => this.ActiveControl = null, setStatus, documentFieldsTooltip, documentTable, startFieldIndex: 1);
		}

		public void selectNone() {
			if(!promptSaveCustomer()) return;
			setStateNone();
		}

		public void init(MessageService sq, CustomerData customer, BookingStatus status, BookingPassanger passanger) {
			this.service = sq;
			this.status = status;
			this.customer = customer;
			this.passanger = passanger;
			this.passangersDisplays = new Dictionary<int, PassangerDisplay>();

			ignore__ = true;
			documentTypeCombobox.DataSource = new BindingSource{ DataSource = Documents.DocumentsName.documentsNames };
			documentTypeCombobox.DisplayMember = "Value";
			ignore__ = false;

			setupPassangersDisplay(passanger.passangerIndex);

			if(status.booked) updateStatusBooked();

			clearPassangerData();
		}

		public void updateStatusBooked() {
			deleteButton.Visible = false;
			deleteButton.Enabled = false;

			editButton.Visible = false;
			editButton.Enabled = false;

			addButton.Visible = false;
			addButton.Enabled = false;

			saveButton.Enabled = false;
			saveButton.Visible = false;

			passangerMenu.Enabled = false;

			passangersDisplay.Enabled = false;
			passangerDataTable.Enabled = false;
		}

		public bool onClose() {
			return promptSaveCustomer();
		}

		private void setupPassangersDisplay(int? defaultPassangerIndex) {
			passangersDisplay.SuspendLayout();
			passangersDisplay.Controls.Clear();
			passangersDisplay.RowStyles.Clear();

			passangersDisplay.RowCount = customer.passangers.Count;

			foreach(var passanger in customer.passangers) {
				var display = addPassangerDisplay(passanger.Value, passanger.Key);
				passangersDisplays.Add(passanger.Key, display);
			}

			passangersDisplay.ResumeLayout(false);
			passangersDisplay.PerformLayout();

			if(defaultPassangerIndex != null) setStateSelect((int) defaultPassangerIndex);
			else setStateNone();
		}

		private void promptFillCustomer(string msg = null) {
			var text = "Данные о пользователе должны быть заполнены корректно";
			statusLabel.Text = text;
			statusTooltip.SetToolTip(statusLabel, text);
		
			var lrf = new LoginRegisterForm(service, customer);
		
			lrf.ShowDialog();
			if(msg != null) lrf.setError(msg);
		
			if(lrf.DialogResult == DialogResult.OK) {
				statusLabel.Text = null;
				statusTooltip.SetToolTip(statusLabel, null);
			}
		}

		private void setStatus(bool error, string msg) {
			statusLabel.Text = error ? msg : null;
			statusLabel.ForeColor = error ? Color.Firebrick : SystemColors.ControlText;
			statusTooltip.SetToolTip(statusLabel, error ? msg : null);
		}

		//returns false if save aborted
		private bool promptSaveCustomer() {
			if(curState == State.edit) {
				var prevPassanger = customer.passangers[(int) currentPassangerIndex];
				var curPassangerRes = formPassangerFromData();

				if(!curPassangerRes.IsSuccess || !prevPassanger.Equals(curPassangerRes.s)) {
					this.Focus();
					var mb = MessageBox.Show(
						"Данные пассажира были изменены. Хотите сохранить изменения?",
						"",
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Warning,
						MessageBoxDefaultButton.Button3
					);

					if(mb == DialogResult.Yes) {
						if(!curPassangerRes.IsSuccess) {
							setStatus(true, curPassangerRes.f);
							return false;
						}
						var result = saveEditedPassanger(curPassangerRes.s, (int) currentPassangerIndex);
						return result != null;
					}
					else if(mb == DialogResult.No) return true;
					else {
						Debug.Assert(mb == DialogResult.Cancel);
						return false;
					}
				} 
				else return true;
			}
			else if(curState == State.add) {
				this.Focus();
				var curPassangerRes = formPassangerFromData();
				var mb = MessageBox.Show(
					"Сохранить данные нового пассажира?",
					"",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Warning,
					MessageBoxDefaultButton.Button3
				);

				if(mb == DialogResult.Yes) {
					if(!curPassangerRes.IsSuccess) {
						setStatus(true, curPassangerRes.f);
						return false;
					}

					var result = saveNewPassanger(curPassangerRes.s);
					if(result != null) setStateSelect((int) result);
					return result != null;
				}
				else if(mb == DialogResult.No) return true;
				else {
					Debug.Assert(mb == DialogResult.Cancel);
					return false;
				}
			}
			else return true;
		}

		private PassangerDisplay addPassangerDisplay(Passanger passanger, int index) {
			var it = new PassangerDisplay{ 
				Passanger = passanger, Data = index,
				Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
				Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
				ToolTip = passangerTooltip,
			};
			it.Click += (a, b) => {
				if(!promptSaveCustomer()) return;
				setStateSelect((int) ((PassangerDisplay) a).Data);
			};
			it.ContextMenuStrip = passangerMenu;

			passangersDisplay.RowCount++;
			passangersDisplay.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			passangersDisplay.Controls.Add(it);

			return it;
		}

		private void saveButton_Click(object sender, EventArgs eventArgs) {
			save();
		}

		private void save() {
			if(status.booked) throw new InvalidOperationException();

			var passangerRes = formPassangerFromData();
			if(!passangerRes.IsSuccess) {
				setStatus(true, passangerRes.f);
				return;
			}
			var passanger = passangerRes.s;

			if(curState == State.add) {
				var result = saveNewPassanger(passanger);
				if(result != null) setStateSelect((int) result); 
			}
			else if(curState == State.edit) {
				var oldPassanger = customer.passangers[(int) currentPassangerIndex];
				if(!oldPassanger.Equals(passanger)) {
					var result = saveEditedPassanger(passanger, (int) currentPassangerIndex);
					if(result != null) setStateSelect((int) result);
				}
			}
			else Debug.Assert(curState == State.select || curState == State.none);
		}

		private Either<Passanger, String> formPassangerFromData() { 
			var it = new Passanger{
				name = formPassanger.name,
				surname = formPassanger.surname,
				middleName = formPassanger.middleName,
				birthday = formPassanger.birthday,
				document = formPassanger.documents[formPassanger.selectedDocument]
			};

			var es = Validation.ErrorString.Create();

			if((it.name?.Length ?? 0) == 0 || (it.surname?.Length ?? 0) == 0) {
				es.ac("ФИО должно быть заполнено");
			}
			var docRes = it.document.validate();
			if(docRes.Error) {
				es.ac("Данные документа должны быть заполнены: ").append(docRes.Message);
			}

			if(es.Error) {
				return Either<Passanger, string>.Failure(es.Message);
			}
			else return Either<Passanger, string>.Success(it);
		}

		private void setDataFromPassanger(Passanger p) {
			formPassanger = new FormPassanger();
			formPassanger.name = p.name;
			formPassanger.surname = p.surname;
			formPassanger.middleName = p.middleName;
			formPassanger.birthday = p.birthday;
			formPassanger.documents = new Dictionary<int, Documents.Document>();

			formPassanger.selectedDocument = p.document.Id;
			formPassanger.documents.Clear();
			formPassanger.documents.Add(formPassanger.selectedDocument, p.document);

			updatePassanger();

			/*((TextBox) generalDataFields.getField(0)).Text = p.name;
			((TextBox) generalDataFields.getField(1)).Text = p.surname;
			((TextBox) generalDataFields.getField(2)).Text = p.middleName;
			((DateTimePicker) generalDataFields.getField(3)).Value = p.birthday;
            
			formPassanger.selectedDocument = p.document.Id;
			formPassanger.documents.Clear();
			formPassanger.documents.Add(Documents.Passport.id, p.document);
			
			var i = 0;
			foreach(var doc in Documents.DocumentsName.documentsNames) {
				if(doc.Key == p.document.Id) break;
				i++;
			}
			Debug.Assert(i != Documents.DocumentsName.documentsNames.Count);

			documentTypeCombobox.SelectedItem = i;*/
		} 

		private void clearPassangerData() {
			formPassanger = new FormPassanger();
			formPassanger.birthday = DateTime.Now;
			formPassanger.documents = new Dictionary<int, Documents.Document>();

			formPassanger.selectedDocument = Documents.Passport.id;
			formPassanger.documents.Clear();
			formPassanger.documents.Add(Documents.Passport.id, new Documents.Passport());

			updatePassanger();
		}

		private void updatePassanger() {
			generalDataFields.Suspend();

			generalDataFields.clear();

			generalDataFields.addField();
			generalDataFields.fieldName("Фамилия:*");
			generalDataFields.textField(formPassanger.surname, text => formPassanger.surname = text);

			generalDataFields.addField();
			generalDataFields.fieldName("Имя:*");
			generalDataFields.textField(formPassanger.name, text => formPassanger.name = text);
			
			generalDataFields.addField();
			generalDataFields.fieldName("Отчество:");
			generalDataFields.textField(formPassanger.middleName, text => formPassanger.middleName = text);

			generalDataFields.addField();
			generalDataFields.fieldName("Дата рождения:*");
			generalDataFields.dateField(formPassanger.birthday, date => formPassanger.birthday = date).Format = DateTimePickerFormat.Short;

			generalDataFields.Resume();


			var i = 0;
			foreach(var doc in Documents.DocumentsName.documentsNames) {
				if(doc.Key == formPassanger.selectedDocument) break;
				i++;
			}
			Debug.Assert(i != Documents.DocumentsName.documentsNames.Count);
			documentTypeCombobox.SelectedIndex = i;

			updateDocument(formPassanger.selectedDocument);

		}

		private void setErr(Exception e) { 
			statusLabel.Text = "Неизвестная ошибка";
			statusTooltip.SetToolTip(statusLabel, e.ToString());
		}

		private void setFine() {
			statusLabel.Text = "";
			statusTooltip.SetToolTip(statusLabel, null);
		}

		private int? saveNewPassanger(Passanger passanger) {
			if(status.booked) throw new InvalidOperationException();

			try {

			if(customer.LoggedIn) {
				var response = service.addPassanger(customer.Get(), passanger);
				if(response) { 
					var id = response.s;
					var index = customer.newPassangerIndex++;

					customer.passangers.Add(index, passanger);
					customer.passangerIds.Add(index, new PassangerIdData(id));

					var display = addPassangerDisplay(passanger, index);
					passangersDisplays.Add(index, display);
				
					setFine();
				
					return index;
				}
				else if(response.f.isLoginError) {
					promptFillCustomer(response.f.LoginError.message);
				}
				else {
					var msg = response.f.InputError.message;
					statusLabel.Text = msg;
					statusTooltip.SetToolTip(statusLabel, msg);
				}
			}
			else {
				var index = customer.newPassangerIndex++;

				customer.passangers.Add(index, passanger);
				customer.passangerIds.Add(index, new PassangerIdData(null));

				var display = addPassangerDisplay(passanger, index);
				passangersDisplays.Add(index, display);
				
				setFine();
				
				return index;
			}

			}catch(Exception e) { setErr(e); }

			return null;
		}

		private int? saveEditedPassanger(Passanger passanger, int index) {
			if(status.booked) throw new InvalidOperationException();

			try {
			var passangerIdData = customer.passangerIds[index];

			if(passangerIdData.IsLocal) {
				customer.passangers[index] = passanger;

				var passangerDisplay = passangersDisplays[index];
				passangerDisplay.Passanger = passanger;
				passangerDisplay.Data = index;

				return index;
			}
			else {
				if(!customer.LoggedIn) throw new InvalidOperationException(
					"Незарегестрированный пользователь не может менять данные пассажиров, зарегестрированных в базе"
				);
				//note: this is a potential bug
				//because button text is not updated
				//and in states that the copy will be saved
				//but actually the passanger itself is saved
				//and not ist copy if there is some error
				//in updating button text
				if(customer.passangers[index].archived) return saveNewPassanger(passanger);

				var response = service.replacePassanger(customer.Get(), passangerIdData.DatabaseId, passanger);
				if(response) { 
					var id = response.s;

					customer.passangers[index] = passanger;
					customer.passangerIds[index] = new PassangerIdData(id);

					var passangerDisplay = passangersDisplays[index];
					passangerDisplay.Passanger = passanger;
					passangerDisplay.Data = index;
					
					setFine();

					return index;
				}
				else if(response.f.isLoginError) {
					promptFillCustomer(response.f.LoginError.message);
				}
				else {
					var msg = response.f.InputError.message;
					statusLabel.Text = msg;
					statusTooltip.SetToolTip(statusLabel, msg);
				}
			}

			} catch(Exception e) { setErr(e); }
			return null;
		}

		private bool removePassanger(int index) {
			if(status.booked) throw new InvalidOperationException();

			try{
			var passangerIdData = customer.passangerIds[index];
			
			if(passangerIdData.IsLocal) {
				if(index == currentPassangerIndex) setStateNone();

				passangersDisplays[index].Dispose();
				passangersDisplays.Remove(index);

				customer.passangers.Remove(index);
				customer.passangerIds.Remove(index);
				return true;
			}
			else {
				if(!customer.LoggedIn) throw new InvalidOperationException(
					"Незарегестрированный пользователь не может удалять пассажиров, зарегестрированных в базе"
				);

				var response = service.removePassanger(customer.Get(), passangerIdData.DatabaseId);
				if(response) { 
					if(index == currentPassangerIndex) setStateNone();

					customer.passangers.Remove(index);
					customer.passangerIds.Remove(index);

					passangersDisplays[index].Dispose();
					passangersDisplays.Remove(index);

					setFine();

					return true;
				}
				else {
					if(response.f.isLoginError) promptFillCustomer(response.f.LoginError.message);
					else {
						var msg = response.f.InputError.message;
						statusLabel.Text = msg;
						statusTooltip.SetToolTip(statusLabel, msg);
					}
				}
			}
			} catch(Exception e) { setErr(e); }

			return false;
		}

		private void editButton_Click(object sender, EventArgs e) {
			setStatus(false, null);
			Debug.Assert(curState == State.select || curState == State.none);
			setStateEdit();
		}

		private void addButton_Click(object sender, EventArgs e) {
			setStatus(false, null);
			Debug.Assert(curState == State.edit || curState == State.select || curState == State.none);
			promptSaveCustomer();
			setStateAdd();
		}

		private void deleteButton_Click(object sender, EventArgs e) {
			setStatus(false, null);
			var result = MessageBox.Show(
				"Вы точно хотите удалить данного пассажира?", "",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2
			);
			if(result == DialogResult.Yes) removePassanger((int) currentPassangerIndex);
		}

		private void удалитьToolStripMenuItem_Click(object sender, EventArgs e) {
			var pass = (PassangerDisplay) passangerMenu.SourceControl;
			var number = (int) pass.Data;
			var result = MessageBox.Show(
				"Вы точно хотите удалить данного пассажира?", "",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2
			);
			if(result == DialogResult.Yes) removePassanger(number);
		}

		private void изменитьToolStripMenuItem_Click(object sender, EventArgs e) {
			var pass = (PassangerDisplay) passangerMenu.SourceControl;
			var number = (int) pass.Data;
			if(!promptSaveCustomer()) return;
			setStateSelect(number);
			setStateEdit();
		}

		private class DocumentFields {
			private int startFieldIndex;

			private int fieldIndex;

			private TableLayoutPanel panel;
			private SetStatus setStatus;
			private ToolTip documentFieldsTooltip;
			private RemoveFocus removeFocus;

			private List<Control> addedControls;

			private List<Control> fields;

			public delegate void RemoveFocus();
			public delegate void SetStatus(bool err, string input);
			public delegate void ValidateText(string input);
			public delegate void ValidateDate(DateTime input);

			public Control getField(int index) {
				return fields[index];
			}

			public DocumentFields(
				RemoveFocus removeFocus,
				SetStatus setStatus,
				ToolTip documentFieldsTooltip,
				TableLayoutPanel panel,
				int startFieldIndex
			) {
				this.removeFocus = removeFocus;
				this.fieldIndex = (this.startFieldIndex = startFieldIndex) - 1;
				this.fields = new List<Control>();
				this.setStatus = setStatus;
				this.documentFieldsTooltip = documentFieldsTooltip;
				this.panel = panel;
				addedControls = new List<Control>();
			}

			public void addField() {
				fieldIndex++;
				if(fieldIndex != 0 && fieldIndex % 3 == 0) {
					panel.RowCount += 3;
					panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
					panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
					panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
				}
			}

			public Label fieldName(string text) {
				var it = new Label();

				it.Dock = DockStyle.Fill;
				it.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
				it.Text = text;
				it.TextAlign = ContentAlignment.BottomLeft;
				it.AutoSize = true;
				documentFieldsTooltip.SetToolTip(it, text);

				panel.Controls.Add(it, fieldIndex%3, fieldIndex/3 * 3 + 1);
				addedControls.Add(it);
				return it;
			}

			public TextBox textField(string defaultValue, ValidateText validate) {
				var it = new TextBox();
				it.Dock = DockStyle.Fill;
				it.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
				it.Text = defaultValue;
				it.KeyDown += (a, b) => { if(b.KeyCode == Keys.Enter) { validateTextField(it, validate); removeFocus(); } };
				it.LostFocus += (a, b) => { validateTextField(it, validate); };
				addedControls.Add(it);
				fields.Add(it);
				panel.Controls.Add(it, fieldIndex%3, fieldIndex/3 * 3 + 2);
				return it;
			}

			private void validateTextField(TextBox it, ValidateText validate) {
				try{ 
					validate(it.Text);
					it.ForeColor = SystemColors.ControlText;
					documentFieldsTooltip.SetToolTip(it, null);
					setStatus(false, null);
				}
				catch(Documents.IncorrectValue iv) {
					it.ForeColor = Color.Firebrick;
					documentFieldsTooltip.SetToolTip(it, iv.Message);
					setStatus(true, iv.Message);
				}
			}

			public DateTimePicker dateField(DateTime defaultValue, ValidateDate validate) {
				var it = new DateTimePicker();
				it.Dock = DockStyle.Fill;
				it.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
				it.Value = defaultValue;
				it.KeyDown += (a, b) => { if(b.KeyCode == Keys.Enter) { validateDateField(it, validate); removeFocus(); } };
				it.LostFocus += (a, b) => { validateDateField(it, validate); };
				addedControls.Add(it);
				fields.Add(it);
				panel.Controls.Add(it, fieldIndex%3, fieldIndex/3 * 3 + 2);
				return it;
			}

			private void validateDateField(DateTimePicker it, ValidateDate validate) {
				try{ 
					validate(it.Value);
					it.ForeColor = SystemColors.ControlText;
					documentFieldsTooltip.SetToolTip(it, null);
					setStatus(false, null);
				}
				catch(Documents.IncorrectValue iv) {
					it.ForeColor = Color.Firebrick;
					documentFieldsTooltip.SetToolTip(it, iv.Message);
					setStatus(true, iv.Message);
				}
			}

			public void clear() {
				this.fieldIndex = this.startFieldIndex - 1;
				documentFieldsTooltip.RemoveAll();
				foreach(var it in addedControls) it.Dispose();
				addedControls.Clear();
				fields.Clear();
				panel.RowCount = 2;
			}

			public void Suspend() {
				panel.SuspendLayout();
			}

			public void Resume() {
				panel.ResumeLayout(false);
				panel.PerformLayout();
			}
		}

		
		private bool ignore__;
		private void documentTypeCombobox_SelectedIndexChanged(object sender, EventArgs e) {
			if(ignore__) return;
			updateDocument(((KeyValuePair<int, string>) documentTypeCombobox.SelectedItem).Key);
		}

		private void updateDocument(int documentId) {
			Documents.Document document;
			formPassanger.selectedDocument = documentId;
			if(!formPassanger.documents.TryGetValue(documentId, out document)) {
				if(documentId == Documents.Passport.id) { 
					document = new Documents.Passport();
				}
				else if(documentId == Documents.InternationalPassport.id) {
					document = new Documents.InternationalPassport();
				}

				Debug.Assert(document != null);
				formPassanger.documents.Add(documentId, document);
			}

			documentFields.Suspend();
			documentFields.clear();

			if(documentId == Documents.Passport.id) {
				var passport = (Documents.Passport) document;

				documentFields.addField();
				documentFields.fieldName("Номер:*");
				documentFields.textField(passport.Number?.ToString(), text => passport.setNumber(text));
			}
			else if(documentId == Documents.InternationalPassport.id) {
				var passport = (Documents.InternationalPassport) document;

				documentFields.addField();
				documentFields.fieldName("Номер:*");
				documentFields.textField(passport.Number?.ToString(), text => passport.setNumber(text));

				documentFields.addField();
				documentFields.fieldName("Дата окончания срока действия:*");
				var exDate = passport.ExpirationDate ?? DateTime.Now;
				documentFields.dateField(exDate, date => passport.ExpirationDate = date)
					.Format = DateTimePickerFormat.Short;
				passport.ExpirationDate = exDate;

				documentFields.addField();
				documentFields.fieldName("Фамилия (на латинице):*");
				documentFields.textField(passport.Surname, text => passport.Surname = text);

				documentFields.addField();
				documentFields.fieldName("Имя (на латинице):*");
				documentFields.textField(passport.Name, text => passport.Name = text);

				documentFields.addField();
				documentFields.fieldName("Отчество (на латинице):");
				documentFields.textField(passport.MiddleName, text => passport.MiddleName = text);
			}

			documentFields.Resume();
		}
	}
}
