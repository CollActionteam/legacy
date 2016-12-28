using CollAction.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollAction.Data.Geonames
{
    public class GeonamesImporter
    {
        private readonly ApplicationDbContext _context;

        public GeonamesImporter(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ImportLocationData()
        {
            string[] tables = new[] { "Locations", "LocationAlternateNames", "LocationContinents", "LocationCountries", "LocationLevel1", "LocationLevel2" };

            // Disable foreign keys for import, truncate
            await _context.Database.ExecuteSqlCommandAsync($"ALTER TABLE public.\"Projects\" DISABLE TRIGGER ALL;");
            foreach (string table in tables)
                await _context.Database.ExecuteSqlCommandAsync($"ALTER TABLE public.\"{table}\" DISABLE TRIGGER ALL;");
            foreach (string table in tables)
                await _context.Database.ExecuteSqlCommandAsync($"DELETE FROM public.\"{table}\";");

            // Import data
            await ImportRootLocations();
            await ImportContinents();
            await ImportCountries();
            await ImportLocationLevel1();
            await ImportLocationLevel2();
            await ImportAlternateNames();

            // Enable foreign keys
            await _context.Database.ExecuteSqlCommandAsync($"ALTER TABLE public.\"Projects\" DISABLE TRIGGER ALL;");
            foreach (string table in tables)
            {
                await _context.Database.ExecuteSqlCommandAsync($"ALTER TABLE public.\"{table}\" ENABLE TRIGGER ALL;");
                await _context.Database.ExecuteSqlCommandAsync($"REINDEX TABLE public.\"{table}\";");
                await _context.Database.ExecuteSqlCommandAsync($"VACUUM FULL ANALYZE public.\"{table}\";");
            }
        }

        private async Task ImportContinents()
        {
            IEnumerable<LocationContinent> continents =
                File.ReadLines($"Geonames{Path.DirectorySeparatorChar}continents.txt")
                    .Select(line =>
                    {
                        string[] parts = line.Split('\t');
                        return new LocationContinent()
                        {
                            Id = parts[0],
                            Name = parts[1],
                            LocationId = int.Parse(parts[2], NumberStyles.Integer)
                        };
                    });
            _context.LocationContinents.AddRange(continents);
            await _context.SaveChangesAsync();
            _context.DetachAll();
        }

        private async Task ImportCountries()
        {
            IEnumerable<LocationCountry> countries =
                File.ReadLines($"Geonames{Path.DirectorySeparatorChar}countryInfo.txt")
                    .Where(line => !line.StartsWith("#", StringComparison.Ordinal))
                    .Select(line =>
                    {
                        string[] parts = line.Split('\t');
                        return new LocationCountry()
                        {
                            Id = parts[0],
                            Name = parts[4],
                            CapitalCity = parts[5],
                            ContinentId = parts[8],
                            LocationId = int.Parse(parts[16], NumberStyles.Integer)
                        };
                    });
            _context.LocationCountries.AddRange(countries);
            await _context.SaveChangesAsync();
            _context.DetachAll();
        }

        private async Task ImportLocationLevel1()
        {
            IEnumerable<LocationLevel1> locationLevel1 =
                File.ReadLines($"Geonames{Path.DirectorySeparatorChar}admin1Codes.txt")
                    .Select(line =>
                    {
                        string[] parts = line.Split('\t');
                        return new LocationLevel1()
                        {
                            Id = parts[0],
                            Name = parts[1],
                            LocationId = int.Parse(parts[3], NumberStyles.Integer)
                        };
                    });
            _context.LocationLevel1.AddRange(locationLevel1);
            await _context.SaveChangesAsync();
            _context.DetachAll();
        }

        private async Task ImportLocationLevel2()
        {
            IEnumerable<LocationLevel2> locationLevel2 =
                File.ReadLines($"Geonames{Path.DirectorySeparatorChar}admin2Codes.txt")
                    .Select(line =>
                    {
                        string[] parts = line.Split('\t');
                        return new LocationLevel2()
                        {
                            Id = parts[0],
                            Name = parts[1],
                            LocationId = int.Parse(parts[3], NumberStyles.Integer)
                        };
                    })
                    .ToList();

            const int batchSize = 100;
            while (locationLevel2.Any())
            {
                IEnumerable<LocationLevel2> batch = locationLevel2.Take(batchSize);
                locationLevel2 = locationLevel2.Skip(batchSize);
                _context.LocationLevel2.AddRange(batch);
                await _context.SaveChangesAsync();
                _context.DetachAll();
            }
        }

        private async Task ImportAlternateNames()
        {
            IEnumerable<LocationAlternateName> names =
                LinesFromZip($"Geonames{Path.DirectorySeparatorChar}alternateNames.zip", "alternateNames.txt")
                .Select(line =>
                {
                    string[] parts = line.Split('\t');
                    return new LocationAlternateName()
                    {
                        Id = int.Parse(parts[0], NumberStyles.Integer),
                        LocationId = int.Parse(parts[1], NumberStyles.Integer),
                        LanguageCode = parts[2],
                        AlternateName = parts[3],
                        IsPreferredName = parts[4].Equals("1", StringComparison.Ordinal),
                        IsShortName = parts[5].Equals("1", StringComparison.Ordinal),
                        IsColloquial = parts[6].Equals("1", StringComparison.Ordinal),
                        IsHistoric = parts[7].Equals("1", StringComparison.Ordinal),
                    };
                });

            const int batchSize = 100;
            while (names.Any())
            {
                IEnumerable<LocationAlternateName> batch = names.Take(batchSize);
                names = names.Skip(batchSize);
                _context.LocationAlternateNames.AddRange(batch);
                await _context.SaveChangesAsync();
                _context.DetachAll();
            }
        }

        private async Task ImportRootLocations()
        {
            IEnumerable<Location> locations =
                LinesFromZip($"Geonames{Path.DirectorySeparatorChar}allCountries.Zip", "allCountries.txt")
                .Select(line =>
                {
                    string[] parts = line.Split('\t');
                    return new Location()
                    {
                        Id = int.Parse(parts[0], NumberStyles.Integer),
                        Name = parts[1],
                        Latitude = decimal.Parse(parts[4], NumberStyles.Float),
                        Longitude = decimal.Parse(parts[5], NumberStyles.Float),
                        FeatureClass = parts[6].Length == 0 ? LocationFeatureClass.Unknown : FeatureClassCodes[parts[6][0]],
                        Feature = string.IsNullOrWhiteSpace(parts[7]) ? LocationFeature.NotAvailable : FeatureCodes[parts[7]],
                        CountryId = parts[8],
                        Level1Id = string.IsNullOrWhiteSpace(parts[10]) ? null : parts[10],
                        Level2Id = string.IsNullOrWhiteSpace(parts[11]) ? null : parts[11],
                        TimeZone = parts[17],
                    };
                });

            const int batchSize = 100;
            while (locations.Any())
            {
                IEnumerable<Location> batch = locations.Take(batchSize);
                locations = locations.Skip(batchSize);
                _context.Locations.AddRange(batch);
                await _context.SaveChangesAsync();
                _context.DetachAll();
            }
        }

        private IEnumerable<string> LinesFromZip(string zipFile, string file)
        {
            using (ZipArchive archive = ZipFile.OpenRead(zipFile))
            using (Stream stream = archive.GetEntry(file).Open())
            using (TextReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    yield return line;
            }
        }

        private static Dictionary<char, LocationFeatureClass> FeatureClassCodes = new Dictionary<char, LocationFeatureClass>()
        {
            { 'A', LocationFeatureClass.CountryStateRegion },
            { 'H', LocationFeatureClass.StreamLake },
            { 'L', LocationFeatureClass.ParksArea },
            { 'P', LocationFeatureClass.CityVillage },
            { 'R', LocationFeatureClass.RoadRailRoad },
            { 'S', LocationFeatureClass.SpotBuildingFarm },
            { 'T', LocationFeatureClass.MountainHillRock },
            { 'U', LocationFeatureClass.Undersea },
            { 'V', LocationFeatureClass.ForestHeath },
        };

        private static Dictionary<string, LocationFeature> FeatureCodes = new Dictionary<string, LocationFeature>()
        {
            { "ADM1", LocationFeature.FirstOrderAdministrativeDivision },
            { "ADM1H", LocationFeature.HistoricalFirstOrderAdministrativeDivision },
            { "ADM2", LocationFeature.SecondOrderAdministrativeDivision },
            { "ADM2H", LocationFeature.HistoricalSecondOrderAdministrativeDivision },
            { "ADM3", LocationFeature.ThirdOrderAdministrativeDivision },
            { "ADM3H", LocationFeature.HistoricalThirdOrderAdministrativeDivision },
            { "ADM4", LocationFeature.FourthOrderAdministrativeDivision },
            { "ADM4H", LocationFeature.HistoricalFourthOrderAdministrativeDivision },
            { "ADM5", LocationFeature.FifthOrderAdministrativeDivision },
            { "ADMD", LocationFeature.AdministrativeDivision },
            { "ADMDH", LocationFeature.HistoricalAdministrativeDivision },
            { "LTER", LocationFeature.LeasedArea },
            { "PCL", LocationFeature.PoliticalEntity },
            { "PCLD", LocationFeature.DependentPoliticalEntity },
            { "PCLF", LocationFeature.FreelyAssociatedState },
            { "PCLH", LocationFeature.HistoricalPoliticalEntity },
            { "PCLI", LocationFeature.IndependentPoliticalEntity },
            { "PCLIX", LocationFeature.SectionOfIndependentPoliticalEntity },
            { "PCLS", LocationFeature.SemiIndependentPoliticalEntity },
            { "PRSH", LocationFeature.Parish },
            { "TERR", LocationFeature.Territory },
            { "ZN", LocationFeature.Zone },
            { "ZNB", LocationFeature.BufferZone },
            { "AIRS", LocationFeature.SeaplaneLandingArea },
            { "ANCH", LocationFeature.Anchorage },
            { "BAY", LocationFeature.Bay },
            { "BAYS", LocationFeature.Bays },
            { "BGHT", LocationFeature.Bight },
            { "BNK", LocationFeature.WaterBank },
            { "BNKR", LocationFeature.StreamBank },
            { "BNKX", LocationFeature.SectionOfBank },
            { "BOG", LocationFeature.Bog },
            { "CAPG", LocationFeature.Icecap },
            { "CHN", LocationFeature.Channel },
            { "CHNL", LocationFeature.LakeChannel },
            { "CHNM", LocationFeature.MarineChannel },
            { "CHNN", LocationFeature.NavigationChannel },
            { "CNFL", LocationFeature.Confluence },
            { "CNL", LocationFeature.Canal },
            { "CNLA", LocationFeature.Aqueduct },
            { "CNLB", LocationFeature.CanalBend },
            { "CNLD", LocationFeature.DrainageCanal },
            { "CNLI", LocationFeature.IrrigationCanal },
            { "CNLN", LocationFeature.NavigationCanal },
            { "CNLQ", LocationFeature.AbandonedCanal },
            { "CNLSB", LocationFeature.UndergroundIrrigationCanal },
            { "CNLX", LocationFeature.SectionOfCanal },
            { "COVE", LocationFeature.Cove },
            { "CRKT", LocationFeature.TidalCreek },
            { "CRNT", LocationFeature.Current },
            { "CUTF", LocationFeature.Cutoff },
            { "DCK", LocationFeature.Dock },
            { "DCKB", LocationFeature.DockingBasin },
            { "DOMG", LocationFeature.IcecapDome },
            { "DPRG", LocationFeature.IcecapDepression },
            { "DTCH", LocationFeature.Ditch },
            { "DTCHD", LocationFeature.DrainageDitch },
            { "DTCHI", LocationFeature.IrrigationDitch },
            { "DTCHM", LocationFeature.DitchMouth },
            { "ESTY", LocationFeature.Estuary },
            { "FISH", LocationFeature.FishingArea },
            { "FJD", LocationFeature.Fjord },
            { "FJDS", LocationFeature.Fjords },
            { "FLLS", LocationFeature.Waterfall },
            { "FLLSX", LocationFeature.SectionOfWaterfall },
            { "FLTM", LocationFeature.MudFlat },
            { "FLTT", LocationFeature.TidalFlat },
            { "GLCR", LocationFeature.Glacier },
            { "GULF", LocationFeature.Gulf },
            { "GYSR", LocationFeature.Geyser },
            { "HBR", LocationFeature.Harbor },
            { "HBRX", LocationFeature.SectionOfHarbor },
            { "INLT", LocationFeature.Inlet },
            { "INLTQ", LocationFeature.FormerInlet },
            { "LBED", LocationFeature.LakeBed },
            { "LGN", LocationFeature.Lagoon },
            { "LGNS", LocationFeature.Lagoons },
            { "LGNX", LocationFeature.SectionOfLagoon },
            { "LK", LocationFeature.Lake },
            { "LKC", LocationFeature.CraterLake },
            { "LKI", LocationFeature.IntermittentLake },
            { "LKN", LocationFeature.SaltLake },
            { "LKNI", LocationFeature.IntermittentSaltLake },
            { "LKO", LocationFeature.OxbowLake },
            { "LKOI", LocationFeature.IntermittentOxbowLake },
            { "LKS", LocationFeature.Lakes },
            { "LKSB", LocationFeature.UndergroundLake },
            { "LKSC", LocationFeature.CraterLakes },
            { "LKSI", LocationFeature.IntermittentLakes },
            { "LKSN", LocationFeature.SaltLakes },
            { "LKSNI", LocationFeature.IntermittentSaltLakes },
            { "LKX", LocationFeature.SectionOfLake },
            { "MFGN", LocationFeature.SaltEvaporationPonds },
            { "MGV", LocationFeature.MangroveSwamp },
            { "MOOR", LocationFeature.Moor },
            { "MRSH", LocationFeature.Marsh },
            { "MRSHN", LocationFeature.SaltMarsh },
            { "NRWS", LocationFeature.Narrows },
            { "OCN", LocationFeature.Ocean },
            { "OVF", LocationFeature.Overfalls },
            { "PND", LocationFeature.Pond },
            { "PNDI", LocationFeature.IntermittentPond },
            { "PNDN", LocationFeature.SaltPond },
            { "PNDNI", LocationFeature.IntermittentSaltPond },
            { "PNDS", LocationFeature.Ponds },
            { "PNDSF", LocationFeature.Fishponds },
            { "PNDSI", LocationFeature.IntermittentPonds },
            { "PNDSN", LocationFeature.SaltPonds },
            { "POOL", LocationFeature.Pool },
            { "POOLI", LocationFeature.IntermittentPool },
            { "RCH", LocationFeature.Reach },
            { "RDGG", LocationFeature.IcecapRidge },
            { "RDST", LocationFeature.Roadstead },
            { "RF", LocationFeature.Reef },
            { "RFC", LocationFeature.CoralReef },
            { "RFX", LocationFeature.SectionOfReef },
            { "RPDS", LocationFeature.Rapids },
            { "RSV", LocationFeature.Reservoir },
            { "RSVI", LocationFeature.IntermittentReservoir },
            { "RSVT", LocationFeature.WaterTank },
            { "RVN", LocationFeature.Ravine },
            { "SBKH", LocationFeature.Sabkha },
            { "SD", LocationFeature.Sound },
            { "SEA", LocationFeature.Sea },
            { "SHOL", LocationFeature.Shoal },
            { "SILL", LocationFeature.Sill },
            { "SPNG", LocationFeature.Spring },
            { "SPNS", LocationFeature.SulphurSpring },
            { "SPNT", LocationFeature.HotSpring },
            { "STM", LocationFeature.Stream },
            { "STMA", LocationFeature.Anabranch },
            { "STMB", LocationFeature.StreamBend },
            { "STMC", LocationFeature.CanalizedStream },
            { "STMD", LocationFeature.Distributary },
            { "STMH", LocationFeature.Headwaters },
            { "STMI", LocationFeature.IntermittentStream },
            { "STMIX", LocationFeature.SectionOfIntermittentStream },
            { "STMM", LocationFeature.StreamMouth },
            { "STMQ", LocationFeature.AbandonedWatercourse },
            { "STMS", LocationFeature.Streams },
            { "STMSB", LocationFeature.LostRiver },
            { "STMX", LocationFeature.SectionOfStream },
            { "STRT", LocationFeature.Strait },
            { "SWMP", LocationFeature.Swamp },
            { "SYSI", LocationFeature.IrrigationSystem },
            { "TNLC", LocationFeature.CanalTunnel },
            { "WAD", LocationFeature.Wadi },
            { "WADB", LocationFeature.WadiBend },
            { "WADJ", LocationFeature.WadiJunction },
            { "WADM", LocationFeature.WadiMouth },
            { "WADS", LocationFeature.Wadies },
            { "WADX", LocationFeature.SectionOfWadi },
            { "WHRL", LocationFeature.Whirlpool },
            { "WLL", LocationFeature.Well },
            { "WLLQ", LocationFeature.AbandonedWell },
            { "WLLS", LocationFeature.Wells },
            { "WTLD", LocationFeature.Wetland },
            { "WTLDI", LocationFeature.IntermittentWetland },
            { "WTRC", LocationFeature.Watercourse },
            { "WTRH", LocationFeature.Waterhole },
            { "AGRC", LocationFeature.AgriculturalColony },
            { "AMUS", LocationFeature.AmusementPark },
            { "AREA", LocationFeature.Area },
            { "BSND", LocationFeature.DrainageBasin },
            { "BSNP", LocationFeature.PetroleumBasin },
            { "BTL", LocationFeature.Battlefield },
            { "CLG", LocationFeature.Clearing },
            { "CMN", LocationFeature.Common },
            { "CNS", LocationFeature.ConcessionArea },
            { "COLF", LocationFeature.Coalfield },
            { "CONT", LocationFeature.Continent },
            { "CST", LocationFeature.Coast },
            { "CTRB", LocationFeature.BusinessCenter },
            { "DEVH", LocationFeature.HousingDevelopment },
            { "FLD", LocationFeature.Field },
            { "FLDI", LocationFeature.IrrigatedField },
            { "GASF", LocationFeature.Gasfield },
            { "GRAZ", LocationFeature.GrazingArea },
            { "GVL", LocationFeature.GravelArea },
            { "INDS", LocationFeature.IndustrialArea },
            { "LAND", LocationFeature.ArcticLand },
            { "LCTY", LocationFeature.Locality },
            { "MILB", LocationFeature.MilitaryBase },
            { "MNA", LocationFeature.MiningArea },
            { "MVA", LocationFeature.ManeuverArea },
            { "NVB", LocationFeature.NavalBase },
            { "OAS", LocationFeature.Oasis },
            { "OILF", LocationFeature.Oilfield },
            { "PEAT", LocationFeature.PeatCuttingArea },
            { "PRK", LocationFeature.Park },
            { "PRT", LocationFeature.Port },
            { "QCKS", LocationFeature.Quicksand },
            { "RES", LocationFeature.Reserve },
            { "RESA", LocationFeature.AgriculturalReserve },
            { "RESF", LocationFeature.ForestReserve },
            { "RESH", LocationFeature.HuntingReserve },
            { "RESN", LocationFeature.NatureReserve },
            { "RESP", LocationFeature.PalmTreeReserve },
            { "RESV", LocationFeature.Reservation },
            { "RESW", LocationFeature.WildlifeReserve },
            { "RGN", LocationFeature.Region },
            { "RGNE", LocationFeature.EconomicRegion },
            { "RGNH", LocationFeature.HistoricalRegion },
            { "RGNL", LocationFeature.LakeRegion },
            { "RNGA", LocationFeature.ArtilleryRange },
            { "SALT", LocationFeature.SaltArea },
            { "SNOW", LocationFeature.Snowfield },
            { "TRB", LocationFeature.TribalArea },
            { "PPL", LocationFeature.PopulatedPlace },
            { "PPLA", LocationFeature.SeatOfAFirstOrderAdministrativeDivision },
            { "PPLA2", LocationFeature.SeatOfASecondOrderAdministrativeDivision },
            { "PPLA3", LocationFeature.SeatOfAThirdOrderAdministrativeDivision },
            { "PPLA4", LocationFeature.SeatOfAFourthOrderAdministrativeDivision },
            { "PPLC", LocationFeature.CapitalOfAPoliticalEntity },
            { "PPLCH", LocationFeature.HistoricalCapitalOfAPoliticalEntity },
            { "PPLF", LocationFeature.FarmVillage },
            { "PPLG", LocationFeature.SeatOfGovernmentOfAPoliticalEntity },
            { "PPLH", LocationFeature.HistoricalPopulatedPlace },
            { "PPLL", LocationFeature.PopulatedLocality },
            { "PPLQ", LocationFeature.AbandonedPopulatedPlace },
            { "PPLR", LocationFeature.ReligiousPopulatedPlace },
            { "PPLS", LocationFeature.PopulatedPlaces },
            { "PPLW", LocationFeature.DestroyedPopulatedPlace },
            { "PPLX", LocationFeature.SectionOfPopulatedPlace },
            { "STLMT", LocationFeature.IsraeliSettlement },
            { "CSWY", LocationFeature.Causeway },
            { "OILP", LocationFeature.OilPipeline },
            { "PRMN", LocationFeature.Promenade },
            { "PTGE", LocationFeature.Portage },
            { "RD", LocationFeature.Road },
            { "RDA", LocationFeature.AncientRoad },
            { "RDB", LocationFeature.RoadBend },
            { "RDCUT", LocationFeature.RoadCut },
            { "RDJCT", LocationFeature.RoadJunction },
            { "RJCT", LocationFeature.RailroadJunction },
            { "RR", LocationFeature.Railroad },
            { "RRQ", LocationFeature.AbandonedRailroad },
            { "RTE", LocationFeature.CaravanRoute },
            { "RYD", LocationFeature.RailroadYard },
            { "ST", LocationFeature.Street },
            { "STKR", LocationFeature.StockRoute },
            { "TNL", LocationFeature.Tunnel },
            { "TNLN", LocationFeature.NaturalTunnel },
            { "TNLRD", LocationFeature.RoadTunnel },
            { "TNLRR", LocationFeature.RailroadTunnel },
            { "TNLS", LocationFeature.Tunnels },
            { "TRL", LocationFeature.Trail },
            { "ADMF", LocationFeature.AdministrativeFacility },
            { "AGRF", LocationFeature.AgriculturalFacility },
            { "AIRB", LocationFeature.Airbase },
            { "AIRF", LocationFeature.Airfield },
            { "AIRH", LocationFeature.Heliport },
            { "AIRP", LocationFeature.Airport },
            { "AIRQ", LocationFeature.AbandonedAirfield },
            { "AMTH", LocationFeature.Amphitheater },
            { "ANS", LocationFeature.AncientSite },
            { "AQC", LocationFeature.AquacultureFacility },
            { "ARCH", LocationFeature.Arch },
            { "ASTR", LocationFeature.AstronomicalStation },
            { "ASYL", LocationFeature.Asylum },
            { "ATHF", LocationFeature.AthleticField },
            { "ATM", LocationFeature.AutomaticTellerMachine },
            { "BANK", LocationFeature.Bank },
            { "BCN", LocationFeature.Beacon },
            { "BDG", LocationFeature.Bridge },
            { "BDGQ", LocationFeature.RuinedBridge },
            { "BLDG", LocationFeature.Building },
            { "BLDO", LocationFeature.OfficeBuilding },
            { "BP", LocationFeature.BoundaryMarker },
            { "BRKS", LocationFeature.Barracks },
            { "BRKW", LocationFeature.Breakwater },
            { "BSTN", LocationFeature.BalingStation },
            { "BTYD", LocationFeature.Boatyard },
            { "BUR", LocationFeature.BurialCave },
            { "BUSTN", LocationFeature.BusStation },
            { "BUSTP", LocationFeature.BusStop },
            { "CARN", LocationFeature.Cairn },
            { "CAVE", LocationFeature.Cave },
            { "CH", LocationFeature.Church },
            { "CMP", LocationFeature.Camp },
            { "CMPL", LocationFeature.LoggingCamp },
            { "CMPLA", LocationFeature.LaborCamp },
            { "CMPMN", LocationFeature.MiningCamp },
            { "CMPO", LocationFeature.OilCamp },
            { "CMPQ", LocationFeature.AbandonedCamp },
            { "CMPRF", LocationFeature.RefugeeCamp },
            { "CMTY", LocationFeature.Cemetery },
            { "COMC", LocationFeature.CommunicationCenter },
            { "CRRL", LocationFeature.Corral },
            { "CSNO", LocationFeature.Casino },
            { "CSTL", LocationFeature.Castle },
            { "CSTM", LocationFeature.CustomsHouse },
            { "CTHSE", LocationFeature.Courthouse },
            { "CTRA", LocationFeature.AtomicCenter },
            { "CTRCM", LocationFeature.CommunityCenter },
            { "CTRF", LocationFeature.FacilityCenter },
            { "CTRM", LocationFeature.MedicalCenter },
            { "CTRR", LocationFeature.ReligiousCenter },
            { "CTRS", LocationFeature.SpaceCenter },
            { "CVNT", LocationFeature.Convent },
            { "DAM", LocationFeature.Dam },
            { "DAMQ", LocationFeature.RuinedDam },
            { "DAMSB", LocationFeature.SubSurfaceDam },
            { "DARY", LocationFeature.Dairy },
            { "DCKD", LocationFeature.DryDock },
            { "DCKY", LocationFeature.Dockyard },
            { "DIKE", LocationFeature.Dike },
            { "DIP", LocationFeature.DiplomaticFacility },
            { "DPOF", LocationFeature.FuelDepot },
            { "EST", LocationFeature.Estate },
            { "ESTO", LocationFeature.OilPalmPlantation },
            { "ESTR", LocationFeature.RubberPlantation },
            { "ESTSG", LocationFeature.SugarPlantation },
            { "ESTT", LocationFeature.TeaPlantation },
            { "ESTX", LocationFeature.SectionOfEstate },
            { "FCL", LocationFeature.Facility },
            { "FNDY", LocationFeature.Foundry },
            { "FRM", LocationFeature.Farm },
            { "FRMQ", LocationFeature.AbandonedFarm },
            { "FRMS", LocationFeature.Farms },
            { "FRMT", LocationFeature.Farmstead },
            { "FT", LocationFeature.Fort },
            { "FY", LocationFeature.Ferry },
            { "GATE", LocationFeature.Gate },
            { "GDN", LocationFeature.Garden },
            { "GHAT", LocationFeature.Ghat },
            { "GHSE", LocationFeature.GuestHouse },
            { "GOSP", LocationFeature.GasOilSeparatorPlant },
            { "GOVL", LocationFeature.LocalGovernmentOffice },
            { "GRVE", LocationFeature.Grave },
            { "HERM", LocationFeature.Hermitage },
            { "HLT", LocationFeature.HaltingPlace },
            { "HMSD", LocationFeature.Homestead },
            { "HSE", LocationFeature.House },
            { "HSEC", LocationFeature.CountryHouse },
            { "HSP", LocationFeature.Hospital },
            { "HSPC", LocationFeature.Clinic },
            { "HSPD", LocationFeature.Dispensary },
            { "HSPL", LocationFeature.Leprosarium },
            { "HSTS", LocationFeature.HistoricalSite },
            { "HTL", LocationFeature.Hotel },
            { "HUT", LocationFeature.Hut },
            { "HUTS", LocationFeature.Huts },
            { "INSM", LocationFeature.MilitaryInstallation },
            { "ITTR", LocationFeature.ResearchInstitute },
            { "JTY", LocationFeature.Jetty },
            { "LDNG", LocationFeature.Landing },
            { "LEPC", LocationFeature.LeperColony },
            { "LIBR", LocationFeature.Library },
            { "LNDF", LocationFeature.Landfill },
            { "LOCK", LocationFeature.Lock },
            { "LTHSE", LocationFeature.Lighthouse },
            { "MALL", LocationFeature.Mall },
            { "MAR", LocationFeature.Marina },
            { "MFG", LocationFeature.Factory },
            { "MFGB", LocationFeature.Brewery },
            { "MFGC", LocationFeature.Cannery },
            { "MFGCU", LocationFeature.CopperWorks },
            { "MFGLM", LocationFeature.Limekiln },
            { "MFGM", LocationFeature.MunitionsPlant },
            { "MFGPH", LocationFeature.PhosphateWorks },
            { "MFGQ", LocationFeature.AbandonedFactory },
            { "MFGSG", LocationFeature.SugarRefinery },
            { "MKT", LocationFeature.Market },
            { "ML", LocationFeature.Mill },
            { "MLM", LocationFeature.OreTreatmentPlant },
            { "MLO", LocationFeature.OliveOilMill },
            { "MLSG", LocationFeature.SugarMill },
            { "MLSGQ", LocationFeature.FormerSugarMill },
            { "MLSW", LocationFeature.Sawmill },
            { "MLWND", LocationFeature.Windmill },
            { "MLWTR", LocationFeature.WaterMill },
            { "MN", LocationFeature.Mine },
            { "MNAU", LocationFeature.GoldMine },
            { "MNC", LocationFeature.CoalMine },
            { "MNCR", LocationFeature.ChromeMine },
            { "MNCU", LocationFeature.CopperMine },
            { "MNFE", LocationFeature.IronMine },
            { "MNMT", LocationFeature.Monument },
            { "MNN", LocationFeature.SaltMine },
            { "MNQ", LocationFeature.AbandonedMine },
            { "MNQR", LocationFeature.Quarry },
            { "MOLE", LocationFeature.Mole },
            { "MSQE", LocationFeature.Mosque },
            { "MSSN", LocationFeature.Mission },
            { "MSSNQ", LocationFeature.AbandonedMission },
            { "MSTY", LocationFeature.Monastery },
            { "MTRO", LocationFeature.MetroStation },
            { "MUS", LocationFeature.Museum },
            { "NOV", LocationFeature.Novitiate },
            { "NSY", LocationFeature.Nursery },
            { "OBPT", LocationFeature.ObservationPoint },
            { "OBS", LocationFeature.Observatory },
            { "OBSR", LocationFeature.RadioObservatory },
            { "OILJ", LocationFeature.OilPipelineJunction },
            { "OILQ", LocationFeature.AbandonedOilWell },
            { "OILR", LocationFeature.OilRefinery },
            { "OILT", LocationFeature.TankFarm },
            { "OILW", LocationFeature.OilWell },
            { "OPRA", LocationFeature.OperaHouse },
            { "PAL", LocationFeature.Palace },
            { "PGDA", LocationFeature.Pagoda },
            { "PIER", LocationFeature.Pier },
            { "PKLT", LocationFeature.ParkingLot },
            { "PMPO", LocationFeature.OilPumpingStation },
            { "PMPW", LocationFeature.WaterPumpingStation },
            { "PO", LocationFeature.PostOffice },
            { "PP", LocationFeature.PolicePost },
            { "PPQ", LocationFeature.AbandonedPolicePost },
            { "PRKGT", LocationFeature.ParkGate },
            { "PRKHQ", LocationFeature.ParkHeadquarters },
            { "PRN", LocationFeature.Prison },
            { "PRNJ", LocationFeature.Reformatory },
            { "PRNQ", LocationFeature.AbandonedPrison },
            { "PS", LocationFeature.PowerStation },
            { "PSH", LocationFeature.HydroelectricPowerStation },
            { "PSTB", LocationFeature.BorderPost },
            { "PSTC", LocationFeature.CustomsPost },
            { "PSTP", LocationFeature.PatrolPost },
            { "PYR", LocationFeature.Pyramid },
            { "PYRS", LocationFeature.Pyramids },
            { "QUAY", LocationFeature.Quay },
            { "RDCR", LocationFeature.TrafficCircle },
            { "RECG", LocationFeature.GolfCourse },
            { "RECR", LocationFeature.Racetrack },
            { "REST", LocationFeature.Restaurant },
            { "RET", LocationFeature.Store },
            { "RHSE", LocationFeature.Resthouse },
            { "RKRY", LocationFeature.Rookery },
            { "RLG", LocationFeature.ReligiousSite },
            { "RLGR", LocationFeature.Retreat },
            { "RNCH", LocationFeature.Ranch },
            { "RSD", LocationFeature.RailroadSiding },
            { "RSGNL", LocationFeature.RailroadSignal },
            { "RSRT", LocationFeature.Resort },
            { "RSTN", LocationFeature.RailroadStation },
            { "RSTNQ", LocationFeature.AbandonedRailroadStation },
            { "RSTP", LocationFeature.RailroadStop },
            { "RSTPQ", LocationFeature.AbandonedRailroadStop },
            { "RUIN", LocationFeature.Ruin },
            { "SCH", LocationFeature.School },
            { "SCHA", LocationFeature.AgriculturalSchool },
            { "SCHC", LocationFeature.College },
            { "SCHL", LocationFeature.LanguageSchool },
            { "SCHM", LocationFeature.MilitarySchool },
            { "SCHN", LocationFeature.MaritimeSchool },
            { "SCHT", LocationFeature.TechnicalSchool },
            { "SECP", LocationFeature.StateExamPrepCentre },
            { "SHPF", LocationFeature.Sheepfold },
            { "SHRN", LocationFeature.Shrine },
            { "SHSE", LocationFeature.Storehouse },
            { "SLCE", LocationFeature.Sluice },
            { "SNTR", LocationFeature.Sanatorium },
            { "SPA", LocationFeature.Spa },
            { "SPLY", LocationFeature.Spillway },
            { "SQR", LocationFeature.Square },
            { "STBL", LocationFeature.Stable },
            { "STDM", LocationFeature.Stadium },
            { "STNB", LocationFeature.ScientificResearchBase },
            { "STNC", LocationFeature.CoastGuardStation },
            { "STNE", LocationFeature.ExperimentStation },
            { "STNF", LocationFeature.ForestStation },
            { "STNI", LocationFeature.InspectionStation },
            { "STNM", LocationFeature.MeteorologicalStation },
            { "STNR", LocationFeature.RadioStation },
            { "STNS", LocationFeature.SatelliteStation },
            { "STNW", LocationFeature.WhalingStation },
            { "STPS", LocationFeature.Steps },
            { "SWT", LocationFeature.SewageTreatmentPlant },
            { "THTR", LocationFeature.Theater },
            { "TMB", LocationFeature.Tomb },
            { "TMPL", LocationFeature.Temple },
            { "TNKD", LocationFeature.CattleDippingTank },
            { "TOWR", LocationFeature.Tower },
            { "TRANT", LocationFeature.TransitTerminal },
            { "TRIG", LocationFeature.TriangulationStation },
            { "TRMO", LocationFeature.OilPipelineTerminal },
            { "TWO", LocationFeature.TempWorkOffice },
            { "UNIP", LocationFeature.UniversityPrepSchool },
            { "UNIV", LocationFeature.University },
            { "USGE", LocationFeature.UnitedStatesGovernmentEstablishment },
            { "VETF", LocationFeature.VeterinaryFacility },
            { "WALL", LocationFeature.Wall },
            { "WALLA", LocationFeature.AncientWall },
            { "WEIR", LocationFeature.Weir },
            { "WHRF", LocationFeature.Wharf },
            { "WRCK", LocationFeature.Wreck },
            { "WTRW", LocationFeature.Waterworks },
            { "ZNF", LocationFeature.FreeTradeZone },
            { "ZOO", LocationFeature.Zoo },
            { "ASPH", LocationFeature.AsphaltLake },
            { "ATOL", LocationFeature.Atoll },
            { "BAR", LocationFeature.Bar },
            { "BCH", LocationFeature.Beach },
            { "BCHS", LocationFeature.Beaches },
            { "BDLD", LocationFeature.Badlands },
            { "BLDR", LocationFeature.BoulderField },
            { "BLHL", LocationFeature.Blowhole },
            { "BLOW", LocationFeature.Blowout },
            { "BNCH", LocationFeature.Bench },
            { "BUTE", LocationFeature.Butte },
            { "CAPE", LocationFeature.Cape },
            { "CFT", LocationFeature.Cleft },
            { "CLDA", LocationFeature.Caldera },
            { "CLF", LocationFeature.Cliff },
            { "CNYN", LocationFeature.Canyon },
            { "CONE", LocationFeature.Cone },
            { "CRDR", LocationFeature.Corridor },
            { "CRQ", LocationFeature.Cirque },
            { "CRQS", LocationFeature.Cirques },
            { "CRTR", LocationFeature.Crater },
            { "CUET", LocationFeature.Cuesta },
            { "DLTA", LocationFeature.Delta },
            { "DPR", LocationFeature.Depression },
            { "DSRT", LocationFeature.Desert },
            { "DUNE", LocationFeature.Dune },
            { "DVD", LocationFeature.Divide },
            { "ERG", LocationFeature.SandyDesert },
            { "FAN", LocationFeature.Fan },
            { "FORD", LocationFeature.Ford },
            { "FSR", LocationFeature.Fissure },
            { "GAP", LocationFeature.Gap },
            { "GRGE", LocationFeature.Gorge },
            { "HDLD", LocationFeature.Headland },
            { "HLL", LocationFeature.Hill },
            { "HLLS", LocationFeature.Hills },
            { "HMCK", LocationFeature.Hammock },
            { "HMDA", LocationFeature.RockDesert },
            { "INTF", LocationFeature.Interfluve },
            { "ISL", LocationFeature.Island },
            { "ISLET", LocationFeature.Islet },
            { "ISLF", LocationFeature.ArtificialIsland },
            { "ISLM", LocationFeature.MangroveIsland },
            { "ISLS", LocationFeature.Islands },
            { "ISLT", LocationFeature.LandTiedIsland },
            { "ISLX", LocationFeature.SectionOfIsland },
            { "ISTH", LocationFeature.Isthmus },
            { "KRST", LocationFeature.KarstArea },
            { "LAVA", LocationFeature.LavaArea },
            { "LEV", LocationFeature.Levee },
            { "MESA", LocationFeature.Mesa },
            { "MND", LocationFeature.Mound },
            { "MRN", LocationFeature.Moraine },
            { "MT", LocationFeature.Mountain },
            { "MTS", LocationFeature.Mountains },
            { "NKM", LocationFeature.MeanderNeck },
            { "NTK", LocationFeature.Nunatak },
            { "NTKS", LocationFeature.Nunataks },
            { "PAN", LocationFeature.Pan },
            { "PANS", LocationFeature.Pans },
            { "PASS", LocationFeature.Pass },
            { "PEN", LocationFeature.Peninsula },
            { "PENX", LocationFeature.SectionOfPeninsula },
            { "PK", LocationFeature.Peak },
            { "PKS", LocationFeature.Peaks },
            { "PLAT", LocationFeature.Plateau },
            { "PLATX", LocationFeature.SectionOfPlateau },
            { "PLDR", LocationFeature.Polder },
            { "PLN", LocationFeature.Plain },
            { "PLNX", LocationFeature.SectionOfPlain },
            { "PROM", LocationFeature.Promontory },
            { "PT", LocationFeature.Point },
            { "PTS", LocationFeature.Points },
            { "RDGB", LocationFeature.BeachRidge },
            { "RDGE", LocationFeature.Ridge },
            { "REG", LocationFeature.StonyDesert },
            { "RK", LocationFeature.Rock },
            { "RKFL", LocationFeature.Rockfall },
            { "RKS", LocationFeature.Rocks },
            { "SAND", LocationFeature.SandArea },
            { "SBED", LocationFeature.DryStreamBed },
            { "SCRP", LocationFeature.Escarpment },
            { "SDL", LocationFeature.Saddle },
            { "SHOR", LocationFeature.Shore },
            { "SINK", LocationFeature.Sinkhole },
            { "SLID", LocationFeature.Slide },
            { "SLP", LocationFeature.Slope },
            { "SPIT", LocationFeature.Spit },
            { "SPUR", LocationFeature.Spur },
            { "TAL", LocationFeature.TalusSlope },
            { "TRGD", LocationFeature.InterduneTrough },
            { "TRR", LocationFeature.Terrace },
            { "UPLD", LocationFeature.Upland },
            { "VAL", LocationFeature.Valley },
            { "VALG", LocationFeature.HangingValley },
            { "VALS", LocationFeature.Valleys },
            { "VALX", LocationFeature.SectionOfValley },
            { "VLC", LocationFeature.Volcano },
            { "APNU", LocationFeature.Apron },
            { "ARCU", LocationFeature.UnderseaArch },
            { "ARRU", LocationFeature.Arrugado },
            { "BDLU", LocationFeature.Borderland },
            { "BKSU", LocationFeature.Banks },
            { "BNKU", LocationFeature.UnderseaBank },
            { "BSNU", LocationFeature.Basin },
            { "CDAU", LocationFeature.Cordillera },
            { "CNSU", LocationFeature.Canyons },
            { "CNYU", LocationFeature.UnderseaCanyon },
            { "CRSU", LocationFeature.ContinentalRise },
            { "DEPU", LocationFeature.Deep },
            { "EDGU", LocationFeature.ShelfEdge },
            { "ESCU", LocationFeature.UnderseaEscarpment },
            { "FANU", LocationFeature.UnderseaFan },
            { "FLTU", LocationFeature.Flat },
            { "FRZU", LocationFeature.FractureZone },
            { "FURU", LocationFeature.Furrow },
            { "GAPU", LocationFeature.UnderseaGap },
            { "GLYU", LocationFeature.Gully },
            { "HLLU", LocationFeature.UnderseaHill },
            { "HLSU", LocationFeature.UnderseaHills },
            { "HOLU", LocationFeature.Hole },
            { "KNLU", LocationFeature.Knoll },
            { "KNSU", LocationFeature.Knolls },
            { "LDGU", LocationFeature.Ledge },
            { "LEVU", LocationFeature.UnderseaLevee },
            { "MESU", LocationFeature.UnderseaMesa },
            { "MNDU", LocationFeature.UnderseaMound },
            { "MOTU", LocationFeature.Moat },
            { "MTU", LocationFeature.UnderseaMountain },
            { "PKSU", LocationFeature.UnderseaPeaks },
            { "PKU", LocationFeature.UnderseaPeak },
            { "PLNU", LocationFeature.UnderseaPlain },
            { "PLTU", LocationFeature.UnderseaPlateau },
            { "PNLU", LocationFeature.Pinnacle },
            { "PRVU", LocationFeature.Province },
            { "RDGU", LocationFeature.UnderseaRidge },
            { "RDSU", LocationFeature.Ridges },
            { "RFSU", LocationFeature.Reefs },
            { "RFU", LocationFeature.UnderseaReef },
            { "RISU", LocationFeature.Rise },
            { "SCNU", LocationFeature.Seachannel },
            { "SCSU", LocationFeature.Seachannels },
            { "SDLU", LocationFeature.UnderseaSaddle },
            { "SHFU", LocationFeature.Shelf },
            { "SHLU", LocationFeature.UnderseaShoal },
            { "SHSU", LocationFeature.Shoals },
            { "SHVU", LocationFeature.ShelfValley },
            { "SILU", LocationFeature.UnderseaSill },
            { "SLPU", LocationFeature.UnderseaSlope },
            { "SMSU", LocationFeature.Seamounts },
            { "SMU", LocationFeature.Seamount },
            { "SPRU", LocationFeature.UnderseaSpur },
            { "TERU", LocationFeature.UnderseaTerrace },
            { "TMSU", LocationFeature.Tablemounts },
            { "TMTU", LocationFeature.Tablemount },
            { "TNGU", LocationFeature.Tongue },
            { "TRGU", LocationFeature.Trough },
            { "TRNU", LocationFeature.Trench },
            { "VALU", LocationFeature.UnderseaValley },
            { "VLSU", LocationFeature.UnderseaValleys },
            { "BUSH", LocationFeature.Bush },
            { "CULT", LocationFeature.CultivatedArea },
            { "FRST", LocationFeature.Forest },
            { "FRSTF", LocationFeature.FossilizedForest },
            { "GRSLD", LocationFeature.Grassland },
            { "GRVC", LocationFeature.CoconutGrove },
            { "GRVO", LocationFeature.OliveGrove },
            { "GRVP", LocationFeature.PalmGrove },
            { "GRVPN", LocationFeature.PineGrove },
            { "HTH", LocationFeature.Heath },
            { "MDW", LocationFeature.Meadow },
            { "OCH", LocationFeature.Orchard },
            { "SCRB", LocationFeature.Scrubland },
            { "TREE", LocationFeature.Tree },
            { "TUND", LocationFeature.Tundra },
            { "VIN", LocationFeature.Vineyard },
            { "VINS", LocationFeature.Vineyards },
            { "ll", LocationFeature.NotAvailable },
        };
    }
}
