using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MapleLib.WzLib;
using System.IO;

namespace WzImporter
{
    internal class Import
    {

        private string outputFileName = "";
        private string[] dirs = { "Hair", "Face", "Cap", "Coat", "Pants", "Longcoat", "Shoes", "Cape", "Shield", "Ring", "Accessory", "PetEquip", "Glove", "Weapon" };
        public bool[] selections = new bool[14];
        public bool cashOnly = false;
        public string inputFromFileName = "";
        public string inputToFileName = "";
        public string outputDir = "";

        public void ImportXML(Form1 form)
        {
            form.UpdateProgress("Starting...");
            form.Update();

            bool writeFile = true;

            outputFileName = outputDir + inputFromFileName.Substring(inputFromFileName.LastIndexOf(@"\"));
            if (File.Exists(outputFileName))
                File.Delete(outputFileName);

            // load up the to and from
            WzFile import_from_Wz = new WzFile(inputFromFileName, WzMapleVersion.BMS);
            WzFile import_to_Wz = new WzFile(inputToFileName, WzMapleVersion.GMS);

            import_from_Wz.ParseWzFile();
            import_to_Wz.ParseWzFile();

            for (int i = 0; i < dirs.Length; i++)
            {
                if (selections[i])
                {
                    form.UpdateProgress("Working on " + dirs[i] + "...");
                    form.Update();

                    // determine what directories we're updating.
                    WzDirectory toDirectory = new WzDirectory();
                    toDirectory = (WzDirectory)import_to_Wz[dirs[i]];

                    WzDirectory fromDirectory = new WzDirectory();
                    fromDirectory = (WzDirectory)import_from_Wz[dirs[i]];

                    List<WzImage> fromImages = fromDirectory.WzImages;
                    List<WzImage> toImages = toDirectory.WzImages;
                    List<string> toImagesNames = new List<string>();
                    foreach (WzImage im in toImages) toImagesNames.Add(im.Name);
                    List<WzImage> imgsToAdd = fromImages
                        .Where(x => !toImagesNames.Contains(x.Name)).ToList();

                    int imCount = 1;
                    foreach (WzImage im in imgsToAdd)
                    {
                        im.ParseImage();

                        form.UpdateProgress("Working on " + dirs[i] + "..." + im.Name + " ( " + imCount + " of " + imgsToAdd.Count + " )");
                        form.Update();
                        imCount++;

                        XmlDocument img = new XmlDocument();
                        MapleLib.WzLib.Serialization.WzClassicXmlSerializer s = new MapleLib.WzLib.Serialization.WzClassicXmlSerializer(0, MapleLib.WzLib.Serialization.LineBreak.None, true);
                        img.LoadXml(s.exportXml(im));

                        if (dirs[i] != "Hair" && dirs[i] != "Face")
                            if (cashOnly && img.SelectSingleNode("//int[@name='cash']") != null && img.SelectSingleNode("//int[@name='cash']").InnerText != "1")
                            {
                                im.UnparseImage();
                                continue;
                            }

                        foreach (XmlNode inoutlinkNode in img.SelectNodes("//canvas[string[@name='_inlink']]"))
                        {
                            string name = inoutlinkNode.Attributes["name"].Value;
                            string link = inoutlinkNode.SelectSingleNode("string[@name='_inlink']").Attributes["value"].Value;
                            if (!link.StartsWith("../"))
                                link = "../../" + link;
                            XmlNode newNode = img.CreateElement("uol");
                            newNode.Attributes.Append(img.CreateAttribute("name"));
                            newNode.Attributes.Append(img.CreateAttribute("value"));
                            newNode.Attributes["name"].Value = name;
                            newNode.Attributes["value"].Value = link;
                            inoutlinkNode.ParentNode.AppendChild(newNode);
                            inoutlinkNode.ParentNode.RemoveChild(inoutlinkNode);
                        }
                        foreach (XmlNode inoutlinkNode in img.SelectNodes("//canvas[string[@name='_outlink']]"))
                        {
                            string name = inoutlinkNode.Attributes["name"].Value;
                            string link = inoutlinkNode.SelectSingleNode("string[@name='_outlink']").Attributes["value"].Value;

                            XmlNode tmpNode = inoutlinkNode;
                            if (!link.Contains(dirs[i]))
                            {
                                string linkimg = link.Substring(0, link.IndexOf(".img/") + 4);
                                string linksub = link.Substring(link.IndexOf(".img/") + 5);
                                WzImage refImage = (WzImage)import_from_Wz.GetObjectFromPath(linkimg);
                                refImage.ParseImage();

                                XmlDocument refImageDoc = new XmlDocument();
                                refImageDoc.LoadXml(s.exportXml(refImage));
                                string[] subs = linksub.Split('/');
                                linksub = "//";
                                for (int j = 0; j < subs.Length - 1; j++)
                                    linksub += "imgdir[@name='" + subs[j] + "']/";
                                linksub += "canvas[@name='" + subs[subs.Length - 1] + "']";

                                XmlNode newNodeFromRef = refImageDoc.SelectSingleNode(linksub).CloneNode(true);
                                newNodeFromRef = img.ImportNode(newNodeFromRef, true);

                                newNodeFromRef.Attributes["name"].Value = name;

                                inoutlinkNode.ParentNode.AppendChild(newNodeFromRef);
                                inoutlinkNode.ParentNode.RemoveChild(inoutlinkNode);
                                refImage.UnparseImage();
                                continue;
                            }
                            else if (link.Contains("Character/"))
                            {
                                int depth = -1;
                                link = link.Replace("Character/", "");
                                link = link.Replace(dirs[i] + "/", "");
                                depth--;



                                //resolve path
                                while (tmpNode.ParentNode != null)
                                {
                                    tmpNode = tmpNode.ParentNode;
                                    depth++;
                                }

                                for (int d = 0; d <= depth; d++)
                                    link = "../" + link;
                            }

                            XmlNode newNode = img.CreateElement("uol");
                            newNode.Attributes.Append(img.CreateAttribute("name"));
                            newNode.Attributes.Append(img.CreateAttribute("value"));
                            newNode.Attributes["name"].Value = name;
                            newNode.Attributes["value"].Value = link;
                            inoutlinkNode.ParentNode.AppendChild(newNode);
                            inoutlinkNode.ParentNode.RemoveChild(inoutlinkNode);
                        }

                        WzImage newImage = (WzImage)new MapleLib.WzLib.Serialization.WzXmlDeserializer(false, null).ParseXML(img.OuterXml)[0];
                        newImage.ParseImage();
                        toDirectory.AddImage(newImage);
                        //newImage.UnparseImage();
                        im.UnparseImage();
                    }
                }
            }
            form.UpdateProgress("Saving " + outputFileName + " ...");
            import_to_Wz.SaveToDisk(outputFileName, false);
            form.UpdateProgress("Ready");
            form.Update();
        }


    }
}