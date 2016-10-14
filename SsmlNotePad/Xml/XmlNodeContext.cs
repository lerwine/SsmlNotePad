using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace Erwine.Leonard.T.SsmlNotePad.Xml
{
    public class XmlNodeContext
    {
        private XmlNodeContext() { }

        public Text.TextRange OuterRange { get; private set; }

        public Text.TextRange InnerRange { get; private set; }

        public string LocalName { get; private set; }

        public string NamespaceURI { get; private set; }

        public XmlNodeContext ParentNode { get; private set; }

        public XmlNodeContext PrecedingNode { get; private set; }

        public XmlNodeContext FollowingNode { get; private set; }

        public XmlNodeContext[] ChildNodes { get; private set; }

        public XmlNodeType NodeType { get; private set; }

        public static int GetStartLineIndex(XNode node)
        {
            if (node is XElement)
                return (node as IXmlLineInfo).LinePosition - 2;
            if (node is XComment)
                return (node as IXmlLineInfo).LinePosition - 5;
            if (node is XDocumentType)
                return (node as IXmlLineInfo).LinePosition - 11;
            if (node is XProcessingInstruction)
                return (node as IXmlLineInfo).LinePosition - 3;

            return (node as IXmlLineInfo).LinePosition - 1;
        }

        internal static IEnumerable<XmlNodeContext> Load(XmlReader xmlReader, Text.TextLineInfo[] lines, Action<int, int> updateLineInfo)
        {
            return Load(XDocument.Load(xmlReader, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo), lines);
        }

        private static IEnumerable<XmlNodeContext> Load(XDocument xDocument, Text.TextLineInfo[] lines)
        {
            XNode xNode = xDocument.FirstNode;
            XmlNodeContext parentNode = null, precedingNode = null;
            List<XmlNodeContext> childNodes = new List<XmlNodeContext>();
            Stack<List<XmlNodeContext>> nested = new Stack<List<XmlNodeContext>>();
            while (xNode != null)
            {
                IXmlLineInfo xmlLineInfo = xNode as IXmlLineInfo;
                Text.TextLineInfo currentTextLineInfo = lines[xmlLineInfo.LineNumber - 1];
                XmlNodeContext currentNode = new XmlNodeContext
                {
                    NodeType = xNode.NodeType,
                    ParentNode = parentNode,
                    OuterRange = new Text.TextRange(currentTextLineInfo.CreateTextPointer(0, GetStartLineIndex(xNode)), xNode.ToString(SaveOptions.DisableFormatting).Length),
                    PrecedingNode = precedingNode
                };
                yield return currentNode;
                if (precedingNode != null)
                    precedingNode.FollowingNode = currentNode;
                childNodes.Add(currentNode);

                if (xNode is XElement)
                {
                    XElement xElement = xNode as XElement;
                    currentNode.LocalName = xElement.Name.LocalName;
                    currentNode.NamespaceURI = xElement.Name.NamespaceName;
                    if (!xElement.IsEmpty)
                    {
                        xNode = xElement.FirstNode;
                        parentNode = currentNode;
                        precedingNode = null;
                        nested.Push(childNodes);
                        childNodes = new List<XmlNodeContext>();
                        continue;
                    }
                }
                else
                {
                    currentNode.LocalName = "";
                    currentNode.NamespaceURI = "";
                }

                currentNode.InnerRange = currentNode.OuterRange;
                while (xNode.NextNode == null && parentNode != null)
                {
                    if (parentNode != null)
                    {
                        parentNode.ChildNodes = childNodes.ToArray();
                        if (childNodes.Count == 1)
                            parentNode.InnerRange = childNodes[0].OuterRange;
                        else
                            parentNode.InnerRange = new Text.TextRange(childNodes[0].OuterRange.Start, childNodes[childNodes.Count - 1].OuterRange.End);
                        parentNode = parentNode.ParentNode;
                    }
                    childNodes = nested.Pop();
                    currentNode = childNodes.LastOrDefault();
                    xNode = xNode.Parent;
                }
                precedingNode = currentNode;
                xNode = xNode.NextNode;
            }
        }

        public IEnumerable<XmlNodeContext> AllChildNodes()
        {
            return ChildNodes.SelectMany(n => (new XmlNodeContext[] { n }).Concat(n.AllChildNodes()));
        }
    }
}