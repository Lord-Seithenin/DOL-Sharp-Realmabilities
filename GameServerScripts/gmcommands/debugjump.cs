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
using DOL.GS.PacketHandler;

namespace DOL.GS.Scripts
{
	[Cmd("]jump", (uint)ePrivLevel.GM, "Teleports yourself to the specified location",
			"']jump <zoneID> <locX> <locY> <locZ> <heading>' (Autoused for *jump in debug mode)")]
	public class OnDebugJump : AbstractCommandHandler, ICommandHandler
	{
		public int OnCommand(GameClient client, string[] args)
		{
			if (args.Length == 6)
			{
				try
				{
					Zone z = WorldMgr.GetZone(Convert.ToUInt16(args[1]));
					if (z == null)
					{
						client.Out.SendMessage("Unknown zone ID: " + args[1], eChatType.CT_System, eChatLoc.CL_SystemWindow);
						return 1;
					}
					ushort RegionID = z.ZoneRegion.ID;
					int X = z.XOffset + Convert.ToInt32(args[2]);
					int Y = z.YOffset + Convert.ToInt32(args[3]);
					int Z = Convert.ToInt32(args[4]);
					ushort Heading = Convert.ToUInt16(args[5]);
					if (!CheckExpansion(client, RegionID)) return 0;
					client.Player.MoveTo(RegionID, X, Y, Z, Heading);
					return 0;
				}
				catch
				{
					return 1;
				}
			}
			else
			{
				DisplaySyntax(client);
				return 0;
			}
		}

		public bool CheckExpansion(GameClient client, ushort RegionID)
		{
			Region reg = WorldMgr.GetRegion(RegionID);
			if (reg == null)
			{
				client.Out.SendMessage("Unknown region (" + RegionID.ToString() + ").", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return false;
			}
			else if (reg.Expansion >= client.ClientType)
			{
				client.Out.SendMessage("Region (" + reg.Description + ") is not supported by your client.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return false;
			}
			return true;
		}
	}
}
