﻿namespace SwissTransport
{
    public interface ITransport
    {
        Stations GetStations(string query);
        StationBoardRoot GetStationBoard(string station, string id);
        Connections GetConnections(string fromStation, string toStattion);
        Connections GetConnectionsByDateTime(string fromStation, string toStattion, string onDate, string onTime);
    }
}