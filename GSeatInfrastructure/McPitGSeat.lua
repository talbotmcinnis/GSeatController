GS = {}

package.path  = package.path..";"..lfs.currentdir().."/LuaSocket/?.lua"
package.cpath = package.cpath..";"..lfs.currentdir().."/LuaSocket/?.dll"
	
-- Prev Export functions.
GS.PrevExport = {}
GS.PrevExport.LuaExportStart = LuaExportStart
GS.PrevExport.LuaExportStop = LuaExportStop
GS.PrevExport.LuaExportAfterNextFrame = LuaExportAfterNextFrame

LuaExportStart = function()
	-- Works once just before mission start.
	GS.socket = require("socket")
	GS.host = host or "127.0.0.1"
	GS.port = port or 8804
	-- c = socket.try(socket.connect(host, port))	--  connect to the listener socket
	-- c:setoption("tcp-nodelay",true)  -- set immediate transmission mode
	GS.udpsend = GS.socket.udp()
	
	GS.JSON = loadfile("Scripts\\JSON.lua")()
	
	GS.payload = {
		Command = "Start"
	}
	GS.socket.try(GS.udpsend:sendto(GS.JSON:encode(GS.payload).." \n", GS.host, GS.port))
	
	-- Chain previously-included export as necessary
	if GS.PrevExport.LuaExportStart then
		GS.PrevExport.LuaExportStart()
	end
end

LuaExportStop = function()
	GS.payload = {
		Command = "Stop"
	}
	GS.socket.try(GS.udpsend:sendto(GS.JSON:encode(GS.payload).." \n", GS.host, GS.port))
	
	-- Chain previously-included export as necessary
	if GS.PrevExport.LuaExportStop then
		GS.PrevExport.LuaExportStop()
	end
end

LuaExportAfterNextFrame = function()
	local gs_accel = LoGetAccelerationUnits();
	local gs_ownData = LoGetSelfData();
	local altAglM = LoGetAltitudeAboveGroundLevel();
	local velocity = LoGetVectorVelocity();
	
	GS.payload = 
	{
		Acceleration = {
			X = gs_accel.x,
			Y = gs_accel.y,
			Z = gs_accel.z
		},
		Pitch = gs_ownData.Pitch,
		Roll = gs_ownData.Bank,
		AltAGLM = altAglM,
		Velocity = {
			X = velocity.x,
			Y = velocity.y,
			Z = velocity.z
		}
	}
	
	GS.socket.try(GS.udpsend:sendto(GS.JSON:encode(GS.payload).." \n", GS.host, GS.port))
	
	-- Chain previously-included export as necessary
	if GS.PrevExport.LuaExportAfterNextFrame then
		GS.PrevExport.LuaExportAfterNextFrame()
	end
end