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
using DBus;

namespace Espresso
{
	delegate void HasInhibitChangedHandler(Boolean hasInhibit);

	[Interface("org.freedesktop.PowerManagement.Inhibit")]
	interface IPowerManagementInhibit
	{
		Boolean HasInhibit();
		event HasInhibitChangedHandler HasInhibitChanged;

		uint Inhibit(String application, String reason);
		void UnInhibit(UInt32 cookie);
	}

	class DBusSleepInhibitor : ISleepInhibitor
	{
		private readonly IPowerManagementInhibit _pmInhibit;

		public DBusSleepInhibitor()
		{
			ObjectPath path = new ObjectPath("/org/freedesktop/PowerManagement/Inhibit");
			_pmInhibit = Bus.Session.GetObject<IPowerManagementInhibit>("org.freedesktop.PowerManagement", path);
		}

		private UInt32? _cookie;

		public Boolean IsInhibited
		{
			get
			{
				if (_cookie.HasValue && !_pmInhibit.HasInhibit())
				{
					throw new SleepInhibitorException("DBusSleepInhibitor has an inhibit cookie, but HasInhibit() returned false.");
				}
				return _cookie.HasValue;
			}
			set
			{
				if (value)
				{
					_cookie = _pmInhibit.Inhibit("Espresso", "Espresso keeps the computer awake.");
					if (_cookie.HasValue && !_pmInhibit.HasInhibit())
					{
						throw new SleepInhibitorException("DBusSleepInhibitor has an inhibit cookie, but HasInhibit() returned false.");
					}
				}
				else
				{
					_pmInhibit.UnInhibit(_cookie.Value);
					_cookie = null;
				}
			}
		}
	}
}

