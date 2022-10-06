using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MapleLib.WzLib;
using System.IO;
using System.Windows.Forms;
using MapleLib.WzLib.WzProperties;

namespace WzImporter
{
    internal class Import
    {

        private string outputFileName = "";
        private string linkNodeErrorsFileName = "";
        private List<string> linkErrors = new List<string>();
        private string[] dirs = { "Hair", "Face", "Cap", "Coat", "Pants", "Longcoat", "Shoes", "Cape", "Shield", "Ring", "Accessory", "PetEquip", "Glove", "Weapon" };
        public bool[] selections = new bool[14];
        public bool cashOnly = false;
        public bool includeString = false;
        public string inputFromFileName = "";
        public string inputToFileName = "";
        public string outputDir = "";

        public void ImportXML(Form1 form)
        {
            form.UpdateProgress("Starting...");

            outputFileName = outputDir + inputFromFileName.Substring(inputFromFileName.LastIndexOf(@"\"));
            if (File.Exists(outputFileName))
                File.Delete(outputFileName);
            if (File.Exists(outputFileName.Replace("Character.wz", "String.wz")))
                File.Delete(outputFileName.Replace("Character.wz", "String.wz"));

            linkNodeErrorsFileName = outputFileName.Replace("Character.wz", "LinkErrors.txt");
            if (File.Exists(linkNodeErrorsFileName))
                File.Delete(linkNodeErrorsFileName);

            // load up the to and from
            WzFile import_from_Wz = new WzFile(inputFromFileName, WzMapleVersion.BMS);
            WzFile import_to_Wz = new WzFile(inputToFileName, WzMapleVersion.GMS);
            WzFile import_from_StringWz = new WzFile(inputFromFileName, WzMapleVersion.BMS);
            WzFile import_to_StringWz = new WzFile(inputToFileName, WzMapleVersion.GMS);

            if (includeString)
            {
                if (File.Exists(inputFromFileName.Replace("Character.wz", "String.wz")))
                {
                    import_from_StringWz = new WzFile(inputFromFileName.Replace("Character.wz", "String.wz"), WzMapleVersion.BMS);
                    import_from_StringWz.ParseWzFile();
                }
                else
                {
                    string message = "Unable to find String.wz to import from at " + inputFromFileName.Replace("Character.wz", "String.wz");
                    string title = "Error";
                    MessageBox.Show(message, title);
                    return;
                }

                if (File.Exists(inputToFileName.Replace("Character.wz", "String.wz")))
                {
                    import_to_StringWz = new WzFile(inputToFileName.Replace("Character.wz", "String.wz"), WzMapleVersion.GMS);
                    import_to_StringWz.ParseWzFile();
                }
                else
                {
                    string message = "Unable to find String.wz to import to at " + inputToFileName.Replace("Character.wz", "String.wz");
                    string title = "Error";
                    MessageBox.Show(message, title);
                    return;
                }
            }

            import_from_Wz.ParseWzFile();
            import_to_Wz.ParseWzFile();

            for (int i = 0; i < dirs.Length; i++)
            {
                if (selections[i])
                {
                    form.UpdateProgress("Working on " + dirs[i] + "...");

                    // determine what directories we're updating and determine deltas.
                    WzDirectory toDirectory = new WzDirectory();
                    toDirectory = (WzDirectory)import_to_Wz[dirs[i]];

                    WzDirectory fromDirectory = new WzDirectory();
                    fromDirectory = (WzDirectory)import_from_Wz[dirs[i]];

                    WzImage toStringImg = new WzImage();
                    WzImage fromStringImg = new WzImage();

                    List<WzImage> fromImages = fromDirectory.WzImages;
                    List<WzImage> toImages = toDirectory.WzImages;
                    List<string> toImagesNames = new List<string>();
                    foreach (WzImage im in toImages) toImagesNames.Add(im.Name);
                    List<WzImage> imgsToAdd = fromImages
                        .Where(x => !toImagesNames.Contains(x.Name)).ToList();

                    if (includeString)
                    {
                        toStringImg = (WzImage)import_to_StringWz["Eqp.img"];
                        fromStringImg = (WzImage)import_from_StringWz["Eqp.img"];
                        toStringImg.ParseImage();
                        fromStringImg.ParseImage();
                    }

                    int imCount = 1;
                    foreach (WzImage im in imgsToAdd)
                    {
                        im.ParseImage();

                        form.UpdateProgress("Working on " + dirs[i] + "..." + im.Name + " ( " + imCount + " of " + imgsToAdd.Count + " )");
                        imCount++;

                        XmlDocument img = new XmlDocument();
                        MapleLib.WzLib.Serialization.WzClassicXmlSerializer s = new MapleLib.WzLib.Serialization.WzClassicXmlSerializer(0, MapleLib.WzLib.Serialization.LineBreak.None, true);
                        img.LoadXml(s.exportXml(im));

                        if (dirs[i] != "Hair" && dirs[i] != "Face")
                            if (cashOnly && img.SelectSingleNode("//int[@name='cash']") != null && img.SelectSingleNode("//int[@name='cash']").Attributes["value"].Value != "1")
                            {
                                im.UnparseImage();
                                continue;
                            }

                        // convert _inlink nodes using WzUOLProperty supported in v83
                        foreach (XmlNode inoutlinkNode in img.SelectNodes("//canvas[string[@name='_inlink']]"))
                        {
                            string name = inoutlinkNode.Attributes["name"].Value;
                            string link = inoutlinkNode.SelectSingleNode("string[@name='_inlink']").Attributes["value"].Value;

                            XmlNode tmpNode = inoutlinkNode;
                            while (tmpNode.ParentNode != null && tmpNode.Attributes["name"].Value != img.Name)
                            {
                                link = "../" + link;
                                tmpNode = tmpNode.ParentNode;
                            }

                            if (!ValidateLink(inoutlinkNode, ref link))
                                linkErrors.Add("Link Error: " + im.Name + " - " + inoutlinkNode.Name + " | " + link);
                            XmlNode newNode = img.CreateElement("uol");
                            newNode.Attributes.Append(img.CreateAttribute("name"));
                            newNode.Attributes.Append(img.CreateAttribute("value"));
                            newNode.Attributes["name"].Value = name;
                            newNode.Attributes["value"].Value = link;
                            inoutlinkNode.ParentNode.AppendChild(newNode);
                            inoutlinkNode.ParentNode.RemoveChild(inoutlinkNode);
                        }
                        // convert _outlink nodes using WzUOLProperty supported in v83
                        foreach (XmlNode inoutlinkNode in img.SelectNodes("//canvas[string[@name='_outlink']]"))
                        {
                            string name = inoutlinkNode.Attributes["name"].Value;
                            string link = inoutlinkNode.SelectSingleNode("string[@name='_outlink']").Attributes["value"].Value;

                            XmlNode tmpNode = inoutlinkNode;
                            if (!link.Contains(dirs[i]))
                            {
                                // if the reference is outside of this directory, WzUOLProperty cannot work, so fetch the actual image
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
                                //outlink to another image in the same directory. <uol> will work, but not as an absolute path.
                                link = link.Replace("Character/", "");
                                link = link.Replace(dirs[i] + "/", "");
                                XmlNode tmpNode2 = inoutlinkNode;
                                while (tmpNode2.ParentNode != null && tmpNode2.Attributes["name"].Value != im.Name)
                                {
                                    link = "../" + link;
                                    tmpNode2 = tmpNode2.ParentNode;
                                }
                            }

                            XmlNode newNode = img.CreateElement("uol");
                            newNode.Attributes.Append(img.CreateAttribute("name"));
                            newNode.Attributes.Append(img.CreateAttribute("value"));
                            newNode.Attributes["name"].Value = name;
                            newNode.Attributes["value"].Value = link;
                            inoutlinkNode.ParentNode.AppendChild(newNode);
                            inoutlinkNode.ParentNode.RemoveChild(inoutlinkNode);
                        }

                        // remove any of these int properties that are N/A for v83
                        foreach (XmlNode fixedGradeNode in img.SelectNodes("//int[@name='fixedGrade' or @name='fixedPotential' or @name='specialGrade' or @name='exItem' or @name='charmEXP' or @name='charismaEXP' or @name='willEXP' or @name='setItemID' or @name='epicItem']"))
                            fixedGradeNode.ParentNode.RemoveChild(fixedGradeNode);

                        WzImage newImage = (WzImage)new MapleLib.WzLib.Serialization.WzXmlDeserializer(false, null).ParseXML(img.OuterXml)[0];
                        newImage.ParseImage();
                        toDirectory.AddImage(newImage);
                        im.UnparseImage();

                        if (includeString)
                        {
                            //possible the string for the item hasn't been added yet for the new version
                            WzSubProperty fromStringProp = (WzSubProperty)fromStringImg["Eqp"][dirs[i]][Convert.ToInt32(im.Name.Replace(".img", "")).ToString()];
                            if (fromStringProp != null)
                            {
                                WzSubProperty toStringProp = (WzSubProperty)fromStringProp.DeepClone();
                                WzSubProperty temp = (WzSubProperty)toStringImg["Eqp"][dirs[i]];
                                temp.AddProperty(toStringProp);
                                toStringImg.Changed = true;
                            }
                        }
                    }
                }
            }
            form.UpdateProgress("Saving " + outputFileName + " ...");
            import_to_Wz.SaveToDisk(outputFileName, false);
            if (includeString)
                import_to_StringWz.SaveToDisk(outputFileName.Replace("Character.wz", "String.wz"), false);
            if (linkErrors.Count > 0)
                File.WriteAllLines(linkNodeErrorsFileName, linkErrors);
            form.UpdateProgress("Ready");
        }

        private bool ValidateLink(XmlNode linkNode, ref string link)
        {
            string tmpLink = link;
            try
            {
                while (tmpLink.Contains("../"))
                {
                    linkNode = linkNode.ParentNode;
                    tmpLink = tmpLink.Remove(tmpLink.IndexOf("../"), 3);
                }
                tmpLink = ConvertToXpath(tmpLink);
                if (linkNode is XmlDocument)
                    tmpLink = "*/" + tmpLink;
                if (linkNode.SelectSingleNode(tmpLink) != null)
                {
                    if (linkNode.SelectSingleNode(tmpLink).Name == "canvas")
                        return true;
                    else if (linkNode.SelectSingleNode(tmpLink).Name == "uol")
                    {
                        link = linkNode.SelectSingleNode(tmpLink).Attributes["value"].Value;
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        private string ConvertToXpath(string path)
        {
            string xpath = "";
            string[] names = path.Split('/');
            foreach(string name in names)
            {
                xpath += "*[@name='" + name + "']/";
            }
            if(xpath.EndsWith("/"))
                xpath = xpath.Substring(0, xpath.Length - 1);
            return xpath;
        }
    }
}