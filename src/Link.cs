/*
 * File: Link.cs
 * Project: src
 * Created Date: 28/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 01/10/2022
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Shun Suzuki. All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace AUTD3Sharp
{
    [ComVisible(false)]
    public class Link : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal IntPtr LinkPtr => handle;

        internal Link(IntPtr handle) : base(false)
        {
            SetHandle(handle);
        }

        protected override bool ReleaseHandle() => true;
    }

    public class SOEM
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)] public delegate void OnLostCallbackDelegate(string str);

        private string? _ifname;
        private ushort _sendCycle;
        private ushort _sync0Cycle;
        private bool _freerun;
        private bool _highPrecision;
        private Action<string>? _onLost;

        public SOEM()
        {
            _ifname = null;
            _sendCycle = 1;
            _sync0Cycle = 1;
            _freerun = false;
            _highPrecision = false;
            _onLost = null;
        }

        public SOEM Ifname(string ifname)
        {
            _ifname = ifname;
            return this;
        }

        public SOEM SendCycle(ushort sendCycle)
        {
            _sendCycle = sendCycle;
            return this;
        }

        public SOEM Sync0Cycle(ushort sync0Cycle)
        {
            _sync0Cycle = sync0Cycle;
            return this;
        }

        public SOEM FreeRun(bool freerun)
        {
            _freerun = freerun;
            return this;
        }

        public SOEM HighPrecision(bool highPrecision)
        {
            _highPrecision = highPrecision;
            return this;
        }

        public SOEM OnLost(Action<string> onLost)
        {
            _onLost = onLost;
            return this;
        }

        public Link Build()
        {
            IntPtr onLostHandler;
            if (_onLost is null)
            {
                onLostHandler = IntPtr.Zero;
            }
            else
            {
                var callback = new OnLostCallbackDelegate(_onLost);
                onLostHandler = Marshal.GetFunctionPointerForDelegate(callback);
            }
            NativeMethods.LinkSOEM.AUTDLinkSOEM(out var handle, _ifname, _sync0Cycle, _sendCycle, _freerun, onLostHandler, _highPrecision);
            return new Link(handle);
        }

        public static IEnumerable<EtherCATAdapter> EnumerateAdapters()
        {
            var size = NativeMethods.LinkSOEM.AUTDGetAdapterPointer(out var handle);
            for (var i = 0; i < size; i++)
            {
                var sbDesc = new StringBuilder(128);
                var sbName = new StringBuilder(128);
                NativeMethods.LinkSOEM.AUTDGetAdapter(handle, i, sbDesc, sbName);
                yield return new EtherCATAdapter(sbDesc.ToString(), sbName.ToString());
            }
            NativeMethods.LinkSOEM.AUTDFreeAdapterPointer(handle);
        }
    }

    public sealed class TwinCAT
    {
        public TwinCAT()
        {
        }

        public Link Build()
        {
            NativeMethods.LinkTwinCAT.AUTDLinkTwinCAT(out var handle);
            return new Link(handle);
        }
    }


    public sealed class RemoteTwinCAT
    {
        private string _serverIp;
        private readonly string _serverAmsNetId;
        private string _clientAmsNetId;

        public RemoteTwinCAT(string serverAmsNetId)
        {
            _serverAmsNetId = serverAmsNetId;
            _clientAmsNetId = string.Empty;
            _serverIp = string.Empty;
        }

        public RemoteTwinCAT ServerIp(string serverIp)
        {
            _serverIp = serverIp;
            return this;
        }

        public RemoteTwinCAT LocalAmsNetId(string localAmsNetId)
        {
            _clientAmsNetId = localAmsNetId;
            return this;
        }

        public Link Build()
        {
            NativeMethods.LinkRemoteTwinCAT.AUTDLinkRemoteTwinCAT(out var handle, _serverIp, _serverAmsNetId, _clientAmsNetId);
            return new Link(handle);
        }
    }

    public sealed class Emulator
    {
        private ushort _port;

        public Emulator()
        {
            _port = 50632;
        }

        public Emulator Port(ushort port)
        {
            _port = port;
            return this;
        }

        public Link Build()
        {
            NativeMethods.LinkEmulator.AUTDLinkEmulator(out var handle, _port);
            return new Link(handle);
        }
    }

    public readonly struct EtherCATAdapter : IEquatable<EtherCATAdapter>
    {
        public string Desc { get; }
        public string Name { get; }

        internal EtherCATAdapter(string desc, string name)
        {
            Desc = desc;
            Name = name;
        }

        public override string ToString() => $"{Desc}, {Name}";
        public bool Equals(EtherCATAdapter other) => Desc.Equals(other.Desc) && Name.Equals(other.Name);
        public static bool operator ==(EtherCATAdapter left, EtherCATAdapter right) => left.Equals(right);
        public static bool operator !=(EtherCATAdapter left, EtherCATAdapter right) => !left.Equals(right);
        public override bool Equals(object? obj) => obj is EtherCATAdapter adapter && Equals(adapter);
        public override int GetHashCode() => Desc.GetHashCode() ^ Name.GetHashCode();
    }
}
