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
using System.Runtime.InteropServices;

namespace Espresso
{
	sealed class Win32SleepInhibitor : ISleepInhibitor
	{
		public Win32SleepInhibitor()
		{
			Marshal.PrelinkAll(typeof(Kernel32)); // Force a TypeLoadException if kernel32.dll is not available.
		}

		private Kernel32.EXECUTION_STATE _state = 0;

		public Boolean IsInhibited
		{
			get { return FlagHelper.IsFlagSet(_state, Kernel32.EXECUTION_STATE.ES_SYSTEM_REQUIRED); }
			set
			{
				Kernel32.EXECUTION_STATE targetState = Kernel32.EXECUTION_STATE.ES_CONTINUOUS;

				if (value)
				{
					targetState |= Kernel32.EXECUTION_STATE.ES_SYSTEM_REQUIRED;
					//if (ControlDisplay)
					//{
					//	targetState |= Kernel32.EXECUTION_STATE.ES_DISPLAY_REQUIRED;
					//}
				}

				_state = Kernel32.SetThreadExecutionState(targetState);
				if (!FlagHelper.IsFlagSet(_state, targetState))
				{
					throw new SleepInhibitorException(String.Format("Call to SetThreadExecutionState() failed to set targetState '{0}'.", targetState));
				}
			}
		}
	}

	static class FlagHelper
	{
		public static Boolean IsFlagSet<T>(T value, T flag) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enum type.");

			return (value.ToInt64(null) & flag.ToInt64(null)) != 0;
		}
	}

	static class Kernel32
	{
		// http://msdn.microsoft.com/en-us/library/windows/desktop/aa373208(v=vs.85).aspx
		[Flags]
		public enum EXECUTION_STATE : uint
		{
			ES_AWAYMODE_REQUIRED = 0x00000040,
			ES_CONTINUOUS = 0x80000000,
			ES_DISPLAY_REQUIRED = 0x00000002,
			ES_SYSTEM_REQUIRED = 0x00000001,
			[Obsolete("Legacy flag, should not be used.")]
			ES_USER_PRESENT   = 0x00000004,
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
	}
}

