﻿//
// ClanConverter.cs
//
// Author:
//       Urist_McAurelian <Discord: Urist_McAurelian#2289>
//
// Copyright (c) 2021 Urist_McAurelian
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using CsvHelper;
using static XmlTool.Program;

namespace XmlTool {
	//Functionally same result after running through twice.
	public class ClanConverter {
		public static void CSVtoXML(string fileInput, string fileOutput, XmlWriter localizationWriter, XmlWriter module_strings_writer) {
			StreamReader reader = new StreamReader(fileInput);
			CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);

			ClanRecord kingdomsRecord = new ClanRecord();
			IEnumerable<ClanRecord> records = csv.EnumerateRecords(kingdomsRecord);

			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;

			using (XmlWriter writer = XmlWriter.Create(fileOutput, settings)) {
				writer.WriteStartElement("Factions");

				writeHeadderComment(writer);

				foreach (ClanRecord record in records) {
					if (record.id.Equals("TODO")) break;
					if (record.id.Equals("VANILLA")) break;
					if (record.id.Equals("")) continue;

					writer.WriteStartElement("Faction");

					//Changes
					record.is_bandit = record.is_bandit.ToLower();
					record.is_clan_type_mercenary = record.is_clan_type_mercenary.ToLower();
					record.is_mafia = record.is_mafia.ToLower();
					record.is_minor_faction = record.is_minor_faction.ToLower();
					record.is_nomad = record.is_nomad.ToLower();
					record.is_outlaw = record.is_outlaw.ToLower();
					record.is_sect = record.is_sect.ToLower();

					//Defaults
					if (record.tier.Equals("")) record.tier = "4";

					//Temporary

					//Write
					writer.WriteAttributeString("id", record.id);
					writer.WriteAttributeString("tier", record.tier);
					if (!record.owner.Equals("")) writer.WriteAttributeString("owner", "Hero." + record.owner);
					if (!record.label_color.Equals("")) writer.WriteAttributeString("label_color", record.label_color);
					if (!record.color.Equals("")) writer.WriteAttributeString("color", record.color);
					if (!record.color2.Equals("")) writer.WriteAttributeString("color2", record.color2);
					if (!record.alternative_color.Equals("")) writer.WriteAttributeString("alternative_color", record.alternative_color);
					if (!record.alternative_color2.Equals("")) writer.WriteAttributeString("alternative_color2", record.alternative_color2);
					if (!record.culture.Equals("")) writer.WriteAttributeString("culture", "Culture." + record.culture);
					if (!record.super_faction.Equals("")) writer.WriteAttributeString("super_faction", "Kingdom." + record.super_faction);
					if (!record.default_party_template.Equals("")) writer.WriteAttributeString("default_party_template", "PartyTemplate." + record.default_party_template);
					if (!record.is_bandit.Equals("")) writer.WriteAttributeString("is_bandit", record.is_bandit);
					if (!record.is_minor_faction.Equals("")) writer.WriteAttributeString("is_minor_faction", record.is_minor_faction);
					if (!record.is_outlaw.Equals("")) writer.WriteAttributeString("is_outlaw", record.is_outlaw);
					if (!record.is_clan_type_mercenary.Equals("")) writer.WriteAttributeString("is_clan_type_mercenary", record.is_clan_type_mercenary);
					if (!record.is_nomad.Equals("")) writer.WriteAttributeString("is_nomad", record.is_nomad);
					if (!record.is_sect.Equals("")) writer.WriteAttributeString("is_sect", record.is_sect);
					if (!record.is_mafia.Equals("")) writer.WriteAttributeString("is_mafia", record.is_mafia);
					if (!record.settlement_banner_mesh.Equals("")) writer.WriteAttributeString("settlement_banner_mesh", record.settlement_banner_mesh);
					if (!record.flag_mesh.Equals("")) writer.WriteAttributeString("flag_mesh", record.flag_mesh);

					if (!record.name.Equals("")) writer.WriteAttributeString("name", GetLocalizedString(localizationWriter, record.name, record.id, "name", "Factions.Faction"));
					if (!record.short_name.Equals("")) writer.WriteAttributeString("short_name", GetLocalizedString(localizationWriter, record.short_name, record.id, "short_name", "Factions.Faction"));
					if (!record.text.Equals("")) writer.WriteAttributeString("text", GetLocalizedString(localizationWriter, record.text, record.id, "text", "Factions.Faction"));

					if (!record.initial_posX.Equals("")) writer.WriteAttributeString("initial_posX", record.initial_posX);
					if (!record.initial_posY.Equals("")) writer.WriteAttributeString("initial_posY", record.initial_posY);

					if (!record.banner_key.Equals("")) writer.WriteAttributeString("banner_key", record.banner_key);

					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}
		}
		public static void XMLtoCSV(string xmlInput, string csvOutput) {
			List<ClanRecord> records = new List<ClanRecord>();

			using (XmlReader root = XmlReader.Create(xmlInput)) {
				root.MoveToContent();
				while (root.Read()) {
					if (root.NodeType.Equals(XmlNodeType.Element)) break;
				}

				while (!root.EOF) {
					if (root.NodeType != XmlNodeType.Element) {
						root.Read();
						continue;
					}
					ClanRecord record = new ClanRecord();

					record.id = root.GetAttribute("id");
					record.tier = root.GetAttribute("tier");
					record.owner = TrimD(root.GetAttribute("owner"));
					record.label_color = root.GetAttribute("label_color");
					record.color = root.GetAttribute("color");
					record.color2 = root.GetAttribute("color2");
					record.alternative_color = root.GetAttribute("alternative_color");
					record.alternative_color2 = root.GetAttribute("alternative_color2");
					record.culture = TrimD(root.GetAttribute("culture"));
					record.super_faction = TrimD(root.GetAttribute("super_faction"));
					record.default_party_template = TrimD(root.GetAttribute("default_party_template"));
					record.is_bandit = root.GetAttribute("is_bandit");
					record.is_minor_faction = root.GetAttribute("is_minor_faction");
					record.is_outlaw = root.GetAttribute("is_outlaw");
					record.is_clan_type_mercenary = root.GetAttribute("is_clan_type_mercenary");
					record.is_nomad = root.GetAttribute("is_nomad");
					record.is_sect = root.GetAttribute("is_sect");
					record.is_mafia = root.GetAttribute("is_mafia");
					record.settlement_banner_mesh = root.GetAttribute("settlement_banner_mesh");
					record.flag_mesh = root.GetAttribute("flag_mesh");
					record.name = TrimB(root.GetAttribute("name"));
					record.short_name = TrimB(root.GetAttribute("short_name"));
					record.text = TrimB(root.GetAttribute("text"));

					record.initial_posX = root.GetAttribute("initial_posX");
					record.initial_posY = root.GetAttribute("initial_posY");
					record.banner_key = root.GetAttribute("banner_key");

					records.Add(record);
					root.Read();
				}
				using (CsvWriter csvWriter = new CsvWriter(new StreamWriter(csvOutput), CultureInfo.InvariantCulture)) {
					csvWriter.WriteRecords(records);
					csvWriter.Flush();
				}
			}
		}
		public class ClanRecord {
			public string id { get; set; }
			public string tier { get; set; }
			public string owner { get; set; }
			public string label_color { get; set; }
			public string color { get; set; }
			public string color2 { get; set; }
			public string alternative_color { get; set; }
			public string alternative_color2 { get; set; }
			public string culture { get; set; }
			public string super_faction { get; set; }
			public string default_party_template { get; set; }
			public string is_bandit { get; set; }
			public string is_minor_faction { get; set; }
			public string is_outlaw { get; set; }
			public string is_clan_type_mercenary { get; set; }
			public string is_nomad { get; set; }
			public string is_sect { get; set; }
			public string is_mafia { get; set; }
			public string settlement_banner_mesh { get; set; }
			public string flag_mesh { get; set; }
			public string name { get; set; }
			public string short_name { get; set; }
			public string text { get; set; }


			public string initial_posX { get; set; }
			public string initial_posY { get; set; }
			public string banner_key { get; set; }
		}
	}
}
