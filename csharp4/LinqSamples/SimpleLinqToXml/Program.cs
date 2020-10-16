﻿// © Корпорация Майкрософт (Microsoft Corp.).  Все права защищены.
// Этот код выпущен на условиях 
// открытой лицензии Майкрософт (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//(C) Корпорация Майкрософт (Microsoft Corporation). Все права защищены.

using System;
using System.Xml;
using System.Xml.Linq;

// Дополнительные сведения см. в файле Readme.html
namespace Samples
{
    public class Program
    {
        public static void Main()
        {
            // построить предопределенный документ
            // с помощью методики наподобие XML DOM
            XDocument document = CreateDocumentVerbose();

            // отобразить документ в консоли
            Console.WriteLine(document);

            // дамп всех узлов в консоль 
            DumpNode(document);

            // еще раз построить предопределенный документ
            // с использованием функционального подхода
            document = CreateDocumentConcise();
            
            // отобразить документ в консоли
            Console.WriteLine(document);

            // дамп всех узлов в консоль
            DumpNode(document);

            Console.ReadLine();
        }

        // <?xml version="1.0"?>
        // <?order alpha ascending?>
        // <art xmlns='urn:art-org:art'>
        //   <period name='Renaissance' xmlns:a='urn:art-org:artists'>
        //     <a:artist>Leonardo da Vinci</a:artist>
        //     <a:artist>Michelangelo</a:artist>
        //     <a:artist><![CDATA[Donatello]]></a:artist>
        //   </period>
        //   <!– вставьте сюда точку -->
        // </art>
        public static XDocument CreateDocumentVerbose()
        {
            XNamespace nsArt = "urn:art-org:art";
            XNamespace nsArtists = "urn:art-org:artists";

            // создание документа
            XDocument document = new XDocument();

            // создание объявления XML и
            // установка на документ
            document.Declaration = new XDeclaration("1.0", null, null);

            // создание элемента 'art' и
            // добавление в документ 
            XElement art = new XElement(nsArt + "art");
            document.Add(art);

            // создание инструкции по обработке заказа и
            // добавление перед элементом "art" 
            XProcessingInstruction pi = new XProcessingInstruction("order", "alpha ascending");
            art.AddBeforeSelf(pi);

            // создание элемента "period" и
            // добавление в элемент "art"
            XElement period = new XElement(nsArt + "period");
            art.Add(period);

            // добавление атрибута "name" к элементу "period" 
            period.SetAttributeValue("name", "Renaissance");

            // создание объявления пространства имен xmlns:a и
            // добавление элемент периода 
            XAttribute nsdecl = new XAttribute(XNamespace.Xmlns + "a", nsArtists);
            period.Add(nsdecl);

            // создание элементов 'artist' и
            // используемые текстовые узлы
            period.SetElementValue(nsArtists + "artist", "Michelangelo");

            XElement artist = new XElement(nsArtists + "artist", "Leonardo ", "da ", "Vinci");
            period.AddFirst(artist);

            artist = new XElement(nsArtists + "artist");
            period.Add(artist);
            XText cdata = new XText("Donatello");
            artist.Add(cdata);

            // создание комментария и
            // добавление в элемент "art"
            XComment comment = new XComment("insert period here");
            art.Add(comment);

            return document;
        }

        // <?xml version="1.0"?>
        // <?order alpha ascending?>
        // <art xmlns='urn:art-org:art'>
        //   <period name='Renaissance' xmlns:a='urn:art-org:artists'>
        //     <a:artist>Leonardo da Vinci</a:artist>
        //     <a:artist>Michelangelo</a:artist>
        //     <a:artist><![CDATA[Donatello]]></a:artist>
        //   </period>
        //   <!– вставьте сюда точку -->
        // </art>
        public static XDocument CreateDocumentConcise()
        {
            XNamespace nsArt = "urn:art-org:art";
            XNamespace nsArtists = "urn:art-org:artists";

            // одновременное создание документа
            return new XDocument(
                        new XDeclaration("1.0", null, null),
                        new XProcessingInstruction("order", "alpha ascending"),
                        new XElement(nsArt + "art",
                            new XElement(nsArt + "period",
                                new XAttribute("name", "Renaissance"),
                                new XAttribute(XNamespace.Xmlns + "a", nsArtists),
                                new XElement(nsArtists + "artist", "Leonardo da Vinci"),
                                new XElement(nsArtists + "artist", "Michelangelo"),
                                new XElement(nsArtists + "artist", 
                                    new XText("Donatello"))),
                            new XComment("insert period here")));                        
        }

        public static void DumpNode(XNode node) 
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Document:
                    XDocument document = (XDocument)node;
                    Console.WriteLine("StartDocument");
                    XDeclaration declaration = document.Declaration;
                    if (declaration != null)
                    {
                        Console.WriteLine("XmlDeclaration: {0} {1} {2}", declaration.Version, declaration.Encoding, declaration.Standalone);
                    }
                    foreach (XNode n in document.Nodes())
                    {
                        DumpNode(n);
                    }
                    Console.WriteLine("EndDocument");
                    break;
                case XmlNodeType.Element:
                    XElement element = (XElement)node;
                    Console.WriteLine("StartElement: {0}", element.Name);
                    if (element.HasAttributes) 
                    {
                        foreach (XAttribute attribute in element.Attributes())
                        {
                            Console.WriteLine("Attribute: {0} = {1}", attribute.Name, attribute.Value);
                        }
                    }
                    if (!element.IsEmpty)
                    {
                        foreach (XNode n in element.Nodes())
                        {
                            DumpNode(n);
                        }
                    }
                    Console.WriteLine("EndElement: {0}", element.Name);
                    break;
                case XmlNodeType.Text:
                    XText text = (XText)node;
                    Console.WriteLine("Text: {0}", text.Value); 
                    break;
                case XmlNodeType.ProcessingInstruction:
                    XProcessingInstruction pi = (XProcessingInstruction)node;
                    Console.WriteLine("ProcessingInstruction: {0} {1}", pi.Target, pi.Data);
                    break;
                case XmlNodeType.Comment:
                    XComment comment = (XComment)node;
                    Console.WriteLine("Comment: {0}", comment.Value);
                    break;
                case XmlNodeType.DocumentType:
                    XDocumentType documentType = (XDocumentType)node;
                    Console.WriteLine("DocumentType: {0} {1} {2} {3}", documentType.Name, documentType.PublicId, documentType.SystemId, documentType.InternalSubset);
                    break;
                default:
                    break;
            }
        }
    }
}
