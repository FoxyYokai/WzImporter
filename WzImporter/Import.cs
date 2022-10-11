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
        public bool onlyThese = false;
        public List<string> onlyTheseItems = new List<string>();
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
        private string prevTmpFileName = "";

        public void ImportXML(MainForm form)
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
                import_from_Wz.ParseWzFile();
                import_to_Wz.ParseWzFile();

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

                if (onlyThese)
                {
                    FillDirsFromList(onlyTheseItems);
                }

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
                        List<WzImage> imgsToAdd = new List<WzImage>();
                        List<WzImage> fromImages = fromDirectory.WzImages;
                        List<WzImage> toImages = toDirectory.WzImages;
                        if (onlyThese)
                        {
                            List<string> relevantItems = new List<string>();
                            foreach (string item in onlyTheseItems)
                                if (EquipTypeFromId(item.Replace(".img", "")) == i)
                                    relevantItems.Add(item);
                            imgsToAdd = fromImages
                                .Where(x => relevantItems.Contains(x.Name)).ToList();
                        }
                        else
                        {
                            List<string> toImagesNames = new List<string>();
                            foreach (WzImage im in toImages) toImagesNames.Add(im.Name);
                            imgsToAdd = fromImages
                                .Where(x => !toImagesNames.Contains(x.Name)).ToList();
                        }

                        if (includeString)
                        {
                            toStringImg = (WzImage)import_to_StringWz["Eqp.img"];
                            fromStringImg = (WzImage)import_from_StringWz["Eqp.img"];
                            toStringImg.ParseImage();
                            fromStringImg.ParseImage();
                        }

                        int imCount = 1;
                        int count = 1;
                        foreach (WzImage im in imgsToAdd)
                        {
                            im.ParseImage();

                            form.UpdateProgress("Working on " + dirs[i] + "..." + im.Name + " ( " + imCount + " of " + imgsToAdd.Count + " )");
                            imCount++;
                            count++;

                            //work in 300 image chunk sizes to keep memory usage reasonable
                            if (count == 300)
                            {
                                count = 1;
                                SaveTempFile(ref toDirectory);
                                toDirectory = (WzDirectory)import_to_Wz[dirs[i]];
                            }

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
                                int itemId = 0;
                                if (Int32.TryParse(im.Name.Replace(".img", ""), out itemId))
                                {
                                    WzSubProperty fromStringProp = (WzSubProperty)fromStringImg["Eqp"][dirs[i]][itemId.ToString()];
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
                }

                // Save and Clean up
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
                form.UpdateProgress("Ready");
            }
            finally
            {
                import_to_Wz.Dispose();
                import_from_Wz.Dispose();
                import_from_StringWz.Dispose();
                import_to_StringWz.Dispose();
                if (prevTmpFileName != "")
                {
                    File.Delete(prevTmpFileName);
                }
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
                XmlNode myOriginNode = inLinkNode.SelectSingleNode("vector[@name='origin']");
                XmlNode myZNode = inLinkNode.SelectSingleNode("*[@name='z']");
                string myOriginX = "";
                string myOriginY = "";
                string myZ = "";
                if (myOriginNode != null)
                {
                    myOriginX = myOriginNode.SelectSingleNode("@x").Value;
                    myOriginY = myOriginNode.SelectSingleNode("@y").Value;
                }
                if (myZNode != null)
                {
                    myZ = myZNode.SelectSingleNode("@value").Value;
                }

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

                // it's possible for newer versions to ref an image and all they change is the origin (common with weapons)
                // v83 doesn't support this, so need to make a copy of the ref'd image and change the origin
                // if the origin of the referenced node doesn't exist, assume we need to keep ours.
                string outOriginX = "";
                string outOriginY = "";
                string outZ = "";
                XmlNode originNode = tmpNode.SelectSingleNode(ConvertToXpath(link.Replace("../", "")));
                if (originNode != null)
                    originNode = originNode.SelectSingleNode("vector[@name='origin']");
                XmlNode zNode = tmpNode.SelectSingleNode(ConvertToXpath(link.Replace("../", "")));
                if (zNode != null)
                    zNode = zNode.SelectSingleNode("*[@name='z']");
                if (originNode != null)
                {
                    outOriginX = originNode.SelectSingleNode("@x").Value;
                    outOriginY = originNode.SelectSingleNode("@y").Value;
                }
                if (zNode != null)
                {
                    outZ = zNode.SelectSingleNode("@value").Value;
                }

                // the referenced image has a different origin or Z, so we have to make a copy and keep ours.
                if (myOriginX != outOriginX || myOriginY != outOriginY || myZ != outZ)
                {
                    string xpathFromLink = ConvertToXpath(link.Replace("../", ""));
                    XmlNode refNode = tmpNode.SelectSingleNode(xpathFromLink);
                    if (refNode != null)
                    {
                        refNode = refNode.CloneNode(true);
                        if (myOriginX != outOriginX || myOriginY != outOriginY)
                        {
                            refNode.SelectSingleNode("vector[@name='origin']/@x").Value = myOriginX;
                            refNode.SelectSingleNode("vector[@name='origin']/@y").Value = myOriginY;
                        }
                        if (myZ != outZ)
                        {
                            //some z properties are int. some are string. change depending on the value we need.
                            if (refNode.SelectSingleNode("*[@name='z']") != null)
                            {
                                refNode.SelectSingleNode("*[@name='z']/@value").Value = myZ;
                                if (!Int32.TryParse(myZ, out _))
                                {
                                    XmlNode newZNode = img.CreateElement("string");
                                    XmlNode oldZNode = refNode.SelectSingleNode("*[@name='z']");
                                    newZNode.Attributes.Append(img.CreateAttribute("name")).Value = oldZNode.Attributes["name"].Value;
                                    newZNode.Attributes.Append(img.CreateAttribute("value")).Value = oldZNode.Attributes["value"].Value;
                                    refNode.InsertAfter(newZNode, oldZNode);
                                    refNode.RemoveChild(oldZNode);
                                }
                                else
                                {
                                    XmlNode newZNode = img.CreateElement("int");
                                    XmlNode oldZNode = refNode.SelectSingleNode("*[@name='z']");
                                    newZNode.Attributes.Append(img.CreateAttribute("name")).Value = oldZNode.Attributes["name"].Value;
                                    newZNode.Attributes.Append(img.CreateAttribute("value")).Value = oldZNode.Attributes["value"].Value;
                                    refNode.InsertAfter(newZNode, oldZNode);
                                    refNode.RemoveChild(oldZNode);
                                }
                            }
                        }
                        inLinkNode.ParentNode.AppendChild(refNode);
                        inLinkNode.ParentNode.RemoveChild(inLinkNode);
                    }
                    continue;
                }


                if (!ValidateLink(tmpNode, ref link))
                {
                    if (secondPass)
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
                string myOriginX = "";
                string myOriginY = "";
                string myZ = "";
                XmlNode myOriginNode = outLinkNode.SelectSingleNode("vector[@name='origin']");
                XmlNode myZNode = outLinkNode.SelectSingleNode("*[@name='z']");
                if (myOriginNode != null)
                {
                    myOriginX = myOriginNode.SelectSingleNode("@x").Value;
                    myOriginY = myOriginNode.SelectSingleNode("@y").Value;
                }
                if (myZNode != null)
                {
                    myZ = myZNode.SelectSingleNode("@value").Value;
                }

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
                    string outOriginX = "";
                    string outOriginY = "";
                    string outZ = "";
                    if (refImage.GetFromPath(linksub)["origin"] != null) //if the referenced image doesn't have an origin, assume we need to keep ours.
                    {
                        outOriginX = ((System.Drawing.Point)refImage.GetFromPath(linksub)["origin"].WzValue).X.ToString();
                        outOriginY = ((System.Drawing.Point)refImage.GetFromPath(linksub)["origin"].WzValue).Y.ToString();
                    }
                    if (refImage.GetFromPath(linksub)["z"] != null) //if the referenced image doesn't have a z, we assume to keep ours
                    {
                        outZ = refImage.GetFromPath(linksub)["z"].WzValue.ToString();
                    }

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
                    // it's possible in later versions to ref an eternal image, but change the origin. not possible in v83
                    if (myOriginX != outOriginX || myOriginY != outOriginY)
                    {
                        if (newNodeFromRef.SelectSingleNode("vector[@name='origin']/@x") != null)
                            newNodeFromRef.SelectSingleNode("vector[@name='origin']/@x").Value = myOriginX;
                        if (newNodeFromRef.SelectSingleNode("vector[@name='origin']/@y") != null)
                            newNodeFromRef.SelectSingleNode("vector[@name='origin']/@y").Value = myOriginY;
                    }
                    if (myZ != outZ)
                    {
                        if (newNodeFromRef.SelectSingleNode("*[@name='z']/@value") != null)
                        {
                            //some z properties are int. some are string. change depending on the value we need.
                            newNodeFromRef.SelectSingleNode("*[@name='z']/@value").Value = myZ;
                            if (!Int32.TryParse(myZ, out _))
                            {
                                XmlNode newZNode = img.CreateElement("string");
                                XmlNode oldZNode = newNodeFromRef.SelectSingleNode("*[@name='z']");
                                newZNode.Attributes.Append(img.CreateAttribute("name")).Value = oldZNode.Attributes["name"].Value;
                                newZNode.Attributes.Append(img.CreateAttribute("value")).Value = oldZNode.Attributes["value"].Value;
                                newNodeFromRef.InsertAfter(newZNode, oldZNode);
                                newNodeFromRef.RemoveChild(oldZNode);
                            }
                            else
                            {
                                XmlNode newZNode = img.CreateElement("int");
                                XmlNode oldZNode = newNodeFromRef.SelectSingleNode("*[@name='z']");
                                newZNode.Attributes.Append(img.CreateAttribute("name")).Value = oldZNode.Attributes["name"].Value;
                                newZNode.Attributes.Append(img.CreateAttribute("value")).Value = oldZNode.Attributes["value"].Value;
                                newNodeFromRef.InsertAfter(newZNode, oldZNode);
                                newNodeFromRef.RemoveChild(oldZNode);
                            }
                        }
                    }

                    outLinkNode.ParentNode.AppendChild(newNodeFromRef);
                    outLinkNode.ParentNode.RemoveChild(outLinkNode);
                    refImage.UnparseImage();
                    continue;
                }
                else if (link.Contains("Character/"))
                {
                    //outlink to another image in the same directory. <uol> will work, but not as an absolute path.
                    string linkimg = link.Substring(0, link.IndexOf(".img/") + 4);
                    string linksub = link.Substring(link.IndexOf(".img/") + 5);
                    WzImage refImage = (WzImage)import_from_Wz.GetObjectFromPath(linkimg);
                    refImage.ParseImage();
                    string outOriginX = "";
                    string outOriginY = "";
                    string outZ = "";
                    if (refImage.GetFromPath(linksub)["origin"] != null) //if the referenced image doesn't have an origin, assume we need to keep ours.
                    {
                        outOriginX = ((System.Drawing.Point)refImage.GetFromPath(linksub)["origin"].WzValue).X.ToString();
                        outOriginY = ((System.Drawing.Point)refImage.GetFromPath(linksub)["origin"].WzValue).Y.ToString();
                    }
                    if (refImage.GetFromPath(linksub)["z"] != null)
                    {
                        outZ = refImage.GetFromPath(linksub)["z"].WzValue.ToString();
                    }

                    // it's possible for newer versions to ref an image and all they change is the origin (common with weapons)
                    // v83 doesn't support this, so need to make a copy of the ref'd image and change the origin
                    if (myOriginX != outOriginX || myOriginY != outOriginY || myZ != outZ)
                    {
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
                        if (myOriginX != outOriginX || myOriginY != outOriginY)
                        {
                            newNodeFromRef.SelectSingleNode("vector[@name='origin']/@x").Value = myOriginX;
                            newNodeFromRef.SelectSingleNode("vector[@name='origin']/@y").Value = myOriginY;
                        }
                        if (myZ != outZ)
                        {
                            //some z properties are int. some are string. change depending on the value we need.
                            if (newNodeFromRef.SelectSingleNode("*[@name='z']") != null)
                            {
                                newNodeFromRef.SelectSingleNode("*[@name='z']/@value").Value = myZ;
                                if (!Int32.TryParse(myZ, out _))
                                {
                                    XmlNode newZNode = img.CreateElement("string");
                                    XmlNode oldZNode = newNodeFromRef.SelectSingleNode("*[@name='z']");
                                    newZNode.Attributes.Append(img.CreateAttribute("name")).Value = oldZNode.Attributes["name"].Value;
                                    newZNode.Attributes.Append(img.CreateAttribute("value")).Value = oldZNode.Attributes["value"].Value;
                                    newNodeFromRef.InsertAfter(newZNode, oldZNode);
                                    newNodeFromRef.RemoveChild(oldZNode);
                                }
                                else
                                {
                                    XmlNode newZNode = img.CreateElement("int");
                                    XmlNode oldZNode = newNodeFromRef.SelectSingleNode("*[@name='z']");
                                    newZNode.Attributes.Append(img.CreateAttribute("name")).Value = oldZNode.Attributes["name"].Value;
                                    newZNode.Attributes.Append(img.CreateAttribute("value")).Value = oldZNode.Attributes["value"].Value;
                                    newNodeFromRef.InsertAfter(newZNode, oldZNode);
                                    newNodeFromRef.RemoveChild(oldZNode);
                                }
                            }
                        }
                        outLinkNode.ParentNode.AppendChild(newNodeFromRef);
                        outLinkNode.ParentNode.RemoveChild(outLinkNode);
                        refImage.UnparseImage();
                        continue;
                    }
                    else
                        refImage.UnparseImage();

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
        private void SaveTempFile(ref WzDirectory toDirectory)
        {
            string tmpFileName = Path.GetTempFileName();
            import_to_Wz.SaveToDisk(tmpFileName, false);
            import_to_Wz.Dispose();
            import_to_Wz = new WzFile(tmpFileName, WzMapleVersion.GMS);
            import_to_Wz.ParseWzFile();
            if (prevTmpFileName != "")
            {
                File.Delete(prevTmpFileName);
                prevTmpFileName = tmpFileName;
            }
            else
                prevTmpFileName = tmpFileName;
        }
        private void FillDirsFromList(List<string> onlyItems)
        {
            foreach (string item in onlyItems)
            {
                // { "Hair", "Face", "Cap", "Coat", "Pants", "Longcoat", "Shoes", "Cape", "Shield", "Ring", "Accessory", "PetEquip", "Glove", "Weapon" }
                int i = EquipTypeFromId(item.Replace(".img", ""));
                if (i == -1)
                {
                    onlyItems.Remove(item);
                    continue;
                }
                selections[i] = true;
            }
        }
        private int EquipTypeFromId(string id)
        {
            // { "Hair", "Face", "Cap", "Coat", "Pants", "Longcoat", "Shoes", "Cape", "Shield", "Ring", "Accessory", "PetEquip", "Glove", "Weapon" }
            if (!Int32.TryParse(id, out int i))
                return -1;

            if (i / 100000 >= 13)
            {
                return 13;  //weapons
            }
            else
            {
                switch (i / 10000)
                {
                    case (2):
                        return 1;  //face                        
                    case (3):
                        return 0;  //hair                        
                    case (100):
                        return 2;  //hat                       
                    case (101):
                    case (102):
                    case (103):
                    case (112):
                    case (113):
                    case (114):
                        return 10;  //accessory
                    case (104):
                        return 3;  //top  
                    case (105):
                        return 5;  //overall     
                    case (106):
                        return 4;  //pants  
                    case (107):
                        return 6;  //shoes      
                    case (108):
                        return 12;  //gloves   
                    case (109):
                        return 8; //shield
                    case (110):
                        return 7; //cape
                    case (111):
                        return 9;  //ring                       
                    case (180):
                        return 11;  //pet equip                        
                    default:
                        return -1; //unsupported or invalid id

                }
            }

        }
    }
}