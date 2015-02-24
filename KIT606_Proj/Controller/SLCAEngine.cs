using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KIT606_Proj.Controller {
    class SLCAEngine : CAEngine{
        private List<Item2> SLCAs = new List<Item2>();

        public void getSLCA(bool fwd) {
            cursors = new int[Ls2.Count];
            SLCAs.Clear();

            if (fwd) {
                fwdSLCA();
            } else {
                for (int i = 0; i < Ls2.Count; i++) {
                    cursors[i] = Ls2[i].Count - 1;
                }
                    bwdSLCA();
            }
        }

        private void bwdSLCA() {
            for (int i = 0; i < Ls2.Count; i++) {
                cursors[i] = Ls2[i].Count - 1;
            }
            int u = -1;
            Item2 v = null;
            while (!bwdEof()) {
                if (bwdGetCA(out v)) {
                    if(u != Ls2[0].IndexOf(v)) SLCAs.Add(v);
                    u = v.PIDPos;
                } else break;
                bwdAdvance();
            }
        }

        private void fwdSLCA() {
            int u = -1;
            Item2 v = null;
            while (!fwdEof()) {
                if (fwdGetCA(out v)) {
                    if (u != v.PIDPos) SLCAs.Add(Ls2[0][u]);
                    u = Ls2[0].IndexOf(v);
                    fwdAdvance();
                } else {
                    SLCAs.Add(Ls2[0][u]);
                    break;
                }
            }
        }

        public string printSLCAs(bool isConsole) {
            StringBuilder sb = new StringBuilder();
            sb.Append("SLCAs for keywords {");
            foreach (string keyword in keywords)
            {
                sb.Append(keyword);
                sb.Append(",");
            }
            sb.Replace(',', '}', sb.Length - 1, 1);
            sb.Append("\r\n");
            for (int i = 0; i < SLCAs.Count; i++)
            {
                Item2 item = SLCAs[i];
                sb.Append("[ID:");
                sb.Append(item.ID);
                sb.Append(", Name:");
                sb.Append(item.name);
                sb.Append(", PIDPos:");
                sb.Append(item.PIDPos);
                sb.Append(", Ndesc:");
                sb.Append(item.Ndesc);
                sb.Append(", Value:");
                sb.Append(item.value == null ? "NULL" : item.value);
                sb.Append("]");
                sb.Append("\r\n");
            }
            sb.Append("# of SLCAs:");
            sb.Append(SLCAs.Count);
            sb.Append("\r\n");
            sb.Append("------------------------------------------------------------------------------------\r\n");
            if (isConsole)
                Console.Write(sb.ToString());
            else
                return sb.ToString();
            return null;
        }




    }
}

