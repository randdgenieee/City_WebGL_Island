using System.Collections.Generic;

public class XMLParser
{
	protected XMLElement rootElement;

	protected XMLElement currentElement;

	protected bool rootflag;

	protected List<XMLAttribute> attributeList;

	protected string xmlString;

	public XMLElement XMLRootElement => rootElement;

	public string XMLString
	{
		get
		{
			return xmlString;
		}
		set
		{
			xmlString = value;
			rootflag = false;
		}
	}

	public XMLParser(string xmlString)
	{
		this.xmlString = xmlString;
	}

	public XMLParser()
	{
		xmlString = "";
	}

	public virtual XMLElement Parse(string xmlString)
	{
		this.xmlString = xmlString;
		return Parse();
	}

	public XMLElement Parse()
	{
		rootflag = false;
		string text = "";
		string tagName = "";
		string name = "";
		string str = "";
		XMLTokenType xMLTokenType = XMLTokenType.None;
		XMLTokenType xMLTokenType2 = XMLTokenType.None;
		XMLParserAttrbuteMode xMLParserAttrbuteMode = XMLParserAttrbuteMode.Name;
		attributeList = new List<XMLAttribute>();
		int i = 0;
		for (int length = xmlString.Length; i < length; i++)
		{
			char c = xmlString[i];
			switch (c)
			{
			case '<':
				xMLTokenType2 = xMLTokenType;
				switch (xMLTokenType2)
				{
				case XMLTokenType.Entity:
					EntityHandler(text);
					break;
				case XMLTokenType.Text:
					TextHandler(text);
					break;
				}
				switch (xmlString[i + 1])
				{
				case '?':
					xMLTokenType = XMLTokenType.Declaration;
					i++;
					break;
				case '!':
					xMLTokenType = XMLTokenType.EntityElement;
					i++;
					break;
				case '/':
					xMLTokenType = XMLTokenType.EndElement;
					i++;
					break;
				default:
					switch (xMLTokenType2)
					{
					case XMLTokenType.Declaration:
						xMLTokenType = XMLTokenType.Declaration;
						break;
					case XMLTokenType.EntityElement:
						xMLTokenType = XMLTokenType.EntityElement;
						break;
					default:
						xMLTokenType = XMLTokenType.StartElement;
						break;
					}
					break;
				}
				text = "";
				break;
			case '>':
				xMLTokenType2 = xMLTokenType;
				switch (xMLTokenType)
				{
				case XMLTokenType.Declaration:
					DeclarationHandler(text);
					break;
				case XMLTokenType.EntityElement:
					EntityHandler(text);
					break;
				case XMLTokenType.StartElement:
					StartElementHandler(text);
					if (xmlString[i - 1] == '/')
					{
						EndElementHandler(text);
					}
					break;
				case XMLTokenType.EndElement:
					EndElementHandler(text);
					break;
				case XMLTokenType.Attribute:
					StartElementHandler(tagName);
					xMLTokenType2 = XMLTokenType.StartElement;
					break;
				}
				text = "";
				xMLTokenType = XMLTokenType.Text;
				break;
			case ' ':
				switch (xMLTokenType)
				{
				case XMLTokenType.StartElement:
					xMLTokenType2 = xMLTokenType;
					xMLTokenType = XMLTokenType.Attribute;
					tagName = text;
					text = "";
					xMLParserAttrbuteMode = XMLParserAttrbuteMode.Name;
					break;
				case XMLTokenType.Text:
					text += c.ToString();
					break;
				case XMLTokenType.Attribute:
					if (xMLParserAttrbuteMode == XMLParserAttrbuteMode.Value)
					{
						text += c.ToString();
					}
					break;
				}
				break;
			case '=':
				if (xMLTokenType == XMLTokenType.Attribute)
				{
					switch (xMLParserAttrbuteMode)
					{
					case XMLParserAttrbuteMode.Name:
						name = text.Trim();
						text = "";
						xMLParserAttrbuteMode = XMLParserAttrbuteMode.Assignment;
						break;
					case XMLParserAttrbuteMode.Value:
						text += c.ToString();
						break;
					}
				}
				else
				{
					text += c.ToString();
				}
				break;
			case '"':
				if (xMLTokenType == XMLTokenType.Attribute)
				{
					switch (xMLParserAttrbuteMode)
					{
					case XMLParserAttrbuteMode.Assignment:
						xMLParserAttrbuteMode = XMLParserAttrbuteMode.Value;
						break;
					case XMLParserAttrbuteMode.Value:
						AttributeHandler(name, text);
						text = "";
						xMLParserAttrbuteMode = XMLParserAttrbuteMode.Name;
						break;
					}
				}
				break;
			case '&':
				xMLTokenType2 = xMLTokenType;
				xMLTokenType = XMLTokenType.Entity;
				str = text;
				text = "";
				break;
			case ';':
				if (xMLTokenType == XMLTokenType.Entity)
				{
					xMLTokenType = xMLTokenType2;
					text = str + ParseEntityReference(text).ToString();
				}
				else
				{
					text += c.ToString();
				}
				break;
			default:
				text += c.ToString();
				break;
			}
		}
		return rootElement;
	}

	protected virtual void DeclarationHandler(string content)
	{
	}

	protected virtual void EntityHandler(string content)
	{
	}

	protected virtual void StartElementHandler(string tagName)
	{
		XMLElement xMLElement;
		if (!rootflag)
		{
			xMLElement = (rootElement = new XMLElement(tagName.Trim(), null));
			rootflag = true;
		}
		else
		{
			xMLElement = new XMLElement(tagName.Trim(), currentElement);
		}
		int i = 0;
		for (int count = attributeList.Count; i < count; i++)
		{
			xMLElement.Attributes.Add(attributeList[i]);
		}
		attributeList = new List<XMLAttribute>();
		currentElement = xMLElement;
	}

	protected virtual void EndElementHandler(string tagName)
	{
		if (rootflag && rootElement != currentElement)
		{
			currentElement = (XMLElement)currentElement.Parent;
		}
	}

	protected virtual void AttributeHandler(string name, string value)
	{
		attributeList.Add(new XMLAttribute(name.Trim(), value));
	}

	protected virtual void TextHandler(string text)
	{
		if (!rootflag || text.Trim().Length <= 0)
		{
			return;
		}
		if (currentElement.Children.Count != 0)
		{
			IXMLNode iXMLNode = currentElement.Children[currentElement.Children.Count - 1];
			if (iXMLNode.type == XMLNodeType.Text)
			{
				iXMLNode.value += text;
				return;
			}
		}
		new XMLText(text, currentElement);
	}

	public static char ParseEntityReference(string entity)
	{
		if (!(entity == "lt"))
		{
			if (!(entity == "gt"))
			{
				if (!(entity == "quot"))
				{
					if (!(entity == "apos"))
					{
						if (entity == "amp")
						{
							return '&';
						}
						return '\0';
					}
					return '\'';
				}
				return '"';
			}
			return '>';
		}
		return '<';
	}

	public static string GetEntityReference(char c)
	{
		switch (c)
		{
		case '<':
			return "&lt;";
		case '>':
			return "&gt;";
		case '"':
			return "&quot;";
		case '\'':
			return "&apos;";
		case '&':
			return "&amp;";
		default:
			return c.ToString();
		}
	}
}
