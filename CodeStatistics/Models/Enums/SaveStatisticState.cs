using System;

namespace CodeStatistics.Models.Enums;

[Flags]
public enum SaveStatisticState
{
    Unset = 0x0000,
    SettingSet = 0x0001,
    FileStatisticSet = 0x0010,
    AmountStatisticSet = 0x0100,
    AllSet = 0x0111,
}
