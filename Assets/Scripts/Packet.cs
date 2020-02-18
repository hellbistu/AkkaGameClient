using System;
using System.IO;

public class Packet
{
        private short ptype;
        private int length;
        private byte[] pdata;

        public Packet(short ptype, byte[] pdata)
        {
                this.ptype = ptype;
                this.length = pdata.Length;
                this.pdata = pdata;
        }

        public short GetPtype()
        {
                return this.ptype;
        }
        
        public int GetLength()
        {
                return this.length;
        }
        
        public byte[] GetPdata()
        {
                return this.pdata;
        }

        public byte[] Marshal()
        {
                byte[] ptypeBytres = BitConverter.GetBytes(this.ptype);
                byte[] lengthBytres = BitConverter.GetBytes(this.length);
                
                MemoryStream ms = new MemoryStream();
                ms.Write(ptypeBytres,0,ptypeBytres.Length);
                ms.Write(lengthBytres,0,lengthBytres.Length);
                ms.Write(pdata,0,pdata.Length);
                
                return ms.ToArray();
        }
}