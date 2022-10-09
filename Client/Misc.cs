﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client {
	static class Misc {
		public static Control addDummyButton(Control it) {
			/*
				HACK: for some reason first (or maybe last) programmatically added  focusable control receives focus
				automatically and it cannot be removed by setting ActiveControl to null
				so this dummy button is added in order for these contros to lose their focus.
			*/
			var name = "HACK_Client_Misc_addDummyButton";
			foreach(var button in it.Controls.Find(name, false)) button.Dispose();
			
			var dummy = new Button();
			dummy.Name = name;
			dummy.Size = new Size(0, 0);
			it.Controls.Add(dummy);

			return dummy;
		}

		//https://stackoverflow.com/a/3526775/18704284
		public static void unfocusOnEscape(Form form, KeyEventHandler inner = null) {
			form.KeyPreview = true;

			form.KeyDown += (a, e) => {
				if(e.KeyCode == Keys.Escape) {
					form.ActiveControl = null;
					e.Handled = true;
				}
				else {
					e.Handled = false;
					inner?.Invoke(a, e);
				}
			};
		}
	}
}