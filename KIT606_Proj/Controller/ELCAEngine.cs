using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KIT606_Proj.Controller {
    class ELCAEngine : CAEngine {
        private List<Item2> ELCAs = new List<Item2>();
        private Stack<int[]> S = new Stack<int[]>();

        public void getELCA(bool fwd) {
            cursors = new int[Ls2.Count];
            ELCAs.Clear();

            if (fwd) {
                fwdELCA();
            } else {
                for (int i = 0; i < Ls2.Count; i++) {
                    cursors[i] = Ls2[i].Count - 1;
                }
                bwdELCA();
            }

        }

        public void testELCA(bool fwd) {
            if (fwd) fwdELCA();
            else {
                for (int i = 0; i < Ls2.Count; i++) {
                    cursors[i] = Ls2[i].Count - 1;
                }
                bwdELCA();
            }
        }


        private void fwdELCA() {
            Item2 v = null;
            int[] w;
            while (!fwdEof()) {
                if (fwdGetCA(out v)) {
                    while (S.Count > 0 && S.Peek()[0] != v.PIDPos) {
                        w = S.Pop();
                        modTop(w);
                        if (isELCA(w)) ELCAs.Add(Ls2[0][w[0]]);
                    }

                    S.Push((int[])cursors.Clone());
                }
                fwdAdvance();
            }
            while (S.Count > 0) {
                w = S.Pop();
                modTop(w);
                if (isELCA(w)) ELCAs.Add(Ls2[0][w[0]]);
            }

        }

        public string printELCAs(bool isConsole)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ELCAs for keywords {");
            foreach (string keyword in keywords)
            {
                sb.Append(keyword);
                sb.Append(",");
            }
            sb.Replace(',', '}', sb.Length - 1, 1);
            sb.Append("\r\n");
            for (int i = 0; i < ELCAs.Count; i++)
            {
                Item2 item = ELCAs[i];
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
            sb.Append("# of ELCAs:");
            sb.Append(ELCAs.Count);
            sb.Append("\r\n");
            sb.Append("------------------------------------------------------------------------------------\r\n");
            if (isConsole)
                Console.Write(sb.ToString());
            else
                return sb.ToString();
            return null;
        }

        private bool isELCA(int[] w) {
            for (int i = 0; i < Ls2.Count; i++) {
                if (Ls2[i][w[i]].n == Ls2[i][w[i]].Ndesc) return false;
            }
            return true;
        }

        private void modTop(int[] w) {
            if (S.Count == 0) return;
            int[] pos = S.Peek();
            for (int i = 0; i < Ls2.Count; i++) {
                Ls2[i][pos[i]].n += Ls2[i][w[i]].Ndesc;
            }
        }

        private void bwdELCA() {
            Item2 v = null;
            int[] w;
            while (!bwdEof()) {
                if (bwdGetCA(out v)) {
                    if (S.Count == 0) {
                        ELCAs.Add(v);
                        if (v.PIDPos == -1) break;
                        pushParent();
                    } else {
                        if (v.ID == Ls2[0][S.Peek()[0]].ID) {
                            Console.WriteLine("==");
                            w = S.Pop();
                            if (isELCA(w)) ELCAs.Add(Ls2[0][w[0]]);
                            if (S.Count == 0) {
                                if (v.PIDPos == -1) break;
                                else pushParent();
                            } else if (Ls2[0][v.PIDPos].ID == Ls2[0][S.Peek()[0]].ID) {
                                modTop(cursors);
                            } else pushParent();
                        } else {
                            Console.WriteLine("!=");
                            ELCAs.Add(v);
                            if (Ls2[0][v.PIDPos].ID == Ls2[0][S.Peek()[0]].ID) {
                                modTop(cursors);
                            } else pushParent();
                        }
                    }
                } else break;
                bwdAdvance();
            }
        }

        private void pushParent() {
            int[] p = new int[Ls2.Count];
            for (int i = 0; i < Ls2.Count; i++) {
                Ls2[i][Ls2[i][cursors[i]].PIDPos].n = Ls2[i][cursors[i]].Ndesc;
                p[i] = Ls2[i][cursors[i]].PIDPos;
            }
            S.Push(p);
        }
    }
}
