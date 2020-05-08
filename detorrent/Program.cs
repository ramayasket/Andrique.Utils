using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kw.Common;

namespace detorrent
{
	class Program
	{
		#region

		private const string TORRENTS = @"
2000 - Dune Soundtrack
24 [Season 2] (www.kinokopilka.tv)
24 [Season 3] (www.kinokopilka.tv)
A Discovery of Witches (Seаsоn 01 Episode 01-02) (www.kinokopilka.pro)
ABBYY Lingvo X6 Multilingual Professional
Absolute Zero (www.kinokopilka.pro)
Acrobat.XI.Pro
Actual Multiple Monitors v8.11
Adobe Acrobat Pro DC v2018.011.20040 RePack by KpoJIuK
Amethystium - Transience - 2014
Amethystium
Amy Macdonald
Anaconda.1997.x264.BDRip.(1080p).mkv
Astra
Atom (www.kinokopilka.pro)
Battlefield Earth A Saga of the Year 3000.2000.BDRip.avi
BBC - Order and Disorder
BBC-Atom__aty-sasa
BBC. Нефтяная планета.2015.XviD.SATRip
BBC. Свет и тьма. 2013.XviD.SATRip
BBC.The.Secret.Life.of.Chaos.PDTV.x264.AC3.[rus].mkv
Beta Test (www.kinokopilka.pro)
Blackmore's Night - Night With All Our Yesterdays (2015)
Boyazn.paukov.1990.TRIPLE.BDRip.XviD.AC3.-HQCLUB.avi
Candice Night (2011-2015)
CorelDRAW Graphics Suite 2018 v20.0.0.633 RePack by ALEX
Deep Impact (www.kinokopilka.tv)
Destination Wedding (www.kinokopilka.pro)
DevExpress Universal Complete 16.1.5 Build 20160802
DIO - Discography (10 studio albums) - 1983-2004 (оригинал) ALAC, lossless
E-Type_all
Edge of Tomorrow [BDRip-1080p] (www.kinokopilka.tv)
Enigma - iTunes Discography (1990-2016)
Enigma
Ennio Morricone & VA - The Hateful Eight (2015) [flac]
en_forefront_threat_management_gateway_2010_enterprise_x64_dvd_456235.iso
Every Day  (www.kinokopilka.pro)
Firefly S1 01-08[KinoKopilka]
Firefly S1 09-15[KinoKopilka]
GoodSync Enterprise v10.2.3.3 Final Ml_Rus
GoodSync Enterprise v10.8.1.1 Final Ml_Rus
GrindEQ Math Utils 2007
Hitler - The Rise of Evil (www.kinokopilka.pro)
HWMonitor PRO v1.30 Final Eng
Icecream Ebook Reader 5.0.4 RePack (& Portable) by TryRooM
Image-Line.FL.Studio.Producer.Edition.v12.5.1.5.FIXED-R2R
Impact (www.kinokopilka.tv)
Infovox Desktop 2.220 Engine
Inuyashiki (www.kinokopilka.pro)
Jeepers Creepers 3 [BDRip-720p] (www.kinokopilka.pro)
Jeepers Creepers II [DVDRip-avi] (www.kinokopilka.ru)
JetBrains ReSharper Ultimate 2016.3.2 Eng
JetBrains ReSharper Ultimate 2017.3.3
Joe Dassin 1980 (vinyl rip)
Joe_Dassin___Poyot_1980
Julija_Latynina__Ohota_na_izjubrja.zip
Jurassic Park-III.2001.BD-Rip AVC-x264.AC3.mkv
Jurassic World - Fallen Kingdom [WEB-DL-1080p] (www.kinokopilka.pro)
Kendra Springer - Faith (2012)
King Kong [BDrip-1080p] (www.kinokopilka.tv)
Klaus Schulze - Androgyn (2017)
Korona Rossiyskoy imperii, ili snova neulovimye[kinokopilka]
Kosheen - Solitude - 2013
Kyoryuu Kaicho no densetsu (Legend of Dinosaurs and Monster Birds) [2xRu-Jp-En x264 712x372@844x372 DVDRip tRuAVC].mkv
Ladder
Leawo Prof. Media 7.8.0.0
Loreena McKennitt (1985-2014)
MagdalenGraal
Microsoft Flight Simulator X SP1 ENG & MFSX Acceleration Expansion ENG [torrents.ru]
Microsoft Flight
Microsoft Office 2019 Professional Plus 16.0.10730.20102 RTM-Retail
Mile 22 (www.kinokopilka.pro)
Najoua Belyzel - Discography(2 albums+singles)
Neulovimie mstiteli[kinokopilka]
Nusrat Fateh Ali Khan (1987-2008)
Nusrat Fateh Ali Khan - Allah Hoo Allah Hoo
oCam Screen Recorder 366.0 + Portable
Omnia
PaxAmericana
PBS Nova - Absolute Zero
Pervobitnoe.zlo.2007.x264.BDRip.1080p-kinozal.tv-HD.mkv
Pirates of the Caribbean - On Stranger Tides [BDRip-1080p] (www.kinokopilka.tv)
Priklyucheniya Sherloka Kholmsa i doktora Vatsona[Kinokopilka]
Rammstein - iTunes Discography
Rammstein
Ryibka_N._Dnevnik_Po_Soblazneniyu_M.fb2
Salyut.7.WEB-DL.1080p.m4v
Searching (www.kinokopilka.pro)
Serenity [BDRip-720p] (www.kinokopilka.ru)
Sherlok Kholms i doktor Vatson[Kinokopilka]
Skjelvet (www.kinokopilka.pro)
Slumber (www.kinokopilka.pro)
Sobaka Baskerviley[Kinokopilka]
Soulful Ibiza Sound (2017)
Stardock Start10 1.55 RePack by Tyran.exe
T2 Trainspotting [BDRip-1080p] (www.kinоkopilkа.pro)
The Dune Universe
The Great Race
The Last Stand (HD).m4v
The Lion Guard. Return of the Roar.2015.WEB-DL 1080p.mkv
The Mist [BDRip-720p](www.kinokopilka.tv)
The Nun (www.kinokopilka.pro)
The Predator (www.kinokopilka.pro)
The Rider (www.kinokopilka.pro)
The.Final.Countdown.1980.1080p.Blu-Ray.DTS.x264-iLL.mkv
They [BDRip-1080p] (www.kinokopilka.pro)
Thursday [BDRip-1080p] (www.kinokopilka.tv)
UFO (www.kinokopilka.pro)
uTorrentPro3.5.3.44428.exe
Voyage of Time - Life's Journey [BDRip-720p] (www.kinоkopilkа.pro)
Winamp Pro v5.666 Build 3516 Final Ml_Rus (Patched)
Winamp Pro v5.666 Build 3516 Final Ml_Rus
Winamp
Windows 10 Enterprise LTSC 2019 RS5 [Ru] [En] MSDN ISO ORIGINAL
WinISO Standard v6.4.0.5170 Final Ml_Rus
WinRAR 5.31 Final
XP11_11R2_DLC_EU_v2.iso
XX vek nachinaetsja
[RUS] World War Three - Inside the War Room.m4a
c Zodiac (AT-D-Luxman)
Айд_Рэнд_-_Атлант_расправил_плечи_-_2011.nodrm.epub
Грибной человек.mpg
Домашняя бухгалтерия v5.3.0.83 Final Ml_Rus
Карнап Р. Значение и необходимость. Исследование по семантике и модальной логике. 1959.djvu
Линда
Мартин Джордж Р.Р. - Песнь Льда и Пламени
Михаил Хазин, Сергей Щеглов - Лестница в небо (Владимир Левашев)
Приключения Шерлока Холмса и доктора Ватсона - Сокровища Агры (www.kinokopilka.tv)
Рассел Б. - Основания математики
Русалка. Озеро мертвых (www.kinokopilka.pro)
Телохранитель. Сериал. 2018. NewStudio (WEB-DL 1080p)
Фрэнк Герберт - Хроники Дюны
Хор братии Валаамского монастыря
";
		#endregion

		static void Main(string[] args)
		{
			var torrents = TORRENTS.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
			var files = new DirectoryInfo("C:\\inbox").EnumerateFileSystemInfos().ToArray();

			foreach (var file in files)
			{
				if(file.Name.StartsWith("."))
					continue;

				if (torrents.Contains(file.Name))
				{
					//file.Attributes = FileAttributes.Archive;
				}
				else
				{
					var from = Path.Combine("C:\\inbox", file.Name);
					var to = Path.Combine("C:\\inbox\\.inbox.unallocated", file.Name);

					if (file is FileInfo)
					{
						File.Move(from, to);
					}
					else
					{
						Directory.Move(from, to);
					}
				}
			}
		}
	}
}
