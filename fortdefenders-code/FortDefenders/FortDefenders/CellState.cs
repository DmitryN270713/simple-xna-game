using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortDefenders
{
    /// <summary>
    /// This enum will be used for the indentification process
    /// </summary>
    [Flags]
    public enum CellState
    {
        Free =              0x0,
        House =             0x01,
        Windmill =          0x02,
        Bank =              0x03,
        Marketplace =       0x04,
        Barracks =          0x05,
        ArcheryRange =      0x06,
        BatteringRamRange = 0x07,
        CityHall =          0x08,
        Wall =              0x09,
        Gate =              0x10,
        Tree =              0x30,
        Stone =             0x31,
        Mayor =             0x40,
        Knight =            0x41,
        Archer =            0x42,
        BatteringRam =      0x43,
        Path =              0x7F,
        Forbidden =         0xFF
    }
}
