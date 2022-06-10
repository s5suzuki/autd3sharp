/*
 * File: Link.cs
 * Project: src
 * Created Date: 28/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 10/06/2022
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
    public abstract class Link : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal IntPtr LinkPtr => handle;

        internal Link() : base(false)
        {
            handle = new IntPtr();
            SetHandle(handle);
        }

        protected override bool ReleaseHandle() => true;
    }

    public sealed class SOEM : Link
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)] public delegate void OnLostCallbackDelegate(string str);

        public SOEM(string ifname, int deviceNum, ushort cycleTicks = 2, Action<string>? onLost = null, bool highPresicion = true) : base()
        {
            IntPtr onLostHander;
            if (onLost is null)
            {
                onLostHander = IntPtr.Zero;
            }
            else
            {
                var callback = new OnLostCallbackDelegate(onLost);
                onLostHander = Marshal.GetFunctionPointerForDelegate(callback);
            }
            NativeMethods.LinkSOEM.AUTDLinkSOEM(out handle, ifname, deviceNum, cycleTicks, onLostHander, highPresicion);
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

    public sealed class TwinCAT : Link
    {
        public TwinCAT() : base()
        {
            NativeMethods.LinkTwinCAT.AUTDLinkTwinCAT(out handle);
        }
    }


    public sealed class RemoteTwinCAT : Link
    {
        public RemoteTwinCAT(string remoteIp, string remoteAmsNetId, string localAmsNetId) : base()
        {
            NativeMethods.LinkRemoteTwinCAT.AUTDLinkRemoteTwinCAT(out handle, remoteIp, remoteAmsNetId, localAmsNetId);
        }
    }

    public sealed class Emulator : Link
    {
        public Emulator(ushort port, Controller autd) : base()
        {
            NativeMethods.LinkEmulator.AUTDLinkEmulator(out handle, port, autd.AUTDControllerHandle.CntPtr);
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
