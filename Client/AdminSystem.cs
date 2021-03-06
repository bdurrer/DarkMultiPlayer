using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DarkMultiPlayerCommon;
using MessageStream;

namespace DarkMultiPlayer
{
    public class AdminSystem
    {
        private static AdminSystem singleton;
        private List<string> serverAdmins = new List<string>();
        private object adminLock = new object();

        public static AdminSystem fetch
        {
            get
            {
                return singleton;
            }
        }

        public void HandleAdminMessage(byte[] messageData)
        {
            using (MessageReader mr = new MessageReader(messageData, false))
            {
                AdminMessageType messageType = (AdminMessageType)mr.Read<int>();
                switch (messageType)
                {
                    case AdminMessageType.LIST:
                        {
                            string[] adminNames = mr.Read<string[]>();
                            foreach (string adminName in adminNames)
                            {
                                RegisterServerAdmin(adminName);
                            }
                        }
                        break;
                    case AdminMessageType.ADD:
                        {
                            string adminName = mr.Read<string>();
                            RegisterServerAdmin(adminName);
                        }
                        break;
                    case AdminMessageType.REMOVE:
                        {
                            string adminName = mr.Read<string>();
                            UnregisterServerAdmin(adminName);
                        }
                        break;
                }
            }
        }

        private void RegisterServerAdmin(string adminName)
        {
            lock (adminLock)
            {
                if (!serverAdmins.Contains(adminName))
                {
                    serverAdmins.Add(adminName);
                }
            }
        }

        private void UnregisterServerAdmin(string adminName)
        {
            lock (adminLock)
            {
                if (serverAdmins.Contains(adminName))
                {
                    serverAdmins.Remove(adminName);
                }
            }
        }

        /// <summary>
        /// Check wether the current player is an admin on the server
        /// </summary>
        /// <returns><c>true</c> if the current player is admin; otherwise, <c>false</c>.</returns>
        public bool IsAdmin()
        {
            return IsAdmin(Settings.fetch.playerName);
        }

        /// <summary>
        /// Check wether the specified player is an admin on the server
        /// </summary>
        /// <returns><c>true</c> if the specified player is admin; otherwise, <c>false</c>.</returns>
        /// <param name="playerName">Player name to check for admin.</param>
        public bool IsAdmin(string playerName)
        {
            lock (adminLock)
            {
                return serverAdmins.Contains(playerName);
            }
        }

        public static void Reset()
        {
            lock (Client.eventLock)
            {
                singleton = new AdminSystem();
            }
        }
    }
}

