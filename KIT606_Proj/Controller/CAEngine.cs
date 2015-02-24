using KIT606_Proj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace KIT606_Proj.Controller {
    class CAEngine {
        public List<List<Item2>> Ls2;
        public List<String> keywords;
        public int[] cursors;

        public void printLs2()
        {
            for (int i = 0; i < Ls2.Count; i++)
            {
                Console.Write(String.Format("{0}:",keywords[i]));
                for (int j = 0; j < Ls2[i].Count; j++)
                {
                    Item2 item2 = Ls2[i][j];
                    Console.Write(String.Format("[ID:{0}, name:{1}, PIDPos:{2}, Ndesc:{3}]-", item2.ID, item2.name, item2.PIDPos, item2.Ndesc));
                }
                Console.WriteLine();

            }
        }

        public bool fwdGetCA(out Item2 c) {
            bool success = true;
            do {
                success = true;
                bool EOL = false;
                int maxID = -1;
                int[] tempCur = cursors;

                for (int i = 0; i < cursors.Length; i++) {
                    int Id = Ls2[i][cursors[i]].ID;
                    if (maxID < Id) maxID = Id;
                }

                for (int n = 0; n < Ls2.Count; n++) {
                    int Pos = fwdBinSearch(n, maxID);
                    if (Pos >= Ls2[n].Count) {
                        EOL = true;
                        break;
                    }
                    if (Pos == -1) {
                        success = false;
                        break;
                    } else {
                        tempCur[n] = Pos;
                    }
                }

                if (EOL) {
                    c = null;
                    success = false;
                    break;
                }

                if (success) {
                    cursors = tempCur;
                    c = Ls2[0][cursors[0]];
                    break;
                } else {
                    fwdAdvance();
                    if (fwdEof()) {
                        success = false;
                        c = null;
                        break;
                    }
                }
            } while (true);

            return success;
        }

        public bool bwdGetCA(out Item2 c) {
            bool success;
            do {
                success = true;
                bool FOL = false;
                int minID = Int32.MaxValue;
                int[] tempCur = cursors;

                for (int i = 0; i < cursors.Length; i++) {
                    int Id = Ls2[i][cursors[i]].ID;
                    if (minID > Id) minID = Id;
                }

                for (int n = 0; n < Ls2.Count; n++) {
                    int Pos = bwdBinSearch(n, minID);
                    if (Pos == -1) {
                        FOL = true;
                        break;
                    }
                    if (Pos == -2) {
                        success = false;
                        break;
                    } else {
                        tempCur[n] = Pos;
                    }
                }

                if (FOL) {
                    c = null;
                    success = false;
                    break;
                }

                if (success) {
                    cursors = tempCur;
                    c = Ls2[0][cursors[0]];
                    break;
                } else {
                    bwdAdvance();
                    if (bwdEof()) {
                        success = false;
                        c = null;
                        break;
                    }
                }
            } while (true);
            return success;
        }

        protected void fwdAdvance() {
            for (int i = 0; i < cursors.Length; i++) {
                cursors[i] = cursors[i] + 1;
            }
        }

        protected void bwdAdvance() {
            for (int i = 0; i < cursors.Length; i++) {
                cursors[i] = cursors[i] - 1;
            }
        }


        private int bwdBinSearch(int n, int id) {
            int s = 0;
            int e = cursors[n];
            setInterval(n, id, out s, out e);
            int m = -1;
            while (s <= e) {
                m = (s + e) / 2;
                if (Ls2[n][m].ID == id) {
                    break;
                } else if (Ls2[n][m].ID > id) {
                    e = m - 1;
                } else {
                    s = m + 1;
                }
            }
            if (s > e) {
                m = -2;
            }
            return m;
        }

        private void setInterval(int n, int id, out int s, out int e) {
            if (Ls2[n][cursors[n]].ID == id) {
                s = cursors[n];
                e = s;
                return;
            }
            e = cursors[n];
            s = Ls2[n][cursors[n]].PIDPos;
            while (Ls2[n][s].ID > id) {
                e = s;
                s = Ls2[n][s].PIDPos;
            }
            return;
        }

        private int fwdBinSearch(int n, int id) {
            int e = Ls2[n].Count - 1;
            int s = cursors[n];
            int m = -1;
            while (s <= e) {
                m = (s + e) / 2;
                if (Ls2[n][m].ID == id) break;
                else if (Ls2[n][m].ID > id) {
                    e = m - 1;
                } else {
                    s = m + 1;
                }
            }
            if (s > e) m = -1;
            return m;
        }

        protected bool fwdEof() {
            for (int i = 0; i < cursors.Length; i++) {
                if (cursors[i] >= Ls2[i].Count) return true;
            }
            return false;
        }

        protected bool bwdEof() {
            for (int i = 0; i < cursors.Length; i++) {
                if (cursors[i] == -1) return true;
            }
            return false;
        }

    }
}