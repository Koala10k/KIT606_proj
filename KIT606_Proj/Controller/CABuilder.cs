using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace KIT606_Proj.Controller
{
    class CABuilder
    {
        public List<List<Item2>> Ls2 = new List<List<Item2>>();
        public List<String> keywords = new List<String>();
        public void buildLs2(string xmlPath, string p)
        {
            keywords = Regex.Split(p.Trim(), @" +").ToList<String>();
            Ls2.Clear();
            for (int i = 0; i < keywords.Count; i++)
            {
                Ls2.Add(new List<Item2>());
            }
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            XmlReader xmlReader = XmlReader.Create(xmlPath, settings);
            int currID = 1;
            Stack<Item2> parentsLinks = new Stack<Item2>();
            int depth = 0;
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        Item2 item2 = new Item2(currID++, xmlReader.Name);
                        parentsLinks.Push(item2);
                        depth++;
                        break;
                    case XmlNodeType.Text:
                        for (int i = 0; i < keywords.Count; i++)
                        {
                            if (xmlReader.Value.ToLower().Contains(keywords[i].ToLower()))
                            {
                                Item2[] ancestors = parentsLinks.ToArray();
                                Array.Reverse(ancestors);
                                int j = ancestors.Length - 1;
                                int k = Ls2[i].Count - 1;
                                bool goThrough = false;
                                while (k >= 0 && j >= 0)
                                {
                                    goThrough = true;
                                    if (Ls2[i][k].ID < ancestors[j].ID)
                                        j--;
                                    else if (Ls2[i][k].ID == ancestors[j].ID)
                                        break;
                                    else
                                        k--;
                                }
                                if (j >= 0 && k >= 0)
                                {
                                    if (Ls2[i][k].ID != ancestors[j].ID)
                                    {
                                        throw new Exception("Not Merging into a same ancestors");
                                    }
                                    else
                                    {
                                        int pidPos = k;
                                        while (k >= 0)
                                        {
                                            Ls2[i][k].Ndesc++;
                                            k = Ls2[i][k].PIDPos;
                                        }
                                        j++;
                                        int nextPidPos = Ls2[i].Count;
                                        while (j < ancestors.Length)
                                        {
                                            Item2 it = new Item2(ancestors[j]);
                                            it.PIDPos = pidPos;
                                            it.Ndesc++;
                                            Ls2[i].Add(it);
                                            pidPos = nextPidPos;
                                            nextPidPos++;
                                            j++;
                                        }
                                    }
                                }
                                else
                                {
                                    if (k == -1 && !goThrough)
                                    {
                                        j = 0;
                                        int pidPos = -1;
                                        while (j < ancestors.Length)
                                        {
                                            Item2 it = new Item2(ancestors[j]);
                                            it.Ndesc++;
                                            it.PIDPos = pidPos;
                                            pidPos++;
                                            Ls2[i].Add(it);
                                            j++;
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception(String.Format("Something won't happen happened, j={0}, k={1}, goThrough={2}", j, k, goThrough));
                                    }
                                }
                            }
                        }
                        break;
                    case XmlNodeType.EndElement:
                        depth--;
                        parentsLinks.Pop();
                        break;
                }
            }
            sort();

        }

        private void sort()
        {
            int[] numKeys = new int[Ls2.Count];
            for (int i = 0; i < numKeys.Length; i++)
            {
                numKeys[i] = Ls2[i].Count;
            }
            int s = 0;
            while (s < numKeys.Length)
            {
                int val = numKeys[s];
                int pos = s;
                for (int i = s+1; i < numKeys.Length; i++)
                {
                    if (numKeys[i] < val)
                    {
                        val = numKeys[i];
                        pos = i;
                    }
                }
                if (s != pos)
                {
                    swapItem(s, pos);
                    numKeys[s] += numKeys[pos];
                    numKeys[pos] = numKeys[s] - numKeys[pos];
                    numKeys[s] -= numKeys[pos];
                }

                s++;
            }
        }

        

        public void swapItem(int i, int p)
        {
            string tempKeyword = keywords[i];
            keywords[i] = keywords[p];
            keywords[p] = tempKeyword;

            List<Item2> tempItem = Ls2[i];
            Ls2[i] = Ls2[p];
            Ls2[p] = tempItem;
        }

        private void printLs2()
        {
            for (int i = 0; i < Ls2.Count; i++) {
                Console.WriteLine(keywords[i] + ":");
                for (int j = 0; j < Ls2[i].Count; j++) {
                    Item2 item = Ls2[i][j];
                    Console.WriteLine("[ID:"+item.ID+",name:"+item.name + ",PIDPos:"+item.PIDPos+",Ndesc:"+item.Ndesc+",n:"+item.n);
                }
                Console.WriteLine("-----------------------");
            }
        }



    }

    class Item2
    {
        public int ID { get; set; }
        public int PIDPos { get; set; }
        public int Ndesc { get; set; }
        public string value { get; set; }
        public string name { get; set; }
        public int n { get; set; }

        public Item2(int ID, string name)
        {
            this.ID = ID;
            this.name = name;
            Ndesc = 0;
        }

        public Item2(Item2 other)
        {
            ID = other.ID;
            PIDPos = other.PIDPos;
            Ndesc = other.Ndesc;
            value = other.value;
            name = other.name;
        }

    }
}
