﻿using ASBDDS.Shared.Models.Database.DataDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASBDDS.Shared.Models.Responses
{
    public class SwitchPortAdminResponse
    {
        public SwitchPortAdminResponse()
        {

        }
        public SwitchPortAdminResponse(SwitchPort port)
        {
            Id = port.Id;
            Number = port.Number;
            Type = port.Type;
        }
        public Guid? Id { get; set; }
        public string Number { get; set; }
        public SwitchPortType Type { get; set; }
    }
    public class SwitchAdminResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Serial { get; set; }
        public string Ip { get; set; }
        public virtual Guid? RouterId { get; set; }
        public virtual List<SwitchPortAdminResponse> Ports { get; set; }

        public SwitchAdminResponse() { }

        public SwitchAdminResponse(Switch _switch)
        {
            Id = _switch.Id;
            Name = _switch.Name;
            Serial = _switch.Serial;
            Ip = _switch.Ip;
            RouterId = _switch.RouterGuid;
            Ports = new List<SwitchPortAdminResponse>();
            foreach(var port in _switch.Ports)
            {
                Ports.Add(new SwitchPortAdminResponse(port));
            }
        }
    }
}
