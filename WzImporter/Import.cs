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
using MapleLib.WzLib.Serialization;
using System.Reflection;

namespace WzImporter
{
    internal class Import
    {
        public bool[] selections = new bool[14];
        public bool cashOnly = false;
        public bool includeString = false;
        public string inputFromFileName = "";
        public string inputToFileName = "";
        public string outputDir = "";

        private string outputFileName = "";
        private string linkNodeErrorsFileName = "";
        private string[] dirs = { "Hair", "Face", "Cap", "Coat", "Pants", "Longcoat", "Shoes", "Cape", "Shield", "Ring", "Accessory", "PetEquip", "Glove", "Weapon" };
        private List<string> linkErrors = new List<string>();
        private WzFile import_from_Wz;
        private WzFile import_to_Wz;
        private WzFile import_from_StringWz;
        private WzFile import_to_StringWz;

        public void ImportXML(Form1 form)
        {
            try
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
                import_from_Wz = new WzFile(inputFromFileName, WzMapleVersion.BMS);
                import_to_Wz = new WzFile(inputToFileName, WzMapleVersion.GMS);
                import_from_StringWz = new WzFile(inputFromFileName, WzMapleVersion.BMS);
                import_to_StringWz = new WzFile(inputToFileName, WzMapleVersion.GMS);

                WzClassicXmlSerializer s = new WzClassicXmlSerializer(0, LineBreak.None, true);

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

                        //special face handling for speed and space efficiency. don't fetch these from Accessory.img. Add face 19999.img and append all the emotes there for reference within Face.img           
                        if (dirs[i] == "Face")
                        {
                            if (import_to_Wz["Face"]["00019999.img"] == null)
                            {
                                XmlDocument emoteReferenceFaceDoc = new XmlDocument();
                                using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("WzImporter.00019999.img.xml"))
                                {
                                    using (StreamReader sr = new StreamReader(stream))
                                    {
                                        emoteReferenceFaceDoc.LoadXml(sr.ReadToEnd());
                                    }
                                }
                                WzImage emoteFaceImg = (WzImage)new WzXmlDeserializer(false, null).ParseXML(emoteReferenceFaceDoc.OuterXml)[0];
                                emoteFaceImg.ParseImage();
                                toDirectory.AddImage(emoteFaceImg);
                            }
                        }

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
                            img.LoadXml(s.exportXml(im));

                            // Skip any non-cash items if checked. Hair and Face may not be marked as cash, but all should be processed.
                            if (dirs[i] != "Hair" && dirs[i] != "Face")
                            {
                                if (cashOnly && img.SelectSingleNode("//int[@name='cash']") != null && img.SelectSingleNode("//int[@name='cash']").Attributes["value"].Value != "1")
                                {
                                    im.UnparseImage();
                                    continue;
                                }
                            }

                            // convert _inlink and _outlink nodes using WzUOLProperty supported in v83
                            ProcessInLinks(ref img, i, s, im, false);
                            ProcessOutLinks(ref img, i, s, im, false);

                            // remove any of these int properties that are N/A for v83
                            foreach (XmlNode fixedGradeNode in img.SelectNodes("//int[@name='fixedGrade' or @name='fixedPotential' or @name='specialGrade' or @name='exItem' or @name='charmEXP' or @name='charismaEXP' or @name='willEXP' or @name='setItemID' or @name='epicItem']"))
                                fixedGradeNode.ParentNode.RemoveChild(fixedGradeNode);

                            // Second pass to account for _inlink or _outlink pointing to another _inlink or _outlink -- not supported in v83
                            ProcessInLinks(ref img, i, s, im, true);
                            ProcessOutLinks(ref img, i, s, im, true);

                            // One last pass for uol links in newer versions that point to other links.
                            foreach (XmlNode uolLink in img.SelectNodes("//imgdir/uol"))
                            {
                                string link = uolLink.Attributes["value"].Value;
                                XmlNode tempNode = uolLink.ParentNode;
                                while (tempNode != null && link.Contains("../"))
                                {
                                    tempNode = tempNode.ParentNode;
                                    link = link.Remove(0, 3);
                                }
                                if (UolPointsToUol(tempNode, ref link))
                                {
                                    uolLink.Attributes["value"].Value = link;
                                }
                            }

                            WzImage newImage = (WzImage)new WzXmlDeserializer(false, null).ParseXML(img.OuterXml)[0];
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                import_to_Wz.Dispose();
                import_from_Wz.Dispose();
                import_from_StringWz.Dispose();
                import_to_StringWz.Dispose();
            }
        }

        private bool ValidateLink(XmlNode linkNode, ref string link)
        {
            string tmpLink = link;
            try
            {
                tmpLink = link.Replace("../", "");
                tmpLink = ConvertToXpath(tmpLink);
                if (linkNode is XmlDocument)
                    tmpLink = "*/" + tmpLink;
                if (linkNode.SelectSingleNode(tmpLink) != null)
                {
                    if (linkNode.SelectSingleNode(tmpLink).Name == "canvas")
                        return true;
                    else if (linkNode.SelectSingleNode(tmpLink).Name == "_inlink" || linkNode.SelectSingleNode(tmpLink).Name == "_outlink")
                    {
                        return false; //will catch it in round 2.
                    }
                    else if(linkNode.SelectSingleNode(tmpLink).Name == "uol")
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

        private bool UolPointsToUol(XmlNode linkNode, ref string link)
        {
            string tmpLink = link;
            try
            {
                tmpLink = link.Replace("../", "");
                tmpLink = ConvertToXpath(tmpLink);
                if (linkNode is XmlDocument)
                    tmpLink = "*/" + tmpLink;
                if (linkNode.SelectSingleNode(tmpLink) != null)
                {
                    if (linkNode.SelectSingleNode(tmpLink).Name == "uol")
                    {
                        link = linkNode.SelectSingleNode(tmpLink).Attributes["value"].Value;
                        return true;
                    }
                }
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
            foreach (string name in names)
            {
                xpath += "*[@name='" + name + "']/";
            }
            if (xpath.EndsWith("/"))
                xpath = xpath.Substring(0, xpath.Length - 1);
            return xpath;
        }

        private void ProcessInLinks(ref XmlDocument img, int i, WzClassicXmlSerializer s, WzImage im, bool secondPass)
        {
            // convert _inlink nodes using WzUOLProperty supported in v83
            foreach (XmlNode inLinkNode in img.SelectNodes("//canvas[string[@name='_inlink']]"))
            {
                string name = inLinkNode.Attributes["name"].Value;
                string link = inLinkNode.SelectSingleNode("string[@name='_inlink']").Attributes["value"].Value;

                XmlNode tmpNode = inLinkNode;
                while (tmpNode.ParentNode != null && tmpNode.Name != "imgdir") //move up to the nearest imgdir node to count from.
                {
                    tmpNode = tmpNode.ParentNode;
                }

                while (tmpNode.ParentNode != null && tmpNode.Attributes["name"].Value != im.Name)
                {
                    link = "../" + link;
                    tmpNode = tmpNode.ParentNode;
                }

                if (!ValidateLink(tmpNode, ref link))
                {
                    if(secondPass)
                        linkErrors.Add("Link Error: " + im.Name + " - " + inLinkNode.Name + " | " + link);
                    continue; //first pass
                }
                XmlNode newNode = img.CreateElement("uol");
                newNode.Attributes.Append(img.CreateAttribute("name"));
                newNode.Attributes.Append(img.CreateAttribute("value"));
                newNode.Attributes["name"].Value = name;
                newNode.Attributes["value"].Value = link;
                inLinkNode.ParentNode.AppendChild(newNode);
                inLinkNode.ParentNode.RemoveChild(inLinkNode);
            }
        }

        private void ProcessOutLinks(ref XmlDocument img, int i, WzClassicXmlSerializer s, WzImage im, bool secondPass)
        {
            foreach (XmlNode outLinkNode in img.SelectNodes("//canvas[string[@name='_outlink']]"))
            {
                string name = outLinkNode.Attributes["name"].Value;
                string link = outLinkNode.SelectSingleNode("string[@name='_outlink']").Attributes["value"].Value;

                XmlNode tmpNode = outLinkNode;
                if (!link.Contains(dirs[i]))
                {
                    // if the reference is outside of this directory, WzUOLProperty cannot work, so fetch the actual image

                    //check if this is an accessory being referenced for a face and ref 0019999.img
                    if (dirs[i] == "Face")
                    {
                        if (link.StartsWith("Character/Accessory/01010009.img"))
                        {
                            link = link.Replace("Character/Accessory/01010009.img", "../../../00019999.img");
                            link = link.Replace("/default", "/face");
                            XmlNode newFaceNode = img.CreateElement("uol");
                            newFaceNode.Attributes.Append(img.CreateAttribute("name"));
                            newFaceNode.Attributes.Append(img.CreateAttribute("value"));
                            newFaceNode.Attributes["name"].Value = name;
                            newFaceNode.Attributes["value"].Value = link;
                            outLinkNode.ParentNode.AppendChild(newFaceNode);
                            outLinkNode.ParentNode.RemoveChild(outLinkNode);
                            continue;
                        }
                    }

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

                    outLinkNode.ParentNode.AppendChild(newNodeFromRef);
                    outLinkNode.ParentNode.RemoveChild(outLinkNode);
                    refImage.UnparseImage();
                    continue;
                }
                else if (link.Contains("Character/"))
                {
                    //outlink to another image in the same directory. <uol> will work, but not as an absolute path.
                    link = link.Replace("Character/", "");
                    link = link.Replace(dirs[i] + "/", "");
                    XmlNode tmpNode2 = outLinkNode;
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
                outLinkNode.ParentNode.AppendChild(newNode);
                outLinkNode.ParentNode.RemoveChild(outLinkNode);
            }
        }
    }
}