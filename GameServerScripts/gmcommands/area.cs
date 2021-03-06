/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */
using System;
using DOL.Database;
using DOL.GS.PacketHandler;

namespace DOL.GS.Scripts
{
	[CmdAttribute(
		"&area",
		(uint) ePrivLevel.GM,
		"various commands to help you with areas",
		"/area create <name> <type(circle/square>) <radius> <broadcast(y/n)> <soundid>")]
	public class AreaCommandHandler : ICommandHandler
	{
		public int OnCommand(GameClient client, string[] args)
		{
			if (args.Length == 1)
			{
				ShowSyntax(client.Player);
				return 1;
			}
			switch (args[1].ToLower())
			{
				case "create":
					{
						if (args.Length != 7)
						{
							ShowSyntax(client.Player);
							return 1;
						}
						DBArea area = new DBArea();
						area.Description = args[2];
						if (args[3].ToLower() == "circle")
							area.ClassType = "DOL.GS.Area.Circle";
						else if (args[3].ToLower() == "square")
							area.ClassType = "DOL.GS.Area.Square";
						else
						{
							ShowSyntax(client.Player);
							return 1;
						}
						area.Radius = Convert.ToInt16(args[4]);
						if (args[5].ToLower() == "y")
							area.CanBroadcast = true;
						else if (args[5].ToLower() == "n")
							area.CanBroadcast = false;
						else
						{
							ShowSyntax(client.Player);
							return 1;
						}
						area.Sound = byte.Parse(args[6]);
						area.Region = client.Player.CurrentRegionID;

						AbstractArea newArea = null;
						if (args[3].ToLower() == "circle")
						{
							area.X = client.Player.X;
							area.Y = client.Player.Y;
							area.Z = client.Player.Z;
							newArea = new Area.Circle(area.Description, area.X, area.Y, client.Player.Z, area.Radius);
						}
						else if (args[3].ToLower() == "square")
						{
							area.X = client.Player.X - area.Radius / 2;
							area.Y = client.Player.Y - area.Radius / 2;
							area.Z = client.Player.Z - area.Radius / 2;
							newArea = new Area.Square(area.Description, area.X, area.Y, area.Radius, area.Radius);
						}
						newArea.Sound = area.Sound;
						newArea.CanBroadcast = area.CanBroadcast;
						WorldMgr.GetRegion(client.Player.CurrentRegionID).AddArea(newArea);
						GameServer.Database.AddNewObject(area);
						SendMessage(client.Player, "Area created - Description:" + area.Description + " X:" + area.X +
							" Y:" + area.Y + " Z:" + area.Z + " Radius:" + area.Radius + " Broadcast:" + area.CanBroadcast.ToString() +
							" Sound:" + area.Sound);
							break;
					}
			}
			return 1;
		}
		public void ShowSyntax(GamePlayer player)
		{
			SendMessage(player, "Usage: /area");
			SendMessage(player, "/area create <name> <type(circle/square>) <radius> <broadcast(y/n)> <soundid>");
		}
		public void SendMessage(GamePlayer player, string message)
		{
			player.Out.SendMessage(message, eChatType.CT_System, eChatLoc.CL_SystemWindow);
		}
	}
}