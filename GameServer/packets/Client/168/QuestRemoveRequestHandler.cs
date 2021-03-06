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
using System.Collections;

using DOL.GS.Quests;

namespace DOL.GS.PacketHandler.Client.v168
{
	[PacketHandler(PacketHandlerType.TCP, 0x4F, "Quest remove request")]
	public class QuestRemoveRequestHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			ushort questIndex = packet.ReadShort();
			ushort unk1 = packet.ReadShort();
			ushort unk2 = packet.ReadShort();
			ushort unk3 = packet.ReadShort();

			AbstractQuest quest = null;

			int index = 0;
			lock (client.Player.QuestList)
			{
				foreach (AbstractQuest q in client.Player.QuestList)
				{
					if (q.Step != -1)
						index++;

					if (index == questIndex)
					{
						quest = q;
						break;
					}
				}
			}

			if (quest != null)
			{
				quest.AbortQuest();
			}

			return 1;
		}
	}
}