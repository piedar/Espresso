//
//  This file is part of Espresso <https://github.com/piedar/Espresso>.
//
//  Author(s):
//        Bennjamin Blast <bennjamin.blast@gmail.com>
//
//  Copyright (c) 2016 Bennjamin Blast
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
using System;
using System.IO;
using Gdk;
using Gtk;

namespace Espresso.SystemTray
{
	sealed class GtkTraySleepInhibitor : ISleepInhibitor
	{
		static GtkTraySleepInhibitor()
		{
			Application.Init();
		}

		private readonly StatusIcon _trayIcon = new StatusIcon();
		private readonly ISleepInhibitor _sleepInhibitor = SleepInhibitor.CreateNew();

		public GtkTraySleepInhibitor()
		{
			_trayIcon.File = Icons.EmptyCupFile;
			_trayIcon.Activate += TrayIcon_Activate;
			_trayIcon.PopupMenu += TrayIcon_PopupMenu;
			_trayIcon.Visible = true;
		}

		public Boolean IsInhibited
		{
			get
			{
				return _sleepInhibitor.IsInhibited;
			}
			set
			{
				_sleepInhibitor.IsInhibited = value;
				_trayIcon.File = value ? Icons.FullCupFile : Icons.EmptyCupFile;
			}
		}


		void TrayIcon_Activate(Object sender, EventArgs e)
		{
			IsInhibited = !IsInhibited;
		}

		void TrayIcon_PopupMenu(Object sender, PopupMenuArgs e)
		{
			Menu popup = BuildMenu();
			popup.ShowAll();
			popup.Popup();
		}

		Menu BuildMenu()
		{
			Menu menu = new Menu();

			ImageMenuItem quitItem = new ImageMenuItem("Quit");
			quitItem.Image = new Image(Stock.Quit, IconSize.Menu);
			quitItem.Activated += (Object sender, EventArgs e) => Application.Quit();
			menu.Add(quitItem);

			return menu;
		}

		public void Run()
		{
			Application.Run();
		}

		public void Dispose()
		{
			try
			{
				_trayIcon.Dispose();
				_sleepInhibitor.Dispose();
				GC.SuppressFinalize(this);
			}
			catch { }
		}
	}
}
