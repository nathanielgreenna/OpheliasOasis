/*
 * Hotel
 * TODO: 
 * Changelog:
 * 4/18/2022: created/initially coded by Alec
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OpheliasOasis
{
    class Hotel
    {
        private List<bool> rooms;
        private int roomsOccupied;

        public Hotel() 
        { 
            rooms = new List<bool>(45);
            roomsOccupied = 0;
        }

        List<bool> getRooms()
        {
            return this.rooms;
        }
        void setRooms(List<bool> rooms)
        {
            this.rooms = rooms;
        }
        int getRoomsOccupied()
        {
            return this.roomsOccupied;
        }
        int assignRoom()
        {
            for (int i = 0; i < this.rooms.Count; i++)
            {
                if (!this.rooms[i])
                {
                    this.rooms[i] = true;
                    this.roomsOccupied++;
                    return i + 1;
                }
            }
            return -1;
        }
        void clearRoom(int roomNo)
        {
            int i = roomNo - 1;
            if (i >= 0)
            {
                this.rooms[i] = false;
                this.roomsOccupied--;
            }
        }
    }
}
