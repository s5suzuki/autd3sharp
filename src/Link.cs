/*
 * File: Link.cs
 * Project: src
 * Created Date: 28/04/2021
 * Author: Shun Suzuki
 * -----
 * Last Modified: 28/04/2021
 * Modified By: Shun Suzuki (suzuki@hapis.k.u-tokyo.ac.jp)
 * -----
 * Copyright (c) 2021 Hapis Lab. All rights reserved.
 * 
 */

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace AUTD3Sharp
{
    [ComVisible(false)]
    public class Link : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal IntPtr LinkPtr => handle;

        public Link(IntPtr handle) : base(false)
        {
            SetHandle(handle);
        }

        protected override bool ReleaseHandle() => true;

        public static Link SOEMLink(string ifname, int deviceNum)
        {
            NativeMethods.AUTDSOEMLink(out var link, ifname, deviceNum);
            return new Link(link);
        }

        public static Link EtherCATLink(string ip4Addr, string amsNetId)
        {
            NativeMethods.AUTDTwinCATLink(out var link, ip4Addr, amsNetId);
            return new Link(link);
        }

        public static Link LocalEtherCATLink()
        {
            NativeMethods.AUTDLocalTwinCATLink(out var link);
            return new Link(link);
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
