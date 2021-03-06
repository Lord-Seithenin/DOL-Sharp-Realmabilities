using DOL.Database;
using System;
using log4net;
using System.Reflection;

namespace DOL.GS
{
	public class AreaMgr
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static bool LoadAllAreas()
		{
			try
			{
				DataObject[] DBAreas = GameServer.Database.SelectAllObjects(typeof(DBArea));
				foreach (DBArea thisArea in DBAreas)
				{
					AbstractArea area = null;
					if (thisArea.ClassType == "DOL.GS.Area.Square")
						area = new Area.Square(thisArea.Description, thisArea.X, thisArea.Y, thisArea.Radius, thisArea.Radius);
					else if (thisArea.ClassType == "DOL.GS.Area.Circle")
						area = new Area.Circle(thisArea.Description, thisArea.X, thisArea.Y, thisArea.Z, thisArea.Radius);
					else if (thisArea.ClassType == "DOL.GS.Area.BindArea")
					{
						BindPoint bp = new BindPoint();
						bp.Radius = (ushort)thisArea.Radius;
						bp.X = thisArea.X;
						bp.Y = thisArea.Y;
						bp.Z = thisArea.Z;
						bp.Region = thisArea.Region;
						area = new Area.BindArea(thisArea.Description, bp);
					}
					if (area == null) throw new Exception("area is null");
					area.Sound = thisArea.Sound;
					area.CanBroadcast = thisArea.CanBroadcast;
					area.CheckLOS = thisArea.CheckLOS;
					Region region = WorldMgr.GetRegion(thisArea.Region);
					if (region == null)
						continue;
					region.AddArea(area);
					log.Info("Area added: " + thisArea.Description);
				}
				return true;
			}
			catch (Exception ex)
			{
				log.Error("Loading all areas failed", ex);
				return false;
			}
		}
	}
}