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
using System.Diagnostics;

namespace Espresso
{
	/// <summary>
	/// A SleepInhibitor prevents the computer from automatically going to sleep.
	/// </summary>
	public sealed class SleepInhibitor : ISleepInhibitor, IDisposable
	{
		private readonly ISleepInhibitor _backend;

		/// <summary>
		/// Creates a new <see cref="SleepInhibitor"/> with the specified <paramref name="backend"/>.
		/// </summary>
		/// <param name="backend"></param>
		public SleepInhibitor(ISleepInhibitor backend)
		{
			if (backend == null) throw new ArgumentNullException("backend");
			_backend = backend;
		}

		/// <summary>
		/// Creates a new <see cref="SleepInhibitor"/> with an automatically-chosen backend.
		/// </summary>
		public SleepInhibitor()
			: this(GetPlatformSleepInhibitor()) { }

		/// <summary>
		/// Creates and activates a new <see cref="SleepInhibitor"/> with an automatically-chosen backend.
		/// </summary>
		public static SleepInhibitor StartNew()
		{
			SleepInhibitor inhibitor = new SleepInhibitor();
			inhibitor.IsInhibited = true;
			return inhibitor;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SleepInhibitor"/> is inhibited.
		/// </summary>
		public Boolean IsInhibited
		{
			get { return _backend.IsInhibited; }
			set
			{
				if (value ^ IsInhibited)
				{
					_backend.IsInhibited = value;
				}
			}
		}

		~SleepInhibitor()
		{
			Dispose();
		}

		public void Dispose()
		{
			try
			{
				IsInhibited = false;
				GC.SuppressFinalize(this);
			}
			catch { }
		}

		internal static ISleepInhibitor GetPlatformSleepInhibitor()
		{
			try
			{
				return new Win32SleepInhibitor();
			}
			catch (TypeLoadException ex)
			{
				Debug.WriteLine(String.Format("Failed to load Win32SleepInhibitor because '{0}'", ex));
				return new DBusSleepInhibitor();
			}
		}
	}
}
