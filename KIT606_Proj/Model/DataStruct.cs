using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace KIT606_Proj.Model {
    class DataStruct {
        Node root;
        private int idx = 0;
        public List<Node> latitudeLinks =  new List<Node>();
        public Node leafNode = null;
        private String xmlPath;
        public DataStruct(String xmlPath) {
            this.xmlPath = xmlPath;
            root = new Node(-1);
        }

        public void InitRTree(XmlNode node) {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;
            XmlReader xmlReader = XmlReader.Create(xmlPath, settings);
            Node curr = root;
            String currLabel = "0";
            int depth = -1;
            Node currLeaf = null;
            //List<Node> currLinks = new List<Node>();
            while (xmlReader.Read()) {
                switch (xmlReader.NodeType) {
                    case XmlNodeType.Element:
                        depth++;
                        Node temp = new Node(++idx);
                        temp.name = xmlReader.Name;
                        temp.parent = curr;

                        /*if (latitudeLinks.Count == depth) {
                            latitudeLinks.Add(temp);
                            currLinks.Add(temp);
                        } else {
                            currLinks[depth].next = temp;
                            currLinks[depth] = currLinks[depth].next;
                        }*/

                        if(curr.ID == -1)
                            currLabel = "0";
                        else {
                            currLabel = currLabel +"."+ temp.parent.getChildren().Count.ToString();
                        }
                        temp.DeweyLabel = currLabel;
                        curr.addChild(temp);
                        curr = temp;

                        break;
                    case XmlNodeType.Text:
                        curr.value = xmlReader.Value;
                        if (leafNode == null) {
                            leafNode = currLeaf = curr;
                        } else {
                            currLeaf.nextLeaf = curr;
                            currLeaf = currLeaf.nextLeaf;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        depth--;
                        curr = curr.parent;
                        int lastDot = currLabel.LastIndexOf('.');
                        if(lastDot != -1)
                            currLabel = currLabel.Substring(0, lastDot);
                        break;
                }
                /*if (xmlReader.HasAttributes) {
                    xmlReader.MoveToFirstAttribute();
                    do {
                        curr.addAttr(xmlReader.Name, xmlReader.Value);
                    } while (xmlReader.MoveToNextAttribute());
                    xmlReader.MoveToElement();
                }*/
            }
        }

        public Node getRealRoot() {
            if (root.getChildren().Count == 1)
                return root.getChildren()[0];
            return null;
        }

        public void printLongitudeLinks() {
            foreach(Node curr in latitudeLinks){
                Node currr = curr;
                while (currr != null) { 
                    Console.Write(currr.DeweyLabel + "--->");
                    currr = currr.next;
                }
                Console.WriteLine();
            }
        }

        public void printLeafNodes() {
            if (leafNode != null) {
                Node cur = leafNode;
                do {
                    Console.Write(cur.DeweyLabel + "---->");
                    cur = cur.nextLeaf;
                } while (cur != null);
            }
        }

        override public String ToString() {
            StringBuilder sb = new StringBuilder();
            LoopToString(sb, getRealRoot());
            return sb.ToString();
        }

        private void LoopToString(StringBuilder sb, Node node) {

            sb.Append(node.name);

            sb.Append(':');
            sb.AppendLine(node.DeweyLabel);

            foreach (Node child in node.getChildren()) {
                LoopToString(sb, child);
            }


        }
        public class Node {
            public int ID { get; set; }
            public String DeweyLabel { get; set; }
            public String name { get; set; }
            public String value { get; set; }
            public Node parent { get; set; }
            public Node next { get; set; }
            public Node nextLeaf { get; set; }

            private Dictionary<string, string> attrs = new Dictionary<string, string>();
            private List<Node> children = new List<Node>();

            public Node(int index) {
                ID = index;
                value = null;
                parent = null;
            }

            public void addChild(Node child) {
                children.Add(child);
            }

            public void addAttr(String name, String value) {
                attrs.Add(name, value);
            }

            public List<Node> getChildren() {
                return children;
            }

            public override String ToString() {
                return String.Format("{0}:{1} \r\n", name, ID);
            }
        }
    }
}
