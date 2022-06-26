using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Xml;
using System.Xml.Linq;

namespace TCPConnectionTestApp
{
	public class XMLUtilities
	{
		public XMLUtilities()
		{
		}

		public static string FormatXml(string Xml)
		{
			string str;
			try
			{
				str = XDocument.Parse(Xml).ToString();
			}
			catch (Exception)
			{
				str = Xml;
			}
			return str;
		}

		public static string GetAttributeValueAsString(XmlNode node, string attributeName)
		{
			XmlNode namedItem = node.Attributes.GetNamedItem(attributeName);
			if (namedItem == null)
			{
				throw new Exception(string.Concat("Attribute \"", attributeName, "\" not found."));
			}
			return namedItem.Value;
		}

		public static List<string> GetListOfNames(XmlNode rootNode, string nodeTagName)
		{
			List<string> strs = new List<string>();
			XmlElement xmlElement = null;
			xmlElement = (!(rootNode is XmlDocument) ? rootNode as XmlElement : rootNode.FirstChild as XmlElement);
			foreach (object elementsByTagName in xmlElement.GetElementsByTagName(nodeTagName))
			{
				XmlNode namedItem = ((XmlNode)elementsByTagName).Attributes.GetNamedItem("Name");
				if (namedItem == null)
				{
					continue;
				}
				strs.Add(namedItem.Value);
			}
			return strs;
		}

		public static List<XmlNode> GetNodesByAttributeAndAttributeValue(XmlNode rootNode, string attributeName, string attributeValue)
		{
			List<XmlNode> xmlNodes = new List<XmlNode>();
			XmlElement xmlElement = null;
			xmlElement = (!(rootNode is XmlDocument) ? rootNode as XmlElement : rootNode.FirstChild as XmlElement);
			if (xmlElement == null)
			{
				return null;
			}
			List<XmlNode> xmlNodes1 = new List<XmlNode>();
			XMLUtilities.WalkTreeAndAddToList<List<XmlNode>>(xmlElement, ref xmlNodes1, (XmlNode x) => true);
			foreach (XmlNode xmlNode in xmlNodes1)
			{
				XmlNode namedItem = xmlNode.Attributes.GetNamedItem(attributeName);
				if (namedItem == null || !(namedItem.Value == attributeValue))
				{
					continue;
				}
				xmlNodes.Add(xmlNode);
			}
			return xmlNodes;
		}

		public static List<XmlNode> GetNodesByAttributeName(XmlNode rootNode, string attributeName)
		{
			List<XmlNode> xmlNodes = new List<XmlNode>();
			XmlElement xmlElement = null;
			xmlElement = (!(rootNode is XmlDocument) ? rootNode as XmlElement : rootNode.FirstChild as XmlElement);
			if (xmlElement == null)
			{
				return null;
			}
			XMLUtilities.WalkTreeAndAddToList<List<XmlNode>>(xmlElement, ref xmlNodes, (XmlNode n) => n.Attributes.GetNamedItem(attributeName) != null);
			return xmlNodes;
		}

		public static List<XmlNode> GetNodesByTypeAttributeAndAttributeValue(XmlNode rootNode, string nodeType, string attributeName, string attributeValue)
		{
			List<XmlNode> xmlNodes = new List<XmlNode>();
			XmlElement xmlElement = null;
			xmlElement = (!(rootNode is XmlDocument) ? rootNode as XmlElement : rootNode.FirstChild as XmlElement);
			if (xmlElement == null)
			{
				return null;
			}
			foreach (XmlNode elementsByTagName in xmlElement.GetElementsByTagName(nodeType))
			{
				XmlNode namedItem = elementsByTagName.Attributes.GetNamedItem(attributeName);
				if (namedItem == null || !(namedItem.Value == attributeValue))
				{
					continue;
				}
				xmlNodes.Add(elementsByTagName);
			}
			return xmlNodes;
		}

		public static XmlElement GetParentOfAttributeNode(XmlNode attributeNode)
		{
			XmlNode parentNode = attributeNode;
			XmlElement ownerElement = null;
			while (ownerElement == null)
			{
				switch (parentNode.NodeType)
				{
					case XmlNodeType.Element:
					{
						ownerElement = (XmlElement)parentNode;
						continue;
					}
					case XmlNodeType.Attribute:
					{
						ownerElement = ((XmlAttribute)parentNode).OwnerElement;
						continue;
					}
					case XmlNodeType.Text:
					{
						parentNode = parentNode.ParentNode;
						continue;
					}
					default:
					{
						continue;
					}
				}
			}
			return ownerElement;
		}

		public static XmlElement GetSingleNodeByType(XmlNode doc, string nodeType)
		{
			XmlElement xmlElement = null;
			xmlElement = (!(doc is XmlDocument) ? doc as XmlElement : doc.FirstChild as XmlElement);
			if (doc == null)
			{
				return null;
			}
			XmlNodeList elementsByTagName = xmlElement.GetElementsByTagName(nodeType);
			XmlNode itemOf = null;
			if (elementsByTagName.Count > 0)
			{
				itemOf = elementsByTagName[0];
			}
			return (XmlElement)itemOf;
		}

		public static XmlNode GetSingleNodeByTypeAndCategory(XmlDocument job, string nodeType, string categoryAttribute)
		{
			return XMLUtilities.GetSingleNodeByTypeAttributeAndAttributeValue(job, nodeType, "Category", categoryAttribute);
		}

		public static XmlNode GetSingleNodeByTypeAttributeAndAttributeValue(XmlNode rootNode, string nodeType, string attributeName, string attributeValue)
		{
			XmlElement xmlElement = null;
			xmlElement = (!(rootNode is XmlDocument) ? rootNode as XmlElement : rootNode.FirstChild as XmlElement);
			if (xmlElement == null)
			{
				return null;
			}
			XmlNode xmlNodes = null;
			foreach (XmlNode elementsByTagName in xmlElement.GetElementsByTagName(nodeType))
			{
				XmlNode namedItem = elementsByTagName.Attributes.GetNamedItem(attributeName);
				if (namedItem == null || !(namedItem.Value == attributeValue))
				{
					continue;
				}
				xmlNodes = elementsByTagName;
				return xmlNodes;
			}
			return xmlNodes;
		}

		public static XmlNode GetSpecificListFromChildNode(XmlNode sourceNode, string listName)
		{
			ICollectionView defaultView = CollectionViewSource.GetDefaultView(sourceNode.ChildNodes);
			defaultView.Filter = null;
			defaultView.Filter = (object obj) => {
				XmlNode xmlNodes = obj as XmlNode;
				if (xmlNodes == null)
				{
					return false;
				}
				if (xmlNodes.Name != listName)
				{
					return false;
				}
				XmlNode namedItem = xmlNodes.Attributes.GetNamedItem("Hidden");
				if (namedItem != null && namedItem.Value == "True")
				{
					return false;
				}
				return true;
			};
			return (XmlNode)defaultView.CurrentItem;
		}

		public static string TryAttributeValueAsString(XmlNode node, string attributeName)
		{
			XmlNode namedItem = node.Attributes.GetNamedItem(attributeName);
			if (namedItem == null)
			{
				return null;
			}
			return namedItem.Value;
		}

		public static void WalkTreeAndAddToList<Tlist>(XmlNode parentNode, ref Tlist list, Predicate<XmlNode> conditionTest)
		where Tlist : IList, ICollection, IEnumerable
		{
			if (conditionTest(parentNode))
			{
				list.Add(parentNode);
			}
			foreach (object childNode in parentNode.ChildNodes)
			{
				XMLUtilities.WalkTreeAndAddToList<Tlist>((XmlNode)childNode, ref list, conditionTest);
			}
		}
	}
}