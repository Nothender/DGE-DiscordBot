using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DGE
{
    public class DGEVersion : IVersion
    {

        //TODO: Probably would need to reprogram this, to make it more logical and cleaner, overlapping of variables/functions for different stuff could cause bugs - (trash code in there)

        public string name = "DGE";
        public string version 
        { 
            get { return s_version; } 
            set 
            {
                s_version = ""; //Only taking the numbers and the points of a version string (ex : "[DGE 0.1.3.42]" transforms into "0.1.3.42")
                int v_count = 0; //The number of subversions
                foreach (char c in value)
                {
                    if (int.TryParse(c.ToString(), out int _) || c == '.')
                        s_version += c;
                    if (c == '.')
                    {
                        v_count++;
                        if (v_count >= 4) //If they're are more than 4 versions we cut so we get a DGEVersion
                            break;
                    }
                }
                i_version = Array.ConvertAll(s_version.Split('.'), s => int.TryParse(s, out int x) ? x : 0);
            }
        }
        protected string s_version;
        protected int[] i_version;

        public DGEVersion(string version)
        {
            version = version.Trim();
            if (version.Contains(' '))
                name = version.Trim(new char[2] { '[', ']' }).Split(' ')[0].Trim();
            this.version = version;
        }

        public static DGEVersion FromString(string version)
        {
            return new DGEVersion(version);
        }

        public bool IsNewer(IVersion other)
        {
            if (other is DGEVersion)
            {
                DGEVersion o = other as DGEVersion;
                for (int i = 0; i < 4; i++)
                {
                    if (i_version[i] > o.i_version[i])
                        return true;
                    else if (i_version[i] < o.i_version[i])
                        return false;
                }
                return false; // If they're equal
            }
            throw new Exception($"Cannot compare a DGE version with a {other.GetType().Name}");
        }

        public override string ToString()
        {
            return $"[{name} {s_version}]";
        }

        private static string IntArrayToVersionString(int[] versionArray)
        {
            return string.Join('.', Array.ConvertAll(versionArray, i => i.ToString()));
        }

        public static DGEVersion operator +(DGEVersion version, DGEVersion other)
        {
            int[] v = new int[4]; version.i_version.CopyTo(v, 0);
            for (int i = 0; i < 4; i++)
                v[i] += other.i_version[i];

            return FromString(IntArrayToVersionString(v));
        }

    }
}
