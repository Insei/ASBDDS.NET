using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASBDDS.Shared.Helpers
{
    public class SwitchManufacturer
    {
        public SwitchManufacturers Enum { get; set; }
        public string Name { get; set; }
        public SwitchManufacturer(SwitchManufacturers manufacturer, string name)
        {
            Enum = manufacturer;
            Name = name;
        }

    }
    public class SwitchModel
    {
        public SwitchManufacturer Manufacturer { get; set; }
        public SwitchModels Enum { get; set; }
        public string Name { get; set; }

        public SwitchModel(SwitchManufacturer manufacturer, SwitchModels model, string name)
        {
            Manufacturer = manufacturer;
            Enum = model;
            Name = name;
        }
    }

    public static class SwitchHelper
    {
        private static SwitchManufacturer ubiquiti = new SwitchManufacturer(SwitchManufacturers.UBIQUITI, "Ubiquiti");
        private static List<SwitchModel> switchModels = new List<SwitchModel>()
        {
            new SwitchModel(ubiquiti, SwitchModels.UNIFI_SWITCH_US_24_250W, "UniFi Switch US-24-250W"),
        };

        public static SwitchManufacturers? GetManufacturer(SwitchModels model)
        {
            return switchModels.FirstOrDefault(m => m.Enum == model)?.Manufacturer.Enum;
        }

        public static SwitchManufacturers? GetManufacturer(string manufacturerName)
        {
            return switchModels.FirstOrDefault(m => m.Manufacturer.Name == manufacturerName)?.Manufacturer.Enum;
        }

        public static string GetManufacturer(SwitchManufacturers manufacturer)
        {
            return switchModels.FirstOrDefault(m => m.Manufacturer.Enum == manufacturer)?.Manufacturer.Name;
        }

        public static SwitchModels[] GetModels(SwitchManufacturers manufacturer)
        {
            return switchModels.Where(m => m.Manufacturer.Enum == manufacturer).Select(m => m.Enum).ToArray();
        }
        public static SwitchModels[] GetModels(string manufacturerName)
        {
            return switchModels.Where(m => m.Manufacturer.Name == manufacturerName).Select(m => m.Enum).ToArray();
        }

        public static SwitchModels? GetModel(string name)
        {
            return switchModels.FirstOrDefault(m => m.Name == name)?.Enum;
        }

        public static string GetModel(SwitchModels model)
        {
            return switchModels.FirstOrDefault(m => m.Enum == model)?.Name;
        }
        public static SwitchManufacturers[] GetManufacturers()
        {
            return switchModels.Select(m => m.Manufacturer.Enum).Distinct().ToArray();
        }
    }

    public enum SwitchManufacturers
    {
        UBIQUITI = 1
    }

    public enum SwitchModels
    {
        UNIFI_SWITCH_US_24_250W = 1
    }
}
