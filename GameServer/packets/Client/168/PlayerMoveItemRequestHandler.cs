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
using DOL.Events;
using DOL.GS;
using System.Reflection;
using log4net;

namespace DOL.GS.PacketHandler.Client.v168
{
	[PacketHandlerAttribute(PacketHandlerType.TCP,0x75^168,"Player move item")]
	public class PlayerMoveItemRequestHandler : IPacketHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			ushort id		= (ushort) packet.ReadShort();
			ushort toSlot	= (ushort) packet.ReadShort();
			ushort fromSlot = (ushort) packet.ReadShort();
			ushort itemCount= (ushort) packet.ReadShort();

			//log.Debug("MoveItem, id=" + id.ToString() + " toSlot=" + toSlot.ToString() + " fromSlot=" + fromSlot.ToString() + " itemCount=" + itemCount.ToString());

			//If our slot is > 1000 it is (objectID+1000) of target
			if(toSlot>1000)
			{
				ushort objectID = (ushort)(toSlot-1000);
				GameObject obj = WorldMgr.GetObjectByIDFromRegion(client.Player.CurrentRegionID,objectID);
				if(obj==null || obj.ObjectState!=GameObject.eObjectState.Active)
				{
					client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
					client.Out.SendMessage("Invalid trade target. ("+objectID+")", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					return 0;
				}

				GamePlayer tradeTarget = null;
				//If our target is another player we set the tradetarget
				if(obj is GamePlayer)
				{
					tradeTarget = (GamePlayer) obj;
					if(tradeTarget.Client.ClientState != GameClient.eClientState.Playing)
					{
						client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
						client.Out.SendMessage("Can't trade with inactive players.", eChatType.CT_System,  eChatLoc.CL_SystemWindow);
						return 0;
					}
					if(tradeTarget == client.Player)
					{
						client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
						client.Out.SendMessage("You can't trade with yourself, silly!",eChatType.CT_System,eChatLoc.CL_SystemWindow);
						return 0;
					}
					if(!GameServer.ServerRules.IsAllowedToTrade(client.Player, tradeTarget, false))
					{
						client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
						return 0;
					}
				}

				//Is the item we want to move in our backpack?
				if(fromSlot>=(ushort)eInventorySlot.FirstBackpack && fromSlot<=(ushort)eInventorySlot.LastBackpack)
				{
					if(!WorldMgr.CheckDistance(obj, client.Player, WorldMgr.GIVE_ITEM_DISTANCE))
					{
						client.Out.SendMessage("You are too far away to give anything to " + obj.GetName(0, false) + ".", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
						return 0;
					}

					InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)fromSlot);
					if(item == null)
					{
						client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
						client.Out.SendMessage("Invalid item (slot# "+fromSlot+").",eChatType.CT_System,eChatLoc.CL_SystemWindow);
						return 0;
					}

					client.Player.Notify(GamePlayerEvent.GiveItem, client.Player, new GiveItemEventArgs(client.Player, obj, item));

					//If the item has been removed by the event handlers, return;
					//item = client.Player.Inventory.GetItem((eInventorySlot)fromSlot);
					if(item == null || item.OwnerID == null)
					{
						client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
						return 0;
					}

					if(!item.IsDropable && !(obj is GameNPC && (obj.GetType().ToString() == "DOL.GS.Scripts.Blacksmith" || obj.GetType().ToString() == "DOL.GS.Scripts.Recharger")))
					{
						client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
						client.Out.SendMessage("You can not remove this item!",eChatType.CT_System,eChatLoc.CL_SystemWindow);
						return 0;
					}

					if(tradeTarget!=null)
					{
						tradeTarget.ReceiveTradeItem(client.Player,item);
						client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
						return 1;
					}

					if(obj.ReceiveItem(client.Player, item))
					{
						client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
						return 0;
					}

					client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
					return 0;
				}

				//Is the "item" we want to move money? For Version 1.78+
				if(client.Version >= GameClient.eClientVersion.Version178
				   && fromSlot >= (int)eInventorySlot.Mithril178 && fromSlot <= (int)eInventorySlot.Copper178)
				{
					fromSlot -= eInventorySlot.Mithril178 - eInventorySlot.Mithril;
				}

				//Is the "item" we want to move money?
				if(fromSlot>=(ushort)eInventorySlot.Mithril && fromSlot<=(ushort)eInventorySlot.Copper)
				{
					int[] money=new int[5];
					money[fromSlot-(ushort)eInventorySlot.Mithril]=itemCount;
					long flatmoney = Money.GetMoney(money[0],money[1],money[2],money[3],money[4]);

					if(client.Version >= GameClient.eClientVersion.Version178) // add it back for proper slot update...
						fromSlot += eInventorySlot.Mithril178 - eInventorySlot.Mithril;

					if(!WorldMgr.CheckDistance(obj, client.Player, WorldMgr.GIVE_ITEM_DISTANCE))
					{
						client.Out.SendMessage("You are too far away to give anything to " + obj.GetName(0, false) + ".", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
						return 0;
					}

					if(flatmoney > client.Player.GetCurrentMoney())
					{
						client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
						return 0;
					}

					client.Player.Notify(GamePlayerEvent.GiveMoney, client.Player, new GiveMoneyEventArgs(client.Player, obj, flatmoney));

					if(tradeTarget!=null)
					{
						tradeTarget.ReceiveTradeMoney(client.Player, flatmoney);
						client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
						return 1;
					}

					if(obj.ReceiveMoney(client.Player, flatmoney))
					{
						client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
						return 0;
					}

					client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
					return 0;
				}

				client.Out.SendInventoryItemsUpdate(null);
				return 0;
			}

			//Do we want to move an item from inventory/vault/quiver into inventory/vault/quiver?
			if (((fromSlot>=(ushort)eInventorySlot.Ground && fromSlot<=(ushort)eInventorySlot.LastBackpack)
				|| (fromSlot>=(ushort)eInventorySlot.FirstVault && fromSlot<=(ushort)eInventorySlot.LastVault))
				&&((toSlot>=(ushort)eInventorySlot.Ground && toSlot<=(ushort)eInventorySlot.LastBackpack)
				|| (toSlot>=(ushort)eInventorySlot.FirstVault && toSlot<=(ushort)eInventorySlot.LastVault)))
			{
				//We want to drop the item
				if (toSlot==(ushort)eInventorySlot.Ground)
				{
					InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)fromSlot);
					if (item == null)
					{
						client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
						client.Out.SendMessage("Invalid item (slot# "+fromSlot+").",eChatType.CT_System,eChatLoc.CL_SystemWindow);
						return 0;
					}
					if (fromSlot<(ushort)eInventorySlot.FirstBackpack)
					{
						client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
						return 0;
					}
					if(!item.IsDropable)
					{
						client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
						client.Out.SendMessage("You can not drop this item!",eChatType.CT_System,eChatLoc.CL_SystemWindow);
						return 0;
					}

					if (client.Player.DropItem((eInventorySlot)fromSlot))
					{
						client.Out.SendMessage("You drop " + item.GetName(0, false) + " on the ground!",eChatType.CT_System,eChatLoc.CL_SystemWindow);
						return 1;
					}
					client.Out.SendInventoryItemsUpdate(null);
					return 0;
				}
				//We want to move the item in inventory
				client.Player.Inventory.MoveItem((eInventorySlot)fromSlot, (eInventorySlot)toSlot, itemCount);
				return 1;
			}


			if (((fromSlot>=(ushort)eInventorySlot.Ground && fromSlot<=(ushort)eInventorySlot.LastBackpack)
				|| (fromSlot>=(ushort)eInventorySlot.FirstVault && fromSlot<=(ushort)eInventorySlot.LastVault))
				&& (toSlot==(ushort)eInventorySlot.PlayerPaperDoll || toSlot==(ushort)eInventorySlot.NewPlayerPaperDoll))
			{
				InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)fromSlot);
				if(item==null) return 0;

				toSlot=0;
				if(item.Item_Type >= (int)eInventorySlot.MinEquipable &&
					item.Item_Type <= (int)eInventorySlot.MaxEquipable)
					toSlot = (ushort)item.Item_Type;						
				if (toSlot==0)
				{
					client.Out.SendInventorySlotsUpdate(new int[] {fromSlot});
					return 0;
				}
				if( toSlot == (int)eInventorySlot.LeftBracer || toSlot == (int)eInventorySlot.RightBracer)
				{
					if(client.Player.Inventory.GetItem(eInventorySlot.LeftBracer) == null)
						toSlot = (int)eInventorySlot.LeftBracer;
					else
						toSlot = (int)eInventorySlot.RightBracer;
				}

				if( toSlot == (int)eInventorySlot.LeftRing || toSlot == (int)eInventorySlot.RightRing)
				{
					if(client.Player.Inventory.GetItem(eInventorySlot.LeftRing) == null)
						toSlot = (int)eInventorySlot.LeftRing;
					else
						toSlot = (int)eInventorySlot.RightRing;
				}

				client.Player.Inventory.MoveItem((eInventorySlot)fromSlot,(eInventorySlot)toSlot, itemCount);
				return 1;
			}
			client.Out.SendInventoryItemsUpdate(null);
			return 0;
		}
	}
}
