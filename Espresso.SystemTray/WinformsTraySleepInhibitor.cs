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
using System.Drawing;
using System.Windows.Forms;

namespace Espresso.SystemTray
{
	sealed class WinformsTraySleepInhibitor : IGraphicalSleepInhibitor
	{
		static WinformsTraySleepInhibitor()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
		}

		private readonly NotifyIcon _trayIcon = new NotifyIcon();
		private readonly ISleepInhibitor _sleepInhibitor = SleepInhibitor.CreateNew();

		private readonly Icon _emptyCupIcon = new Icon(Icons.EmptyCupIconFile);
		private readonly Icon _fullCupIcon = new Icon(Icons.FullCupIconFile);

		public WinformsTraySleepInhibitor()
		{
			_trayIcon.Icon = _emptyCupIcon;
			_trayIcon.ContextMenuStrip = BuildMenu();
			_trayIcon.MouseClick += TrayIcon_MouseClick;
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
				_trayIcon.Icon = value ? _fullCupIcon : _emptyCupIcon;
			}
		}

		void TrayIcon_MouseClick(Object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				IsInhibited = !IsInhibited;
			}
		}

		ContextMenuStrip BuildMenu()
		{
			ContextMenuStrip menu = new ContextMenuStrip();

			ToolStripItem quitItem = new ToolStripMenuItem();
			quitItem.Text = "Quit";
			//quitItem.Image
			quitItem.Click += (Object sender, EventArgs e) => Application.Exit();
			menu.Items.Add(quitItem);

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
				_emptyCupIcon.Dispose();
				_fullCupIcon.Dispose();
				_sleepInhibitor.Dispose();
				GC.SuppressFinalize(this);
			}
			catch { }
		}
	}
}
